using ELearningPlatform.Model.Courses.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ELearningPlatform.Repository.Courses.Configuration;

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Description)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(c => c.Price)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        
        //relations conf
        builder.HasOne(c => c.Category)
            .WithMany()
            .HasForeignKey(c => c.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(c => c.ImageUrl)
            .HasMaxLength(200);

        
        //course details conf

        builder.OwnsOne(c => c.CourseDetails, cd =>
        {
            cd.Property(cd => cd.Rating)
                .HasDefaultValue(1);

            cd.Property(cd => cd.Duration)
                .HasDefaultValue(0);

            cd.Property(cd => cd.EducatorFullName)
                .HasDefaultValue("Bilinmeyen Eğitmen")
                .HasMaxLength(100);

        });

        builder.HasMany(c => c.Lessons) 
            .WithOne()
            .HasForeignKey(l => l.CourseId) 
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(c => c.CourseDetails).AutoInclude();
        builder.Navigation(c=> c.Category).AutoInclude();
            
    }
}

