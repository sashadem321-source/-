using System;

namespace JewelryStore
{
    public class JewelryItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Collection { get; set; }
        public decimal BasePrice { get; set; }
        public decimal Weight { get; set; }
        public int StockQuantity { get; set; }
        public string MetalType { get; set; }
        public int Karat { get; set; }
        public string Stones { get; set; }
        public DateTime AddedDate { get; set; }

        public JewelryItem(int id, string name, string collection, decimal price,
                          decimal weight, int stock, string metal, int karat, string stones)
        {
            Id = id;
            Name = name;
            Collection = collection;
            AddedDate = DateTime.Now;
            BasePrice = price >= 0 ? price : 1000;
            Weight = weight > 0 ? weight : 1;
            StockQuantity = stock >= 0 ? stock : 0;
            MetalType = metal;
            Karat = karat;
            Stones = stones;
        }

        public decimal CalculateFinalPrice(decimal discountPercent = 0)
        {
            decimal finalPrice = BasePrice * (100 - discountPercent) / 100;

            if (MetalType.ToLower() == "платина")
                finalPrice += BasePrice * 0.5m;
            if (Stones.ToLower().Contains("бриллиант"))
                finalPrice += BasePrice * 1.0m;
            else if (Stones.ToLower().Contains("изумруд"))
                finalPrice += BasePrice * 0.8m;
            if (Collection.ToLower().Contains("эксклюзив"))
                finalPrice += BasePrice * 0.3m;

            return finalPrice;
        }

        public string GetCategory()
        {
            decimal price = CalculateFinalPrice();
            if (price <= 10000 && Stones.ToLower() == "без камней") return "Эконом";
            if (price > 10000 && price <= 50000) return "Средний";
            if (price > 50000 && price <= 200000) return "Премиум";
            if (price > 200000 && Stones.ToLower().Contains("бриллиант")) return "Люкс";
            return "Стандарт";
        }

        public string GetCareInstructions()
        {
            string instr = "Рекомендации: ";
            if (MetalType.ToLower() == "золото") instr += "Чистить мягкой тканью. ";
            else if (MetalType.ToLower() == "серебро") instr += "Хранить в закрытой шкатулке. ";
            else if (MetalType.ToLower() == "платина") instr += "Протирать замшевой тканью. ";
            if (Stones.ToLower().Contains("бриллиант")) instr += "Профчистка раз в год. ";
            else if (Stones.ToLower().Contains("изумруд")) instr += "Беречь от перепадов температур. ";
            return instr;
        }

        public bool Sell(int quantity)
        {
            if (StockQuantity >= quantity)
            {
                StockQuantity -= quantity;
                return true;
            }
            return false;
        }

        public override string ToString() => $"{Name} ({Collection}) - {BasePrice} руб., {Weight}г, {MetalType} {Karat}, {Stones}";
    }
}