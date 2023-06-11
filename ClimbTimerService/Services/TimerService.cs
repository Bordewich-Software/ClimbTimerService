using System.Collections.Concurrent;
using System.Diagnostics;

namespace ClimbTimerService.Services;

public record StopWatchState(int Hours, int Minutes, int Seconds, TimerState TimerState);

public record StopwatchConfig(string Id, StopwatchFormat Format, int Hours, int Minutes, int Seconds);

public class TimerService
{
    private readonly ConcurrentDictionary<string, InternalStopWatch> _stopwatches = new();
    public List<string> StopwatchIds => _stopwatches.Keys.ToList();

    public List<StopwatchConfig> StopwatchConfigs => _stopwatches.Select(c =>
        new StopwatchConfig(c.Key, c.Value.Format, c.Value.End.Hours, c.Value.End.Minutes, c.Value.End.Seconds)).ToList();

    public bool CreateNewStopwatch(string id, StopwatchFormat format, TimeSpan timeSpan)
    {
        if (_stopwatches.ContainsKey(id))
            throw new StopwatchExistsException($"Stopwatch with Id {id}, already exists. Cannot create");

        return _stopwatches.TryAdd(id, new InternalStopWatch(format, new Stopwatch(), TimerState.Stopped, timeSpan));
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

    public StopWatchState RemainingStopWatchState(string id)
    {
        if (_stopwatches.TryGetValue(id, out var stw))
            return stw.CalculateRemainingStopwatchState();

        return new StopWatchState(0, 0, 0, TimerState.NotSet);
    }

    public StopWatchState StopWatchState(string id)
    {
        if (_stopwatches.TryGetValue(id, out var stw))
            return stw.CalculateStopWatchState();

        return new StopWatchState(0, 0, 0, TimerState.NotSet);
    }
}

public class InternalStopWatch
{
    private Stopwatch Stopwatch { get; init; }
    private TimerState TimerState { get; set; }
    public StopwatchFormat Format { get; }
    public TimeSpan End { get; init; }

    public InternalStopWatch(StopwatchFormat format, Stopwatch stopwatch, TimerState timerState, TimeSpan end)
    {
        Format = format;
        End = end;
        Stopwatch = stopwatch;
        TimerState = timerState;
    }

    public StopWatchState CalculateRemainingStopwatchState()
    {
        var s = End.Subtract(Stopwatch.Elapsed);
        if (s.TotalMilliseconds <= 0.0)
        {
            Stopwatch.Stop();
            TimerState = TimerState.Stopped;
            return new StopWatchState(0, 0, 0, TimerState);
        }

        return new StopWatchState(s.Hours, s.Minutes, s.Seconds, TimerState);
    }

    public StopWatchState CalculateStopWatchState()
    {
        var s = End.Subtract(Stopwatch.Elapsed);
        if (s.TotalMilliseconds <= 0.0)
        {
            Stopwatch.Stop();
            TimerState = TimerState.Stopped;
            return new StopWatchState(End.Hours, End.Minutes, End.Seconds, TimerState);
        }

        return new StopWatchState(Stopwatch.Elapsed.Hours, Stopwatch.Elapsed.Minutes, Stopwatch.Elapsed.Seconds,
            TimerState);
    }

    public TimerState Start()
    {
        Stopwatch.Start();
        TimerState = TimerState.Started;
        return TimerState;
    }

    public TimerState Pause()
    {
        Stopwatch.Stop();
        TimerState = TimerState.Paused;
        return TimerState;
    }

    public TimerState Reset()
    {
        Stopwatch.Reset();
        TimerState = TimerState.Stopped;
        return TimerState;
    }

    public TimerState Restart()
    {
        Stopwatch.Restart();
        TimerState = TimerState.Started;
        return TimerState;
    }

    public TimerState Toggle()
    {
        return TimerState == TimerState.Started ? Pause() : Start();
        ;
    }
}

public enum StopwatchFormat
{
    NotSet,
    Minutes,
    Hours
}

public enum TimerState
{
    NotSet,
    Started,
    Paused,
    Stopped
}