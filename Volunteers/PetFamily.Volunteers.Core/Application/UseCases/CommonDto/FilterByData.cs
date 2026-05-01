namespace PetFamily.Core.Application.UseCases.CommonDto;

public record FilterByData(
    string Name,
    Guid? SpeciesId,
    Guid? BreedId,
    string Color,
    Guid? VolunteerId,
    AddressDto Address);