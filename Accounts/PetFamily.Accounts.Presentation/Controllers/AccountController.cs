using MediatR;
using Microsoft.AspNetCore.Mvc;
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
    public async Task<ActionResult<string>> LoginAsync(
        [FromBody] LoginUserRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request.ToCommand(), cancellationToken);
        return result.ToResponseErrorOrResult();
    }
}