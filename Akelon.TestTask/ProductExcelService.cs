using Akelon.TestTask.Dto;
using ClosedXML.Excel;

namespace Akelon.TestTask;

/// <summary>
/// Сервис работы с продуктами в excel файле.
/// </summary>
public class ProductExcelService : IDisposable
{
    /// <summary>
    /// Наименование таблицы с продуктами.
    /// </summary>
    private const string PRODUCTS_SHEET_NAME = "Товары";

    /// <summary>
    /// Наименование таблицы с клиентами.
    /// </summary>
    private const string CLIENTS_SHEET_NAME = "Клиенты";

    /// <summary>
    /// Наименование таблицы с заявками.
    /// </summary>
    private const string ORDERS_SHEET_NAME = "Заявки";

    /// <summary>
    /// Excel файл с продуктами.
    /// </summary>
    private readonly XLWorkbook _xLWorkbook;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="pathToFile"> Путь до excel файла.</param>
    /// <exception cref="ArgumentException"> Путь до файла не валидный.</exception>
    public ProductExcelService(string pathToFile)
    {
        if (!IsValidPathToFile(pathToFile))
            throw new ArgumentException($"Файл по пути {pathToFile} недоступен");

        _xLWorkbook = new XLWorkbook(pathToFile);
    }
     
    /// <summary>
    /// Получить покупателей продукта.
    /// </summary>
    /// <param name="productName"> Название продукта.</param>
    /// <returns> Покупатели.</returns>
    public List<BuyerDto> GetBuyers(string productName)
    {
        var productCode = GetProductCodeByName(productName);
        var productOrders = GetProductOrders(productCode);

        var clientCodes = productOrders.Select(x => x.ClientCode).ToList();
        var clients = GetClients(clientCodes);

        var buyers = new List<BuyerDto>();
        foreach (var productOrder in productOrders)
        {
            var client = clients.First(c => c.Code == productOrder.ClientCode);
            buyers.Add(new BuyerDto
            {
                FirstName = client.FirstName,
                LastName = client.LastName,
                MiddleName = client.MiddleName,
                ProductCount = productOrder.ProductCount,
                OrderDate = productOrder.OrderDate,
                CompanyName = client.CompanyName,
            });
        }

        return buyers;
    }

    /// <summary>
    /// Получить золотых клиентов.
    /// </summary>
    /// <param name="ordersYear"> Год заказов.</param>
    /// <param name="ordersMonth"> Месяц заказов.</param>
    /// <returns> Золотые клиенты.</returns>
    public List<ClientDto> GetGoldClients(int ordersYear, int ordersMonth)
    {
        var orders = GetOrdersByDate(ordersYear, ordersMonth);
        if (orders.Count == 0)
            return new List<ClientDto>();

        var clientCodes = orders.Select(o => o.ClientCode).GroupBy(o => o);

        var maxClientOrders = clientCodes.Max(o => o.Count());
        var goldClientCodes = clientCodes.Where(c => c.Count() == maxClientOrders).Select(c => c.Key).ToList();

        return GetClients(goldClientCodes);
    }

