namespace PetFamily.Volunteers.Contracts.Controllers.Models.Volunteers.WriteModels.VolunteerRequests;

public record UploadPetPhotoRequest(List<IFormFile> FormFiles);