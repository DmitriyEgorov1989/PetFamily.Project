using MediatR;
using Microsoft.AspNetCore.Mvc;
using PetFamily.Api.Common.Processors;
using PetFamily.Api.Extensions;
using PetFamily.Volunteers.Contracts.Controllers.Models.Volunteers.ReadModels;
using PetFamily.Volunteers.Contracts.Controllers.Models.Volunteers.WriteModels.VolunteerRequests;
using PetFamily.Volunteers.Core.Application.UseCases.Commands.VolunteerCommands.AddPhotoPets;
using PetFamily.Volunteers.Core.Application.UseCases.Commands.VolunteerCommands.CreateVolunteer;
using PetFamily.Volunteers.Core.Application.UseCases.Commands.VolunteerCommands.DeletePet;
using PetFamily.Volunteers.Core.Application.UseCases.Commands.VolunteerCommands.DeletePhotoPets;
using PetFamily.Volunteers.Core.Application.UseCases.Commands.VolunteerCommands.DeleteVolunteer;
using PetFamily.Volunteers.Core.Application.UseCases.Commands.VolunteerCommands.UpdateHelpRequisites;
using PetFamily.Volunteers.Core.Application.UseCases.Commands.VolunteerCommands.UpdateMainInfo;
using PetFamily.Volunteers.Core.Application.UseCases.Commands.VolunteerCommands.UpdateSocialNetwork;
using PetFamily.Volunteers.Core.Application.UseCases.CommonDto;
using PetFamily.Volunteers.Core.Application.UseCases.Queries.GetAllVolunteersWithPagination;
using PetFamily.Volunteers.Core.Application.UseCases.Queries.GetVolunteerById;

namespace PetFamily.Api.Controllers;

public class VolunteerController(IMediator mediator) : ApplicationController
{
    private readonly IMediator _mediator = mediator;

    [HttpPost("Create")]
    public async Task<ActionResult<Guid>> CreateVolunteerAsync(
        [FromBody] CreateVolunteerRequest request,
        CancellationToken cancellationToken = default)
    {
        var createVolunteerCommand =
            new CreateVolunteerCommand(
                request.FullName,
                request.Email,
                request.Description,
                request.Experience,
                request.PhoneNumber,
                request.SocialNetworks,
                request.HelpRequisites
            );

        var result = await _mediator.Send(createVolunteerCommand, cancellationToken);

        return result.ToResponseErrorOrResult();
    }

    [HttpPatch("{id:guid}/main-info")]
    public async Task<ActionResult> UpdateAsync(
        [FromRoute] Guid id,
        [FromBody] UpdateMainInfoVolunteerRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateMainInfoVolunteerCommand(id,
            new UpdateMainInfoVolunteerDto(
                new FullNameDto(request.FirstName, request.MiddleName, request.LastName),
                request.Email,
                request.Description,
                request.Experience,
                request.PhoneNumber));

        var result = await _mediator.Send(command, cancellationToken);

        return result.ToResponseOkOrError();
    }

    [HttpPut("{volunteerId:guid}/social-network")]
    public async Task<ActionResult> UpdateSocialNetworkAsync(
        [FromRoute] Guid volunteerId,
        [FromBody] UpdateSocialNetworkRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateSocialNetworkCommand(
            volunteerId,
            request.SocialNetworks.Select(sn => new SocialNetworkDto(sn.Name, sn.Link))
                .ToList()
        );

        var result = await _mediator.Send(command, cancellationToken);

        return result.ToResponseOkOrError();
    }

    [HttpPut("{id:guid}/help-requisites")]
    public async Task<ActionResult> UpdateHelpRequisitesAsync(
        [FromRoute] Guid id,
        [FromBody] UpdateHelpRequisitesRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateHelpRequisitesCommand(
            id,
            request.HelpRequisites.Select(hr =>
                    new HelpRequisiteDto(hr.Name, hr.Description))
                .ToList()
        );

        var result = await _mediator.Send(command, cancellationToken);

        return result.ToResponseOkOrError();
    }

    [HttpDelete("{id:guid}/soft-delete")]
    public async Task<ActionResult> SoftDeleteAsync(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteVolunteerCommand(id), cancellationToken);

