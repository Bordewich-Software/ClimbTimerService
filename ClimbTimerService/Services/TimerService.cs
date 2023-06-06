using System.Collections.Concurrent;
using System.Diagnostics;

namespace ClimbTimerService.Services;

public record StopWatchState(int Minutes, int Seconds, TimerState TimerState);

public class TimerService
{
    private readonly ConcurrentDictionary<string, InternalStopWatch> _stopwatches = new();
    public List<string> CurrentStopwatchIds => _stopwatches.Keys.ToList();

    public bool CreateNewStopwatch(string id, TimeSpan timeSpan)
    {
        
        if (_stopwatches.ContainsKey(id))
            throw new StopwatchExistsException($"Stopwatch with Id {id}, already exists. Cannot create");

        return _stopwatches.TryAdd(id, new InternalStopWatch(new Stopwatch(), TimerState.Stopped, timeSpan));
    }

    public bool RemoveStopwatch(string id)
    {
        return _stopwatches.TryRemove(id, out _);
    }
    
    public TimerState Start(string id)
    {
        if (_stopwatches.TryGetValue(id, out var stw))
        {
            return stw.Start();
        }
        return TimerState.NotSet;
    }

    public TimerState Pause(string id)
    {
        if (_stopwatches.TryGetValue(id, out var stw))
        {
            return stw.Pause();
        }
        return TimerState.NotSet;
    }

    public TimerState Reset(string id)
    {
        if (_stopwatches.TryGetValue(id, out var stw))
        {
            return stw.Reset();
        }
        return TimerState.NotSet;
    }

    public TimerState Restart(string id)
    {
        if (_stopwatches.TryGetValue(id, out var stw))
        {
            return stw.Restart();
        }
        
        return TimerState.NotSet;
    }
    
    public TimerState Toggle(string id)
    {
        if (_stopwatches.TryGetValue(id, out var stw))
        {
            return stw.Toggle();
        }
        
        return TimerState.NotSet;
    }

    public StopWatchState StopWatchState(string id)
    {
        if (_stopwatches.TryGetValue(id, out var stw))
            return stw.CalculateStopwatchState();

        return new StopWatchState(0, 0, TimerState.NotSet);
    }
}

public class InternalStopWatch
{
    private Stopwatch Stopwatch { get; init; }
    private TimerState TimerState { get; set; }
    private TimeSpan EndTimeSpan { get; init; }
    public InternalStopWatch(Stopwatch stopwatch, TimerState timerState, TimeSpan endTimeSpan)
    {
        EndTimeSpan = endTimeSpan;
        Stopwatch = stopwatch;
        TimerState = timerState;
    }

    public StopWatchState CalculateStopwatchState()
    {
        var s = EndTimeSpan.Subtract(Stopwatch.Elapsed);
        if (s.TotalMilliseconds <= 0.0)
        {
            Stopwatch.Stop();
            TimerState = TimerState.Stopped;
            return new StopWatchState(0, 0, TimerState);
        }
            
        return new StopWatchState(s.Minutes, s.Seconds, TimerState);
    }

    public TimerState Start() {
        Stopwatch.Start();
        TimerState = TimerState.Started;
        return TimerState;
    }
    public TimerState Pause() {
        Stopwatch.Stop();
        TimerState = TimerState.Paused;
        return TimerState;
    }
    public TimerState Reset() {
        Stopwatch.Reset();
        TimerState = TimerState.Stopped;
        return TimerState;
    }
    public TimerState Restart() {
        Stopwatch.Restart();
        TimerState = TimerState.Started;
        return TimerState;
    }
    public TimerState Toggle() {
        return TimerState == TimerState.Started ? Pause() : Start();;
    }
}

public enum TimerState
{
    NotSet,
    Started,
    Paused,
    Stopped
}