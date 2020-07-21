using Colegio.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Colegio.Data.Configuarations
{
    public class Col_InsumoLaboralConfiguration : IEntityTypeConfiguration<Col_InsumoLaboral>
    {
        public void Configure(EntityTypeBuilder<Col_InsumoLaboral> builder)
        {
            builder.HasKey(e => e.InsLabId);
            builder.Property(p => p.Nombre).HasMaxLength(50).IsRequired(false);

            builder.HasOne<Col_Laborales>()
                .WithMany()
                .HasForeignKey(f => f.LaboralId)
                .IsRequired();

            builder.ToTable("Col_InsumoLaboral");
        }
    }
}
