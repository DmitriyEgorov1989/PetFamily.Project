using PetFamily.Core.Application.UseCases.CommonDto;

namespace PetFamily.Volunteers.Contracts.Controllers.Models.Volunteers.WriteModels.VolunteerRequests;

public record UpdateHelpRequisitesRequest(
    Guid VolunteerId,
    List<HelpRequisiteDto> HelpRequisites);