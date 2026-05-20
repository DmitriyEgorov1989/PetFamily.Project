using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetFamily.Accounts.Core.Application.UseCases.AccountManager.CommonDto;
using PetFamily.Accounts.Presentation.Controllers.Models.Accounts;
using PetFamily.Framework;
using PetFamily.Framework.Response;

namespace PetFamily.Accounts.Presentation.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountController : ApplicationController
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

    [HttpPatch("registration")]
    public async Task<ActionResult<Guid>> RegistrationAsync(
        [FromBody] RegistrationUserRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request.ToCommand(), cancellationToken);
        return result.ToResponseErrorOrResult();
    }

    [HttpPatch("login")]
    public async Task<ActionResult<LoginResponse>> LoginAsync(
        [FromBody] LoginUserRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request.ToCommand(), cancellationToken);
        return result.ToResponseErrorOrResult();
    }

    [HttpPatch("refresh-token")]
    public async Task<ActionResult<LoginResponse>> RefreshTokenAsync(
        [FromBody] RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        var result =
            await _mediator.Send(request.ToCommand(), cancellationToken);
        return result.ToResponseErrorOrResult();
    }
}