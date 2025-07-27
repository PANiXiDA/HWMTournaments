using System.ComponentModel.DataAnnotations;

namespace Common.Enums;

public enum TournamentFormat
{
    [Display(Name = "Дуэль")]
    Duel = 0,

    [Display(Name = "Командное 2 на 2")]
    Team2x2 = 1,

    [Display(Name = "Командное 3 на 3")]
    Team3x3 = 2,

    [Display(Name = "ФФА")]
    FFA = 3,
}
