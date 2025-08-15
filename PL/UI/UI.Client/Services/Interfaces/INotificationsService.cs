namespace UI.Client.Services.Interfaces;

public interface INotificationsService
{
    event Func<NotificationMessage, Task>? OnNotify;
    Task NotifyAsync(string message, bool isError = false);
}

public class NotificationMessage
{
    public string Message { get; set; } = default!;
    public bool IsError { get; set; }
}
