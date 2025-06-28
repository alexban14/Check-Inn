using Check_Inn.DAL;
using Check_Inn.Entities;
using Check_Inn.Services;
using Check_Inn.Tests.Helpers;
using Moq;
using NUnit.Framework;
using FluentAssertions;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Data.Entity.Infrastructure;

namespace Check_Inn.Tests.Services
{
    [TestFixture]
    public class AccomodationsServiceTests
    {
        private Mock<ICheckInnContext> _mockContext;
        private Mock<DbSet<Accomodation>> _mockAccomodationsSet;
        private AccomodationsService _service;
        private List<Accomodation> _testData;

        [SetUp]
        public void SetUp()
        {
            _testData = TestDataHelper.GetTestAccomodations();
            _mockAccomodationsSet = MockDbSetHelper.CreateMockDbSet(_testData);
            _mockContext = new Mock<ICheckInnContext>();
            _mockContext.Setup(c => c.Accomodations).Returns(_mockAccomodationsSet.Object);
            _service = new AccomodationsService(_mockContext.Object);
        }

        [Test]
        public void GetAllAcomodation_ShouldReturnAllAccomodations()
        {
            // Act
            var result = _service.GetAllAcomodation();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result.Should().BeEquivalentTo(_testData);
        }

        [Test]
        public void GetAllAccomodationsByAccomodationType_WithValidPackageID_ShouldReturnFilteredAccomodations()
        {
            // Act
            var result = _service.GetAllAccomodationsByAccomodationType(1);

            // Assert
            result.Should().HaveCount(2);
            result.All(p => p.AccomodationPackageID == 1).Should().BeTrue();
        }

        [Test]
        public void SearchAccomodation_WithNullSearchTerm_ShouldReturnPaginatedResults()
        {
            // Act
            var result = _service.SearchAccomodation(null, null, 1, 2);

            // Assert
            result.Should().HaveCount(2);
        }

        [Test]
        public void SearchAccomodation_WithEmptySearchTerm_ShouldReturnPaginatedResults()
        {
            // Act
            var result = _service.SearchAccomodation("", null, 1, 2);

            // Assert
            result.Should().HaveCount(2);
        }

        [Test]
        public void SearchAccomodation_WithValidSearchTerm_ShouldReturnFilteredResults()
        {
            // Act
            var result = _service.SearchAccomodation("suite", null, 1, 10);

            // Assert
            result.Should().HaveCount(1);
            result.First().Name.Should().Be("Premium Suite");
        }

        [Test]
        public void SearchAccomodation_WithAccomodationPackageID_ShouldReturnFilteredResults()
        {
            // Act
            var result = _service.SearchAccomodation(null, 2, 1, 10);

            // Assert
            result.Should().HaveCount(1);
            result.First().AccomodationPackageID.Should().Be(2);
        }

        [Test]
        public void SearchAccomodationCount_WithNullSearchTerm_ShouldReturnTotalCount()
        {
            // Act
            var result = _service.SearchAccomodationCount(null, null);

            // Assert
            result.Should().Be(3);
        }

        [Test]
        public void SearchAccomodationCount_WithValidSearchTerm_ShouldReturnFilteredCount()
        {
            // Act
            var result = _service.SearchAccomodationCount("room", null);

            // Assert
            result.Should().Be(2);
        }

        [Test]
        public void GetAccomodationByID_WithValidID_ShouldReturnCorrectAccomodation()
        {
            // Act
            var result = _service.GetAccomodationByID(1);

            // Assert
            result.Should().NotBeNull();
            result.ID.Should().Be(1);
            result.Name.Should().Be("Room 101");
        }

        [Test]
        public void GetAccomodationByID_WithInvalidID_ShouldReturnNull()
        {
            // Act
            var result = _service.GetAccomodationByID(999);

            // Assert
            result.Should().BeNull();
        }

        [Test]
        public void SaveAccomodation_ShouldAddToContextAndReturnTrue()
        {
            // Arrange
            var newAccomodation = new Accomodation { ID = 4, Name = "New Room", AccomodationPackageID = 2 };
            _mockContext.Setup(c => c.SaveChanges()).Returns(1);

            // Act
            var result = _service.SaveAccomodation(newAccomodation);

            // Assert
            result.Should().BeTrue();
            _mockAccomodationsSet.Verify(m => m.Add(newAccomodation), Times.Once);
            _mockContext.Verify(c => c.SaveChanges(), Times.Once);
        }

        [Test]
        public void UpdateAccomodation_ShouldSetStateToModifiedAndReturnTrue()
        {
            // Arrange
            var accomodationToUpdate = _testData.First();
            var mockEntry = new Mock<IDbEntityEntry<Accomodation>>();
            _mockContext.Setup(c => c.Entry(accomodationToUpdate)).Returns(mockEntry.Object);
            _mockContext.Setup(c => c.SaveChanges()).Returns(1);

            // Act
            var result = _service.UpdateAccomodation(accomodationToUpdate);

            // Assert
            result.Should().BeTrue();
            mockEntry.VerifySet(e => e.State = EntityState.Modified, Times.Once);
            _mockContext.Verify(c => c.SaveChanges(), Times.Once);
        }

        [Test]
        public void DeleteAccomodation_ShouldSetStateToDeletedAndReturnTrue()
        {
            // Arrange
            var accomodationToDelete = _testData.First();
            var mockEntry = new Mock<IDbEntityEntry<Accomodation>>();
            _mockContext.Setup(c => c.Entry(accomodationToDelete)).Returns(mockEntry.Object);
            _mockContext.Setup(c => c.SaveChanges()).Returns(1);

            // Act
            var result = _service.DeleteAccomodation(accomodationToDelete);

            // Assert
            result.Should().BeTrue();
            mockEntry.VerifySet(e => e.State = EntityState.Deleted, Times.Once);
            _mockContext.Verify(c => c.SaveChanges(), Times.Once);
        }
    }
}
