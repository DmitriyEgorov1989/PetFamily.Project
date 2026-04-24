using MediatR;
using Microsoft.AspNetCore.Mvc;
using PetFamily.Api.Controllers.Models.Pets.ReadModels;
using PetFamily.Api.Extensions;
using PetFamily.Core.Application.UseCases.Queries.GetAllPetsWithPaginationAndFilter;

namespace PetFamily.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class PetQueryController(IMediator mediator) : ControllerBase
{
    [HttpGet("all-with-pagination-and-filter")]
    public async Task<ActionResult<GetAllPetsWithPaginationAndFilterResponse>> GetAllAsync(
        [FromQuery] GetAllPetsWithPaginationAndFilterRequest request,
        CancellationToken cancellationToken)
    {
        var query = request.ToQuery();

        var result = await mediator.Send(query, cancellationToken);

        return result.ToResponseErrorOrResult();
    }
}