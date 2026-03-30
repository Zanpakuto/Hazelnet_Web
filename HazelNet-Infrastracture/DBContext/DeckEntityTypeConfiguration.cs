using HazelNet_Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HazelNet_Infrastracture.DBContext;

public class DeckEntityTypeConfiguration :IEntityTypeConfiguration<Deck>
{
    public void Configure(EntityTypeBuilder<Deck> builder)
    {
        builder.HasKey(d => d.Id);
        builder.Property(d => d.DeckName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(d => d.DeckDescription)
            .HasMaxLength(250);
        
        builder.HasMany(d => d.Cards)
            .WithOne(c => c.Deck)
            .HasForeignKey(d => d.DeckId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}