using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Training.Api.Controllers;
using Training.Api.Models;
using Training.AppLogic;
using Training.Domain;
using Training.Domain.Tests.Builders;

namespace Training.Api.Tests
{
    public class TrainingsControllerTests
    {
        private Mock<ITrainingRepository> _trainingRepositoryMock = null!;
        private Mock<IRoomRepository> _roomRepositoryMock = null!;
        private Mock<ICoachService> _coachServiceMock = null!;
        private Mock<IApprenticeService> _apprenticeServiceMock = null!;
        private Mock<IMapper> _mapperMock = null!;
        private TrainingsController _controller = null!;

        [SetUp]
        public void Setup()
        {
            _trainingRepositoryMock = new Mock<ITrainingRepository>();
            _roomRepositoryMock = new Mock<IRoomRepository>();
            _coachServiceMock = new Mock<ICoachService>();
            _apprenticeServiceMock = new Mock<IApprenticeService>();
            _mapperMock = new Mock<IMapper>();

            _controller = new TrainingsController(
                _trainingRepositoryMock.Object,
                _roomRepositoryMock.Object,
                _apprenticeServiceMock.Object,
                _coachServiceMock.Object,
                _mapperMock.Object);
        }

        [Test]
        public void GetAll_ShouldReturnAllTrainings()
        {
            // Arrange
            var trainings = new List<Domain.Training>
            {
                new TrainingBuilder().Build(),
                new TrainingBuilder().Build()
            };
            _trainingRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(trainings);
            _mapperMock.Setup(mapper => mapper.Map<TrainingDetailModel>(It.IsAny<Domain.Training>())).Returns(new TrainingDetailModel());

            // Act
            var result = _controller.GetAll().Result as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            _trainingRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map<TrainingDetailModel>(It.IsIn<Domain.Training>(trainings)), Times.Exactly(trainings.Count));

            var models = result!.Value as IList<TrainingDetailModel>;
            Assert.That(models, Has.Count.EqualTo(trainings.Count));
        }

        [Test]
        public void GetAllRooms_ShouldReturnAllRooms()
        {
            // Arrange
            var rooms = new List<Room>
            {
                Room.CreateNew("Room1"),
                Room.CreateNew("Room2")
            };
            _roomRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(rooms);

            // Act
            var result = _controller.GetAllRooms().Result as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            _roomRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);

