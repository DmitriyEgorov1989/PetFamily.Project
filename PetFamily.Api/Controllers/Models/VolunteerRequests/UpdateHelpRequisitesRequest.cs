namespace PetFamily.Api.Controllers.Models.VolunteerRequests
{
    public record UpdateHelpRequisitesRequest(
        Guid VolunteerId, List<HelpRequisiteDto> HelpRequisites)
    {
    }
}
