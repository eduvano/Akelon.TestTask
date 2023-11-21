using Akelon.TestTask;

var pathToFile = ConsoleViewService.SuggestWriteFilePath();

Console.WriteLine("Обработка файла...");
using var excelService = new ProductExcelService(pathToFile);
Console.WriteLine("Файл обработан");
ConsoleCommand.PrintBigSpace();

while (true)
{
    ConsoleViewService.PrintActions();
    if (!ConsoleCommand.TryReadNumberLine(out var selectedAction))
        continue;

    try
    {
        switch (selectedAction)
        {
            case 1:
                ConsoleViewService.PrintBuyers(excelService);
                break;
            case 2:
                ConsoleViewService.ChangeClientInfo(excelService);
                break;
            case 3:
                ConsoleViewService.PrintGoldClients(excelService);
                break;
            case 4:
                return;
            default:
                Console.WriteLine("Действия с таким номером не существует");
                break;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ошибка: {ex.Message}");
    }
}