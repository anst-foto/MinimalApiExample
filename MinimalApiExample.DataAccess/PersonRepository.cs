using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace MinimalApiExample.DataAccess;

/// <summary>
/// Реализация репозитория для сущностей <see cref="Person"/>, 
/// использующая файл JSON в качестве хранилища.
/// </summary>
/// <remarks>
/// Все операции с файлом выполняются синхронно. При возникновении ошибок ввода-вывода или десериализации 
/// методы возвращают <see langword="false"/>, а исключения не выбрасываются (кроме конструктора).
/// </remarks>
public class PersonRepository : IRepository<Person>
{
    private readonly string _path;

    /// <summary>
    /// Инициализирует новый экземпляр репозитория.
    /// </summary>
    /// <param name="path">Путь к файлу JSON, используемому для хранения данных. 
    /// По умолчанию — "persons.json".</param>
    /// <exception cref="ArgumentNullException">Выбрасывается, если <paramref name="path"/> равен <see langword="null"/> или состоит из пробелов.</exception>
    public PersonRepository(string path = "persons.json")
    {
        if (string.IsNullOrWhiteSpace(path)) 
            throw new ArgumentNullException("Путь к файлу не может быть пустым");
        
        _path = path;
    }

    /// <inheritdoc/>
    public bool Add(Person item)
    {
        try
        {
            var persons = GetAll().ToList();
            persons.Add(item);//FIXME: Добавить сравнение по id, чтобы не было дублей
            var json = JsonSerializer.Serialize(persons);
            File.WriteAllText(_path, json);

            return true;
        }        
        catch(Exception e)
        {
            return false;
        }
    }

    /// <inheritdoc/>
    public bool Delete(Person item)
    {
        try
        {
            var persons = GetAll().ToList();
            var person = persons.SingleOrDefault(p => p.Id == item.Id);
            
            if (person is null) return false;

            persons.Remove(person);

            var json = JsonSerializer.Serialize(persons);
            File.WriteAllText(_path, json);

            return true;
        }        
        catch(Exception e)
        {
            return false;
        }
    }

    /// <inheritdoc/>
    public IEnumerable<Person> GetAll()
    {
        if (!File.Exists(_path)) return [];

        var json = File.ReadAllText(_path);
        return JsonSerializer.Deserialize<IEnumerable<Person>>(json) ?? [];
    }

    /// <inheritdoc/>
    public bool Update(Person item)
    {
        try
        {
            var persons = GetAll().ToList();
            var person = persons.SingleOrDefault(p => p.Id == item.Id);
            
            if (person is null) return false;
            
            persons.Remove(person);
            persons.Add(item);

            var json = JsonSerializer.Serialize(persons);
            File.WriteAllText(_path, json);

            return true;
        }        
        catch(Exception e)
        {
            return false;
        }
    }
}