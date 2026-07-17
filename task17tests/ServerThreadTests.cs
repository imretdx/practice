using System;
using System.Threading;
using Xunit;
using task17;

namespace task17tests;

public class ServerThreadTests
{
    private class MockCommand : ICommand
    {
        public bool Executed { get; private set; }
        public void Execute() => Executed = true;
    }

    [Fact]
    public void HardStopCommand_ShouldStopImmediately()
    {
        var server = new ServerThread();
        var cmd1 = new MockCommand();
        var hardStop = new HardStopCommand(server);
        var cmd2 = new MockCommand();

        server.AddCommand(cmd1);
        server.AddCommand(hardStop);
        server.AddCommand(cmd2); // Эта команда должна проигнорироваться

        server.Start();
        server.UnderlyingThread.Join(2000); // Ждем завершения потока

        Assert.True(cmd1.Executed);
        Assert.False(cmd2.Executed); // Задание выполнено: немедленная остановка
    }

    [Fact]
    public void SoftStopCommand_ShouldProcessAllRemainingCommands()
    {
        var server = new ServerThread();
        var cmd1 = new MockCommand();
        var softStop = new SoftStopCommand(server);
        var cmd2 = new MockCommand();

        server.AddCommand(cmd1);
        server.AddCommand(softStop);
        server.AddCommand(cmd2); // Должна быть выполнена, так как остановка мягкая

        server.Start();
        server.UnderlyingThread.Join(2000);

        Assert.True(cmd1.Executed);
        Assert.True(cmd2.Executed); // Задание выполнено: очередь отработана до конца
    }

    [Fact]
    public void StopCommands_ExecutedInWrongThread_ShouldThrowException()
    {
        var server = new ServerThread();
        var hardStop = new HardStopCommand(server);
        var softStop = new SoftStopCommand(server);

        // Пытаемся запустить команды прямо в текущем главном потоке (вручную), а не внутри сервера
        Assert.Throws<InvalidOperationException>(() => hardStop.Execute());
        Assert.Throws<InvalidOperationException>(() => softStop.Execute());
    }
}
