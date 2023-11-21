namespace Akelon.TestTask.Dto;

/// <summary>
/// ДТО заказа.
/// </summary>
public record OrderDto
{
    /// <summary>
    /// Код клиента.
    /// </summary>
    public int ClientCode { get; init; }

    /// <summary>
    /// Количеств продукта.
    /// </summary>
    public int ProductCount { get; init; }

    /// <summary>
    /// Дата заказа.
    /// </summary>
    public DateTimeOffset OrderDate { get; init; }
}
