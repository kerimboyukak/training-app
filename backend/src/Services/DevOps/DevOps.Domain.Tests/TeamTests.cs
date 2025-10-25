using DevOps.Domain.Tests.Builders;
using Domain;
using Test;

namespace DevOps.Domain.Tests
{
    public class TeamTests
    {
        [Test]
        public void CreateNew_ValidInput_ShouldInitializeFieldsCorrectly()
        {
            //Arrange
            string name = Random.Shared.NextString();

            //Act
            Team team = Team.CreateNew(name);

            //Assert
            Assert.That(team.Id, Is.Not.EqualTo(Guid.Empty));
            Assert.That(team.Name, Is.EqualTo(name));
            Assert.That(team.Developers, Is.Not.Null);
            Assert.That(team.Developers, Has.Count.Zero);
        }

        [Test]
        public void CreateNew_EmptyName_ShouldThrowContractException()
        {
            Assert.That(() => Team.CreateNew(""), Throws.InstanceOf<ContractException>());
        }

        [Test]
        public void Join_DeveloperNotInTeamYet_ShouldAddDeveloperToTeam()
        {
            //Arrange
            Team team = new TeamBuilder().Build();
            Developer developer = new DeveloperBuilder().WithoutTeam().Build();

            //Act
            team.Join(developer);

            //Assert
            Assert.That(team.Developers, Has.Count.EqualTo(1));
            Assert.That(team.Developers.First(), Is.SameAs(developer));
            Assert.That(developer.TeamId, Is.EqualTo(team.Id));
        }

        [Test]
        public void Join_DeveloperAlreadyInTeam_ShouldThrowContractException()
        {
            //Arrange
            TeamBuilder teamBuilder = new TeamBuilder();
            Team team = teamBuilder.Build();
            Developer developer = new DeveloperBuilder().WithTeam(team).Build();
            teamBuilder.WithDeveloper(developer);
            Developer otherDeveloper = new DeveloperBuilder().WithTeam(team).Build();
            teamBuilder.WithDeveloper(otherDeveloper);

            //Act + Assert
            Assert.That(() => team.Join(developer), Throws.InstanceOf<ContractException>());
        }
    }
}