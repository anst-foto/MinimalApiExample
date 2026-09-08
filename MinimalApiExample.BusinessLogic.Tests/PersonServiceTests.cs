using System;
using System.Collections.Generic;
using System.Linq;
using MinimalApiExample.DataAccess;
using Moq;
using Xunit;

namespace MinimalApiExample.BusinessLogic.Tests
{
    public class PersonServiceTests
    {
        private readonly Mock<IRepository<Person>> _repositoryMock;
        private readonly PersonService _service;

        public PersonServiceTests()
        {
            _repositoryMock = new Mock<IRepository<Person>>();
            _service = new PersonService(_repositoryMock.Object);
        }

        #region GetAll

        [Fact]
        public void GetAll_WhenRepositoryReturnsData_ReturnsSameData()
        {
            // Arrange
            var expected = new List<Person>
            {
                new Person(Guid.NewGuid(), "Ivanov", "Ivan", "Ivanovich", new DateTime(1990, 1, 1)),
                new Person(Guid.NewGuid(), "Petrov", "Petr", null, new DateTime(1985, 5, 15))
            };
            _repositoryMock.Setup(r => r.GetAll()).Returns(expected);

            // Act
            var result = _service.GetAll();

            // Assert
            Assert.Equal(expected, result);
            _repositoryMock.Verify(r => r.GetAll(), Times.Once);
        }

        #endregion

        #region Add

        [Fact]
        public void Add_WhenRepositoryReturnsTrue_ReturnsTrue()
        {
            // Arrange
            var person = new Person(Guid.NewGuid(), "Sidorov", "Sidor", "Sidorovich", DateTime.Now);
            _repositoryMock.Setup(r => r.Add(person)).Returns(true);

            // Act
            var result = _service.Add(person);

            // Assert
            Assert.True(result);
            _repositoryMock.Verify(r => r.Add(person), Times.Once);
        }

        [Fact]
        public void Add_WhenRepositoryReturnsFalse_ReturnsFalse()
        {
            // Arrange
            var person = new Person(Guid.NewGuid(), "Test", "Test", null, DateTime.Now);
            _repositoryMock.Setup(r => r.Add(person)).Returns(false);

            // Act
            var result = _service.Add(person);

            // Assert
            Assert.False(result);
            _repositoryMock.Verify(r => r.Add(person), Times.Once);
        }

        #endregion

        #region Update

        [Fact]
        public void Update_WhenRepositoryReturnsTrue_ReturnsTrue()
        {
            // Arrange
            var person = new Person(Guid.NewGuid(), "Old", "Old", null, DateTime.Now);
            _repositoryMock.Setup(r => r.Update(person)).Returns(true);

            // Act
            var result = _service.Update(person);

            // Assert
            Assert.True(result);
            _repositoryMock.Verify(r => r.Update(person), Times.Once);
        }

        [Fact]
        public void Update_WhenRepositoryReturnsFalse_ReturnsFalse()
        {
            // Arrange
            var person = new Person(Guid.NewGuid(), "NonExistent", "Non", null, DateTime.Now);
            _repositoryMock.Setup(r => r.Update(person)).Returns(false);

            // Act
            var result = _service.Update(person);

            // Assert
            Assert.False(result);
            _repositoryMock.Verify(r => r.Update(person), Times.Once);
        }

        #endregion

        #region Delete

        [Fact]
        public void Delete_WhenRepositoryReturnsTrue_ReturnsTrue()
        {
            // Arrange
            var person = new Person(Guid.NewGuid(), "ToDelete", "Del", null, DateTime.Now);
            _repositoryMock.Setup(r => r.Delete(person)).Returns(true);

            // Act
            var result = _service.Delete(person);

            // Assert
            Assert.True(result);
            _repositoryMock.Verify(r => r.Delete(person), Times.Once);
        }

        [Fact]
        public void Delete_WhenRepositoryReturnsFalse_ReturnsFalse()
        {
            // Arrange
            var person = new Person(Guid.NewGuid(), "NonExistent", "Non", null, DateTime.Now);
            _repositoryMock.Setup(r => r.Delete(person)).Returns(false);

            // Act
            var result = _service.Delete(person);

            // Assert
            Assert.False(result);
            _repositoryMock.Verify(r => r.Delete(person), Times.Once);
        }

        #endregion

        #region FindById

        [Fact]
        public void FindById_WhenPersonExists_ReturnsPerson()
        {
            // Arrange
            var id = Guid.NewGuid();
            var expected = new Person(id, "Ivanov", "Ivan", "Ivanovich", new DateTime(1990, 1, 1));
            var persons = new List<Person>
            {
                expected,
                new Person(Guid.NewGuid(), "Petrov", "Petr", null, new DateTime(1985, 5, 15))
            };
            _repositoryMock.Setup(r => r.GetAll()).Returns(persons);

            // Act
            var result = _service.FindById(id);

            // Assert
            Assert.Equal(expected, result);
            _repositoryMock.Verify(r => r.GetAll(), Times.Once);
        }

        [Fact]
        public void FindById_WhenPersonDoesNotExist_ReturnsNull()
        {
            // Arrange
            var persons = new List<Person>
            {
                new Person(Guid.NewGuid(), "Ivanov", "Ivan", "Ivanovich", new DateTime(1990, 1, 1))
            };
            _repositoryMock.Setup(r => r.GetAll()).Returns(persons);
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = _service.FindById(nonExistentId);

            // Assert
            Assert.Null(result);
            _repositoryMock.Verify(r => r.GetAll(), Times.Once);
        }

