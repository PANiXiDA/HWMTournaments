using DTOs.Models;

using Microsoft.AspNetCore.Components;

using UI.Client.Services.Interfaces;

namespace UI.Client.Pages.Tournaments;

public partial class TournamentEdit : ComponentBase
{
    [Inject] private ITournamentsService Tournaments { get; set; } = default!;
    [Inject] private NavigationManager Nav { get; set; } = default!;

    [Parameter] public int? TournamentId { get; set; }

    private TournamentDTO Tournament { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        if (TournamentId.HasValue)
        {
            var dto = await Tournaments.GetAsync(TournamentId!.Value);
            Tournament = dto ?? new TournamentDTO();
        }
        else
        {
            Tournament.StartDate = DateTime.UtcNow;
        }
    }

    protected async Task HandleCreateOrUpdateAsync()
    {
        Tournament.StartDate = DateTime.SpecifyKind(Tournament.StartDate, DateTimeKind.Utc);

        if (TournamentId.HasValue)
        {
            await Tournaments.UpdateAsync(TournamentId!.Value, Tournament);
            Nav.NavigateTo($"/tournaments/update/{TournamentId!.Value}");
            return;
        }

        var id = await Tournaments.CreateAsync(Tournament);
        if (id.HasValue)
        {
            Nav.NavigateTo($"/tournaments/update/{id.Value}");
        }
    }

    protected void Cancel()
    {
        Nav.NavigateTo("/tournaments");
    }
}
