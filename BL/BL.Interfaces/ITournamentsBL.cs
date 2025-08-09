using BL.Interfaces.Core;

using Common.ConvertParams;
using Common.SearchParams;

using Entities;

namespace BL.Interfaces;

public interface ITournamentsBL : ICrudBL<Tournament, int, TournamentsSearchParams, TournamentsConvertParams>
{
}