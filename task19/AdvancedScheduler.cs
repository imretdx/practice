using System;
using System.Collections.Concurrent;
using System.Threading;

namespace task19;

public interface ICommand
{
    void Execute();
}

// Шаг работы. Реализация тестовой команды из методички
public class TestCommand : ICommand
{
    private readonly int _id;
    public int Counter { get; private set; } = 0;

    public TestCommand(int id) => _id = id;

    public void Execute()
    {
        Counter++;
        Console.WriteLine($"Поток {_id} вызов {Counter}");
    }
}

// Маркерная команда для жесткой остановки внутри цикла планировщика
public class HardStopCommand : ICommand

{
    private readonly AdvancedSchedulerServerThread _server;
    public HardStopCommand(AdvancedSchedulerServerThread server) => _server = server;
    public void Execute() => _server.HardStop();
}

public class AdvancedSchedulerServerThread
{
    private readonly BlockingCollection<ICommand> _externalQueue = new();
    private readonly ConcurrentQueue<ICommand> _scheduler = new();
    private readonly Thread _thread;
    private bool _stopRequested = false;

    public Thread UnderlyingThread => _thread;

    public AdvancedSchedulerServerThread()
    {
        _thread = new Thread(ProcessLoop);
    }

    public void Start() => _thread.Start();
    public void HardStop() => _stopRequested = true;

    public void AddCommand(ICommand command) => _externalQueue.Add(command);

    private void ProcessLoop()
    {
        while (!_stopRequested)
        {
            // Переносим новые команды в циклический планировщик
            while (_externalQueue.TryTake(out var newCmd))
            {
                _scheduler.Enqueue(newCmd);
            }

            if (!_scheduler.IsEmpty)
            {
                if (_scheduler.TryDequeue(out var currentTask))
                {
                    try
                    {
                        currentTask.Execute();
                    }
                    catch { }

                    // Если была вызвана жесткая остановка, прекращаем обработку немедленно
                    if (_stopRequested) break;

                    // Если это TestCommand и она еще не выполнилась 3 раза, возвращаем в планировщик
                    if (currentTask is TestCommand testCmd && testCmd.Counter < 3)
                    {
                        _scheduler.Enqueue(testCmd);
                    }
                }
                Thread.Sleep(1);
            }
            else
            {
                try
                {
                    if (_externalQueue.TryTake(out var firstCmd, Timeout.Infinite))
                    {
                        _scheduler.Enqueue(firstCmd);
                    }
                }
                catch { break; }
            }
        }
    }
}
