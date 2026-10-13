using System.Net;
using AutoMapper;
using Core.Infrastructures.CloudinaryServices;
using ELearningPlatform.Model.Categories.Entity;
using ELearningPlatform.Model.Courses.Dtos.Request;
using ELearningPlatform.Model.Courses.Entities;
using ELearningPlatform.Repository.Categories.Abstracts;
using ELearningPlatform.Repository.Courses.Abstracts;
using ELearningPlatform.Repository.UnitOfWorks.Abstracts;
using ELearningPlatform.Service;
using ELearningPlatform.Service.Courses.Concretes;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Service.Test.Helpers;

namespace Service.Test;

[TestFixture]
public class CourseServiceTests
{
    private Mock<ICourseRepository> _courseRepositoryMock = null!;
    private Mock<ICategoryRepository> _categoryRepositoryMock = null!;
    private Mock<IUnitOfWork> _unitOfWorkMock = null!;
    private IMapper _mapper = null!;
    private CourseService _courseService = null!;

    [SetUp]
    public void Setup()
    {
        _courseRepositoryMock = new Mock<ICourseRepository>();
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        // Gerçek AutoMapper profilleriyle çalışıyoruz ki mapping hataları da yakalansın.
        _mapper = new MapperConfiguration(cfg => cfg.AddMaps(typeof(ServiceAssembly).Assembly)).CreateMapper();

        _courseService = new CourseService(_courseRepositoryMock.Object, _categoryRepositoryMock.Object, _mapper,
            _unitOfWorkMock.Object, Mock.Of<ICloudinaryService>(), NullLogger<CourseService>.Instance);
    }

    [Test]
    public void AutoMapperConfiguration_IsValidForCourseUpdate()
    {
        var course = new Course { Id = Guid.NewGuid(), Name = "Eski", Description = "Eski", Price = 1, CategoryId = 1, ImageUrl = "img" };
        _mapper.Map(new UpdateCourseRequest(course.Id, "Yeni", "Yeni açıklama", 50, null, 2), course);

        Assert.That(course.Name, Is.EqualTo("Yeni"));
        Assert.That(course.CategoryId, Is.EqualTo(2));
        Assert.That(course.ImageUrl, Is.EqualTo("img"), "Resim yüklenmediyse eski resim korunmalı");
    }

    [Test]
    public async Task UpdateAsync_WhenOtherCoursesExist_StillUpdates()
    {
        // Eski kod "Id'si farklı başka kurs var mı" diye bakıp hata dönüyordu; yani 2+ kurs varken güncelleme imkansızdı.
        var course = new Course { Id = Guid.NewGuid(), Name = "Eski", Description = "Eski", Price = 10, CategoryId = 1 };
        var otherCourse = new Course { Id = Guid.NewGuid(), Name = "Diğer", Description = "Diğer", Price = 20, CategoryId = 1 };

        _courseRepositoryMock.Setup(x => x.GetAll()).Returns(new[] { course, otherCourse }.AsAsyncQueryable());
        _courseRepositoryMock.Setup(x => x.GetByIdAsync(course.Id)).ReturnsAsync(course);
        _categoryRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(new Category { Id = 1, Name = "Web" });

        var result = await _courseService.UpdateAsync(new UpdateCourseRequest(course.Id, "Yeni", "Yeni açıklama", 99, null, 1));

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(course.Name, Is.EqualTo("Yeni"));
        Assert.That(course.Price, Is.EqualTo(99));
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task UpdateAsync_WithUnknownCategory_ReturnsNotFound()
    {
        var course = new Course { Id = Guid.NewGuid(), Name = "Kurs", Description = "Açıklama", Price = 10, CategoryId = 1 };
        _courseRepositoryMock.Setup(x => x.GetByIdAsync(course.Id)).ReturnsAsync(course);

        var result = await _courseService.UpdateAsync(new UpdateCourseRequest(course.Id, "Kurs", "Açıklama", 10, null, 999));

        Assert.That(result.Status, Is.EqualTo(HttpStatusCode.NotFound));
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    [Test]
    public async Task GetCoursesByCategory_WhenEmpty_ReturnsEmptyList()
    {
        _courseRepositoryMock.Setup(x => x.GetAll()).Returns(Array.Empty<Course>().AsAsyncQueryable());

        var result = await _courseService.GetCoursesByCategoryIdAsync(5);

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Data, Is.Empty);
    }
}
