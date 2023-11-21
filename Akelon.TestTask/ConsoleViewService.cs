using Akelon.TestTask.Dto;
using BetterConsoles.Tables;
using BetterConsoles.Tables.Configuration;

namespace Akelon.TestTask;

/// <summary>
/// Сервис представления консоли.
/// </summary>
public class ConsoleViewService
{
    /// <summary>
    /// Предложить пользователю напечатать пользователю путь до файла.
    /// </summary>
    /// <returns> Путь до файла.</returns>
    public static string SuggestWriteFilePath()
    {
        Console.WriteLine("Укажите путь до файла с данными:");
        var pathToFile = string.Empty;
        while (string.IsNullOrEmpty(pathToFile))
        {
            if (!ConsoleCommand.TryReadStringLine(out pathToFile))
                continue;

            if (!ProductExcelService.IsValidPathToFile(pathToFile))
            {
                pathToFile = string.Empty;
                Console.WriteLine("Укажите путь до файла с данными:");
                continue;
            }
        }

        return pathToFile;
    }

    /// <summary>
    /// Напечатать действия для excel файлом.
    /// </summary>
    public static void PrintActions()
    {
        Console.WriteLine();

        Console.WriteLine("Введите цифру действия, которое нужно выполнить:");
        Console.WriteLine("1. Получить по именованию товара информацию о клиентах, которые его заказывали");
        Console.WriteLine("2. Изменение информации контактного лица клиента");
        Console.WriteLine("3. Получить золотого клиента за определенный месяц года");
        Console.WriteLine("4. Выход");
    }

    /// <summary>
    /// Напечатать покупателей.
    /// </summary>
    /// <param name="excelService"> Сервис работы с продуктами в excel файле.</param>
    /// <returns> true - покупатели напечатаны, false - покупатели не напечатаны.</returns>
    public static void PrintBuyers(ProductExcelService excelService)
    {
        Console.WriteLine("Введите наименование товара:");
        if (!ConsoleCommand.TryReadStringLine(out var productName))
            return;

        var buyers = excelService.GetBuyers(productName);

        var table = new Table("Дата", "Кол-во", "ФИО", "Организация");
        table.Config = TableConfig.Unicode();
        buyers.ForEach(b => table = table.AddRow(b.OrderDate.ToString("d"), b.ProductCount, $"{b.LastName} {b.FirstName} {b.MiddleName}", b.CompanyName));
        Console.Write(table.ToString());
    }

    /// <summary>
    /// Обновить клиента.
    /// </summary>
    /// <param name="excelService"> Сервис работы с продуктами в excel файле.</param>
    /// <returns> true - клиент не обновлён, false - клиент не обновлён.</returns>
    public static void ChangeClientInfo(ProductExcelService excelService)
    {
        Console.WriteLine("Введете код клиента, для которого нужно изменить информацию:");
        if (!ConsoleCommand.TryReadNumberLine(out var clientCode))
            return;

        if (excelService.GetClients([clientCode]).Count == 0)
            return;

        Console.WriteLine("Введете новое название организации:");
        if (!ConsoleCommand.TryReadStringLine(out var newCompanyName))
            return;

        Console.WriteLine("Введете новую фамилию контактного лица:");
        if (!ConsoleCommand.TryReadStringLine(out var newLastName))
            return;

        Console.WriteLine("Введете новое имя контактного лица:");
        if (!ConsoleCommand.TryReadStringLine(out var newFirstName))
            return;

        Console.WriteLine("Введете новое отчество контактного лица (если есть):");
        if (!ConsoleCommand.TryReadStringLine(out var newMiddleName))
            return;

        var updateClientDto = new UpdateClientDto
        {
            FirstName = newFirstName,
            LastName = newLastName,
            MiddleName = newMiddleName,
            CompanyName = newCompanyName,
        };
        Console.WriteLine(excelService.UpdateClient(clientCode, updateClientDto));
    }

    /// <summary>
    /// Напечатать золотых клиентов.
    /// </summary>
    /// <param name="excelService"> Сервис работы с продуктами в excel файле.</param>
    /// <returns> true - клиенты напечатаны, false - клиенты не напечатаны.</returns>
    public static void PrintGoldClients(ProductExcelService excelService)
    {
        Console.WriteLine("Введите год:");
        if (!ConsoleCommand.TryReadNumberLine(out var ordersYear))
            return;

        Console.WriteLine("Введите месяц:");
        if (!ConsoleCommand.TryReadNumberLine(out var ordersMonth))
            return;

        var goldClients = excelService.GetGoldClients(ordersYear, ordersMonth);
        if (goldClients.Count == 0)
        {
            Console.WriteLine("Заказов за эту дату не найдено, невозможно определить золотого клиента");
            return;
        }

        var table = new Table("Код", "ФИО", "Организация");
        table.Config = TableConfig.Unicode();
        goldClients.ForEach(b => table = table.AddRow(b.Code, $"{b.LastName} {b.FirstName} {b.MiddleName}", b.CompanyName));
        Console.Write(table.ToString());
    }
}
