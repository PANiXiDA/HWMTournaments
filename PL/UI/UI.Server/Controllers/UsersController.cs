using BL.Interfaces;

using Common.ConvertParams;
using Common.Mappers;
using Common.SearchParams;
using Common.SearchParams.Core;

using Dev.Template.AspNetCore.API.Extensions;

using DTOs.Core;
using DTOs.Models;
using DTOs.Requests;

using Gen.IdentityService.ApplicationUserService;
using Gen.IdentityService.Enums;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using UI.Server.Controllers.Core;

using static Common.Constants.IdentityServiceConstants;

namespace UI.Server.Controllers;

[Route("api/v1/users")]
public sealed class UsersController : BaseApiController
{
    private readonly ILogger<UsersController> _logger;

    private readonly ApplicationUserService.ApplicationUserServiceClient _applicationUserServiceClient;
    private readonly IUsersBL _usersBL;

    public UsersController(
        ILogger<UsersController> logger,
        ApplicationUserService.ApplicationUserServiceClient applicationUserServiceClient,
        IUsersBL usersBL)
    {
        _logger = logger;

        _applicationUserServiceClient = applicationUserServiceClient;
        _usersBL = usersBL;
    }

    [HttpGet]
    [Route("{id:int}")]
    [ProducesResponseType(typeof(RestApiResponse<UserDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RestApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RestApiResponse<UserDTO>>> Get([FromRoute] int id, [FromQuery] UsersConvertParams? convertParams)
    {
        convertParams ??= new UsersConvertParams();
        var response = UsersMapper.EntityToDto(await _usersBL.GetAsync(id));
        return StatusCode(StatusCodes.Status200OK, RestApiResponseBuilder<UserDTO>.Success(response));
    }

    [HttpGet]
    [ProducesResponseType(typeof(RestApiResponse<SearchResult<UserDTO>>), StatusCodes.Status200OK)]
    [Route("get-by-filter")]
    public async Task<ActionResult<RestApiResponse<SearchResult<UserDTO>>>> Get([FromQuery] UsersSearchParams searchParams, [FromQuery] UsersConvertParams? convertParams)
    {
        convertParams ??= new UsersConvertParams();
        var searchResult = await _usersBL.GetAsync(searchParams, convertParams);
        var viewModel = new SearchResult<UserDTO>(searchResult.Total, UsersMapper.FromEntityToDTOList(searchResult.Objects), searchResult.RequestedPage, searchResult.RequestedObjectsCount);
        return StatusCode(StatusCodes.Status200OK, RestApiResponseBuilder<SearchResult<UserDTO>>.Success(viewModel));
    }

    [HttpPost]
    [ProducesResponseType(typeof(RestApiResponse<int>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(RestApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RestApiResponse<int>>> Create([FromBody] UserDTO request)
    {
        var user = UsersMapper.DTOToEntity(request);

        var grpcResponse = await _applicationUserServiceClient.CreateAsync(user.ApplicationUser);
        user.ApplicationUserId = grpcResponse.Id;

        request.Id = await _usersBL.AddOrUpdateAsync(user);

        await _applicationUserServiceClient.AddClaimAsync(
            new AddClaimRequest
            {
                ApplicationUserId = user.ApplicationUserId,
                Type = CustomJwtClaimTypes.UserId,
                Value = request.Id.ToString()
            });

        return StatusCode(StatusCodes.Status201Created, RestApiResponseBuilder<int>.Success(request.Id));
    }

    [HttpPut]
    [Authorize(Roles = $"{nameof(ApplicationUserRole.Developer)}")]
    [ProducesResponseType(typeof(RestApiResponse<NoContent>), StatusCodes.Status200OK)]
    public async Task<ActionResult<RestApiResponse<NoContent>>> Update([FromBody] UserDTO request)
    {
        var user = UsersMapper.DTOToEntity(request);
        user.ApplicationUserId = User.GetApplicationUserId();
        user.ApplicationUser!.Id = User.GetApplicationUserId();

        await _applicationUserServiceClient.UpdateAsync(user.ApplicationUser);
        await _usersBL.AddOrUpdateAsync(user);

        return StatusCode(StatusCodes.Status200OK, RestApiResponseBuilder<NoContent>.Success(new NoContent()));
    }

    [HttpDelete]
    [Route("{id:int}")]
    [Authorize]
    [ProducesResponseType(typeof(RestApiResponse<NoContent>), StatusCodes.Status204NoContent)]
    public async Task<ActionResult<RestApiResponse<NoContent>>> Delete([FromRoute] int id)
    {
        await _applicationUserServiceClient.DeleteAsync(new DeleteApplicationUserRequest { Id = User.GetApplicationUserId() });
        await _usersBL.DeleteAsync(id);
        return StatusCode(StatusCodes.Status204NoContent, RestApiResponseBuilder<NoContent>.Success(new NoContent()));
    }

    [HttpPost]
    [ProducesResponseType(typeof(RestApiResponse<int>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(RestApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RestApiResponse<int>>> Registration([FromBody] RegistrationRequest request)
    {
        var user = UsersMapper.RegistrationRequestToEntity(request);
        await _usersBL.AddOrUpdateAsync(user);
        return StatusCode(StatusCodes.Status201Created, RestApiResponseBuilder<int>.Success(user.Id));
    }
}
