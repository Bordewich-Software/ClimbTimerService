using ClimbTimerService.Services;
using HotChocolate.Language;

namespace ClimbTimerService.Types.Timer;

[ExtendObjectType(OperationType.Query)]
public class TimerQuery
{
    public List<StopwatchConfig> StopwatchConfigs(TimerService timerService) => timerService.StopwatchConfigs;
    public StopwatchConfig? StopwatchConfig(string id, TimerService timerService) => timerService.StopwatchConfig(id);
}