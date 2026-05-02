namespace PetFamily.Core.Application.UseCases.Comands.VolunteerComands.DeleteVolunteer;

public record DeleteVolunteerCommand(Guid VolunteerId)
    : IReq