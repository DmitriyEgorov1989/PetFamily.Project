namespace PetFamily.Api.Controllers.Models.Volunteers.WriteModels.VolunteerRequests
{
    public record UpdateHelpRequisitesRequest(
        Guid VolunteerId, List<HelpRequisiteDto> HelpRequisites)
    {
    }
}
