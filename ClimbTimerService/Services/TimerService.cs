using System.Collections.Concurrent;
using System.Diagnostics;

namespace ClimbTimerService.Services;

public record StopWatchState(int Minutes, int Seconds, TimerState TimerState);

public class TimerService
{
    private readonly ConcurrentDictionary<string, InternalStopWatch> _stopwatches = new();
    public List<string> CurrentStopwatchIds => _stopwatches.Keys.ToList();

    public bool CreateNewStopwatch(string id)
    {
        if (_stopwatches.ContainsKey(id))
            throw new StopwatchExistsException($"Stopwatch with Id {id}, already exists. Cannot create");

        return _stopwatches.TryAdd(id, new InternalStopWatch(new Stopwatch(), TimerState.Stopped));
    }

    public bool RemoveStopwatch(string id)
    {
        return _stopwatches.TryRemove(id, out _);
    }
    
    public TimerState Start(string id)
    {
        if (_stopwatches.TryGetValue(id, out var stw))
        {
            stw.Stopwatch.Start();
            stw.TimerState = TimerState.Started;
            return stw.TimerState;
        }
        return TimerState.NotSet;
    }

    public TimerState Pause(string id)
    {
        if (_stopwatches.TryGetValue(id, out var stw))
        {
            stw.Stopwatch.Stop();
            stw.TimerState = TimerState.Paused;
            return stw.TimerState;
        }
        return TimerState.NotSet;
    }

    public TimerState Reset(string id)
    {
        if (_stopwatches.TryGetValue(id, out var stw))
        {
            stw.Stopwatch.Reset();
            stw.TimerState = TimerState.Stopped;
            return stw.TimerState;
        }
        return TimerState.NotSet;
    }

    public TimerState Restart(string id)
    {
        if (_stopwatches.TryGetValue(id, out var stw))
        {
            stw.Stopwatch.Restart();
            stw.TimerState = TimerState.Started;
            return stw.TimerState;
        }
        
        return TimerState.NotSet;
    }
    
    public TimerState Toggle(string id)
    {
        if (_stopwatches.TryGetValue(id, out var stw))
        {
            return stw.TimerState == TimerState.Started ? Pause(id) : Start(id);;
        }
        
        return TimerState.NotSet;
    }

    public StopWatchState StopWatchState(string id)
    {
        if (_stopwatches.TryGetValue(id, out var stw))
            return new StopWatchState(stw.Stopwatch.Elapsed.Minutes, stw.Stopwatch.Elapsed.Seconds, stw.TimerState);
        
        return new StopWatchState(0, 0, TimerState.NotSet);
    }
}

public class InternalStopWatch
{
    public InternalStopWatch(Stopwatch stopwatch, TimerState timerState)
    {
        Stopwatch = stopwatch;
        TimerState = timerState;
    }

    public Stopwatch Stopwatch { get; set; }
    public TimerState TimerState { get; set; }
}

public enum TimerState
{
    NotSet,
    Started,
    Paused,
    Stopped
}