using System;
using System.Collections.Generic;

namespace MinimalApiExample.BusinessLogic;

/// <summary>
/// Определяет контракт бизнес-сервиса для работы с сущностями типа <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">Тип сущности, с которой работает сервис.</typeparam>
/// <remarks>
/// Сервисный слой может содержать дополнительную бизнес-логику (валидацию, вычисления,
/// преобразования) поверх базовых CRUD-операций, предоставляемых репозиторием.
/// </remarks>
public interface IService<T>
{
    /// <summary>
    /// Возвращает все сущности из хранилища.
    /// </summary>
    /// <returns>Перечисление всех сущностей типа <typeparamref name="T"/>.</returns>
    public IEnumerable<T> GetAll();

    /// <summary>
    /// Добавляет новую сущность в хранилище.
    /// </summary>
    /// <param name="item">Добавляемая сущность.</param>
    /// <returns>
    /// <see langword="true"/>, если операция выполнена успешно; 
    /// <see langword="false"/>, если произошла ошибка (например, нарушение бизнес-правил или проблема с хранилищем).
    /// </returns>
    public bool Add(T item);

    /// <summary>
    /// Обновляет существующую сущность в хранилище.
    /// </summary>
    /// <param name="item">Сущность с обновлёнными данными. Идентификатор должен совпадать с существующей записью.</param>
    /// <returns>
    /// <see langword="true"/>, если обновление выполнено успешно; 
    /// <see langword="false"/>, если сущность с таким идентификатором не найдена или произошла ошибка.
    /// </returns>
    public bool Update(T item);

    /// <summary>
    /// Удаляет сущность из хранилища.
    /// </summary>
    /// <param name="item">Удаляемая сущность. Идентификатор используется для поиска.</param>
    /// <returns>
    /// <see langword="true"/>, если удаление выполнено успешно; 
    /// <see langword="false"/>, если сущность не найдена или произошла ошибка.
    /// </returns>
    public bool Delete(T item);

    /// <summary>
    /// Находит сущность по её уникальному идентификатору.
    /// </summary>
    /// <param name="id">Уникальный идентификатор сущности.</param>
    /// <returns>
    /// Найденная сущность типа <typeparamref name="T"/> или <see langword="null"/>, если сущность с указанным <paramref name="id"/> не найдена.
    /// </returns>
    public T? FindById(Guid id);

    /// <summary>
    /// Выполняет поиск сущностей по имени (строковому полю, например, фамилии или названию).
    /// </summary>
    /// <param name="name">Строка для поиска (может быть частичным совпадением).</param>
    /// <returns>Перечисление сущностей, соответствующих критерию поиска. Если совпадений нет, возвращается пустая коллекция.</returns>
    /// <remarks>
    /// Конкретная реализация определяет, какое именно поле (или поля) участвует в поиске,
    /// а также чувствительность к регистру и поддержку частичного совпадения.
    /// </remarks>
    public IEnumerable<T> FindByName(string name);
}