using System.Text.Json;

namespace Q5_InventoryRecords
{
    // b) Marker interface
    public interface IInventoryEntity
    {
        int Id { get; }
    }

    // a) Immutable record
    public record InventoryItem(int Id, string Name, int Quantity, DateTime DateAdded) : IInventoryEntity;

    // c) Generic inventory logger
    public class InventoryLogger<T> where T : IInventoryEntity
    {
        private readonly List<T> _log = new();
        private readonly string _filePath;

        public InventoryLogger(string filePath)
        {
            _filePath = filePath;
        }

        public void Add(T item) => _log.Add(item);

        public List<T> GetAll() => new(_log);

        public void SaveToFile()
        {
            try
            {
                var json = JsonSerializer.Serialize(_log, new JsonSerializerOptions { WriteIndented = true });
                using var writer = new StreamWriter(_filePath);
                writer.Write(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Save error: {ex.Message}");
            }
        }

        public void LoadFromFile()
        {
            try
            {
                using var reader = new StreamReader(_filePath);
                var json = reader.ReadToEnd();
                var data = JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
                _log.Clear();
                _log.AddRange(data);
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Data file not found. Nothing loaded.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Load error: {ex.Message}");
            }
        }
    }

    // f/g) Integration
    public class InventoryApp
    {
        private InventoryLogger<InventoryItem> _logger;

        public InventoryApp(string filePath)
        {
            _logger = new InventoryLogger<InventoryItem>(filePath);
        }

        public void SeedSampleData()
        {
            _logger.Add(new InventoryItem(1, "Ballpoint Pens (Pack of 10)", 50, DateTime.Today));
            _logger.Add(new InventoryItem(2, "A4 Paper Reams", 25, DateTime.Today.AddDays(-1)));
            _logger.Add(new InventoryItem(3, "Staplers", 10, DateTime.Today.AddDays(-2)));
            _logger.Add(new InventoryItem(4, "Markers", 30, DateTime.Today.AddDays(-5)));
            _logger.Add(new InventoryItem(5, "Folders", 60, DateTime.Today.AddDays(-3)));
        }

        public void SaveData() => _logger.SaveToFile();

        public void LoadData() => _logger.LoadFromFile();

        public void PrintAllItems()
        {
            foreach (var item in _logger.GetAll())
            {
                Console.WriteLine($"#{item.Id} {item.Name} - Qty: {item.Quantity}, Added: {item.DateAdded:d}");
            }
        }

        public static void Main()
        {
            string path = "inventory_data.json";
            // First session
            var app = new InventoryApp(path);
            app.SeedSampleData();
            app.SaveData();

            // Simulate new session
            app = new InventoryApp(path);
            app.LoadData();
            app.PrintAllItems();
        }
    }
}
