using System;
using System.Collections.Generic;
using System.Linq;

namespace JewelryStore
{
    public class Catalog
    {
        private List<JewelryItem> items = new List<JewelryItem>();
        private Dictionary<string, List<JewelryItem>> collections = new Dictionary<string, List<JewelryItem>>();
        private Dictionary<JewelryItem, int> viewCount = new Dictionary<JewelryItem, int>();

        public void AddJewelryItem(JewelryItem item)
        {
            items.Add(item);
            if (!collections.ContainsKey(item.Collection))
                collections[item.Collection] = new List<JewelryItem>();
            collections[item.Collection].Add(item);
            viewCount[item] = 0;
        }

        public List<JewelryItem> GetAllItems() => items;

        public List<JewelryItem> GetItemsByCollection(string name) =>
            collections.ContainsKey(name) ? new List<JewelryItem>(collections[name]) : new List<JewelryItem>();

        public List<JewelryItem> FindByMetal(string metal) =>
            items.Where(i => i.MetalType.Equals(metal, StringComparison.OrdinalIgnoreCase) && i.StockQuantity > 0).ToList();

        public List<JewelryItem> FindByKarat(int karat) =>
            items.Where(i => i.Karat == karat && i.StockQuantity > 0).ToList();

        public List<JewelryItem> FindByStone(string stone) =>
            items.Where(i => i.Stones.Contains(stone, StringComparison.OrdinalIgnoreCase) && i.StockQuantity > 0).ToList();

        public List<JewelryItem> FindByPriceRange(decimal min, decimal max) =>
            items.Where(i => i.CalculateFinalPrice() >= min && i.CalculateFinalPrice() <= max && i.StockQuantity > 0).ToList();

        public void RecordView(JewelryItem item) { if (viewCount.ContainsKey(item)) viewCount[item]++; }

        public List<JewelryItem> GetPopularItems(int count = 5) =>
            viewCount.OrderByDescending(v => v.Value).Take(count).Select(v => v.Key).ToList();

        public List<JewelryItem> GetLowStockItems(int threshold = 2) =>
            items.Where(i => i.StockQuantity <= threshold && i.StockQuantity > 0).ToList();

        public List<JewelryItem> GetNewArrivals(int days = 30) =>
            items.Where(i => i.AddedDate >= DateTime.Now.AddDays(-days)).ToList();

        public List<JewelryItem> Search(string query) =>
            items.Where(i => i.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                            i.Collection.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                            i.MetalType.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                            i.Stones.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();

        public JewelryItem FindById(int id) => items.FirstOrDefault(i => i.Id == id);

        public void ShowCatalog()
        {
            Console.WriteLine("=== КАТАЛОГ ===");
            foreach (var col in collections)
            {
                Console.WriteLine($"\n--- {col.Key} ---");
                foreach (var item in col.Value)
                    Console.WriteLine($"  [{item.Id}] {item.Name} - {item.CalculateFinalPrice():F0} руб. (в наличии: {item.StockQuantity})");
            }
        }
    }
}