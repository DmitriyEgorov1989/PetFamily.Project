using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PetFamily.Accounts.Core.Application.UseCases.AccountManager.Commands.RefreshLoginToken;
using PetFamily.Accounts.Core.Application.UseCases.AccountManager.CommonDto;
using PetFamily.Accounts.Core.Application.UseCases.AccountManager.Queries.GetMe;
using PetFamily.Accounts.Presentation.Controllers.Models.Accounts;
using PetFamily.Framework;
using PetFamily.Framework.Response;

namespace PetFamily.Accounts.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public partial class AccountController : ApplicationController
{
    private readonly IMediator _mediator;

    public AccountController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [HttpGet]
    public ActionResult Test()
    {
        return Ok("test");
    }

    [HttpPost("register")]
    public async Task<ActionResult<Guid>> RegistrationAsync(
        [FromBody] RegistrationUserRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request.ToCommand(), cancellationToken);
        return result.ToResponseErrorOrResult();
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> LoginAsync(
        [FromBody] LoginUserRequest request,
        CancellationToken cancellationToken)
    {
        var result =
            await _mediator.Send(request.ToCommand(), cancellationToken);
        if (result.IsSuccess)
        {
            HttpContext.AddRefreshTokenCookie(result.Value.RefreshToken);
        }
        return result.ToResponseErrorOrResult();
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<LoginResponse>> RefreshTokenAsync(
        CancellationToken cancellationToken)
    {
        var refreshToken = HttpContext.Request.Cookies["refreshToken"];
        if (refreshToken == null)
            return BadRequest("Refresh token is missing");

        var result =
            await _mediator.Send(new RefreshLoginTokenCommand(refreshToken), cancellationToken);

        if (result.IsSuccess)
        {
            HttpContext.AddRefreshTokenCookie(result.Value.RefreshToken);
        }

        return result.ToResponseErrorOrResult();
    }

    [HttpGet("get-me")]
    public async Task<ActionResult<GetMeResponse>> GetMeAsync(
        CancellationToken cancellationToken)
    {
        var refreshToken = HttpContext.Request.Cookies["refreshToken"];
        if (refreshToken == null)
            return BadRequest("Refresh token is missing");

        var result =
            await _mediator.Send(new GetMeRequest(refreshToken), cancellationToken);

        return result.ToResponseErrorOrResult();
    }
}