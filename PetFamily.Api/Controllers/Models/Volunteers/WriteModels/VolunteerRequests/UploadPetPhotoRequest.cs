namespace PetFamily.Api.Controllers.Models.Volunteers.WriteModels.VolunteerRequests
{
    public record UploadPetPhotoRequest(List<IFormFile> FormFiles);
}
