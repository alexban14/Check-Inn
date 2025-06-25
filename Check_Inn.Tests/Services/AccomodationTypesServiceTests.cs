using Check_Inn.DAL;
using Check_Inn.Entities;
using Check_Inn.Services;
using Check_Inn.Tests.Helpers;
using Moq;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Check_Inn.Tests.Services
{
    [TestFixture]
    public class AccomodationTypesServiceTests
    {
        private Mock<CheckInnMySqlContext> _mockContext;
        private Mock<DbSet<AccomodationType>> _mockAccomodationTypesSet;
        private AccomodationTypesService _service;
        private List<AccomodationType> _testData;

        [SetUp]
        public void SetUp()
        {
            _testData = TestDataHelper.GetTestAccomodationTypes();
            _mockAccomodationTypesSet = MockDbSetHelper.CreateMockDbSet(_testData);
            _mockContext = new Mock<CheckInnMySqlContext>();
            _mockContext.Setup(c => c.AccomodationTypes).Returns(_mockAccomodationTypesSet.Object);
            _service = new AccomodationTypesService(_mockContext.Object);
        }

        [Test]
        public void GetAllAccomodationTypes_ShouldReturnAllTypes()
        {
            // Act
            var result = _service.GetAllAccomodationTypes();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result.Should().BeEquivalentTo(_testData);
        }

        [Test]
        public void SearchAccomodationType_WithEmptySearchTerm_ShouldReturnAll()
        {
            // Act
            var result = _service.SearchAccomodationType("");

            // Assert
            result.Should().HaveCount(3);
        }

        [Test]
        public void SearchAccomodationType_WithNullSearchTerm_ShouldReturnAll()
        {
            // Act
            var result = _service.SearchAccomodationType(null);

            // Assert
            result.Should().HaveCount(3);
        }

        [Test]
        public void SearchAccomodationType_WithValidSearchTerm_ShouldReturnFilteredResults()
        {
            // Act
            var result = _service.SearchAccomodationType("hotel");

            // Assert
            result.Should().HaveCount(1);
            result.First().Name.Should().Be("Hotel");
        }

        [Test]
        public void SearchAccomodationType_WithCaseInsensitiveSearch_ShouldReturnResults()
        {
            // Act
            var result = _service.SearchAccomodationType("APARTMENT");

            // Assert
            result.Should().HaveCount(1);
            result.First().Name.Should().Be("Apartment");
        }

        [Test]
        public void SearchAccomodationType_WithPartialMatch_ShouldReturnResults()
        {
            // Act
            var result = _service.SearchAccomodationType("vil");

            // Assert
            result.Should().HaveCount(1);
            result.First().Name.Should().Be("Villa");
        }

        [Test]
        public void SearchAccomodationType_WithNoMatches_ShouldReturnEmpty()
        {
            // Act
            var result = _service.SearchAccomodationType("nonexistent");

            // Assert
            result.Should().BeEmpty();
        }

        [Test]
        public void GetAccomodationTypeByID_WithValidID_ShouldReturnCorrectType()
        {
            // Act
            var result = _service.GetAccomodationTypeByID(1);

            // Assert
            result.Should().NotBeNull();
            result.ID.Should().Be(1);
            result.Name.Should().Be("Hotel");
        }

        [Test]
        public void GetAccomodationTypeByID_WithInvalidID_ShouldReturnNull()
        {
            // Act
            var result = _service.GetAccomodationTypeByID(999);

            // Assert
            result.Should().BeNull();
        }

        [Test]
        public void SaveAccomodationType_ShouldAddToContextAndReturnTrue()
        {
            // Arrange
            var newType = new AccomodationType { ID = 4, Name = "Hostel" };
            _mockContext.Setup(c => c.SaveChanges()).Returns(1);

            // Act
            var result = _service.SaveAccomodationType(newType);

            // Assert
            result.Should().BeTrue();
            _mockAccomodationTypesSet.Verify(m => m.Add(newType), Times.Once);
            _mockContext.Verify(c => c.SaveChanges(), Times.Once);
        }

        [Test]
        public void SaveAccomodationType_WhenSaveChangesFails_ShouldReturnFalse()
        {
            // Arrange
            var newType = new AccomodationType { ID = 4, Name = "Hostel" };
            _mockContext.Setup(c => c.SaveChanges()).Returns(0);

            // Act
            var result = _service.SaveAccomodationType(newType);

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void UpdateAccomodationType_ShouldSetStateToModifiedAndReturnTrue()
        {
            // Arrange
            var typeToUpdate = _testData.First();
            var mockEntry = new Mock<System.Data.Entity.Infrastructure.DbEntityEntry<AccomodationType>>();
            _mockContext.Setup(c => c.Entry(typeToUpdate)).Returns(mockEntry.Object);
            _mockContext.Setup(c => c.SaveChanges()).Returns(1);

            // Act
            var result = _service.UpdateAccomodationType(typeToUpdate);

            // Assert
            result.Should().BeTrue();
            mockEntry.VerifySet(e => e.State = EntityState.Modified, Times.Once);
            _mockContext.Verify(c => c.SaveChanges(), Times.Once);
        }

        [Test]
        public void DeleteAccomodationType_ShouldSetStateToDeletedAndReturnTrue()
        {
            // Arrange
            var typeToDelete = _testData.First();
            var mockEntry = new Mock<System.Data.Entity.Infrastructure.DbEntityEntry<AccomodationType>>();
            _mockContext.Setup(c => c.Entry(typeToDelete)).Returns(mockEntry.Object);
            _mockContext.Setup(c => c.SaveChanges()).Returns(1);

            // Act
            var result = _service.DeleteAccomodationType(typeToDelete);

            // Assert
            result.Should().BeTrue();
            mockEntry.VerifySet(e => e.State = EntityState.Deleted, Times.Once);
            _mockContext.Verify(c => c.SaveChanges(), Times.Once);
        }
    }
}
