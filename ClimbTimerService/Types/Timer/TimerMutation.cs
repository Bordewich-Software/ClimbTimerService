using ClimbTimerService.Services;
using HotChocolate.Language;

namespace ClimbTimerService.Types.Timer;

[ExtendObjectType(OperationType.Mutation)]
public class TimerMutation
{
    public TimerState Reset(string id, TimerService timerService) => timerService.Reset(id);

    public TimerState Restart(string id, TimerService timerService) => timerService.Restart(id);

    public TimerState Toggle(string id, TimerService timerService) => timerService.Toggle(id);

    [Error<StopwatchExistsException>]
    public bool CreateStopWatch(string id, StopwatchFormat format, int hours, int minutes, int seconds, TimerService timerService) => timerService.CreateNewStopwatch(id, format, new TimeSpan(hours, minutes, seconds));

    public bool RemoveStopwatch(string id, TimerService timerService) => timerService.RemoveStopwatch(id);
}