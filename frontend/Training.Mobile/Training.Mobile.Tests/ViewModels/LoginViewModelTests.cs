using Moq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Training.Mobile.Services;
using Training.Mobile.Services.Backend;
using Training.Mobile.Services.Identity;
using Training.Mobile.ViewModels;

namespace Training.Mobile.Tests.ViewModels
{
    [TestFixture]
    public class LoginViewModelTests
    {
        private Mock<IIdentityService> _identityServiceMock = null!;
        private Mock<ITokenProvider> _tokenProviderMock = null!;
        private Mock<INavigationService> _navigationServiceMock = null!;
        private Mock<IToastService> _toastServiceMock = null!;
        private Mock<IUserService> _userServiceMock = null!;
        private LoginViewModel _model = null!;

        [SetUp]
        public void BeforeEachTest()
        {
            _identityServiceMock = new Mock<IIdentityService>();
            _tokenProviderMock = new Mock<ITokenProvider>();
            _navigationServiceMock = new Mock<INavigationService>();
            _toastServiceMock = new Mock<IToastService>();
            _userServiceMock = new Mock<IUserService>();

            _model = new LoginViewModel(_identityServiceMock.Object, _tokenProviderMock.Object, _navigationServiceMock.Object, _toastServiceMock.Object, _userServiceMock.Object);
        }

