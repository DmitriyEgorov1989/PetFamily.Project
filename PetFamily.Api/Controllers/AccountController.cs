//using MediatR;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using PetFamily.Api.Controllers.Models.Accounts;
//using PetFamily.Api.Extensions;

//namespace PetFamily.Api.Controllers;

//[ApiController]
//[Route("[controller]")]
//public class AccountController : ControllerBase
//{
//    private readonly IMediator _mediator;

//    public AccountController(IMediator mediator)
//    {
//        _mediator = mediator;
//    }

//    [Authorize]
//    [HttpGet]
//    public ActionResult Test()
//    {
//        return Ok("test");
//    }

//    [HttpPatch("registration")]
//    public async Task<ActionResult<Guid>> RegistrationAsync(
//        [FromBody] RegistrationUserRequest request,
//        CancellationToken cancellationToken)
//    {
//        var result = await _mediator.Send(request.ToCommand(), cancellationToken);
//        return result.ToResponseErrorOrResult();
//    }


//    [HttpPatch("login")]
//    public async Task<ActionResult<string>> LoginAsync(
//        [FromBody] LoginUserRequest request,
//        CancellationToken cancellationToken)
//    {
//        var result = await _mediator.Send(request.ToCommand(), cancellationToken);
//        return result.ToResponseErrorOrResult();
//    }
//}