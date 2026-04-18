using CSharpFunctionalExtensions;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using NSubstitute;
using PetFamily.Core.Application.UseCases.Comands.VolunteerComands.AddPet;
using PetFamily.Core.Application.UseCases.Commands.VolunteerCommands.AddPet;
using PetFamily.Core.Application.UseCases.CommonDto;
using PetFamily.Core.Domain.Models.SharedKernel.VO;
using PetFamily.Core.Domain.Models.VolunteerAggregate;
using PetFamily.Core.Domain.Models.VolunteerAggregate.VO;
using PetFamily.Core.Ports;
using Serilog;
using Xunit;

namespace PetFamily.UnitTests.Core.Application.UseCases.Commnands.VolunteerCommands.AddPet
{
    public class AddPetHandlerShould
    {
        private readonly ILogger _logger = Substitute.For<ILogger>();
        private readonly IVolunteerRepository _volunteerRepository = Substitute.For<IVolunteerRepository>();
        private readonly IValidator<AddPetCommand> _validator = Substitute.For<IValidator<AddPetCommand>>();

        [Fact]
        public async Task BeAddPetIfValidationFailedReturnErrorList()
        {
            //arrange
            var volunteer = ExistedVolunteer().Value;
            var pet = ExistedPetDto().Value;

            _volunteerRepository
                .GetByIdAsync(Arg.Any<VolunteerId>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<Volunteer>(volunteer));

            _validator.ValidateAsync(Arg.Any<AddPetCommand>(), Arg.Any<CancellationToken>())
                             .Returns(Task.FromResult(
                                       new ValidationResult(new[]
                                       {
                                         new ValidationFailure("Name", "Name is required")
                                       })));

            //act
            var command = new AddPetCommand((Guid)volunteer.Id, pet);
            var handler = new AddPetHandler(_logger, _volunteerRepository, _validator);
            var result = await handler.Handle(command, new CancellationToken());

            //assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().NotBeNull();
        }

        [Fact]
        public async Task BeAddPetIfValidationSucessReturnSucessAndGuidPet()
        {
            //arrange
            var volunteer = ExistedVolunteer().Value;
            var pet = ExistedPetDto().Value;

            _volunteerRepository
                .GetByIdAsync(Arg.Any<VolunteerId>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<Volunteer>(volunteer));

            _validator.ValidateAsync(Arg.Any<AddPetCommand>(), Arg.Any<CancellationToken>())
                             .Returns(Task.FromResult(new ValidationResult()));

            var command = new AddPetCommand((Guid)volunteer.Id, pet);
            var handler = new AddPetHandler(_logger, _volunteerRepository, _validator);
            ;
            //act

            var result = await handler.Handle(command, new CancellationToken());

            //assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be((Guid)volunteer.Pets.ToList()[0].Id);
            volunteer.Pets.Count.Should().Be(1);
        }

        private Maybe<Volunteer> ExistedVolunteer()
        {
            return Volunteer.Create(
                VolunteerId.NewId(),
                FullName.Create("ivan", "ivanivich", "ivanov").Value,
                Email.Create("diman@mail.ru").Value,
                "description",
                Experience.Create(5).Value,
                PhoneNumber.Create("89258761315").Value,
                HelpRequisites.Create(null),
                SocialNetworks.Create(null)).Value;
        }

        private Maybe<PetDto> ExistedPetDto()
        {
            return new PetDto(
                "test",
                "description",
                new PetSpeciesInfoDto(Guid.NewGuid(), Guid.NewGuid()),
                "test",
                "test",
                new AddressDto("testCity", "testRegion", "testHouse"),
                10,
                5,
                "89258761315",
                true,
                DateTime.UtcNow,
                true,
                1);
        }
    }
}