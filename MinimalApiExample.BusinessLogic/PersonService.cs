using System;
using System.Collections.Generic;
using System.Linq;
using MinimalApiExample.DataAccess;

namespace MinimalApiExample.BusinessLogic;

/// <summary>
/// Реализация бизнес-сервиса для работы с сущностями <see cref="Person"/>.
/// </summary>
/// <remarks>
/// Сервис выступает в роли посредника между внешним миром (контроллерами/клиентами) и репозиторием.
/// Помимо проксирования CRUD-операций, содержит дополнительную логику поиска (<see cref="FindByName"/>)
/// и обработки отсутствия данных (<see cref="FindById"/>).
/// </remarks>
public class PersonService : IService<Person>
{
    private readonly IRepository<Person> _repository;

    /// <summary>
    /// Инициализирует новый экземпляр сервиса с указанным репозиторием.
    /// </summary>
    /// <param name="repository">Репозиторий для доступа к данным сущностей <see cref="Person"/>.</param>
    public PersonService(IRepository<Person> repository)
    {
        _repository = repository;
    }

    /// <inheritdoc/>
    public bool Add(Person item) => _repository.Add(item);

    /// <inheritdoc/>
    public bool Delete(Person item) => _repository.Delete(item);

    /// <summary>
    /// Находит человека по уникальному идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор искомого человека.</param>
    /// <returns>
    /// Объект <see cref="Person"/> с указанным <paramref name="id"/>,
    /// или <see langword="null"/>, если такой человек не найден.
    /// </returns>
    public Person? FindById(Guid id)
    {
        var persons = _repository.GetAll();
        if (!persons.Any()) return null;

        var person = persons.SingleOrDefault(p => p.Id == id);
        return person;
    }

    /// <summary>
    /// Выполняет поиск людей по имени (фамилии, имени или отчеству).
    /// </summary>
    /// <param name="name">Часть имени для поиска (регистрозависимый поиск).</param>
    /// <returns>
    /// Перечисление людей, у которых фамилия, имя или отчество содержат подстроку <paramref name="name"/>.
    /// Если совпадений нет, возвращается пустая коллекция.
    /// </returns>
    /// <remarks>
    /// Поиск выполняется по полям <see cref="Person.LastName"/>, <see cref="Person.FirstName"/>
    /// и <see cref="Person.Patronymic"/> (если отчество не <see langword="null"/>).
    /// Регистр учитывается (т.е. поиск чувствителен к регистру).
    /// </remarks>
    public IEnumerable<Person> FindByName(string name)
    {
        var persons = _repository.GetAll();
        if (!persons.Any()) return [];

        var result = persons.Where(p => 
            p.LastName.Contains(name) || 
            p.FirstName.Contains(name) || 
            (p.Patronymic?.Contains(name) ?? false)
        );
        return result;
    }

    /// <inheritdoc/>
    public IEnumerable<Person> GetAll() => _repository.GetAll();

    /// <inheritdoc/>
    public bool Update(Person item) => _repository.Update(item);
}