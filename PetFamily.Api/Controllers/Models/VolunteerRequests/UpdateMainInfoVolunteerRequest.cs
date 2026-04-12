namespace PetFamily.Api.Controllers.Models.VolunteerRequests
{
    public record UpdateMainInfoVolunteerRequest(
      string FirstName,
      string MiddleName,
      string LastName,
      string Email,
      string Description,
      int Experience,
      string PhoneNumber);
}