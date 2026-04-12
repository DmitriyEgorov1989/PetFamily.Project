using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFamily.Core.Domain.Models.SharedKernel.VO;
using PetFamily.Core.Domain.Models.VolunteerAggregate;
using PetFamily.Core.Domain.Models.VolunteerAggregate.VO;

namespace PetFamily.Infrastructure.Adapters.Postgres.Configurations
{
    public class VolunteersConfiguration : IEntityTypeConfiguration<Volunteer>
    {
        public void Configure(EntityTypeBuilder<Volunteer> builder)
        {
            builder.ToTable("volunteers");

            builder.HasKey(x => x.Id);

            builder.Property(v => v.Id)
                    .HasColumnName("volunteer_id")
                    .ValueGeneratedNever()
                   .HasConversion(id => id.Id,
                                  value => VolunteerId.Create(value).Value)
                   .IsRequired();

            builder.ComplexProperty(v => v.FullName, fn =>
            {
                fn.IsRequired();
                fn.Property(fn => fn.FirstName)
                  .HasMaxLength(FullName.MAX_LENGTH_FULLNAME)
                  .HasColumnName("first_name")
                  .IsRequired();

                fn.Property(fn => fn.MiddleName)
                  .HasMaxLength(FullName.MAX_LENGTH_FULLNAME)
                  .HasColumnName("middle_name")
                  .IsRequired();

                fn.Property(fn => fn.LastName)
                   .HasMaxLength(FullName.MAX_LENGTH_FULLNAME)
                   .HasColumnName("last_name")
                   .IsRequired();
            });

            builder.Property(v => v.Email)
                   .HasConversion(e => e.Address,
                   value => Email.Create(value).Value)
                   .IsRequired();

            builder.Property(v => v.Description)
                   .IsRequired();

            builder.Property(v => v.Experience)
                    .HasConversion(e => e.Year,
                    value => Experience.Create(value).Value);

            builder.Property(v => v.PhoneNumber)
                    .HasColumnName("phone_number")
                    .HasConversion(e => e.Value,
                    value => PhoneNumber.Create(value).Value);

            builder.OwnsOne(v => v.SocialNetworks, sb =>
            {
                sb.ToJson();

                sb.OwnsMany(s => s.ListSocialNetworks, lb =>
                {
                    lb.Property(sn => sn.Name)
                    .HasColumnName("name_social_network")
                    .IsRequired();

                    lb.Property(sn => sn.Link)
                    .HasColumnName("link_social_network")
                    .IsRequired();
                });
            });

            builder.OwnsOne(v => v.HelpRequisites, hb =>
            {
                hb.ToJson();

                hb.OwnsMany(h => h.ListHelpRequisites, hb =>
                {
                    hb.Property(h => h.Name)
                       .HasColumnName("name_help_requisite")
                      .HasMaxLength(HelpRequisite.MAX_LENGTH_NAME)
                      .IsRequired();

                    hb.Property(h => h.Description)
                       .HasColumnName("description_help_requisite")
                       .HasMaxLength(HelpRequisite.MAX_LENGTH_DESCRIPTION)
                       .IsRequired();
                });
            });

            builder.Property(v => v.IsDelete);
            builder.Property(v => v.DateDelete);
        }
    }
}