            var models = result!.Value as IList<RoomDetailModel>;
            Assert.That(models, Has.Count.EqualTo(rooms.Count));
        }

        [Test]
        public void GetFutureTrainings_ShouldReturnFutureTrainings()
        {
            // Arrange
            var futureTrainings = new List<Domain.Training>
            {
                new TrainingBuilder().Build(),
                new TrainingBuilder().Build()
            };
            _trainingRepositoryMock.Setup(repo => repo.GetFutureTrainingsAsync()).ReturnsAsync(futureTrainings);
            _mapperMock.Setup(mapper => mapper.Map<TrainingDetailModel>(It.IsAny<Domain.Training>())).Returns(new TrainingDetailModel());

            // Act
            var result = _controller.GetFutureTrainings().Result as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            _trainingRepositoryMock.Verify(repo => repo.GetFutureTrainingsAsync(), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map<TrainingDetailModel>(It.IsIn<Domain.Training>(futureTrainings)), Times.Exactly(futureTrainings.Count));

            var models = result!.Value as IList<TrainingDetailModel>;
            Assert.That(models, Has.Count.EqualTo(futureTrainings.Count));
        }

        [Test]
        public void GetPastTrainings_ShouldReturnPastTrainings()
        {
            // Arrange
            var pastTrainings = new List<Domain.Training>
            {
                new TrainingBuilder().Build(),
                new TrainingBuilder().Build()
            };
            _trainingRepositoryMock.Setup(repo => repo.GetPastTrainingsAsync()).ReturnsAsync(pastTrainings);
            _mapperMock.Setup(mapper => mapper.Map<TrainingDetailModel>(It.IsAny<Domain.Training>())).Returns(new TrainingDetailModel());

            // Act
            var result = _controller.GetPastTrainings().Result as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            _trainingRepositoryMock.Verify(repo => repo.GetPastTrainingsAsync(), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map<TrainingDetailModel>(It.IsIn<Domain.Training>(pastTrainings)), Times.Exactly(pastTrainings.Count));

            var models = result!.Value as IList<TrainingDetailModel>;
            Assert.That(models, Has.Count.EqualTo(pastTrainings.Count));
        }

        [Test]
        public void GetByCode_TrainingExists_ShouldReturnTrainingDetails()
        {
            // Arrange
            var code = "Training-1";
            var training = new TrainingBuilder().Build();
            var trainingDetailModel = new TrainingDetailModel();
            _trainingRepositoryMock.Setup(repo => repo.GetByCodeAsync(It.IsAny<Code>())).ReturnsAsync(training);
            _mapperMock.Setup(mapper => mapper.Map<TrainingDetailModel>(It.IsAny<Domain.Training>())).Returns(trainingDetailModel);

            // Act
            var result = _controller.GetByCode(code).Result as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            _trainingRepositoryMock.Verify(repo => repo.GetByCodeAsync(It.IsAny<Code>()), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map<TrainingDetailModel>(training), Times.Once);

            var model = result!.Value as TrainingDetailModel;
            Assert.That(model, Is.Not.Null);
        }

        [Test]
        public void GetByCode_TrainingDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            var code = "Training-1";

            _trainingRepositoryMock.Setup(repo => repo.GetByCodeAsync(It.IsAny<Code>())).ReturnsAsync((Domain.Training?)null);

            // Act
            var result = _controller.GetByCode(code).Result as NotFoundResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            _trainingRepositoryMock.Verify(repo => repo.GetByCodeAsync(It.IsAny<Code>()), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map<TrainingDetailModel>(It.IsAny<Domain.Training>()), Times.Never);
        }

        [Test]
        public void GetParticipation_ParticipationExists_ShouldReturnParticipationDetails()
        {
            // Arrange
            var trainingCode = new Code("Training", 1);
            var apprenticeId = "apprentice-1";
            var training = new TrainingBuilder().WithTrainingCode(trainingCode).Build();
            var apprentice = Apprentice.CreateNew(apprenticeId, "John", "Doe", "Company");
            var participation = Participation.CreateNew(trainingCode.ToString(), apprenticeId);
            training.Register(apprentice);
            var participationDetailModel = new ParticipationDetailModel();

            _trainingRepositoryMock.Setup(repo => repo.GetByCodeAsync(It.Is<Code>(c => c.Name == "Training" && c.Sequence == 1)))
                .ReturnsAsync(training);
            _mapperMock.Setup(mapper => mapper.Map<ParticipationDetailModel>(It.IsAny<Participation>()))
                .Returns(participationDetailModel);

            // Act
            var result = _controller.GetParticipation(trainingCode.ToString(), apprenticeId).Result as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            _trainingRepositoryMock.Verify(repo => repo.GetByCodeAsync(It.Is<Code>(c => c.Name == "Training" && c.Sequence == 1)), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map<ParticipationDetailModel>(It.Is<Participation>(p => p.TrainingCode.ToString() == trainingCode.ToString() && p.ApprenticeId == apprenticeId)), Times.Once);

            var model = result!.Value as ParticipationDetailModel;
            Assert.That(model, Is.Not.Null);
        }

        [Test]
        public async Task GetParticipation_ParticipationDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            var trainingCode = "Training-1";
            var apprenticeId = "apprentice-1";
            var training = new TrainingBuilder().Build();
            _trainingRepositoryMock.Setup(repo => repo.GetByCodeAsync(It.IsAny<Code>())).ReturnsAsync(training);

            // Act
            var result = await _controller.GetParticipation(trainingCode, apprenticeId) as NotFoundResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            _trainingRepositoryMock.Verify(repo => repo.GetByCodeAsync(It.IsAny<Code>()), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map<ParticipationDetailModel>(It.IsAny<Participation>()), Times.Never);
        }


        [Test]
        public async Task GetParticipation_TrainingDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            var code = "Training-1";
            var apprenticeId = "apprentice-1";
            _trainingRepositoryMock.Setup(repo => repo.GetByCodeAsync(It.IsAny<Code>())).ReturnsAsync((Domain.Training?)null);

            // Act
            var result = await _controller.GetParticipation(code, apprenticeId) as NotFoundResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            _trainingRepositoryMock.Verify(repo => repo.GetByCodeAsync(It.IsAny<Code>()), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map<ParticipationDetailModel>(It.IsAny<Participation>()), Times.Never);
        }


        [Test]
        public void CreateTraining_ShouldCreateTraining()
        {
            // Arrange
            var model = new TrainingCreateModel
            {
                Name = "Training",
                Description = "Description",
                MaximumCapacity = 10,
                RoomCode = new Code("Room", 1),
                CoachId = "Coach",
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1)
            };
            var training = new TrainingBuilder().Build();
            var trainingDetailModel = new TrainingDetailModel();
            _coachServiceMock.Setup(service => service.CreateTraining(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<Code>(), It.IsAny<string>(), It.IsAny<TimeWindow>())).ReturnsAsync(training);
            _mapperMock.Setup(mapper => mapper.Map<TrainingDetailModel>(It.IsAny<Domain.Training>())).Returns(trainingDetailModel);

            // Act
            var result = _controller.CreateTraining(model).Result as CreatedAtActionResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            _coachServiceMock.Verify(service => service.CreateTraining(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<Code>(), It.IsAny<string>(), It.IsAny<TimeWindow>()), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map<TrainingDetailModel>(training), Times.Once);

            var createdModel = result!.Value as TrainingDetailModel;
            Assert.That(createdModel, Is.Not.Null);
        }

        [Test]
        public void RegisterApprentice_ShouldRegisterApprentice()
        {
            // Arrange
            var apprentice = Apprentice.CreateNew("id", "John", "Doe", "Company");
            _apprenticeServiceMock.Setup(service => service.RegisterApprentice(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(apprentice);

            // Act
            var result = _controller.RegisterApprentice("code", "id").Result as CreatedAtActionResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            _apprenticeServiceMock.Verify(service => service.RegisterApprentice(It.IsAny<string>(), It.IsAny<string>()), Times.Once);

            var registeredApprentice = result!.Value as Apprentice;
            Assert.That(registeredApprentice, Is.Not.Null);
        }

        [Test]
        public void RegisterExternalApprentice_ShouldRegisterExternalApprentice()
        {
            // Arrange
            var apprentice = Apprentice.CreateNew("id", "John", "Doe", "Company");
            var model = new ApprenticeCreateModel
            {
                FirstName = "John",
                LastName = "Doe",
                Company = "Company"
            };
            _apprenticeServiceMock.Setup(service => service.RegisterExternalApprentice(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(apprentice);

            // Act
            var result = _controller.RegisterExternalApprentice("code", model).Result as CreatedAtActionResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            _apprenticeServiceMock.Verify(service => service.RegisterExternalApprentice(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);

            var registeredApprentice = result!.Value as Apprentice;
            Assert.That(registeredApprentice, Is.Not.Null);
        }

        [Test]
        public void FinishParticipation_ShouldFinishParticipation()
        {
            // Arrange
            var trainingCode = "Training-1";
            var apprenticeId = "Apprentice-1";
            var participation = Participation.CreateNew(trainingCode, apprenticeId);

            var participationDetailModel = new ParticipationDetailModel();
            _apprenticeServiceMock.Setup(service => service.FinishParticipation(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(participation);
            _mapperMock.Setup(mapper => mapper.Map<ParticipationDetailModel>(It.IsAny<Participation>())).Returns(participationDetailModel);

            // Act
            var result = _controller.FinishParticipation("code", "id").Result as CreatedAtActionResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            _apprenticeServiceMock.Verify(service => service.FinishParticipation(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map<ParticipationDetailModel>(participation), Times.Once);

            var finishedParticipation = result!.Value as ParticipationDetailModel;
            Assert.That(finishedParticipation, Is.Not.Null);
        }
    }
}