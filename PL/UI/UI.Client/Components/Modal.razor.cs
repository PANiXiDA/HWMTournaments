using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;
using System.Reflection;

using Common.Helpers;

using Microsoft.AspNetCore.Components;
using UI.Client.Services;

namespace UI.Client.Components;

public enum ModalMode { Info, Form }

public partial class Modal<TModel> : ComponentBase where TModel : class, new()
{
    [Inject] private HttpClient Http { get; set; } = default!;
    [Inject] private NotificationService Notifications { get; set; } = default!;

    [Parameter] public bool IsOpen { get; set; }
    [Parameter] public EventCallback<bool> IsOpenChanged { get; set; }

    [Parameter] public string? Title { get; set; }

    [Parameter] public string? InfoHtml { get; set; }

    [Parameter] public TModel? InitialModel { get; set; }

    [Parameter] public string? EndpointUrl { get; set; }
    [Parameter] public string HttpMethod { get; set; } = "POST";
    [Parameter] public Dictionary<string, string>? Headers { get; set; }
    [Parameter] public string? SubmitText { get; set; }

    [Parameter] public EventCallback OnSuccess { get; set; }

    private ModalMode _mode => InitialModel is null ? ModalMode.Info : ModalMode.Form;

    private TModel? _current;
    protected PropertyInfo[] _properties = Array.Empty<PropertyInfo>();

    private TaskCompletionSource<bool>? _closedTcs;
    protected bool IsSubmitting { get; set; }

    protected override void OnParametersSet()
    {
        if (_mode == ModalMode.Form)
        {
            _current = new TModel();
            if (InitialModel is not null)
                CopyPublicProps(InitialModel, _current);

            _properties = typeof(TModel)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.GetCustomAttribute<DisplayAttribute>() != null && p.CanRead && p.CanWrite)
                .ToArray();
        }
        else
        {
            _current = null;
            _properties = Array.Empty<PropertyInfo>();
        }
    }

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (!IsOpen && _closedTcs is { Task.IsCompleted: false })
        {
            _closedTcs.TrySetResult(true);
        }
        return Task.CompletedTask;
    }

    private static void CopyPublicProps(object from, object to)
    {
        var fProps = from.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var tProps = to.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                          .ToDictionary(p => p.Name);
        foreach (var fp in fProps)
        {
            if (!fp.CanRead) continue;
            if (tProps.TryGetValue(fp.Name, out var tp) && tp.CanWrite)
            {
                var val = fp.GetValue(from);
                tp.SetValue(to, val);
            }
        }
    }

    protected string? GetString(PropertyInfo p) => p.GetValue(_current!)?.ToString();

    protected string? GetDateTimeLocal(PropertyInfo p)
    {
        var v = p.GetValue(_current!);
        if (v is DateTime dt) return dt.ToString("yyyy-MM-ddTHH:mm");
        return null;
    }

    protected void SetFromBool(ChangeEventArgs e, PropertyInfo p)
    {
        if (_current is null) return;
        var isChecked = e?.Value is bool b ? b : bool.TryParse(e?.Value?.ToString(), out var parsed) && parsed;
        p.SetValue(_current, isChecked);
        StateHasChanged();
    }

    protected void SetFromDateTimeLocal(ChangeEventArgs e, PropertyInfo p)
    {
        if (_current is null)
        {
            return;
        }
        if (DateTime.TryParse(e?.Value?.ToString(), out var dt))
        {
            p.SetValue(_current, dt);
        }
        else
        {
            p.SetValue(_current, null);
        }
        StateHasChanged();
    }

    protected void SetFromString(ChangeEventArgs e, PropertyInfo p)
    {
        if (_current is null) return;

        var s = e?.Value?.ToString();
        var targetType = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
        object? converted = null;

        if (targetType == typeof(string)) converted = s;
        else if (Enum.TryParse(targetType, s, out var en)) converted = en;
        else if (targetType == typeof(int) && int.TryParse(s, out var i)) converted = i;
        else if (targetType == typeof(long) && long.TryParse(s, out var l)) converted = l;
        else if (targetType == typeof(double) && double.TryParse(s, out var d)) converted = d;
        else if (targetType == typeof(float) && float.TryParse(s, out var f)) converted = f;
        else if (targetType == typeof(decimal) && decimal.TryParse(s, out var dec)) converted = dec;
        else if (targetType == typeof(bool)) converted = s == "true";
        else if (targetType == typeof(DateTime) && DateTime.TryParse(s, out var dt)) converted = dt;
        else if (string.IsNullOrWhiteSpace(s)) converted = null;

        p.SetValue(_current, converted);
        StateHasChanged();
    }

    protected async Task HandleSubmit()
    {
        if (IsSubmitting || _current is null) return;

        try
        {
            IsSubmitting = true;

            if (string.IsNullOrWhiteSpace(EndpointUrl))
            {
                await Notifications.NotifyAsync("Не задан EndpointUrl для модалки.", isError: true);
                return;
            }

            using var req = new HttpRequestMessage(new HttpMethod(HttpMethod), EndpointUrl)
            {
                Content = JsonContent.Create(_current)
            };

            if (Headers is not null)
            {
                foreach (var (k, v) in Headers)
                    req.Headers.TryAddWithoutValidation(k, v);
            }

            using var resp = await Http.SendAsync(req);
            var json = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode)
            {
                var detail = ApiErrorHelper.TryExtractDetail(json);
                var message = string.IsNullOrWhiteSpace(detail)
                    ? "Запрос не выполнен. Проверьте данные и попробуйте ещё раз."
                    : $"Ошибка: {detail}";
                await Notifications.NotifyAsync(message, isError: true);
                return;
            }

            await CloseAsync(); // ждём закрытия
            if (OnSuccess.HasDelegate)
                await OnSuccess.InvokeAsync();
            else
                await Notifications.NotifyAsync("Успешно!");
        }
        catch (Exception ex)
        {
            await Notifications.NotifyAsync($"Произошла ошибка: {ex.Message}", isError: true);
        }
        finally
        {
            IsSubmitting = false;
        }
    }

    private async Task CloseAsync()
    {
        if (IsOpenChanged.HasDelegate)
        {
            _closedTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            await IsOpenChanged.InvokeAsync(false);
            // дождаться фактического закрытия и рендера
            await _closedTcs.Task;
        }
    }
}
