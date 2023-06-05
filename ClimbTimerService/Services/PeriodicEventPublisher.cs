using HotChocolate.Subscriptions;

namespace ClimbTimerService.Services;

public class PeriodicEventPublisher: BackgroundService
{
    private readonly TimerService _timerService;
    private readonly ITopicEventSender _eventSender;
    private readonly PeriodicTimer _timer = new(TimeSpan.FromMilliseconds(500));

    public PeriodicEventPublisher(TimerService timerService, ITopicEventSender eventSender)
    {
        _timerService = timerService;
        _eventSender = eventSender;
    }
    
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested && await _timer.WaitForNextTickAsync(cancellationToken))
        {
            foreach (var stopwatchId in _timerService.CurrentStopwatchIds)
            {
                await _eventSender.SendAsync($"ElapsedTime_{stopwatchId}", _timerService.StopWatchState(stopwatchId), cancellationToken);    
            }
        }
    }
}