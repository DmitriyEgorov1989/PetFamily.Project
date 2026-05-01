namespace PetFamily.Core.Application.UseCases.CommonDto;

public class PetDto
{
    public Guid PetId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid SpeciesId { get; set; }
    public Guid BreedId { get; set; }
    public Guid VolunteerId { get; set; }
    public string Color { get; set; } = string.Empty;
    public string HealthInfo { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string House { get; set; } = string.Empty;
    public decimal Weight { get; set; }
    public int Height { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public bool IsSterilized { get; set; }
    public DateTime BirthDate { get; set; }
    public bool IsVaccined { get; set; }
    public int PetHelpStatus { get; set; }
    public HelpRequisiteDto[] PetHelpRequisites { get; set; } = [];
    public PetPhotoDto[] PetPhotos { get; set; } = [];
    public DateTime CreatedUtc { get; set; }
}