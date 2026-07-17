using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using task19;

namespace task19tests;

public class AdvancedSchedulerTests
{
    [Fact]
    public void Scheduler_ShouldExecuteFiveCommandsThreeTimesAndHardStop()
    {
        var server = new AdvancedSchedulerServerThread();
        
        // Создаем 5 экземпляров TestCommand
        var testCommands = Enumerable.Range(1, 5)
            .Select(id => new TestCommand(id))
            .ToList();

        // Добавляем их в планировщик
        testCommands.ForEach(cmd => server.AddCommand(cmd));

        server.Start();

        // Даем потоку поработать, чтобы команды успели сделать свои шаги
        Thread.Sleep(200);

        // Посылаем команду HardStop
        server.AddCommand(new HardStopCommand(server));

        server.UnderlyingThread.Join(1000);

        // Проверяем, что каждый из 5 экземпляров выполнился ровно 3 раза
        Assert.All(testCommands, cmd => Assert.Equal(3, cmd.Counter));
    }
}
