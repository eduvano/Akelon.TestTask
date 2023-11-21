namespace Akelon.TestTask.Dto;

/// <summary>
/// ДТО покупателя.
/// </summary>
public record BuyerDto
{
    /// <summary>
    /// Наименование компании.
    /// </summary>
    public string CompanyName { get; init; } = null!;

    /// <summary>
    /// Имя.
    /// </summary>
    public string FirstName { get; init; } = null!;

    /// <summary>
    /// Фамилия.
    /// </summary>
    public string LastName { get; init; } = null!;

    /// <summary>
    /// Отчество.
    /// </summary>
    public string? MiddleName { get; init; }

    /// <summary>
    /// Количество продуктов.
    /// </summary>
    public int ProductCount { get; init; }

    /// <summary>
    /// Дата покупки.
    /// </summary>
    public DateTimeOffset OrderDate { get; init; }
}
