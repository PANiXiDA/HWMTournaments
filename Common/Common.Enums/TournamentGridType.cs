using System.ComponentModel.DataAnnotations;

namespace Common.Enums;

public enum TournamentGridType
{
    [Display(Name = "Single Elimination")]
    SingleElimination = 0,

    [Display(Name = "Double Elimination")]
    DoubleElimination = 1,

    [Display(Name = "Leaderboard")]
    Leaderboard = 2,

    [Display(Name = "RoundRobin")]
    RoundRobin = 3,
}
