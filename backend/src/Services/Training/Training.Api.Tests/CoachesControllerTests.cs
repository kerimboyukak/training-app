using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Training.Api.Controllers;
using Training.Api.Models;
using Training.AppLogic;
using Training.Domain;


namespace Training.Api.Tests
{
    public class CoachesControllerTests
    {
        private Mock<ICoachRepository> _coachRepositoryMock = null!;
        private Mock<IMapper> _mapperMock = null!;
        private CoachesController _controller = null!;

        [SetUp]
        public void Setup()
        {
            _coachRepositoryMock = new Mock<ICoachRepository>();
            _mapperMock = new Mock<IMapper>();

            _controller = new CoachesController(
                _coachRepositoryMock.Object,
                _mapperMock.Object);
        }
        [Test]
        public void GetByNumber_CoachExists_ShouldReturnCoachDetails()
        {
            // Arrange
            var coach = Coach.CreateNew("123", "John", "Doe");
            var coachDetailModel = new CoachDetailModel
            {
                Id = "123",
                FirstName = "John",
                LastName = "Doe"
            };

            _coachRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<string>())).ReturnsAsync(coach);
            _mapperMock.Setup(mapper => mapper.Map<CoachDetailModel>(It.IsAny<Coach>())).Returns(coachDetailModel);

            // Act
            var result = _controller.GetByNumber("123").Result as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            _coachRepositoryMock.Verify(repo => repo.GetByIdAsync("123"), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map<CoachDetailModel>(coach), Times.Once);

            var model = result!.Value as CoachDetailModel;
            Assert.That(model, Is.Not.Null);
            Assert.That(model!.Id, Is.EqualTo("123"));
            Assert.That(model.FirstName, Is.EqualTo("John"));
            Assert.That(model.LastName, Is.EqualTo("Doe"));
        }

        [Test]
        public void GetByNumber_CoachDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            _coachRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<string>())).ReturnsAsync((Coach?)null);

            // Act
            var result = _controller.GetByNumber("123").Result as NotFoundResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            _coachRepositoryMock.Verify(repo => repo.GetByIdAsync("123"), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map<CoachDetailModel>(It.IsAny<Coach>()), Times.Never);
        }

    }
}
