using System;
using System.Collections.Concurrent;
using System.Threading;

namespace task18;

public interface ICommand
{
    void Execute();
    bool IsCompleted { get; } // Показывает планировщику, завершена ли длительная операция
}

public interface IScheduler
{
    bool HasCommand();
    ICommand Select();
    void Add(ICommand cmd);
}

// Реализация циклического планировщика (Round-Robin)
public class RoundRobinScheduler : IScheduler
{
    private readonly ConcurrentQueue<ICommand> _tasks = new();

    public bool HasCommand() => !_tasks.IsEmpty;

    public void Add(ICommand cmd) => _tasks.Enqueue(cmd);

    public ICommand Select()
    {
        if (_tasks.TryDequeue(out var cmd))
        {
            return cmd;
        }
        throw new InvalidOperationException("Планировщик пуст.");
    }
}

// Серверный поток, совмещающий внешнюю очередь и планировщик без мертвых блокировок
public class SchedulerServerThread
{
    private readonly BlockingCollection<ICommand> _externalQueue = new();
    private readonly IScheduler _scheduler;
    private readonly Thread _thread;
    private bool _stopRequested = false;

    public Thread UnderlyingThread => _thread;

    public SchedulerServerThread(IScheduler scheduler)
    {
        _scheduler = scheduler ?? throw new ArgumentNullException(nameof(scheduler));
        _thread = new Thread(ProcessLoop);
    }

    public void Start() => _thread.Start();
    public void Stop() => _stopRequested = true;

    public void AddCommand(ICommand command) => _externalQueue.Add(command);

    private void ProcessLoop()
    {
        while (!_stopRequested)
        {
            // 1. Сначала переносим ВСЕ новые команды из внешней очереди в планировщик (без блокировки)
            while (_externalQueue.TryTake(out var newCmd))
            {
                _scheduler.Add(newCmd);
            }

            // 2. Если в планировщике ЕСТЬ активные длительные задачи
            if (_scheduler.HasCommand())
            {
                var currentTask = _scheduler.Select();
                
                try
                {
                    currentTask.Execute(); // Выполняем один квант времени (шаг)
                }
                catch { /* Перехват исключений */ }

                // Если задача НЕ завершена, возвращаем её обратно в планировщик для Round-Robin
                if (!currentTask.IsCompleted)
                {
                    _scheduler.Add(currentTask);
                }
                
                // Делаем микро-паузу, чтобы дать процессору передохнуть при псевдопараллелизме
                Thread.Sleep(1);
            }
            else
            {
                // 3. Если в планировщике НЕТ работы — засыпаем на внешней очереди блокирующим образом (0% CPU)
                try
                {
                    if (_externalQueue.TryTake(out var firstCmd, Timeout.Infinite))
                    {
                        _scheduler.Add(firstCmd);
                    }
                }
                catch (ObjectDisposedException) { break; }
                catch (ArgumentOutOfRangeException) { break; }
            }
        }
    }
}
