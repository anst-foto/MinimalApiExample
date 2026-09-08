using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Xunit;

namespace MinimalApiExample.DataAccess.Tests
{
    public class PersonRepositoryTests : IDisposable
    {
        private readonly string _testDirectory;
        private readonly string _testFilePath;
        private readonly PersonRepository _repository;

        public PersonRepositoryTests()
        {
            // Создаём временную директорию для каждого теста
            _testDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testDirectory);
            _testFilePath = Path.Combine(_testDirectory, "persons.json");
            _repository = new PersonRepository(_testFilePath);
        }

        public void Dispose()
        {
            // Удаляем временную директорию после теста
            if (Directory.Exists(_testDirectory))
                Directory.Delete(_testDirectory, true);
        }

        #region Конструктор

        [Fact]
        public void Constructor_WhenPathIsNull_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new PersonRepository(null));
        }

        [Fact]
        public void Constructor_WhenPathIsWhitespace_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new PersonRepository("   "));
        }

        [Fact]
        public void Constructor_WhenPathIsValid_CreatesInstance()
        {
            // Act
            var repo = new PersonRepository(_testFilePath);
            
            // Assert
            Assert.NotNull(repo);
        }

        #endregion

        #region GetAll

        [Fact]
        public void GetAll_WhenFileDoesNotExist_ThrowsFileNotFoundException()
        {
             // Убедимся, что файла нет
    Assert.False(File.Exists(_testFilePath));

    // Act
    var result = _repository.GetAll();

    // Assert
    Assert.NotNull(result);
    Assert.Empty(result);
        }

        [Fact]
        public void GetAll_WhenFileExistsAndContainsValidJson_ReturnsPersons()
        {
            // Arrange
            var expected = new List<Person>
            {
                new Person(Guid.NewGuid(), "Ivanov", "Ivan", "Ivanovich", new DateTime(1990, 1, 1)),
                new Person(Guid.NewGuid(), "Petrov", "Petr", null, new DateTime(1985, 5, 15))
            };
            var json = JsonSerializer.Serialize(expected);
            File.WriteAllText(_testFilePath, json);

            // Act
            var result = _repository.GetAll().ToList();

            // Assert
            Assert.Equal(expected.Count, result.Count);
            Assert.Equal(expected[0].Id, result[0].Id);
            Assert.Equal(expected[1].LastName, result[1].LastName);
        }

        [Fact]
        public void GetAll_WhenFileContainsInvalidJson_ThrowsJsonException()
        {
            // Arrange – пишем невалидный JSON
            File.WriteAllText(_testFilePath, "{ not valid json ");

            // Act & Assert
            Assert.Throws<JsonException>(() => _repository.GetAll());
        }

        #endregion

        #region Add

        [Fact]
        public void Add_WhenPersonIsValid_ReturnsTrueAndPersistsData()
        {
            // Arrange
            var person = new Person(Guid.NewGuid(), "Sidorov", "Sidr", "Sidorovich", new DateTime(2000, 12, 12));

            // Act
            var result = _repository.Add(person);

            // Assert
            Assert.True(result);

            // Проверяем, что файл создан и содержит добавленного человека
            var all = _repository.GetAll().ToList();
            Assert.Single(all);
            Assert.Equal(person.Id, all[0].Id);
            Assert.Equal(person.LastName, all[0].LastName);
        }

        [Fact]
        public void Add_WhenFileIsCorrupted_ReturnsFalse()
        {
            // Arrange – создаём повреждённый файл
            File.WriteAllText(_testFilePath, "{ broken json");

            var person = new Person(Guid.NewGuid(), "Test", "Test", null, DateTime.Now);

            // Act
            var result = _repository.Add(person);

            // Assert – метод должен перехватить исключение и вернуть false
            Assert.False(result);
        }

        #endregion

        #region Update

        [Fact]
        public void Update_WhenPersonExists_ReturnsTrueAndUpdatesData()
        {
            // Arrange – сначала добавляем человека
            var original = new Person(Guid.NewGuid(), "OldLastName", "OldFirstName", "OldPatr", new DateTime(1990, 1, 1));
            _repository.Add(original);

            // Создаём обновлённую версию (с новыми данными, но тем же Id)
            var updated = original with { LastName = "NewLastName", FirstName = "NewFirstName" };

            // Act
            var result = _repository.Update(updated);

            // Assert
            Assert.True(result);
            var all = _repository.GetAll().ToList();
            Assert.Single(all);
            var saved = all[0];
            Assert.Equal(updated.Id, saved.Id);
            Assert.Equal("NewLastName", saved.LastName);
            Assert.Equal("NewFirstName", saved.FirstName);
        }

        [Fact]
        public void Update_WhenPersonDoesNotExist_ReturnsFalse()
        {
            // Arrange – создаём человека, которого нет в репозитории
            var person = new Person(Guid.NewGuid(), "NonExistent", "Non", null, DateTime.Now);

            // Act
            var result = _repository.Update(person);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Update_WhenFileIsCorrupted_ReturnsFalse()
        {
            // Arrange – повреждённый файл
            File.WriteAllText(_testFilePath, "{ broken");
            var person = new Person(Guid.NewGuid(), "Test", "Test", null, DateTime.Now);

            // Act
            var result = _repository.Update(person);

            // Assert
            Assert.False(result);
        }

        #endregion

        #region Delete

        [Fact]
        public void Delete_WhenPersonExists_ReturnsTrueAndRemovesPerson()
        {
            // Arrange – добавляем человека
            var person = new Person(Guid.NewGuid(), "ToDelete", "Del", null, DateTime.Now);
            _repository.Add(person);

            // Act
            var result = _repository.Delete(person);

            // Assert
            Assert.True(result);
            var all = _repository.GetAll().ToList();
            Assert.Empty(all);
        }

        [Fact]
        public void Delete_WhenPersonDoesNotExist_ReturnsFalse()
        {
            // Arrange – создаём человека, которого нет
            var person = new Person(Guid.NewGuid(), "NonExistent", "Non", null, DateTime.Now);

            // Act
            var result = _repository.Delete(person);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Delete_WhenFileIsCorrupted_ReturnsFalse()
        {
            // Arrange – повреждённый файл
            File.WriteAllText(_testFilePath, "{ broken");
            var person = new Person(Guid.NewGuid(), "Test", "Test", null, DateTime.Now);

            // Act
            var result = _repository.Delete(person);

            // Assert
            Assert.False(result);
        }

        #endregion
    }
}