        [Fact]
        public void FindById_WhenRepositoryReturnsEmptyCollection_ReturnsNull()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetAll()).Returns(new List<Person>());

            // Act
            var result = _service.FindById(Guid.NewGuid());

            // Assert
            Assert.Null(result);
            _repositoryMock.Verify(r => r.GetAll(), Times.Once);
        }

        #endregion

        #region FindByName

        [Fact]
        public void FindByName_WhenMatchInLastName_ReturnsMatchingPersons()
        {
            // Arrange
            var persons = new List<Person>
            {
                new Person(Guid.NewGuid(), "Ivanov", "Ivan", "Ivanovich", new DateTime(1990, 1, 1)),
                new Person(Guid.NewGuid(), "Petrov", "Petr", null, new DateTime(1985, 5, 15)),
                new Person(Guid.NewGuid(), "Sidorov", "Sidor", "Sidorovich", new DateTime(2000, 12, 12))
            };
            _repositoryMock.Setup(r => r.GetAll()).Returns(persons);

            // Act
            var result = _service.FindByName("Ivan").ToList();

            // Assert
            Assert.Single(result);
            Assert.Equal("Ivanov", result[0].LastName);
            _repositoryMock.Verify(r => r.GetAll(), Times.Once);
        }

        [Fact]
        public void FindByName_WhenMatchInFirstName_ReturnsMatchingPersons()
        {
            // Arrange
            var persons = new List<Person>
            {
                new Person(Guid.NewGuid(), "Ivanov", "Ivan", "Ivanovich", new DateTime(1990, 1, 1)),
                new Person(Guid.NewGuid(), "Petrov", "Petr", null, new DateTime(1985, 5, 15))
            };
            _repositoryMock.Setup(r => r.GetAll()).Returns(persons);

            // Act
            var result = _service.FindByName("Petr").ToList();

            // Assert
            Assert.Single(result);
            Assert.Equal("Petrov", result[0].LastName);
            _repositoryMock.Verify(r => r.GetAll(), Times.Once);
        }

        [Fact]
        public void FindByName_WhenMatchInPatronymic_ReturnsMatchingPersons()
        {
            // Arrange
            var persons = new List<Person>
            {
                new Person(Guid.NewGuid(), "Ivanov", "Ivan", "Ivanovich", new DateTime(1990, 1, 1)),
                new Person(Guid.NewGuid(), "Petrov", "Petr", null, new DateTime(1985, 5, 15)),
                new Person(Guid.NewGuid(), "Sidorov", "Sidor", "Sidorovich", new DateTime(2000, 12, 12))
            };
            _repositoryMock.Setup(r => r.GetAll()).Returns(persons);

            // Act
            var result = _service.FindByName("ovich").ToList();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, p => p.LastName == "Ivanov");
            Assert.Contains(result, p => p.LastName == "Sidorov");
            _repositoryMock.Verify(r => r.GetAll(), Times.Once);
        }

        [Fact]
        public void FindByName_WhenMultipleMatches_ReturnsAll()
        {
            // Arrange
            var persons = new List<Person>
            {
                new Person(Guid.NewGuid(), "Ivanov", "Ivan", "Ivanovich", new DateTime(1990, 1, 1)),
                new Person(Guid.NewGuid(), "Ivanov", "Petr", null, new DateTime(1985, 5, 15))
            };
            _repositoryMock.Setup(r => r.GetAll()).Returns(persons);

            // Act
            var result = _service.FindByName("Ivanov").ToList();

            // Assert
            Assert.Equal(2, result.Count);
            _repositoryMock.Verify(r => r.GetAll(), Times.Once);
        }

        [Fact]
        public void FindByName_WhenNoMatch_ReturnsEmptyCollection()
        {
            // Arrange
            var persons = new List<Person>
            {
                new Person(Guid.NewGuid(), "Ivanov", "Ivan", "Ivanovich", new DateTime(1990, 1, 1))
            };
            _repositoryMock.Setup(r => r.GetAll()).Returns(persons);

            // Act
            var result = _service.FindByName("Sidorov");

            // Assert
            Assert.Empty(result);
            _repositoryMock.Verify(r => r.GetAll(), Times.Once);
        }

        [Fact]
        public void FindByName_WhenRepositoryReturnsEmpty_ReturnsEmptyCollection()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetAll()).Returns(new List<Person>());

            // Act
            var result = _service.FindByName("anything");

            // Assert
            Assert.Empty(result);
            _repositoryMock.Verify(r => r.GetAll(), Times.Once);
        }

        [Fact]
        public void FindByName_WhenNameIsEmptyString_ReturnsAllPersons()
        {
            // Arrange
            var persons = new List<Person>
            {
                new Person(Guid.NewGuid(), "Ivanov", "Ivan", "Ivanovich", new DateTime(1990, 1, 1)),
                new Person(Guid.NewGuid(), "Petrov", "Petr", null, new DateTime(1985, 5, 15))
            };
            _repositoryMock.Setup(r => r.GetAll()).Returns(persons);

            // Act
            var result = _service.FindByName("").ToList();

            // Assert
            // Поскольку string.Contains("") возвращает true для любой строки, вернутся все.
            Assert.Equal(2, result.Count);
            _repositoryMock.Verify(r => r.GetAll(), Times.Once);
        }

        #endregion
    }
}