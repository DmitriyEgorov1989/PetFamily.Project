using CSharpFunctionalExtensions;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using NSubstitute;
using PetFamily.Core.Application.UseCases.CommonDto;
using PetFamily.Core.Application.UseCases.Queries.GetVolunteerById;
using PetFamily.Core.Ports.DataBaseForRead;
using PetFamily.SharedKernel.Errors;
using Serilog;
using Xunit;

namespace PetFamily.UnitTests.Core.Application.UseCases.Queries.GetVolunteerById;

public class GetVolunteerByIdShould
{
    private readonly ILogger _logger = Substitute.For<ILogger>();
    private readonly IReadVollunteersRepository _readRepository = Substitute.For<IReadVollunteersRepository>();

    private readonly IValidator<GetVolunteerByIdQuery> _validator =
        Substitute.For<IValidator<GetVolunteerByIdQuery>>();

    [Fact]
    public async Task BeGetVolunteerByIdReturnVolunteerDtoAndSuccess()
    {
        //arrange
        var volunteer = ExistedVolunteer();

        _readRepository.GetVolunteerById(
                volunteer.VolunteerId, CancellationToken.None)
            .Returns(Task.FromResult(Result.Success<Maybe<VolunteerDto>, Error>(volunteer)));
        _validator.ValidateAsync(Arg.Any<GetVolunteerByIdQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new ValidationResult()));

        var query = new GetVolunteerByIdQuery(volunteer.VolunteerId);
        var handler = new GetVolunteerByIdHandler(_readRepository, _logger, _validator);

        //act
        var result = await handler.Handle(query, CancellationToken.None);

        //assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Volunteer.VolunteerId.Should().Be(volunteer.VolunteerId);
        await _readRepository.Received(1).GetVolunteerById(
            volunteer.VolunteerId, CancellationToken.None);
        await _validator.Received(1)
            .ValidateAsync(
                Arg.Any<GetVolunteerByIdQuery>(),
                Arg.Any<CancellationToken>());
    }

    private VolunteerDto ExistedVolunteer()
    {
        return
            new VolunteerDto(
                Guid.Parse("11111111-1111-1111-1111-111111111111"),
                "Иван",
                "Сергеевич",
                "Петров",
                "ivan.petrov@example.com",
                "Помогаю приютам с выгулом собак и организацией мероприятий",
                "3 года волонтёрства в приютах",
                "+79991234567",
                new[]
                {
                    new HelpRequisiteDto(
                        "Сбербанк",
                        "Иван Петров")
                },
                new[]
                {
                    new SocialNetworkDto(
                        "Telegram",
                        "@ivan_petrov"
                    ),
                    new SocialNetworkDto(
                        "Instagram",
                        "@petrov_life"
                    )
                });
    }
}