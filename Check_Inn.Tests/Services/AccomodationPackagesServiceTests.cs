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

namespace Check_Inn.Tests.Services
{
    [TestFixture]
    public class AccomodationPackagesServiceTests
    {
        private Mock<ICheckInnContext> _mockContext;
        private Mock<DbSet<AccomodationPackage>> _mockPackagesSet;
        private AccomodationPackagesService _service;
        private List<AccomodationPackage> _testData;

        [SetUp]
        public void SetUp()
        {
            _testData = TestDataHelper.GetTestAccomodationPackages();
            _mockPackagesSet = MockDbSetHelper.CreateMockDbSet(_testData);
            _mockContext = new Mock<ICheckInnContext>();
            _mockContext.Setup(c => c.AccomodationPackages).Returns(_mockPackagesSet.Object);
            _service = new AccomodationPackagesService(_mockContext.Object);
        }

        [Test]
        public void GetAllAcomodationPackages_ShouldReturnAllPackages()
        {
            // Act
            var result = _service.GetAllAcomodationPackages();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result.Should().BeEquivalentTo(_testData);
        }

        [Test]
        public void GetAllAccomodationPackagesByAccomodationType_WithValidTypeID_ShouldReturnFilteredPackages()
        {
            // Act
            var result = _service.GetAllAccomodationPackagesByAccomodationType(1);

            // Assert
            result.Should().HaveCount(2);
            result.All(p => p.AccomodationTypeID == 1).Should().BeTrue();
        }

        [Test]
        public void GetAllAccomodationPackagesByAccomodationType_WithNonExistentTypeID_ShouldReturnEmpty()
        {
            // Act
            var result = _service.GetAllAccomodationPackagesByAccomodationType(999);

            // Assert
            result.Should().BeEmpty();
        }

        [Test]
        public void SearchAccomodationPackage_WithNullSearchTerm_ShouldReturnPaginatedResults()
        {
            // Act
            var result = _service.SearchAccomodationPackage(null, null, 1, 2);

            // Assert
            result.Should().HaveCount(2);
        }

        [Test]
        public void SearchAccomodationPackage_WithEmptySearchTerm_ShouldReturnPaginatedResults()
        {
            // Act
            var result = _service.SearchAccomodationPackage("", null, 1, 2);

            // Assert
            result.Should().HaveCount(2);
        }

        [Test]
        public void SearchAccomodationPackage_WithValidSearchTerm_ShouldReturnFilteredResults()
        {
            // Act
            var result = _service.SearchAccomodationPackage("standard", null, 1, 10);

            // Assert
            result.Should().HaveCount(1);
            result.First().Name.Should().Be("Standard Package");
        }

        [Test]
        public void SearchAccomodationPackage_WithAccomodationTypeID_ShouldReturnFilteredResults()
        {
            // Act
            var result = _service.SearchAccomodationPackage(null, 3, 1, 10);

            // Assert
            result.Should().HaveCount(1);
            result.First().AccomodationTypeID.Should().Be(3);
        }

        [Test]
        public void SearchAccomodationPackage_WithBothFilters_ShouldReturnCombinedFilteredResults()
        {
            // Act
            var result = _service.SearchAccomodationPackage("package", 1, 1, 10);

            // Assert
            result.Should().HaveCount(2);
            result.All(p => p.AccomodationTypeID == 1).Should().BeTrue();
            result.All(p => p.Name.ToLower().Contains("package")).Should().BeTrue();
        }

        // [Test]
        // public void SearchAccomodationPackage_WithPagination_ShouldReturnCorrectPage()
        // {
        //     // Act
        //     var firstPage = _service.SearchAccomodationPackage(null, null, 1, 2);
        //     var secondPage = _service.SearchAccomodationPackage(null, null, 2, 2);
        //
        //     // Assert
        //     firstPage.Should().HaveCount(2);
        //     // Second page should have remaining items (1 item since we have 3 total)
        //     secondPage.Should().HaveCount(1);
        //     // Verify no overlap between pages
        //     var firstPageIds = firstPage.Select(p => p.ID).ToList();
        //     var secondPageIds = secondPage.Select(p => p.ID).ToList();
        //     firstPageIds.Should().NotIntersectWith(secondPageIds);
        // }

