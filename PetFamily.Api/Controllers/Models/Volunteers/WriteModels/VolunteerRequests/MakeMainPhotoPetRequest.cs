using PetFamily.Core.Application.UseCases.Commands.VolunteerCommands.MakeMainPhotoPets;

namespace PetFamily.Api.Controllers.Models.Volunteers.WriteModels.VolunteerRequests;

public record MakeMainPhotoPetRequest(string PathStorage)
{
    public MakeMainPhotoPetCommand ToCommand(Guid volunteerId, Guid petId)
    {
        return new MakeMainPhotoPetCommand(volunteerId, petId, PathStorage);
    }
}