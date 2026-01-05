using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructure.EntitiesConfiguration
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.HasKey(p => p.PaymentId);

            builder.Property(p => p.PaymentId)
                   .ValueGeneratedOnAdd();

            builder.Property(p => p.Amount)
                   .IsRequired()
                   .HasPrecision(18, 2);

            builder.Property(p => p.PaymentDate)
                   .IsRequired();

            builder.Property(p => p.PaymentStatus)
                   .IsRequired();

            builder.Property(p => p.TransactionId)
                   .HasMaxLength(100);

            builder.Property(p => p.QrCodeUri)
                   .HasMaxLength(500);
        }
    }
}
