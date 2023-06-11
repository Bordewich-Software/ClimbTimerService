using ClimbTimerService.Services;
using HotChocolate.Language;

namespace ClimbTimerService.Types.Timer;

[ExtendObjectType(OperationType.Query)]
public class TimerQuery
{
    public StopWatchState ElapsedTime(string id, TimerService timerService) => timerService.RemainingStopWatchState(id);
    public List<string> StopwatchIds(TimerService timerService) => timerService.StopwatchIds;
    public List<StopwatchConfig> StopwatchConfigs(TimerService timerService) => timerService.StopwatchConfigs;
}