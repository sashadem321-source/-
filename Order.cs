using System;
using System.Collections.Generic;
using System.Linq;

namespace JewelryStore
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? PickupDate { get; set; }
        public string Status { get; set; }
        public bool IsGift { get; set; }
        public bool HasEngraving { get; set; }

        private Customer customer;
        private List<OrderItem> items = new List<OrderItem>();
        private string giftMessage = "";
        private string engravingText = "";

        public class OrderItem
        {
            public JewelryItem Jewelry { get; set; }
            public int Quantity { get; set; }
            public decimal PriceAtPurchase { get; set; }
        }

        public Order(int id, Customer customer)
        {
            Id = id;
            this.customer = customer;
            OrderDate = DateTime.Now;
            Status = "оформлен";
            IsGift = false;
            HasEngraving = false;
        }

        public bool AddItem(JewelryItem jewelry, int quantity)
        {
            if (jewelry.StockQuantity >= quantity)
            {
                items.Add(new OrderItem { Jewelry = jewelry, Quantity = quantity, PriceAtPurchase = jewelry.BasePrice });
                return true;
            }
            return false;
        }

        public decimal CalculateTotal()
        {
            decimal total = items.Sum(i => i.PriceAtPurchase * i.Quantity);
            total = total * (100 - customer.GetPersonalDiscount()) / 100;
            if (total >= 100000) total *= 0.97m;
            if (IsGift) total += 500;
            if (HasEngraving) total += 1000;
            return total;
        }

        public (decimal monthlyPayment, int months) CalculateInstallment(int months = 6)
        {
            if (months > 12) months = 12;
            if (months < 1) months = 1;
            decimal total = CalculateTotal();
            decimal monthly = total / months;
            if (monthly < 5000 && months > 1)
            {
                months = (int)Math.Ceiling(total / 5000);
                monthly = total / months;
            }
            return (monthly, months);
        }

        public void AddGiftWrapping(string message = "") { IsGift = true; giftMessage = message; }
        
        public bool AddEngraving(string text) 
        { 
            if (text.Length <= 30) 
            { 
                HasEngraving = true; 
                engravingText = text; 
                return true; 
            } 
            return false; 
        }
        
        public bool CreatePreOrder(JewelryItem jewelry) 
        { 
            if (jewelry.StockQuantity == 0) 
            { 
                PickupDate = DateTime.Now.AddDays(30); 
                Status = "предзаказ"; 
                return true; 
            } 
            return false; 
        }
        
        public List<OrderItem> GetItems() => items;
        
        public void UpdateStatus(string newStatus) 
        { 
            Status = newStatus; 
            if (newStatus == "готов") 
                PickupDate = DateTime.Now.AddDays(1); 
            else if (newStatus == "выдан") 
                PickupDate = DateTime.Now; 
        }

        public void ShowOrderInfo()
        {
            Console.WriteLine($"=== ЗАКАЗ №{Id} ===");
            Console.WriteLine($"Клиент: {customer.FullName}");
            Console.WriteLine($"Дата: {OrderDate:dd.MM.yyyy HH:mm}, Статус: {Status}");
            Console.WriteLine("\nСостав:");
            foreach (var i in items)
                Console.WriteLine($"  {i.Jewelry.Name} x{i.Quantity} - {i.PriceAtPurchase * i.Quantity:F0} руб.");
            if (IsGift) 
                Console.WriteLine($"\nПодарочная упаковка: ДА{(giftMessage != "" ? $"\n  Сообщение: {giftMessage}" : "")}");
            if (HasEngraving) 
                Console.WriteLine($"Гравировка: ДА\n  Текст: {engravingText}");
            Console.WriteLine($"\nОбщая стоимость: {CalculateTotal():F0} руб.");
            Console.WriteLine($"Скидка клиента: {customer.GetPersonalDiscount()}%");
            var inst = CalculateInstallment();
            Console.WriteLine($"Рассрочка: {inst.monthlyPayment:F0} руб./мес. на {inst.months} мес.");
            if (PickupDate.HasValue) 
                Console.WriteLine($"Готовность: {PickupDate.Value:dd.MM.yyyy}");
        }
    }
}