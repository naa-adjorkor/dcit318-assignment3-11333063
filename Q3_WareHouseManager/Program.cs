namespace Q3_WareHouseManager
{
    // a) Marker interface
    public interface IInventoryItem
    {
        int Id { get; }
        string Name { get; }
        int Quantity { get; set; }
    }

    // b) ElectronicItem
    public class ElectronicItem : IInventoryItem
    {
        public int Id { get; }
        public string Name { get; }
        public int Quantity { get; set; }
        public string Brand { get; }
        public int WarrantyMonths { get; }

        public ElectronicItem(int id, string name, int quantity, string brand, int warrantyMonths)
        {
            Id = id; Name = name; Quantity = quantity; Brand = brand; WarrantyMonths = warrantyMonths;
        }

        public override string ToString() => $"[Electronic] #{Id} {Name} ({Brand}) x{Quantity}, Warranty: {WarrantyMonths}m";
    }

    // c) GroceryItem
    public class GroceryItem : IInventoryItem
    {
        public int Id { get; }
        public string Name { get; }
        public int Quantity { get; set; }
        public DateTime ExpiryDate { get; }

        public GroceryItem(int id, string name, int quantity, DateTime expiryDate)
        {
            Id = id; Name = name; Quantity = quantity; ExpiryDate = expiryDate;
        }

        public override string ToString() => $"[Grocery] #{Id} {Name} x{Quantity}, Expires: {ExpiryDate:d}";
    }

    // e) Custom exceptions
    public class DuplicateItemException : Exception { public DuplicateItemException(string msg) : base(msg) { } }
    public class ItemNotFoundException : Exception { public ItemNotFoundException(string msg) : base(msg) { } }
    public class InvalidQuantityException : Exception { public InvalidQuantityException(string msg) : base(msg) { } }

    // d) Generic repository
    public class InventoryRepository<T> where T : IInventoryItem
    {
        private readonly Dictionary<int, T> _items = new();

        public void AddItem(T item)
        {
            if (_items.ContainsKey(item.Id))
                throw new DuplicateItemException($"Item with ID {item.Id} already exists.");
            _items[item.Id] = item;
        }

        public T GetItemById(int id)
        {
            if (!_items.TryGetValue(id, out var item))
                throw new ItemNotFoundException($"Item with ID {id} not found.");
            return item;
        }

        public void RemoveItem(int id)
        {
            if (!_items.Remove(id))
                throw new ItemNotFoundException($"Item with ID {id} not found.");
        }

        public List<T> GetAllItems() => new(_items.Values);

        public void UpdateQuantity(int id, int newQuantity)
        {
            if (newQuantity < 0) throw new InvalidQuantityException("Quantity cannot be negative.");
            var item = GetItemById(id);
            item.Quantity = newQuantity;
        }
    }

    // f) Manager
    public class WareHouseManager
    {
        private readonly InventoryRepository<ElectronicItem> _electronics = new();
        private readonly InventoryRepository<GroceryItem> _groceries = new();

        public void SeedData()
        {
            _electronics.AddItem(new ElectronicItem(1, "Laptop", 5, "Acer", 24));
            _electronics.AddItem(new ElectronicItem(2, "Router", 10, "TP-Link", 12));
            _electronics.AddItem(new ElectronicItem(3, "Smartphone", 7, "Tecno", 18));

            _groceries.AddItem(new GroceryItem(100, "Rice (5kg)", 20, DateTime.Today.AddMonths(12)));
            _groceries.AddItem(new GroceryItem(101, "Tomato Paste", 35, DateTime.Today.AddMonths(8)));
            _groceries.AddItem(new GroceryItem(102, "Milk", 15, DateTime.Today.AddDays(30)));
        }

        public void PrintAllItems<T>(InventoryRepository<T> repo) where T : IInventoryItem
        {
            foreach (var item in repo.GetAllItems())
                Console.WriteLine(item);
        }

        public void IncreaseStock<T>(InventoryRepository<T> repo, int id, int quantity) where T : IInventoryItem
        {
            try
            {
                var item = repo.GetItemById(id);
                repo.UpdateQuantity(id, item.Quantity + quantity);
                Console.WriteLine($"Stock increased: ID {id} is now {item.Quantity}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[IncreaseStock] {ex.Message}");
            }
        }

        public void RemoveItemById<T>(InventoryRepository<T> repo, int id) where T : IInventoryItem
        {
            try
            {
                repo.RemoveItem(id);
                Console.WriteLine($"Removed item ID {id}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Remove] {ex.Message}");
            }
        }

        public static void Main()
        {
            var manager = new WareHouseManager();
            manager.SeedData();

            Console.WriteLine("Grocery Items:");
            manager.PrintAllItems(manager._groceries);
            Console.WriteLine("\nElectronic Items:");
            manager.PrintAllItems(manager._electronics);

            Console.WriteLine("\n-- Exception Scenarios --");
            // Add duplicate
            try { manager._electronics.AddItem(new ElectronicItem(1, "Duplicate Laptop", 1, "BrandX", 6)); }
            catch (Exception ex) { Console.WriteLine($"[Duplicate] {ex.Message}"); }

            // Remove non-existent
            manager.RemoveItemById(manager._groceries, 999);

            // Update with invalid quantity
            try { manager._groceries.UpdateQuantity(100, -5); }
            catch (Exception ex) { Console.WriteLine($"[InvalidQuantity] {ex.Message}"); }

            // Normal increase
            manager.IncreaseStock(manager._groceries, 100, 10);
        }
    }
}
