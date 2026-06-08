using CSharpFunctionalExtensions;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using NSubstitute;
using PetFamily.SharedKernel.Errors;
using PetFamily.Volunteers.Core.Application.UseCases.CommonDto;
using PetFamily.Volunteers.Core.Application.UseCases.Queries.GetAllPetsByVolunteerId;
using PetFamily.Volunteers.Core.Ports.DataBaseForRead;
using Serilog;
using Xunit;

namespace PetFamily.UnitTests.Core.Application.UseCases.Queries.GetAllPetsByVolunteerId;

public class GetAllPetsByVolunteerIdShould
{
    private readonly ILogger _logger = Substitute.For<ILogger>();
    private readonly IPetsReadRepository _petsReadRepository = Substitute.For<IPetsReadRepository>();

    private readonly IValidator<GetAllPetsByVolunteerIdQuery> _validator =
        Substitute.For<IValidator<GetAllPetsByVolunteerIdQuery>>();

    [Fact]
    public async Task BeGetAllPetsByVolunteerIdReturnPetsAndSuccess()
    {
        //arrange
        var volunteerId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var pets = ExistedPets(volunteerId);

        _petsReadRepository.GetAllByVolunteerIdAsync(
                volunteerId, CancellationToken.None)
            .Returns(Task.FromResult(Result.Success<List<PetDto>, Error>(pets)));
        _validator.ValidateAsync(Arg.Any<GetAllPetsByVolunteerIdQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new ValidationResult()));

        var query = new GetAllPetsByVolunteerIdQuery(volunteerId);
        var handler = new GetAllPetsByVolunteerIdHandler(_logger, _validator, _petsReadRepository);

        //act
        var result = await handler.Handle(query, CancellationToken.None);

        //assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Pets.Should().HaveCount(2);
        result.Value.Pets.Should().OnlyContain(p => p.VolunteerId == volunteerId);
        await _petsReadRepository.Received(1).GetAllByVolunteerIdAsync(
            volunteerId, CancellationToken.None);
        await _validator.Received(1)
            .ValidateAsync(
                Arg.Any<GetAllPetsByVolunteerIdQuery>(),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public void BeGetAllPetsByVolunteerIdValidatorReturnErrorWhenVolunteerIdIsEmpty()
    {
        //arrange
        var validator = new GetAllPetsByVolunteerIdValidator();
        var query = new GetAllPetsByVolunteerIdQuery(Guid.Empty);

        //act
        var result = validator.Validate(query);

        //assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(GetAllPetsByVolunteerIdQuery.VolunteerId));
    }

    [Fact]
    public void BeGetAllPetsByVolunteerIdValidatorReturnSuccessWhenVolunteerIdIsValid()
    {
        //arrange
        var validator = new GetAllPetsByVolunteerIdValidator();
        var query = new GetAllPetsByVolunteerIdQuery(Guid.NewGuid());

        //act
        var result = validator.Validate(query);

        //assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    private static List<PetDto> ExistedPets(Guid volunteerId)
    {
        return
        [
            new PetDto
            {
                PetId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Name = "Barsik",
                Description = "Calm cat",
                SpeciesId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                BreedId = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                VolunteerId = volunteerId,
                Color = "Black",
                HealthInfo = "Healthy",
                City = "Moscow",
                Region = "Moscow",
                House = "1",
                Weight = 4,
                Height = 25,
                PhoneNumber = "+79991234567",
                BirthDate = DateTime.UtcNow.AddYears(-2),
                CreatedUtc = DateTime.UtcNow
            },
            new PetDto
            {
                PetId = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                Name = "Sharik",
                Description = "Friendly dog",
                SpeciesId = Guid.Parse("66666666-6666-6666-6666-666666666666"),
                BreedId = Guid.Parse("77777777-7777-7777-7777-777777777777"),
                VolunteerId = volunteerId,
                Color = "White",
                HealthInfo = "Healthy",
                City = "Moscow",
                Region = "Moscow",
                House = "2",
                Weight = 12,
                Height = 45,
                PhoneNumber = "+79991234567",
                BirthDate = DateTime.UtcNow.AddYears(-3),
                CreatedUtc = DateTime.UtcNow
            }
        ];
    }
}
