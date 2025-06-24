using AutoMapper;
using ELearningPlatform.Model.Categories.Dtos.Request;
using ELearningPlatform.Model.Categories.Dtos.Response;
using ELearningPlatform.Model.Categories.Entity;
using ELearningPlatform.Repository.Categories.Abstracts;
using ELearningPlatform.Repository.UnitOfWorks.Abstracts;
using ELearningPlatform.Service.Categories.Concretes;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;

namespace Service.Test;

[TestFixture]
public class CategoryServiceTests
{
    private Mock<ICategoryRepository> _categoryRepositoryMock;
    private Mock<IMapper> _mapperMock;
    private Mock<IUnitOfWork> _unitOfWorkMock;
    private Mock<ILogger<CategoryService>> _loggerMock;
    private CategoryService _categoryService;

    [SetUp]
    public void Setup()
    {
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _mapperMock = new Mock<IMapper>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<CategoryService>>();

        _categoryService = new CategoryService(
            _categoryRepositoryMock.Object,
            _mapperMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }
    [Test]
    public async Task CreateAsync_WhenCategoryNameExists_ShouldReturnFail()
    {
        // Arrange
        var request = new CreateCategoryRequest("TestCategory");

        _categoryRepositoryMock.Setup(repo => repo.Where(It.IsAny<System.Linq.Expressions.Expression<System.Func<Category, bool>>>()))
            .Returns(new List<Category> { new Category { Name = "TestCategory" } }.AsQueryable());

        // Act
        var result = await _categoryService.CreateAsync(request);

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual(HttpStatusCode.BadRequest, result.Status);
        Assert.AreEqual("Aynı isimde bir kategori var.", result.Message);
    }

    [Test]
    public async Task CreateAsync_WhenCategoryNameNotExists_ShouldCreateCategory()
    {
        // Arrange
        var request = new CreateCategoryRequest("NewCategory");
        var categoryEntity = new Category { Name = request.Name };
        var createdResponse = new CreateCategoryResponse(1, request.Name, DateTime.Now);

        _categoryRepositoryMock.Setup(repo => repo.Where(It.IsAny<System.Linq.Expressions.Expression<System.Func<Category, bool>>>()))
            .Returns(Enumerable.Empty<Category>().AsQueryable());

        _mapperMock.Setup(m => m.Map<Category>(request))
            .Returns(categoryEntity);

        _mapperMock.Setup(m => m.Map<CreateCategoryResponse>(categoryEntity))
            .Returns(createdResponse);

        _categoryRepositoryMock.Setup(repo => repo.AddAsync(categoryEntity))
            .Returns(new ValueTask(Task.CompletedTask));

        _unitOfWorkMock.Setup(uow => uow.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _categoryService.CreateAsync(request);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(HttpStatusCode.Created, result.Status);
        Assert.IsNotNull(result.Data);
        Assert.AreEqual(createdResponse.Id, result.Data.Id);
        Assert.AreEqual(createdResponse.Name, result.Data.Name);

        _categoryRepositoryMock.Verify(r => r.AddAsync(categoryEntity), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
}