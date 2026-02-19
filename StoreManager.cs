using System;
using System.Collections.Generic;
using System.Linq;

namespace JewelryStore
{
    public class StoreManager
    {
        private Catalog catalog = new Catalog();
        private List<Order> orders = new List<Order>();
        private List<Customer> customers = new List<Customer>();
        private int nextOrderId = 1000;
        private int nextCustomerId = 1;
        private decimal dailyRevenue = 0;
        private int dailyOrdersCount = 0;
        private DateTime currentDate = DateTime.Now.Date;

        public void InitializeCatalog()
        {
            catalog.AddJewelryItem(new JewelryItem(101, "Кольцо обручальное", "Классика", 45000, 3.5m, 5, "Золото", 585, "Бриллиант"));
            catalog.AddJewelryItem(new JewelryItem(102, "Кольцо с бриллиантом", "Престиж", 85000, 4.2m, 3, "Золото", 585, "Бриллиант"));
            catalog.AddJewelryItem(new JewelryItem(103, "Серьги", "Эксклюзив", 125000, 6.8m, 2, "Золото", 750, "Изумруд"));
            catalog.AddJewelryItem(new JewelryItem(104, "Серьги-пусеты", "Изумрудный", 67000, 3.1m, 4, "Золото", 750, "Изумруд"));
            catalog.AddJewelryItem(new JewelryItem(105, "Подвеска", "Серебряная сказка", 8500, 2.3m, 10, "Серебро", 925, "Фианит"));
            catalog.AddJewelryItem(new JewelryItem(106, "Кулон", "Нежность", 12400, 3.0m, 8, "Серебро", 925, "Без камней"));
            catalog.AddJewelryItem(new JewelryItem(107, "Обручальное кольцо", "Платинум", 185000, 5.2m, 2, "Платина", 950, "Бриллиант"));
            catalog.AddJewelryItem(new JewelryItem(108, "Помолвочное кольцо", "Элит", 245000, 4.8m, 1, "Платина", 950, "Бриллиант"));
            catalog.AddJewelryItem(new JewelryItem(109, "Браслет", "Золотая осень", 35000, 8.5m, 6, "Золото", 585, "Рубин"));
            catalog.AddJewelryItem(new JewelryItem(110, "Браслет", "Модерн", 28700, 7.2m, 7, "Серебро", 925, "Сапфир"));
        }

        public void AddCustomer(Customer customer) 
        { 
            customer.Id = GetNextCustomerId(); 
            customers.Add(customer); 
        }
        
        public Order CreateOrder(Customer customer) 
        { 
            Order o = new Order(GetNextOrderId(), customer); 
            orders.Add(o); 
            return o; 
        }
        
        public Catalog GetCatalog() => catalog;
        public int GetNextOrderId() => nextOrderId++;
        public int GetNextCustomerId() => nextCustomerId++;
        
        public Customer FindCustomerByPhone(string phone) => 
            customers.FirstOrDefault(c => c.Phone == phone);
            
        public Customer FindCustomerByEmail(string email) => 
            customers.FirstOrDefault(c => c.Email == email);
            
        public Customer FindCustomerById(int id) => 
            customers.FirstOrDefault(c => c.Id == id);
            
        public List<Order> GetActiveOrders() => 
            orders.Where(o => o.Status != "выдан" && o.Status != "отменен").ToList();
            
        public List<Order> GetAllOrders() => orders;
        public List<Customer> GetAllCustomers() => customers;

        public bool CompleteOrder(int orderId, decimal paidAmount)
        {
            Order order = orders.FirstOrDefault(o => o.Id == orderId);
            if (order == null) return false;
            
            decimal total = order.CalculateTotal();
            if (paidAmount >= total)
            {
                order.UpdateStatus("готов");
                foreach (var item in order.GetItems()) 
                    item.Jewelry.Sell(item.Quantity);
                    
                if (currentDate == DateTime.Now.Date) 
                { 
                    dailyRevenue += total; 
                    dailyOrdersCount++; 
                }
                else 
                { 
                    currentDate = DateTime.Now.Date; 
                    dailyRevenue = total; 
                    dailyOrdersCount = 1; 
                }
                return true;
            }
            return false;
        }

        public (decimal dailyRevenue, int dailyOrders, decimal monthlyRevenue, decimal avgCheck, string bestSellingItem) GetStoreStats()
        {
            decimal avg = dailyOrdersCount > 0 ? dailyRevenue / dailyOrdersCount : 0;
            var sales = new Dictionary<string, int>();
            
            foreach (var o in orders)
                foreach (var i in o.GetItems())
                {
                    if (sales.ContainsKey(i.Jewelry.Name)) 
                        sales[i.Jewelry.Name] += i.Quantity;
                    else 
                        sales[i.Jewelry.Name] = i.Quantity;
                }
                
            string best = sales.OrderByDescending(s => s.Value).FirstOrDefault().Key ?? "Нет данных";
            return (dailyRevenue, dailyOrdersCount, dailyRevenue * 30, avg, best);
        }

        public Dictionary<string, int> GetSalesByMetal()
        {
            var stats = new Dictionary<string, int>();
            foreach (var o in orders)
                foreach (var i in o.GetItems())
                {
                    if (stats.ContainsKey(i.Jewelry.MetalType)) 
                        stats[i.Jewelry.MetalType] += i.Quantity;
                    else 
                        stats[i.Jewelry.MetalType] = i.Quantity;
                }
            return stats;
        }

        public Dictionary<string, int> GetSalesByStone()
        {
            var stats = new Dictionary<string, int>();
            foreach (var o in orders)
                foreach (var i in o.GetItems())
                {
                    if (stats.ContainsKey(i.Jewelry.Stones)) 
                        stats[i.Jewelry.Stones] += i.Quantity;
                    else 
                        stats[i.Jewelry.Stones] = i.Quantity;
                }
            return stats;
        }
    }
}