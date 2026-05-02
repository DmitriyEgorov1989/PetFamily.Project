namespace PetFamily.Volunteers.Core.Application.UseCases.CommonDto;

public class PetWriteDto
{
    public PetWriteDto()
    {
    }

    public PetWriteDto(string name, string description,
        string color, string healthInfo, AddressDto address,
        decimal weight, int height, string phoneNumber,
        bool isSterilized, DateTime birthDate,
        bool isSVacined, int petHelpStatus, HelpRequisiteDto[] petHelpRequisites)
    {
        Name = name;
        Description = description;
        Color = color;
        HealthInfo = healthInfo;
        Address = address;
        Weight = weight;
        Height = height;
        PhoneNumber = phoneNumber;
        IsSterilized = isSterilized;
        BirthDate = birthDate;
        PetHelpStatus = petHelpStatus;
        PetHelpRequisites = petHelpRequisites;
    }

    public string Name { get; set; }
    public string Description { get; set; }
    public string Color { get; set; }
    public string HealthInfo { get; set; }
    public AddressDto Address { get; set; }
    public decimal Weight { get; set; }
    public int Height { get; set; }
    public string PhoneNumber { get; set; }
    public bool IsSterilized { get; set; }
    public DateTime BirthDate { get; set; }
    public bool IsVaccined { get; set; }
    public int PetHelpStatus { get; set; }
    public HelpRequisiteDto[] PetHelpRequisites { get; set; }
}