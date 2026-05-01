using Microsoft.AspNetCore.Mvc;
using PetFamily.Api.Controllers.Models.Pets.ReadModels;

namespace PetFamily.Api.Controllers;

public class PetQueryController(IMediator mediator) : ApplicationController
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