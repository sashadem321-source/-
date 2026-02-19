using System;
using System.Collections.Generic;
using System.Linq;

namespace JewelryStore
{
    public class Customer
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? AnniversaryDate { get; set; }
        public string PreferredStyle { get; set; }

        private List<Order> orderHistory = new List<Order>();
        private decimal totalSpent = 0;
        private string status = "Новый";
        private int discountPercent = 0;

        public Customer(int id, string name, string phone, string email,
                       DateTime? birthDate = null, DateTime? anniversary = null)
        {
            Id = id;
            FullName = name;
            Phone = phone;
            Email = email;
            RegistrationDate = DateTime.Now;
            BirthDate = birthDate;
            AnniversaryDate = anniversary;
            PreferredStyle = "классика";
        }

        public void AddOrder(Order order)
        {
            orderHistory.Add(order);
            totalSpent += order.CalculateTotal();
            UpdateStatus();
        }

        public void UpdateStatus()
        {
            if (totalSpent >= 500000) { status = "VIP"; discountPercent = 10; }
            else if (totalSpent >= 200000) { status = "Постоянный+"; discountPercent = 7; }
            else if (totalSpent >= 50000) { status = "Постоянный"; discountPercent = 5; }
            else { status = "Новый"; discountPercent = 0; }
        }

        public decimal GetPersonalDiscount()
        {
            decimal discount = discountPercent;
            if (BirthDate.HasValue && BirthDate.Value.Month == DateTime.Now.Month && BirthDate.Value.Day == DateTime.Now.Day)
                discount += 10;
            if (AnniversaryDate.HasValue && AnniversaryDate.Value.Month == DateTime.Now.Month && AnniversaryDate.Value.Day == DateTime.Now.Day)
                discount += 15;
            return discount;
        }

        public int CalculateAge()
        {
            if (!BirthDate.HasValue) return 0;
            int age = DateTime.Now.Year - BirthDate.Value.Year;
            if (DateTime.Now < BirthDate.Value.AddYears(age)) age--;
            return age;
        }

        public (int totalOrders, decimal totalSpent, string favoriteMetal, string favoriteStone) GetPurchaseStats()
        {
            var metals = new Dictionary<string, int>();
            var stones = new Dictionary<string, int>();
            foreach (var order in orderHistory)
                foreach (var item in order.GetItems())
                {
                    if (metals.ContainsKey(item.Jewelry.MetalType)) metals[item.Jewelry.MetalType] += item.Quantity;
                    else metals[item.Jewelry.MetalType] = item.Quantity;
                    if (stones.ContainsKey(item.Jewelry.Stones)) stones[item.Jewelry.Stones] += item.Quantity;
                    else stones[item.Jewelry.Stones] = item.Quantity;
                }
            return (orderHistory.Count, totalSpent,
                   metals.OrderByDescending(m => m.Value).FirstOrDefault().Key ?? "",
                   stones.OrderByDescending(s => s.Value).FirstOrDefault().Key ?? "");
        }

        public List<string> GeneratePersonalRecommendation(List<JewelryItem> catalog)
        {
            var rec = new List<string>();
            var stats = GetPurchaseStats();
            if (!string.IsNullOrEmpty(stats.favoriteMetal))
                rec.Add($"Вам может понравиться: украшения из {stats.favoriteMetal}");
            if (!string.IsNullOrEmpty(stats.favoriteStone))
                rec.Add($"Рекомендуем с {stats.favoriteStone}");
            rec.Add($"Новинки в стиле {PreferredStyle}");
            return rec;
        }

        public void ShowCustomerInfo()
        {
            Console.WriteLine($"Клиент: {FullName} (ID: {Id})");
            Console.WriteLine($"Телефон: {Phone}, Email: {Email}");
            Console.WriteLine($"Регистрация: {RegistrationDate:dd.MM.yyyy}");
            if (BirthDate.HasValue) Console.WriteLine($"ДР: {BirthDate.Value:dd.MM.yyyy} (возраст: {CalculateAge()})");
            if (AnniversaryDate.HasValue) Console.WriteLine($"Годовщина: {AnniversaryDate.Value:dd.MM.yyyy}");
            Console.WriteLine($"Статус: {status}, Скидка: {GetPersonalDiscount()}%");
            var s = GetPurchaseStats();
            Console.WriteLine($"Заказов: {s.totalOrders}, Сумма: {s.totalSpent:F0} руб.");
            if (!string.IsNullOrEmpty(s.favoriteMetal)) Console.WriteLine($"Любимый металл: {s.favoriteMetal}");
            if (!string.IsNullOrEmpty(s.favoriteStone)) Console.WriteLine($"Любимый камень: {s.favoriteStone}");
        }

        public int GetDiscountPercent() => discountPercent;
    }
}