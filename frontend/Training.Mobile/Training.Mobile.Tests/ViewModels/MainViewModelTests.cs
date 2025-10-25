using Moq;
using NUnit.Framework;
using System.Threading.Tasks;
using Training.Mobile.Services;
using Training.Mobile.ViewModels;

namespace Training.Mobile.Tests.ViewModels
{
    [TestFixture]
    public class MainViewModelTests
    {
        private Mock<INavigationService> _navigationServiceMock;
        private MainViewModel _viewModel;

        [SetUp]
        public void SetUp()
        {
            _navigationServiceMock = new Mock<INavigationService>();
            _viewModel = new MainViewModel(_navigationServiceMock.Object);
        }

        [Test]
        public void Constructor_InitializesNavigateToLoginCommand()
        {
            // Assert
            Assert.IsNotNull(_viewModel.NavigateToLoginCommand);
        }

        [Test]
        public async Task NavigateToLoginCommand_ExecutesNavigateToLogin()
        {
            // Act
            await Task.Run(() => _viewModel.NavigateToLoginCommand.Execute(null));

            // Assert
            _navigationServiceMock.Verify(s => s.NavigateAsync("LoginPage"), Times.Once);
        }
    }
}