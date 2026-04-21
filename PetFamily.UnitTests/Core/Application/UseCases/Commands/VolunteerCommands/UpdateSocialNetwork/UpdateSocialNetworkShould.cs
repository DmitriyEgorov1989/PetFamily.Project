using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using NSubstitute;
using PetFamily.Core.Application.UseCases.Comands.Volunteer.UpdateMainInfo;
using PetFamily.Core.Application.UseCases.CommonDto;
using PetFamily.Core.Domain.Models.SharedKernel.VO;
using PetFamily.Core.Domain.Models.VolunteerAggregate;
using PetFamily.Core.Domain.Models.VolunteerAggregate.VO;
using PetFamily.Core.Ports;
using Serilog;
using Xunit;

namespace PetFamily.UnitTests.Core.Application.UseCases.Commands.VolunteerCommands.UpdateSocialNetwork;

public class UpdateSocialNetworkShould
{
    private readonly ILogger _logger = Substitute.For<ILogger>();

    private readonly IUnitOfWork _unitOfWork =
        Substitute.For<IUnitOfWork>();

    private readonly IValidator<UpdateMainInfoVolunteerCommand> _validator =
        Substitute.For<IValidator<UpdateMainInfoVolunteerCommand>>();

    private readonly IVolunteerRepository _volunteerRepository =
        Substitute.For<IVolunteerRepository>();

    [Fact]
    public async Task BeUpdateMainInfoReturnIsSuccess()
    {
        //arrange
        var volunteer = ExistedVolunteer();
        var newFirstName = "newName";
        var newMiddleName = "newMiddleName";
        var newLastName = "newLastName";
        var newEmail = "newEmail@mail.ru";
        var newDescription = "newDescription";
        var newExperience = 3;
        var newPhoneNumber = "89280951317";
        var newMainInfoDto =
            new UpdateMainInfoVolunteerDto(
                new FullNameDto(newFirstName, newMiddleName, newLastName),
                newEmail,
                newDescription,
                newExperience,
                newPhoneNumber);


        _volunteerRepository
            .GetByIdAsync(volunteer.Id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(volunteer));

        _validator.ValidateAsync(Arg.Any<UpdateMainInfoVolunteerCommand>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new ValidationResult()));

        var command =
            new UpdateMainInfoVolunteerCommand((Guid)volunteer.Id, newMainInfoDto);
        var handler =
            new UpdateMainInfoVolunteerHandler(_volunteerRepository, _logger, _validator, _unitOfWork);

        //act
        var result = await handler.Handle(command, CancellationToken.None);

        //assert
        result.IsSuccess.Should().BeTrue();
        volunteer.FullName.FirstName
            .Should().Be(newFirstName);
        volunteer.FullName.MiddleName
            .Should().Be(newMiddleName);
        volunteer.FullName.LastName
            .Should().Be(newLastName);
        volunteer.Email.Address
            .Should().Be(newEmail);
        volunteer.Description
            .Should().Be(newDescription);
        volunteer.Experience.Year
            .Should().Be(newExperience);
        volunteer.PhoneNumber.Value
            .Should().Be(newPhoneNumber);
        await _validator.Received(1).ValidateAsync(Arg.Any<UpdateMainInfoVolunteerCommand>(),
            Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData(null, "new", "", "new@mail.ru",
        "newDescription", 3, null)]
    public async Task BeUpdateMainInfoPartialReturnIsSuccess(
        string newFirstName, string newMiddleName, string newLastName,
        string newEmail, string newDescription, int newExperience, string newPhoneNumber)
    {
        //arrange
        var volunteer = ExistedVolunteer();
        var newMainInfoDto =
            new UpdateMainInfoVolunteerDto(
                new FullNameDto(newFirstName, newMiddleName, newLastName),
                newEmail,
                newDescription,
                newExperience,
                newPhoneNumber);


        _volunteerRepository
            .GetByIdAsync(volunteer.Id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(volunteer));

        _validator.ValidateAsync(Arg.Any<UpdateMainInfoVolunteerCommand>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new ValidationResult()));

        var command =
            new UpdateMainInfoVolunteerCommand((Guid)volunteer.Id, newMainInfoDto);
        var handler =
            new UpdateMainInfoVolunteerHandler(_volunteerRepository, _logger, _validator, _unitOfWork);

        //act
        var result = await handler.Handle(command, CancellationToken.None);

        //assert
        result.IsSuccess.Should().BeTrue();
        volunteer.FullName.FirstName.Should().NotBeNullOrEmpty();
        volunteer.FullName.MiddleName
            .Should().Be(newMiddleName);
        volunteer.FullName.LastName
            .Should().Be("ivanov");
        volunteer.Email.Address
            .Should().Be(newEmail);
        volunteer.Description
            .Should().Be(newDescription);
        volunteer.Experience.Year
            .Should().Be(newExperience);
        await _validator.Received(1).ValidateAsync(Arg.Any<UpdateMainInfoVolunteerCommand>(),
            Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    private Volunteer ExistedVolunteer()
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
}