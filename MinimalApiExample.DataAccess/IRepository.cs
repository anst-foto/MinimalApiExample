using System.Collections.Generic;

namespace MinimalApiExample.DataAccess;

/// <summary>
/// Определяет базовый контракт для репозитория, работающего с сущностями типа <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">Тип сущности, с которой работает репозиторий.</typeparam>
public interface IRepository<T>
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
    /// <see langword="false"/>, если произошла ошибка (например, дублирование идентификатора или проблема ввода-вывода).
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
}