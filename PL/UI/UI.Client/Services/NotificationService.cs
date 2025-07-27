namespace UI.Client.Services;

public class NotificationService
{
    public event Func<NotificationMessage, Task>? OnNotify;

    public Task NotifyAsync(string message, bool isError = false)
    {
        var notificationMessage = new NotificationMessage { Message = message, IsError = isError };

        var handlers = OnNotify;
        if (handlers is null) return Task.CompletedTask;

        foreach (var @delegate in handlers.GetInvocationList())
        {
            if (@delegate is Func<NotificationMessage, Task> cb)
            {
                _ = SafeInvokeAsync(cb, notificationMessage);
            }
        }
        return Task.CompletedTask;
    }

    private static async Task SafeInvokeAsync(Func<NotificationMessage, Task> func, NotificationMessage message)
    {
        try { await func(message); } catch {  }
    }
}


public class NotificationMessage
{
    public string Message { get; set; } = default!;
    public bool IsError { get; set; }
}
