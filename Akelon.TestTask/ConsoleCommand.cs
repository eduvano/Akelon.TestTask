namespace Akelon.TestTask;

/// <summary>
/// Дополнительные консоли команды.
/// </summary>
public class ConsoleCommand
{
    /// <summary>
    /// Прочитать введённую пользователем строку.
    /// </summary>
    /// <param name="input"> Введенная строка.</param>
    /// <returns> true - пользователь ввел не пустое значение, false - пользователь ввел пустое значение.</returns>
    public static bool TryReadStringLine(out string input)
    {
        var inputLine = Console.ReadLine();
        if (string.IsNullOrEmpty(inputLine))
        {
            Console.WriteLine("Введено пустое значение");
            Console.WriteLine();

            input = string.Empty;

            return false;
        }

        input = inputLine;
        return true;
    }

    /// <summary>
    /// Прочитать введённое пользователем число.
    /// </summary>
    /// <param name="number"> Введенное число.</param>
    /// <returns> true - пользователь ввел число, false - пользовать ввел не число.</returns>
    public static bool TryReadNumberLine(out int number)
    {
        Console.WriteLine();

        var inputLine = Console.ReadLine();
        if (int.TryParse(inputLine, out number))
            return true;

        Console.WriteLine("Введено не числовое значение");
        Console.WriteLine();

        return false;
    }

    /// <summary>
    /// Напечатать большое пустое пространство.
    /// </summary>
    public static void PrintBigSpace()
    {
        Console.WriteLine();
        Console.WriteLine("-------------------------------");
        Console.WriteLine();
    }

}