        return result.ToResponseOkOrError();
    }

    [HttpPatch("{id:guid}/add-pet")]
    public async Task<ActionResult<Guid>> AddPetAsync(
        [FromRoute] Guid id,
        [FromBody] AddPetRequest request,
        CancellationToken cancellationToken)
    {
        var command = request.ToCommand(id);

        var result =
            await _mediator.Send(command, cancellationToken);

        return result.ToResponseErrorOrResult();
    }

    [HttpPatch("{volunteerId:guid}/include/{petId:guid}/add-photo")]
    public async Task<ActionResult<Guid>> UploadPetPhotoAsync(
        [FromRoute] Guid volunteerId,
        [FromRoute] Guid petId,
        [FromForm] UploadPetPhotoRequest request,
        CancellationToken cancellationToken)
    {
        await using var fileProcessor = new FileProcessor();
        var createFileDtos = fileProcessor.Process(request.FormFiles);

        var uploadPetPhotoCommand =
            new UploadPhotoPetsCommand(
                volunteerId,
                petId,
                createFileDtos
            );

        var resultResponse = await _mediator.Send(uploadPetPhotoCommand, cancellationToken);

        return resultResponse.ToResponseErrorOrResult();
    }

    [HttpDelete("{volunteerId:guid}/include/{petId:guid}/name-photo")]
    public async Task<ActionResult<string>> DeletePetPhotoAsync(
        [FromRoute] Guid volunteerId,
        [FromRoute] Guid petId,
        [FromForm] DeletePhotoPetsRequest request,
        CancellationToken cancellationToken)
    {
        var deletePetPhotoCommand =
            new DeletePhotoPetsCommand(volunteerId, petId, request.FileName);

        var resultResponse =
            await _mediator.Send(deletePetPhotoCommand, cancellationToken);

        return resultResponse.ToResponseErrorOrResult();
    }

    [HttpGet("{all-with-pagination}")]
    public async Task<ActionResult<GetAllVolunteersWithPaginationResponse>> GetAllWithPaginationAsync(
        [FromQuery] GetAllVolunteersWithPaginationRequest request,
        CancellationToken cancellationToken)
    {
        var result =
            await _mediator.Send(
                request.ToQuery(), cancellationToken);

        return result.ToResponseErrorOrResult();
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<GetVolunteerByIdResponse>> GetVolunteerByIdAsync(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var result =
            await _mediator.Send(
                new GetVolunteerByIdQuery(id), cancellationToken);

        return result.ToResponseErrorOrResult();
    }

    [HttpPatch("{id:guid}/include/{petId:guid}")]
    public async Task<ActionResult> UpdatePetAsync(
        [FromRoute] Guid id,
        [FromRoute] Guid petId,
        [FromBody] UpdatePetRequest request,
        CancellationToken cancellationToken)
    {
        var command = request.ToCommand(petId, id);

        var result = await _mediator.Send(command, cancellationToken);

        return result.ToResponseOkOrError();
    }

    [HttpDelete("{id:guid}/include/{petId:guid}")]
    public async Task<ActionResult> DeletePetAsync(
        [FromRoute] Guid id,
        [FromRoute] Guid petId,
        CancellationToken cancellationToken)
    {
        var command = new DeletePetCommand(petId, id);

        var result = await _mediator.Send(command, cancellationToken);

        return result.ToResponseOkOrError();
    }

    [HttpPatch("{id:guid}/include/{petId:guid}/photo")]
    public async Task<ActionResult<string>> MakeMainPetPhotoAsync(
        [FromRoute] Guid id,
        [FromRoute] Guid petId,
        [FromBody] MakeMainPhotoPetRequest request,
        CancellationToken cancellationToken)
    {
        var command = request.ToCommand(id, petId);

        var result = await _mediator.Send(command, cancellationToken);

        return result.ToResponseErrorOrResult();
    }

    [HttpPatch("{id:guid}/include/{petId:guid}/change-status")]
    public async Task<ActionResult<string>> ChangeStatusPetAsync(
        [FromRoute] Guid id,
        [FromRoute] Guid petId,
        [FromBody] ChangeStatusPetRequest request,
        CancellationToken cancellationToken)
    {
        var command = request.ToCommand(id, petId);

        var result = await _mediator.Send(command, cancellationToken);

        return result.ToResponseOkOrError();
    }
}