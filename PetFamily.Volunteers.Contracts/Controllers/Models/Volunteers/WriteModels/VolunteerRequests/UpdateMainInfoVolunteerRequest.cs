namespace PetFamily.Volunteers.Contracts.Controllers.Models.Volunteers.WriteModels.VolunteerRequests;

public record UpdateMainInfoVolunteerRequest(
    string FirstName,
    string MiddleName,
    string LastName,
    string Email,
    string Description,
    int Experience,
    string PhoneNumber);