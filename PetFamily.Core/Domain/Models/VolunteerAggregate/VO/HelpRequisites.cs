using PetFamily.Core.Domain.Models.SharedKernel.VO;
using System.Diagnostics.CodeAnalysis;

namespace PetFamily.Core.Domain.Models.VolunteerAggregate.VO
{
    /// <summary>
    /// Value object -список реквизитов для помощи,в основном чтобы хранить в json
    /// </summary>
    public record HelpRequisites
    {
        [ExcludeFromCodeCoverage]
        private HelpRequisites()
        {
            ListHelpRequisites = [];
        }
        private HelpRequisites(IEnumerable<HelpRequisite> list)
        {
            ListHelpRequisites = list.ToList();
        }
        public List<HelpRequisite> ListHelpRequisites { get; }

        public static HelpRequisites Create(IEnumerable<HelpRequisite>? list)
        {
            if (list == null || !list.Any())
                return HelpRequisites.Empty();

            return new HelpRequisites(list);
        }

        private static HelpRequisites Empty() => new();
    }
}