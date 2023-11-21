namespace Akelon.TestTask.Dto;

/// <summary>
/// ДТО клиента.
/// </summary>
public class ClientDto
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
    /// Код клиента.
    /// </summary>
    public int Code { get; init; }
}
