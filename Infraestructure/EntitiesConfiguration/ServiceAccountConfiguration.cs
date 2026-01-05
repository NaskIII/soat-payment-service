using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infraestructure.EntitiesConfiguration
{
    public class ServiceAccountConfiguration : IEntityTypeConfiguration<ServiceAccounts>
    {
        public void Configure(EntityTypeBuilder<ServiceAccounts> builder)
        {
            var clientNameCOnverter = new ValueConverter<Name, string>(
                clientName => clientName.Value,
                value => new Name(value)
            );

            builder.HasKey(sa => sa.Id);
            builder.Property(sa => sa.Id).ValueGeneratedOnAdd();

            builder.Property(sa => sa.ServiceName)
                .HasConversion(clientNameCOnverter)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(sa => sa.ServiceName).IsUnique();
        }
    }
}
