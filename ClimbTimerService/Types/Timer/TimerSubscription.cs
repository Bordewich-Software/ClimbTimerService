using ClimbTimerService.Services;
using HotChocolate.Language;

namespace ClimbTimerService.Types.Timer;

[ExtendObjectType(OperationType.Subscription)]
public class TimerSubscription
{
    [Subscribe]
    [Topic("ElapsedTime_{id}")]
    public StopWatchState ElapsedTime(string id, [EventMessage] StopWatchState elapsed) => elapsed;
}