        [Test]
        public void SearchAccomodationPackage_WithZeroAccomodationTypeID_ShouldIgnoreFilter()
        {
            // Act
            var result = _service.SearchAccomodationPackage(null, 0, 1, 10);

            // Assert
            result.Should().HaveCount(3);
        }

        [Test]
        public void SearchAccomodationPackageCount_WithNullSearchTerm_ShouldReturnTotalCount()
        {
            // Act
            var result = _service.SearchAccomodationPackageCount(null, null);

            // Assert
            result.Should().Be(3);
        }

        [Test]
        public void SearchAccomodationPackageCount_WithValidSearchTerm_ShouldReturnFilteredCount()
        {
            // Act
            var result = _service.SearchAccomodationPackageCount("premium", null);

            // Assert
            result.Should().Be(1);
        }

        [Test]
        public void SearchAccomodationPackageCount_WithAccomodationTypeID_ShouldReturnFilteredCount()
        {
            // Act
            var result = _service.SearchAccomodationPackageCount(null, 1);

            // Assert
            result.Should().Be(2);
        }

        [Test]
        public void GetAccomodationPackageByID_WithValidID_ShouldReturnCorrectPackage()
        {
            // Act
            var result = _service.GetAccomodationPackageByID(1);

            // Assert
            result.Should().NotBeNull();
            result.ID.Should().Be(1);
            result.Name.Should().Be("Standard Package");
        }

        [Test]
        public void GetAccomodationPackageByID_WithInvalidID_ShouldReturnNull()
        {
            // Act
            var result = _service.GetAccomodationPackageByID(999);

            // Assert
            result.Should().BeNull();
        }

        [Test]
        public void SaveAccomodationPackage_ShouldAddToContextAndReturnTrue()
        {
            // Arrange
            var newPackage = new AccomodationPackage { ID = 4, Name = "New Package", AccomodationTypeID = 2 };
            _mockContext.Setup(c => c.SaveChanges()).Returns(1);

            // Act
            var result = _service.SaveAccomodationPackage(newPackage);

            // Assert
            result.Should().BeTrue();
            _mockPackagesSet.Verify(m => m.Add(newPackage), Times.Once);
            _mockContext.Verify(c => c.SaveChanges(), Times.Once);
        }

        [Test]
        public void UpdateAccomodationPackage_ShouldSetStateToModifiedAndReturnTrue()
        {
            // Arrange
            var packageToUpdate = _testData.First();
            var mockEntry = new Mock<IDbEntityEntry<AccomodationPackage>>();
            _mockContext.Setup(c => c.Entry(packageToUpdate)).Returns(mockEntry.Object);
            _mockContext.Setup(c => c.SaveChanges()).Returns(1);

            // Act
            var result = _service.UpdateAccomodationPackage(packageToUpdate);

            // Assert
            result.Should().BeTrue();
            mockEntry.VerifySet(e => e.State = EntityState.Modified, Times.Once);
            _mockContext.Verify(c => c.SaveChanges(), Times.Once);
        }

        [Test]
        public void DeleteAccomodationPackage_ShouldSetStateToDeletedAndReturnTrue()
        {
            // Arrange
            var packageToDelete = _testData.First();
            var mockEntry = new Mock<IDbEntityEntry<AccomodationPackage>>();
            _mockContext.Setup(c => c.Entry(packageToDelete)).Returns(mockEntry.Object);
            _mockContext.Setup(c => c.SaveChanges()).Returns(1);

            // Act
            var result = _service.DeleteAccomodationPackage(packageToDelete);

            // Assert
            result.Should().BeTrue();
            mockEntry.VerifySet(e => e.State = EntityState.Deleted, Times.Once);
            _mockContext.Verify(c => c.SaveChanges(), Times.Once);
        }
    }
}
