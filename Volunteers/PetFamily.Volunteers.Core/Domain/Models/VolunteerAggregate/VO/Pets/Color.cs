using CSharpFunctionalExtensions;
using PetFamily.SharedKernel.Errors;

namespace PetFamily.Volunteers.Core.Domain.Models.VolunteerAggregate.VO.Pets;

public record Color
{
    private Color(string colorName)
    {
        Name = colorName;
    }

    public string Name { get; set; }

    public static Result<Color, Error> Create(string colorName)
    {
        if (string.IsNullOrWhiteSpace(colorName))
            return GeneralErrors.ValueIsInvalid(nameof(colorName));
        return new Color(colorName);
    }
}