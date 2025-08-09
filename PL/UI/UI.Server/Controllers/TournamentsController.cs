using BL.Interfaces;

using Common.ConvertParams;
using Common.Mappers;
using Common.SearchParams;
using Common.SearchParams.Core;

using DTOs.Core;
using DTOs.Models;

using Microsoft.AspNetCore.Mvc;

using UI.Server.Controllers.Core;

namespace UI.Server.Controllers;

[Route("api/v1/tournaments")]
public sealed class TournamentsController : BaseApiController
{
    private readonly ITournamentsBL _tournamentsBL;

    public TournamentsController(ITournamentsBL tournamentsBL)
    {
        _tournamentsBL = tournamentsBL;
    }

    [HttpGet]
    [Route("{id:int}")]
    [ProducesResponseType(typeof(RestApiResponse<TournamentDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RestApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RestApiResponse<TournamentDTO>>> Get([FromRoute] int id, [FromQuery] TournamentsConvertParams? convertParams)
    {
        var response = TournamentsMapper.EntityToDTO(await _tournamentsBL.GetAsync(id, convertParams));
        return StatusCode(StatusCodes.Status200OK, RestApiResponseBuilder<TournamentDTO>.Success(response));
    }

    [HttpGet]
    [Route("get-by-filter")]
    [ProducesResponseType(typeof(RestApiResponse<SearchResult<TournamentDTO>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<RestApiResponse<SearchResult<TournamentDTO>>>> Get([FromQuery] TournamentsSearchParams searchParams, [FromQuery] TournamentsConvertParams? convertParams)
    {
        var searchResult = await _tournamentsBL.GetAsync(searchParams, convertParams);
        var viewModel = new SearchResult<TournamentDTO>(searchResult.Total, TournamentsMapper.FromEntityToDTOList(searchResult.Objects), searchParams.Page, searchParams.ObjectsCount ?? searchResult.Total);
        return StatusCode(StatusCodes.Status200OK, RestApiResponseBuilder<SearchResult<TournamentDTO>>.Success(viewModel));
    }

    [HttpPost]
    [ProducesResponseType(typeof(RestApiResponse<int>), StatusCodes.Status201Created)]
    public async Task<ActionResult<RestApiResponse<int>>> Create([FromBody] TournamentDTO request)
    {
        request.Id = await _tournamentsBL.AddOrUpdateAsync(TournamentsMapper.DTOToEntity(request));
        return StatusCode(StatusCodes.Status201Created, RestApiResponseBuilder<int>.Success(request.Id));
    }

    [HttpPut]
    [Route("{id:int}")]
    [ProducesResponseType(typeof(RestApiResponse<NoContent>), StatusCodes.Status200OK)]
    public async Task<ActionResult<RestApiResponse<NoContent>>> Update([FromBody] TournamentDTO request)
    {
        await _tournamentsBL.AddOrUpdateAsync(TournamentsMapper.DTOToEntity(request));
        return StatusCode(StatusCodes.Status200OK, RestApiResponseBuilder<NoContent>.Success(new NoContent()));
    }

    [HttpDelete]
    [Route("{id:int}")]
    [ProducesResponseType(typeof(RestApiResponse<NoContent>), StatusCodes.Status204NoContent)]
    public async Task<ActionResult<RestApiResponse<NoContent>>> Delete([FromRoute] int id)
    {
        await _tournamentsBL.DeleteAsync(id);
        return StatusCode(StatusCodes.Status204NoContent, RestApiResponseBuilder<NoContent>.Success(new NoContent()));
    }
}