    /// <summary>
    /// Обновить клиента в таблице.
    /// </summary>
    /// <param name="clientCode"> Код клиента.</param>
    /// <param name="updateClientDto"> ДТО обновления клиента.</param>
    /// <returns> true - клиент успешно обновлен, false - клиента не удалось обновить.</returns>
    public bool UpdateClient(int clientCode, UpdateClientDto updateClientDto)
    {
        try
        {
            var clientsSheet = _xLWorkbook.Worksheet(CLIENTS_SHEET_NAME);
            var clientCodeCell = clientsSheet.Column("A").Search(clientCode.ToString()).FirstOrDefault();
            if (clientCodeCell is null)
                return false;

            var clientRowNumber = clientCodeCell.Address.RowNumber;
            var clientRow = clientsSheet.Row(clientRowNumber);

            var fullName = $"{updateClientDto.LastName} {updateClientDto.FirstName} {updateClientDto.MiddleName}";
            clientRow.Cell(4).Value = fullName;
            clientRow.Cell(2).Value = updateClientDto.CompanyName;

            _xLWorkbook.Save();

            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Валиден ли путь до файла с продуктами.
    /// </summary>
    /// <param name="pathToFile"> Путь до файла.</param>
    /// <returns> true - путь валиден, false - путь не валиден.</returns>
    public static bool IsValidPathToFile(string pathToFile)
    {
        if (!File.Exists(pathToFile))
        {
            Console.WriteLine("Файл по указанному пути не найден или путь не является валидным");
            return false;
        }
        else if (Path.GetExtension(pathToFile) == "xlsx")
        {
            Console.WriteLine("Файл имеет расширение отличное от xlsx");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Получить заказы по дате.
    /// </summary>
    /// <param name="year"> Год.</param>
    /// <param name="month"> Месяц.</param>
    /// <returns> Заказы.</returns>
    private List<OrderDto> GetOrdersByDate(int year, int month)
    {
        var ordersSheet = _xLWorkbook.Worksheet(ORDERS_SHEET_NAME);
        var orderRowNumbers = ordersSheet
            .Column("F")
            .CellsUsed(c => 
            {
                if (!DateTime.TryParse(c.GetString(), out var orderDate))
                    return false;

                return orderDate.Year == year && orderDate.Month == month;
            })
            .Select(x => x.Address.RowNumber)
            .ToList();

        var orders = new List<OrderDto>();
        foreach (var orderRowNumber in orderRowNumbers)
        {
            var orderCells = ordersSheet.Columns("C,E,F").Select(c => c.Cell(orderRowNumber)).ToArray();
            orders.Add(new OrderDto
            {
                ClientCode = orderCells[0].GetValue<int>(),
                ProductCount = orderCells[1].GetValue<int>(),
                OrderDate = orderCells[2].GetDateTime(),
            });
        }

        return orders;
    }

    /// <summary>
    /// Получить код продукта по имени.
    /// </summary>
    /// <param name="productName"> Имя продукта.</param>
    /// <returns> Код продукта.</returns>
    /// <exception cref="ArgumentException"> Не удалось найти продукт.</exception>
    private string GetProductCodeByName(string productName)
    {
        var productsSheet = _xLWorkbook.Worksheet(PRODUCTS_SHEET_NAME);
        var productNameCell = productsSheet
            .Column("B")
            .Search(productName, System.Globalization.CompareOptions.IgnoreCase)
            .FirstOrDefault();

        if (productNameCell is null)
            throw new ArgumentException($"Не удалось найти продукт с именем '{productName}'");

        return productsSheet.Column("A").Cell(productNameCell.Address.RowNumber).GetString();
    }

    /// <summary>
    /// Получить заказы продукта.
    /// </summary>
    /// <param name="productCode"> Код продукта.</param>
    /// <returns> Заказы.</returns>
    private List<OrderDto> GetProductOrders(string productCode)
    {
        var ordersSheet = _xLWorkbook.Worksheet(ORDERS_SHEET_NAME);
        var orderRowNumbers = ordersSheet
            .Column("B")
            .Search(productCode)
            .Select(x => x.Address.RowNumber)
            .ToList();

        var orders = new List<OrderDto>();
        foreach (var orderRowNumber in orderRowNumbers)
        {
            var orderCells = ordersSheet.Columns("C,E,F").Select(c => c.Cell(orderRowNumber)).ToArray();
            orders.Add(new OrderDto
            {
                ClientCode = orderCells[0].GetValue<int>(),
                ProductCount = orderCells[1].GetValue<int>(),
                OrderDate = orderCells[2].GetDateTime(),
            });
        }

        return orders;
    }

    /// <summary>
    /// Получить клиентов.
    /// </summary>
    /// <param name="clientCodes"> Коды клиентов.</param>
    /// <returns> Клиенты.</returns>
    /// <exception cref="ArgumentException"> Не удалось найти клиента.</exception>
    public List<ClientDto> GetClients(List<int> clientCodes)
    {
        var clientsSheet = _xLWorkbook.Worksheet(CLIENTS_SHEET_NAME);
        var clients = new List<ClientDto>();
        foreach (var clientCode in clientCodes)
        {
            var clientCodeCell = clientsSheet.Column("A").Search(clientCode.ToString()).FirstOrDefault()
                ?? throw new ArgumentException($"Не удалось найти клиента с кодом '{clientCode}'");

            var clientRowNumber = clientCodeCell.Address.RowNumber;

            var fullName = clientsSheet.Column("D").Cell(clientRowNumber).GetString();
            var nameParts = fullName.Split(' ');
            clients.Add(new ClientDto
            {
                CompanyName = clientsSheet.Column("B").Cell(clientRowNumber).GetString(),
                FirstName = nameParts[1],
                LastName = nameParts[0],
                MiddleName = nameParts[2],
                Code = clientCode,
            });
        }

        return clients;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _xLWorkbook.Dispose();
        GC.SuppressFinalize(this);
    }
}
