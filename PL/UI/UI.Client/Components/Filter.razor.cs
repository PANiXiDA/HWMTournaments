using System.ComponentModel.DataAnnotations;
using System.Reflection;

using Common.SearchParams.Core;

using Microsoft.AspNetCore.Components;

namespace UI.Client.Components;

public partial class Filter<TSearchParams> where TSearchParams : BaseSearchParams, new()
{
    [Parameter, EditorRequired]
    public TSearchParams SearchParams { get; set; } = default!;

    [Parameter]
    public EventCallback<TSearchParams> SearchParamsChanged { get; set; }

    [Parameter]
    public EventCallback OnApplyFilters { get; set; }

    protected PropertyInfo[] _properties = Array.Empty<PropertyInfo>();
    private bool _expanded = false;

    private void ToggleExpanded() => _expanded = !_expanded;

    protected override void OnParametersSet()
    {
        _properties = typeof(TSearchParams)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(property => property.GetCustomAttribute<DisplayAttribute>() != null)
            .ToArray();
    }

    protected string? BindValue(PropertyInfo prop) => prop.GetValue(SearchParams)?.ToString();

    protected async Task BindValueChanged(ChangeEventArgs changeEvent, PropertyInfo property)
    {
        var raw = changeEvent.Value;
        var s = raw?.ToString();
        var targetType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
        object? converted = null;

        if (targetType == typeof(string))
        {
            converted = s;
        }
        else if (Enum.TryParse(targetType, s, out var e))
        {
            converted = e;
        }
        else if (int.TryParse(s, out var i))
        {
            converted = i;
        }
        else if (long.TryParse(s, out var l))
        {
            converted = l;
        }
        else if (double.TryParse(s, out var d))
        {
            converted = d;
        }
        else if (float.TryParse(s, out var f))
        {
            converted = f;
        }
        else if (decimal.TryParse(s, out var dec))
        {
            converted = dec;
        }
        else if (targetType == typeof(bool))
        {
            if (raw is bool b) converted = b;
            else converted = (s == "true");
        }
        else if (DateTime.TryParse(s, out var dt))
        {
            converted = dt;
        }
        else if (string.IsNullOrWhiteSpace(s))
        {
            converted = null;
        }

        property.SetValue(SearchParams, converted);
        await SearchParamsChanged.InvokeAsync(SearchParams);
    }

    private async Task OnApply()
    {
        await SearchParamsChanged.InvokeAsync(SearchParams);
        await OnApplyFilters.InvokeAsync();
    }

    private async Task OnReset()
    {
        var except = new[] { nameof(BaseSearchParams.Page), nameof(BaseSearchParams.ObjectsCount) };

        foreach (var property in _properties.Where(p => !except.Contains(p.Name)))
        {
            var type = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

            if (type == typeof(bool))
            {
                property.SetValue(SearchParams, false);
            }
            else
            {
                property.SetValue(SearchParams, null);
            }
        }

        StateHasChanged();

        await SearchParamsChanged.InvokeAsync(SearchParams);
        await OnApplyFilters.InvokeAsync();
    }

}
