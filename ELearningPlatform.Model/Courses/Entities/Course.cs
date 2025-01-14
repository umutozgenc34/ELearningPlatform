using AutoMapper.Features;
using Core.Models;
using ELearningPlatform.Model.Categories.Entity;
using ELearningPlatform.Model.Lessons.Entity;

namespace ELearningPlatform.Model.Courses.Entities;

public sealed class Course : BaseEntity<Guid>
{
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }

    public string? UserId { get; set; } 
    public int CategoryId { get; set; }
    public Category Category { get; set; } = default!;
    public CourseDetails CourseDetails { get; set; } = default!;


    public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
}
