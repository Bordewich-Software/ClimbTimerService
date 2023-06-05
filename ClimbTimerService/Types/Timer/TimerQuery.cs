using ClimbTimerService.Services;
using HotChocolate.Language;

namespace ClimbTimerService.Types.Timer;

[ExtendObjectType(OperationType.Query)]
public class TimerQuery
{
    public StopWatchState ElapsedTime(string id, TimerService timerService) => timerService.StopWatchState(id);

    public List<string> CurrentStopwatches(TimerService timerService) => timerService.CurrentStopwatchIds;
}