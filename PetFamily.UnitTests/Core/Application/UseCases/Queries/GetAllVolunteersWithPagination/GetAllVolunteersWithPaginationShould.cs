using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using NSubstitute;
using PetFamily.Core.Application.UseCases.CommonDto;
using PetFamily.Core.Application.UseCases.ComonDto;
using PetFamily.Core.Application.UseCases.Queries.GetAllVolunteersWithPagination;
using PetFamily.Core.Ports.DataBaseForRead;
using Serilog;
using Xunit;

namespace PetFamily.UnitTests.Core.Application.UseCases.Queries.GetAllVolunteersWithPagination
{
    public class GetAllVolunteersWithPaginationShould
    {
        private readonly IReadRepository _readRepository = Substitute.For<IReadRepository>();
        private readonly ILogger _logger = Substitute.For<ILogger>();

        private readonly IValidator<GetAllVolunteersWithPaginationQuery> _validator =
            Substitute.For<IValidator<GetAllVolunteersWithPaginationQuery>>();

        [Fact]
        public async Task BeGetVolunteersReturnListVolunteersAndSuccess()
        {
            //arrange
            var volunteers = ExistedVolunteers();
            var pageNumber = 1;
            var pageSize = 2;
            _readRepository.
                GetAllVolunteersWithPaginationAsync(
                    Arg.Any<int>(), Arg.Any<int>(), CancellationToken.None)
                .Returns(volunteers);
            _validator.ValidateAsync(Arg.Any<GetAllVolunteersWithPaginationQuery>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(new ValidationResult()));

            var query = new GetAllVolunteersWithPaginationQuery(pageNumber, pageSize);
            var handler = new GetAllVolunteersWithPaginationHandler(_readRepository, _logger, _validator);

            //act
            var result = await handler.Handle(query, CancellationToken.None);

            //assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Volunteers.Count().Should().Be(3);
            await _readRepository.
                Received(1).
                GetAllVolunteersWithPaginationAsync(
                Arg.Any<int>(), Arg.Any<int>(), CancellationToken.None);
            await _validator.Received(1)
                .ValidateAsync(
                    Arg.Any<GetAllVolunteersWithPaginationQuery>(),
                    Arg.Any<CancellationToken>());
        }

        private List<VolunteerDto> ExistedVolunteers()
        {
            var volunteers = new List<VolunteerDto>
            {
                new(
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
                    }
                ),

                new(
                    Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    "Анна",
                    "Викторовна",
                    "Смирнова",
                    "anna.smirnova@example.com",
                    "Занимаюсь передержкой животных и поиском новых хозяев",
                    "5 лет работы с бездомными животными",
                    "+79997654321",
                    new[]
                    {
                        new HelpRequisiteDto(
                            "Тинькофф",
                            "Анна Смирнова"
                        )
                    },
                    new[]
                    {
                        new SocialNetworkDto(
                            "VK",
                            "vk.com/anna_smirnova"
                        )
                    }
                ),
                new(
                Guid.Parse("33333333-3333-3333-2222-222222222222"),
                "Анна",
                "Викторовна",
                "Смирнова",
                "anna.smirnova@example.com",
                "Занимаюсь передержкой животных и поиском новых хозяев",
                "5 лет работы с бездомными животными",
                "+79997654321",
                new[]
                {
                    new HelpRequisiteDto(
                        "Тинькофф",
                        "Анна Смирнова"
                    )
                },
                new[]
                {
                    new SocialNetworkDto(
                        "VK",
                        "vk.com/anna_smirnova"
                    )
                }
                )
            };
            return volunteers;
        }
    }
}