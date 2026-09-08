using System;

namespace MinimalApiExample.DataAccess;

/// <summary>
/// Представляет запись о человеке с персональными данными.
/// </summary>
/// <param name="Id">Уникальный идентификатор человека.</param>
/// <param name="LastName">Фамилия.</param>
/// <param name="FirstName">Имя.</param>
/// <param name="Patronymic">Отчество (может быть <see langword="null"/>).</param>
/// <param name="DateOfBirth">Дата рождения.</param>
public record Person(Guid Id, string LastName, string FirstName, string? Patronymic, DateTime DateOfBirth);