using System.Diagnostics.CodeAnalysis;

namespace PetFamily.Core.Domain.Models.VolunteerAggregate.VO
{
    /// <summary>
    /// Value object -список социальных сетей,в основном чтобы хранить в json
    /// </summary
    public record SocialNetworks
    {
        [ExcludeFromCodeCoverage]
        private SocialNetworks()
        {
            ListSocialNetworks = [];
        }
        private SocialNetworks(IEnumerable<SocialNetwork> list)
        {
            ListSocialNetworks = list.ToList();
        }
        public List<SocialNetwork> ListSocialNetworks { get; }

        public static SocialNetworks Create(IEnumerable<SocialNetwork>? list)
        {
            if (list == null)
                return SocialNetworks.Empty();

            return new SocialNetworks(list);
        }
        private static SocialNetworks Empty() => new();
    }
}