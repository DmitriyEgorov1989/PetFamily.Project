using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using NSubstitute;
using PetFamily.Core.Application.UseCases.Comands.VolunteerComands.ComonDto;
using PetFamily.Core.Application.UseCases.Commands.VolunteerCommands.CreateVolunteer;
using PetFamily.Core.Domain.Models.VolunteerAggregate;
using PetFamily.Core.Ports;
using Serilog;
using Xunit;

namespace PetFamily.UnitTests.Core.Application.UseCases.Commands.VolunteerCommands.CreateVolunteer
{
    public class CreateVolunteerHandlerShould
    {
        private readonly ILogger _logger =
            Substitute.For<ILogger>();
        private readonly IVolunteerRepository _volunteerRepository =
            Substitute.For<IVolunteerRepository>();
        private readonly IValidator<CreateVolunteerCommand> _validator =
            Substitute.For<IValidator<CreateVolunteerCommand>>();

        [Fact]
        public async Task BeCreateVolunteerReturnGuidVolunteerAndSucess()
        {
            //arrange
            var volunteerId = Guid.NewGuid();
            _volunteerRepository
                .AddAsync(Arg.Any<Volunteer>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(volunteerId));

            _validator.ValidateAsync(Arg.Any<CreateVolunteerCommand>(), Arg.Any<CancellationToken>())
                             .Returns(Task.FromResult(new ValidationResult()));

            var command = new CreateVolunteerCommand(
                new FullNameDto("ivan", "ivanivich", "ivanov"),
                "diman@mail.ru",
                "description",
                5,
                "89258761315",
                [new SocialNetworkDto("test", "test")],
                [new HelpRequisiteDto("test", "test")]);
            var handler = new CreateVolunteerHandler(_volunteerRepository, _logger, _validator);

            //act
            var result = await handler.Handle(command, new CancellationToken());

            //assert
            result.IsSuccess.Should().BeTrue();
            //Что запустилась 1 раз
            await _validator.Received(1)
            .ValidateAsync(Arg.Any<CreateVolunteerCommand>(), Arg.Any<CancellationToken>());
            //Проверяет что ушли правильные значения
            await _volunteerRepository.Received(1).AddAsync(
                 Arg.Is((Volunteer v) =>
                 v.Email.Address == "diman@mail.ru" &&
                 v.Description == "description" &&
                v.Experience.Year == 5),
            Arg.Any<CancellationToken>());
        }
    }
}