        [Test]
        public void Constructor_ShouldInitializeLogoutCommandCorrectly()
        {
            // Arrange
            _tokenProviderMock.SetupGet(provider => provider.AuthIdentityToken).Returns(string.Empty);

            // Act
            var model = new LoginViewModel(_identityServiceMock.Object, _tokenProviderMock.Object, _navigationServiceMock.Object, _toastServiceMock.Object, _userServiceMock.Object);

            // Assert
            Assert.That(model.LogoutCommand.CanExecute(null), Is.False);

            // Arrange
            _tokenProviderMock.SetupGet(provider => provider.AuthIdentityToken).Returns(Guid.NewGuid().ToString());

            // Act
            model = new LoginViewModel(_identityServiceMock.Object, _tokenProviderMock.Object, _navigationServiceMock.Object, _toastServiceMock.Object, _userServiceMock.Object);

            // Assert
            Assert.That(model.LogoutCommand.CanExecute(null), Is.True);
        }
        [Test]
        public void LoginCommand_ShouldNotProceedIfDependenciesAreNull()
        {
            // Arrange
            var modelWithNullIdentityService = new LoginViewModel(null!, _tokenProviderMock.Object, _navigationServiceMock.Object, _toastServiceMock.Object, _userServiceMock.Object);
            var modelWithNullTokenProvider = new LoginViewModel(_identityServiceMock.Object, null!, _navigationServiceMock.Object, _toastServiceMock.Object, _userServiceMock.Object);
            var modelWithNullNavigationService = new LoginViewModel(_identityServiceMock.Object, _tokenProviderMock.Object, null!, _toastServiceMock.Object, _userServiceMock.Object);
            var modelWithNullToastService = new LoginViewModel(_identityServiceMock.Object, _tokenProviderMock.Object, _navigationServiceMock.Object, null!, _userServiceMock.Object);
            var modelWithNullUserService = new LoginViewModel(_identityServiceMock.Object, _tokenProviderMock.Object, _navigationServiceMock.Object, _toastServiceMock.Object, null!);

            // Act
            modelWithNullIdentityService.LoginCommand.Execute(null);
            modelWithNullTokenProvider.LoginCommand.Execute(null);
            modelWithNullNavigationService.LoginCommand.Execute(null);
            modelWithNullToastService.LoginCommand.Execute(null);
            modelWithNullUserService.LoginCommand.Execute(null);

            // Assert
            _identityServiceMock.Verify(service => service.LoginAsync(), Times.Never);
            _tokenProviderMock.VerifySet(provider => provider.AuthAccessToken = It.IsAny<string>(), Times.Never);
            _tokenProviderMock.VerifySet(provider => provider.AuthIdentityToken = It.IsAny<string>(), Times.Never);
            _navigationServiceMock.Verify(service => service.NavigateAsync(It.IsAny<string>()), Times.Never);
            _toastServiceMock.Verify(service => service.DisplayToastAsync(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void LoginCommand_SuccessfulLogin_ShouldSaveAccessAndIdentityTokensAndNavigateToTrainingPage()
        {
            //Arrange
            ILoginResult successfulLoginResult = CreateSuccessfulLoginResult();
            _identityServiceMock.Setup(service => service.LoginAsync()).ReturnsAsync(successfulLoginResult);
            _userServiceMock.Setup(service => service.CheckIfUserIsCoachAsync(It.IsAny<string>())).ReturnsAsync(true);

            //Act
            _model.LoginCommand.Execute(null);

            //Assert
            _identityServiceMock.Verify(service => service.LoginAsync(), Times.Once);
            _tokenProviderMock.VerifySet(provider => provider.AuthAccessToken = successfulLoginResult.AccessToken, Times.Once);
            _tokenProviderMock.VerifySet(provider => provider.AuthIdentityToken = successfulLoginResult.IdentityToken, Times.Once);
            _userServiceMock.Verify(service => service.CheckIfUserIsCoachAsync(It.IsAny<string>()), Times.Once);
            _navigationServiceMock.Verify(service => service.NavigateAsync("TrainingPage"), Times.Once);
            _toastServiceMock.Verify(service => service.DisplayToastAsync(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void LoginCommand_FailedLogin_ShouldShowToastMessage()
        {
            //Arrange
            ILoginResult failedLoginResult = CreateFailedLoginResult();
            _identityServiceMock.Setup(service => service.LoginAsync()).ReturnsAsync(failedLoginResult);

            //Act
            _model.LoginCommand.Execute(null);

            //Assert
            _identityServiceMock.Verify(service => service.LoginAsync(), Times.Once);
            _tokenProviderMock.VerifySet(provider => provider.AuthAccessToken = It.IsAny<string>(), Times.Never);
            _tokenProviderMock.VerifySet(provider => provider.AuthIdentityToken = It.IsAny<string>(), Times.Never);
            _navigationServiceMock.Verify(service => service.NavigateAsync(It.IsAny<string>()), Times.Never);
            _toastServiceMock.Verify(service => service.DisplayToastAsync(failedLoginResult.ErrorDescription), Times.Once);
        }
        [Test]
        public void Login_ShouldNotProceedIfIsBusy()
        {
            // Arrange
            _model.IsBusy = true;

            // Act
            _model.LoginCommand.Execute(null);

            // Assert
            _identityServiceMock.Verify(service => service.LoginAsync(), Times.Never);
            _tokenProviderMock.VerifySet(provider => provider.AuthAccessToken = It.IsAny<string>(), Times.Never);
            _tokenProviderMock.VerifySet(provider => provider.AuthIdentityToken = It.IsAny<string>(), Times.Never);
            _navigationServiceMock.Verify(service => service.NavigateAsync(It.IsAny<string>()), Times.Never);
            _toastServiceMock.Verify(service => service.DisplayToastAsync(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void Login_ShouldNotProceedIfDependenciesAreNull()
        {
            // Arrange
            var modelWithNullIdentityService = new LoginViewModel(null!, _tokenProviderMock.Object, _navigationServiceMock.Object, _toastServiceMock.Object, _userServiceMock.Object);
            var modelWithNullTokenProvider = new LoginViewModel(_identityServiceMock.Object, null!, _navigationServiceMock.Object, _toastServiceMock.Object, _userServiceMock.Object);
            var modelWithNullNavigationService = new LoginViewModel(_identityServiceMock.Object, _tokenProviderMock.Object, null!, _toastServiceMock.Object, _userServiceMock.Object);
            var modelWithNullToastService = new LoginViewModel(_identityServiceMock.Object, _tokenProviderMock.Object, _navigationServiceMock.Object, null!, _userServiceMock.Object);
            var modelWithNullUserService = new LoginViewModel(_identityServiceMock.Object, _tokenProviderMock.Object, _navigationServiceMock.Object, _toastServiceMock.Object, null!);

            // Act
            modelWithNullIdentityService.LoginCommand.Execute(null);
            modelWithNullTokenProvider.LoginCommand.Execute(null);
            modelWithNullNavigationService.LoginCommand.Execute(null);
            modelWithNullToastService.LoginCommand.Execute(null);
            modelWithNullUserService.LoginCommand.Execute(null);

            // Assert
            _identityServiceMock.Verify(service => service.LoginAsync(), Times.Never);
            _tokenProviderMock.VerifySet(provider => provider.AuthAccessToken = It.IsAny<string>(), Times.Never);
            _tokenProviderMock.VerifySet(provider => provider.AuthIdentityToken = It.IsAny<string>(), Times.Never);
            _navigationServiceMock.Verify(service => service.NavigateAsync(It.IsAny<string>()), Times.Never);
            _toastServiceMock.Verify(service => service.DisplayToastAsync(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void LoginCommand_ShouldBeDisabledWhenOtherLoginIsInProgress()
        {
            //Arrange
            ILoginResult successfulLoginResult = CreateSuccessfulLoginResult();

            _identityServiceMock.Setup(service => service.LoginAsync()).ReturnsAsync(() =>
            {
                Assert.That(_model.IsBusy, Is.True);
                Assert.That(_model.LoginCommand.CanExecute(null), Is.False);
                return successfulLoginResult;
            });
            _model.LoginCommand.Execute(null);

            //Assert
            Assert.That(_model.IsBusy, Is.False);
            Assert.That(_model.LoginCommand.CanExecute(null), Is.True);
        }

        [Test]
        public void IsLoggedIn_ShouldReturnTrueIfIdentityTokenIsPresent()
        {
            //Arrange
            _tokenProviderMock.SetupGet(provider => provider.AuthIdentityToken).Returns(Guid.NewGuid().ToString);

            //Act
            bool isLoggedIn = _model.IsLoggedIn;

            //Assert
            Assert.That(isLoggedIn, Is.True);
        }

        [Test]
        public void IsLoggedIn_ShouldReturnFalseIfIdentityTokenIsNotPresent()
        {
            //Arrange
            _tokenProviderMock.SetupGet(provider => provider.AuthIdentityToken).Returns(string.Empty);

            //Act
            bool isLoggedIn = _model.IsLoggedIn;

            //Assert
            Assert.That(isLoggedIn, Is.False);
        }

        [Test]
        public void IsLoggedOut_ShouldReturnTrueIfIdentityTokenIsNotPresent()
        {
            //Arrange
            _tokenProviderMock.SetupGet(provider => provider.AuthIdentityToken).Returns(string.Empty);

            //Act
            bool isLoggedOut = _model.IsLoggedOut;

            //Assert
            Assert.That(isLoggedOut, Is.True);
        }

        [Test]
        public void IsLoggedOut_ShouldReturnFalseIfIdentityTokenIsPresent()
        {
            //Arrange
            _tokenProviderMock.SetupGet(provider => provider.AuthIdentityToken).Returns(Guid.NewGuid().ToString);

            //Act
            bool isLoggedOut = _model.IsLoggedOut;

            //Assert
            Assert.That(isLoggedOut, Is.False);
        }

        [Test]
        public void LogoutCommand_ShouldClearIdentityToken()
        {
            //Act
            _model.LogoutCommand.Execute(null);

            //Assert
            _tokenProviderMock.VerifySet(provider => provider.AuthIdentityToken = string.Empty, Times.Once);
            _userServiceMock.Verify(service => service.ClearUserInfoAsync(), Times.Once);
        }
     
        [Test]
        public void Title_ShouldRaisePropertyChangedEvent()
        {
            //Arrange
            bool eventRaised = false;
            _model.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(_model.Title))
                {
                    eventRaised = true;
                }
            };

            //Act
            _model.Title = "New Title";

            //Assert
            Assert.That(eventRaised, Is.True);
        }
        [Test]
        public void Title_Getter_ShouldReturnCorrectValue()
        {
            //Arrange
            _model.Title = "Test Title";

            //Act
            var title = _model.Title;

            //Assert
            Assert.That(title, Is.EqualTo("Test Title"));
        }
        [Test]
        public void SetProperty_ShouldRaisePropertyChangedEvent()
        {
            //Arrange
            bool eventRaised = false;
            _model.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(_model.IsLoggedIn))
                {
                    eventRaised = true;
                }
            };

            //Act
            _model.IsLoggedIn = true;

            //Assert
            Assert.That(eventRaised, Is.True);
        }

        [Test]
        public void SetProperty_ShouldNotRaisePropertyChangedEventIfValueIsSame()
        {
            //Arrange
            bool eventRaised = false;
            _model.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(_model.IsLoggedIn))
                {
                    eventRaised = true;
                }
            };

            //Act
            _model.IsLoggedIn = true; // Set to a different value first to ensure the event can be raised
            eventRaised = false; // Reset the flag
            _model.IsLoggedIn = true; // Set to the same value to test if the event is raised

            //Assert
            Assert.That(eventRaised, Is.False);
        }

        [Test]
        public void IsLoggedOut_Setter_ShouldRaisePropertyChangedEvent()
        {
            //Arrange
            bool eventRaised = false;
            _model.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(_model.IsLoggedOut))
                {
                    eventRaised = true;
                }
            };

            //Act
            _model.IsLoggedOut = true;

            //Assert
            Assert.That(eventRaised, Is.True);
        }

        private ILoginResult CreateSuccessfulLoginResult()
        {
            string accessToken = Guid.NewGuid().ToString();
            string identityToken = Guid.NewGuid().ToString();
            var successfulLoginResultMock = new Mock<ILoginResult>();
            successfulLoginResultMock.SetupGet(result => result.IsError).Returns(false);
            successfulLoginResultMock.SetupGet(result => result.AccessToken).Returns(accessToken);
            successfulLoginResultMock.SetupGet(result => result.IdentityToken).Returns(identityToken);
            return successfulLoginResultMock.Object;
        }

        private ILoginResult CreateFailedLoginResult()
        {
            string errorDescription = Guid.NewGuid().ToString();
            var failedLoginResultMock = new Mock<ILoginResult>();
            failedLoginResultMock.SetupGet(result => result.IsError).Returns(true);
            failedLoginResultMock.SetupGet(result => result.ErrorDescription).Returns(errorDescription);
            return failedLoginResultMock.Object;
        }
    }
}