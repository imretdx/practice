using System;
using System.Collections.Concurrent;
using System.Threading;

namespace task17;

public interface ICommand
{
    void Execute();
}

public class ServerThread
{
    private readonly BlockingCollection<ICommand> _queue = new();
    private readonly Thread _thread;
    private bool _stopRequested = false;

    public Thread UnderlyingThread => _thread;

    public ServerThread()
    {
        _thread = new Thread(ProcessQueue);
    }

    public void Start() => _thread.Start();

    // Добавление команды в очередь сервера
    public void AddCommand(ICommand command) => _queue.Add(command);

    // Главный цикл обработки команд
    private void ProcessQueue()
    {
        while (!_stopRequested)
        {
            try
            {
                // Метод Take() блокирует поток БЕЗ затрат процессора, если очередь пуста
                if (_queue.TryTake(out var command, Timeout.Infinite))
                {
                    command.Execute();
                }
            }
            catch (Exception)
            {
                // Перехват исключений из команд (по требованию ЛР №5)
            }
        }
    }

    // Немедленная остановка
    public void HardStop()
    {
        _stopRequested = true;
        _queue.CompleteAdding();
    }

    // Мягкая остановка после завершения всех команд
    public void SoftStop()
    {
        // Переключаем поведение: дорабатываем оставшиеся команды и выходим
        _queue.CompleteAdding();
        while (_queue.TryTake(out var command))
        {
            try { command.Execute(); } catch { }
        }
        _stopRequested = true;
    }
}

// Команда немедленной жесткой остановки
public class HardStopCommand : ICommand
{
    private readonly ServerThread _serverThread;
    public HardStopCommand(ServerThread serverThread) => _serverThread = serverThread;

    public void Execute()
    {
        if (Thread.CurrentThread != _serverThread.UnderlyingThread)
            throw new InvalidOperationException("Команда HardStop может быть выполнена только внутри целевого потока.");

        _serverThread.HardStop();
    }
}

// Команда мягкой плановой остановки
public class SoftStopCommand : ICommand
{
    private readonly ServerThread _serverThread;
    public SoftStopCommand(ServerThread serverThread) => _serverThread = serverThread;

    public void Execute()
    {
        if (Thread.CurrentThread != _serverThread.UnderlyingThread)
            throw new InvalidOperationException("Команда SoftStop может быть выполнена только внутри целевого потока.");

        _serverThread.SoftStop();
    }
}
