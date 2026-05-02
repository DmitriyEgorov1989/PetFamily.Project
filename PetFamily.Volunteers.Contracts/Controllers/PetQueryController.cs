using MediatR;
using Microsoft.AspNetCore.Mvc;
using PetFamily.Api.Extensions;
using PetFamily.Volunteers.Contracts.Controllers.Models.Pets.ReadModels;
using PetFamily.Volunteers.Core.Application.UseCases.Queries.GetAllPetsWithPaginationAndFilter;

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