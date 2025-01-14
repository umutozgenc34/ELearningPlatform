using ELearningPlatform.Model.Lessons.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ELearningPlatform.Repository.Lessons.Configurations;

public class LessonConfiguration : IEntityTypeConfiguration<Lesson>
{
    public void Configure(EntityTypeBuilder<Lesson> builder)
    {
        builder.HasKey(x => x.Id);

        // Foreign Key (CourseId)
        builder.Property(x => x.CourseId)
            .IsRequired();

        // Title sütunu
        builder.Property(x => x.Title)
            .HasMaxLength(200)
            .IsRequired(false); 

        // VideoUrl sütunu
        builder.Property(x => x.VideoUrl)
            .IsRequired(); 

        // LessonOrder sütunu
        builder.Property(x => x.LessonOrder)
            .IsRequired(); 

        // Created sütunu
        builder.Property(x => x.Created)
            .IsRequired(); 

        // Updated sütunu
        builder.Property(x => x.Updated)
            .IsRequired();


        builder.HasOne(x => x.Course) 
            .WithMany(c => c.Lessons)  
            .HasForeignKey(x => x.CourseId)  
            .OnDelete(DeleteBehavior.Cascade);
    }
}

