namespace ClimbTimerService.Services;

public class StopwatchExistsException : Exception
{
    public StopwatchExistsException(string message): base(message)
    {
        
    }
}