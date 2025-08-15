using Microsoft.AspNetCore.Components;

using UI.Client.Services.Interfaces;

namespace UI.Client.Components;

public partial class Toast : ComponentBase, IDisposable
{
    [Inject] private INotificationsService NotificationsService { get; set; } = default!;

    private bool _visible;
    private NotificationMessage _current = new();
    private CancellationTokenSource? _cts;

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            NotificationsService.OnNotify += ShowAsync;
        }
        return Task.CompletedTask;
    }

    private async Task ShowAsync(NotificationMessage notificationMessage)
    {
        _cts?.Cancel();
        _cts = new CancellationTokenSource();

        _current = notificationMessage;
        _visible = true;
        await InvokeAsync(StateHasChanged);

        try
        {
            await Task.Delay(3000, _cts.Token);
        }
        catch (OperationCanceledException) { }

        if (_cts?.IsCancellationRequested == true)
        {
            return;
        }

        _visible = false;
        await InvokeAsync(StateHasChanged);
    }

    private void Hide()
    {
        _cts?.Cancel();
        _visible = false;
        _ = InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        NotificationsService.OnNotify -= ShowAsync;
    }
}
