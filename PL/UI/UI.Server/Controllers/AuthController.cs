using BL.Interfaces;

using Common.Mappers;

using DTOs.Core;
using DTOs.Requests;

using Microsoft.AspNetCore.Mvc;

using UI.Server.Controllers.Core;

namespace UI.Server.Controllers;

[Route("api/v1/auth")]
public sealed class AuthController : BaseApiController
{
    private readonly ILogger<AuthController> _logger;

    private readonly IUsersBL _usersBL;

    public AuthController(
        ILogger<AuthController> logger,
        IUsersBL usersBL)
    {
        _logger = logger;

        _usersBL = usersBL;
    }

    [HttpPost]
    [Route("registration")]
    [ProducesResponseType(typeof(RestApiResponse<int>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(RestApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RestApiResponse<int>>> Registration([FromBody] RegistrationRequest request)
    {
        var user = UsersMapper.RegistrationRequestToEntity(request);
        await _usersBL.AddOrUpdateAsync(user);
        return StatusCode(StatusCodes.Status201Created, RestApiResponseBuilder<int>.Success(user.Id));
    }
}
