namespace Akelon.TestTask.Dto;

/// <summary>
/// ДТО обновления клиента.
/// </summary>
public record UpdateClientDto
{
    /// <summary>
    /// Наименование организации.
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
}