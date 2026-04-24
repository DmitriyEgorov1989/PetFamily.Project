using PetFamily.Core.Application.UseCases.Commands.VolunteerCommands.ChangeStatusPet;

namespace PetFamily.Api.Controllers.Models.Volunteers.WriteModels.VolunteerRequests
{
    public record ChangeStatusPetRequest(int HelpStatus)
    {
        public ChangeStatusPetCommand ToCommand(Guid volunteerId, Guid petId)
        {
            return new ChangeStatusPetCommand(volunteerId, petId, HelpStatus);
        }
    }
}
