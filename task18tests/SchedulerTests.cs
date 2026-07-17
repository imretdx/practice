using System;
using System.Threading;
using System.Collections.Generic;
using Xunit;
using task18;

namespace task18tests;

public class SchedulerTests
{
    // Имитация длительной команды (например, шаги рендеринга или сетевой игры)
    private class LongRunningCommand : ICommand
    {
        private readonly string _name;
        private readonly List<string> _executionLog;
        private int _currentStep = 0;
        private const int TotalSteps = 3;

        public bool IsCompleted => _currentStep >= TotalSteps;

        public LongRunningCommand(string name, List<string> executionLog)
        {
            _name = name;
            _executionLog = executionLog;
        }

        public void Execute()
        {
            if (IsCompleted) return;
            _currentStep++;
            
            lock (_executionLog)
            {
                _executionLog.Add($"{_name}_Step{_currentStep}");
            }
        }
    }

    [Fact]
    public void Scheduler_ShouldExecuteCommandsInRoundRobinInterleavedOrder()
    {
        var scheduler = new RoundRobinScheduler();
        var server = new SchedulerServerThread(scheduler);
        var executionLog = new List<string>();

        var cmdA = new LongRunningCommand("A", executionLog);
        var cmdB = new LongRunningCommand("B", executionLog);

        // Добавляем две тяжелые команды в очередь
        server.AddCommand(cmdA);
        server.AddCommand(cmdB);

        server.Start();
        
        // Ждем, пока команды отработают свои шаги квантования
        Thread.Sleep(500);
        server.Stop();
        server.UnderlyingThread.Join(1000);

        // Проверяем чередование шагов (псевдопараллелизм)
        // Лог должен быть вперемешку, например: A_Step1, B_Step1, A_Step2, B_Step2...
        Assert.Equal(6, executionLog.Count);
        Assert.Equal("A_Step1", executionLog[0]);
        Assert.Equal("B_Step1", executionLog[1]);
        Assert.Equal("A_Step2", executionLog[2]);
        Assert.Equal("B_Step2", executionLog[3]);
    }
}
