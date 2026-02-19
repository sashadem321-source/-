using System;
using System.Collections.Generic;
using System.Linq;

namespace JewelryStore
{
    public class StoreMenu
    {
        private StoreManager manager;
        private Customer currentCustomer = null;

        public StoreMenu()
        {
            manager = new StoreManager();
            manager.InitializeCatalog();
            Customer test = new Customer(manager.GetNextCustomerId(), "Иванов Иван Иванович",
                                       "+7 (999) 123-45-67", "ivanov@mail.com",
                                       new DateTime(1985, 5, 15), new DateTime(2010, 6, 20));
            manager.AddCustomer(test);
        }

        public void ShowMainMenu()
        {
            bool running = true;
            while (running)
            {
                Console.Clear();
                Console.WriteLine("=== ЮВЕЛИРНЫЙ САЛОН 'БРИЛЛИАНТ' ===");
                Console.WriteLine($"Дата: {DateTime.Now:dd.MM.yyyy}");
                Console.WriteLine(currentCustomer != null ? $"Клиент: {currentCustomer.FullName}" : "Гость");
                Console.WriteLine(new string('=', 40));
                Console.WriteLine("1. Каталог украшений");
                Console.WriteLine("2. Поиск украшений");
                Console.WriteLine("3. Оформить заказ");
                Console.WriteLine("4. Мои заказы");
                Console.WriteLine("5. Регистрация клиента");
                Console.WriteLine("6. Личный кабинет");
                Console.WriteLine("7. Статистика салона");
                Console.WriteLine("8. Выход из аккаунта");
                Console.WriteLine("9. Выход");
                Console.Write("Выберите: ");

                switch (Console.ReadLine())
                {
                    case "1": ShowCatalog(); break;
                    case "2": SearchJewelry(); break;
                    case "3": CreateOrder(); break;
                    case "4": ShowOrders(); break;
                    case "5": RegisterCustomer(); break;
                    case "6": CustomerCabinet(); break;
                    case "7": ShowStoreStats(); break;
                    case "8": currentCustomer = null; Console.WriteLine("Вы вышли"); break;
                    case "9": running = false; break;
                    default: Console.WriteLine("Неверный выбор!"); break;
                }
                if (running) { Console.WriteLine("\nНажмите Enter..."); Console.ReadLine(); }
            }
        }

        private void ShowCatalog()
        {
            Console.Clear();
            manager.GetCatalog().ShowCatalog();
            Console.WriteLine("\nФильтры: 1-Металл, 2-Проба, 3-Камни, 4-Цена, 5-Назад");
            switch (Console.ReadLine())
            {
                case "1": FilterByMetal(); break;
                case "2": FilterByKarat(); break;
                case "3": FilterByStone(); break;
                case "4": FilterByPrice(); break;
            }
        }

        private void FilterByMetal()
        {
            Console.Clear();
            Console.Write("Введите металл (Золото/Серебро/Платина): ");
            var res = manager.GetCatalog().FindByMetal(Console.ReadLine());
            Console.WriteLine($"Найдено: {res.Count}");
            foreach (var i in res) 
                Console.WriteLine($"[{i.Id}] {i.Name} - {i.CalculateFinalPrice():F0} руб.");
        }

        private void FilterByKarat()
        {
            Console.Clear();
            Console.Write("Введите пробу (585/750/925/950/999): ");
            if (int.TryParse(Console.ReadLine(), out int k))
                foreach (var i in manager.GetCatalog().FindByKarat(k))
                    Console.WriteLine($"[{i.Id}] {i.Name} - {i.CalculateFinalPrice():F0} руб.");
        }

        private void FilterByStone()
        {
            Console.Clear();
            Console.Write("Введите камень: ");
            foreach (var i in manager.GetCatalog().FindByStone(Console.ReadLine()))
                Console.WriteLine($"[{i.Id}] {i.Name} - {i.CalculateFinalPrice():F0} руб.");
        }
        private void FilterByPrice()
        {
            Console.Clear();
            Console.Write("Мин. цена: "); 
            decimal min = decimal.Parse(Console.ReadLine());
            Console.Write("Макс. цена: "); 
            decimal max = decimal.Parse(Console.ReadLine());
            foreach (var i in manager.GetCatalog().FindByPriceRange(min, max))
                Console.WriteLine($"[{i.Id}] {i.Name} - {i.CalculateFinalPrice():F0} руб.");
        }

        private void SearchJewelry()
        {
            Console.Clear();
            Console.Write("Введите текст для поиска: ");
            foreach (var i in manager.GetCatalog().Search(Console.ReadLine()))
                Console.WriteLine($"[{i.Id}] {i.Name} - {i.CalculateFinalPrice():F0} руб. ({i.MetalType} {i.Karat}, {i.Stones})");
        }

        private void CreateOrder()
        {
            Console.Clear();
            if (currentCustomer == null)
            {
                Console.Write("Введите телефон: ");
                currentCustomer = manager.FindCustomerByPhone(Console.ReadLine());
                if (currentCustomer == null) 
                { 
                    Console.WriteLine("Клиент не найден"); 
                    return; 
                }
            }

            Order order = manager.CreateOrder(currentCustomer);
            bool adding = true;
            while (adding)
            {
                Console.Clear();
                Console.WriteLine($"Заказ №{order.Id}");
                foreach (var i in manager.GetCatalog().GetAllItems().Where(x => x.StockQuantity > 0))
                    Console.WriteLine($"[{i.Id}] {i.Name} - {i.BasePrice} руб. (в наличии: {i.StockQuantity})");
                
                Console.Write("ID товара (0-выход): ");
                if (int.TryParse(Console.ReadLine(), out int id) && id != 0)
                {
                    var item = manager.GetCatalog().FindById(id);
                    if (item != null)
                    {
                        Console.Write("Количество: ");
                        if (int.TryParse(Console.ReadLine(), out int q))
                        {
                            if (order.AddItem(item, q)) 
                                Console.WriteLine("Добавлено");
                            else 
                                Console.WriteLine("Недостаточно");
                        }
                    }
                }
                else adding = false;
            }

            Console.Write("Подарочная упаковка? (да/нет): ");
            if (Console.ReadLine().ToLower() == "да")
            {
                Console.Write("Текст открытки: ");
                order.AddGiftWrapping(Console.ReadLine());
            }

            Console.Write("Гравировка? (да/нет): ");
            if (Console.ReadLine().ToLower() == "да")
            {
                Console.Write("Текст (до 30 символов): ");
                order.AddEngraving(Console.ReadLine());
            }

            order.ShowOrderInfo();
            Console.Write("Подтвердить заказ? (да/нет): ");
            if (Console.ReadLine().ToLower() == "да")
            {
                Console.Write("Сумма оплаты: ");
                if (decimal.TryParse(Console.ReadLine(), out decimal paid))
                {
                    if (manager.CompleteOrder(order.Id, paid)) 
                        Console.WriteLine("Заказ оформлен!");
                    else 
                        Console.WriteLine("Ошибка");
                }
            }
        }

        private void ShowOrders()
        {
            Console.Clear();
            var active = manager.GetActiveOrders();
            if (active.Count == 0) 
                Console.WriteLine("Нет активных заказов");
            else 
                foreach (var o in active) 
                    Console.WriteLine($"Заказ №{o.Id} от {o.OrderDate:dd.MM.yyyy}, {o.Status}, {o.CalculateTotal():F0} руб.");
        }

        private void RegisterCustomer()
        {
            Console.Clear();
            Console.Write("ФИО: "); 
            string name = Console.ReadLine();
            Console.Write("Телефон: "); 
            string phone = Console.ReadLine();
            Console.Write("Email: "); 
            string email = Console.ReadLine();

            DateTime? bd = null;
            Console.Write("Указать дату рождения? (да/нет): ");
            if (Console.ReadLine().ToLower() == "да")
            {
                Console.Write("Дата (дд.мм.гггг): ");
                if (DateTime.TryParse(Console.ReadLine(), out DateTime d)) 
                    bd = d;
            }

            DateTime? ann = null;
            Console.Write("Указать годовщину? (да/нет): ");
            if (Console.ReadLine().ToLower() == "да")
            {
                Console.Write("Дата (дд.мм.гггг): ");
                if (DateTime.TryParse(Console.ReadLine(), out DateTime d)) 
                    ann = d;
            }

            Customer c = new Customer(0, name, phone, email, bd, ann);
            manager.AddCustomer(c);
            currentCustomer = c;
            Console.WriteLine($"Клиент зарегистрирован! ID: {c.Id}");
        }

        private void CustomerCabinet()
        {
            Console.Clear();
            if (currentCustomer == null)
            {
                Console.Write("Введите телефон: ");
                currentCustomer = manager.FindCustomerByPhone(Console.ReadLine());
            }

            if (currentCustomer != null)
            {
                currentCustomer.ShowCustomerInfo();
                Console.WriteLine("\n1-История заказов, 2-Рекомендации, 3-Назад");
                switch (Console.ReadLine())
                {
                    case "1":
                        foreach (var o in manager.GetAllOrders())
                            Console.WriteLine($"Заказ №{o.Id} - {o.Status} - {o.CalculateTotal():F0} руб.");
                        break;
                    case "2":
                        foreach (var r in currentCustomer.GeneratePersonalRecommendation(manager.GetCatalog().GetAllItems()))
                            Console.WriteLine($"• {r}");
                        break;
                }
            }
            else 
                Console.WriteLine("Клиент не найден");
        }

        private void ShowStoreStats()
        {
            Console.Clear();
            var s = manager.GetStoreStats();
            Console.WriteLine($"Выручка за день: {s.dailyRevenue:F0} руб.");
            Console.WriteLine($"Заказов: {s.dailyOrders}");
            Console.WriteLine($"Прогноз месяца: {s.monthlyRevenue:F0} руб.");
            Console.WriteLine($"Средний чек: {s.avgCheck:F0} руб.");
            Console.WriteLine($"Хит продаж: {s.bestSellingItem}");

            Console.WriteLine("\nПо металлам:");
            foreach (var m in manager.GetSalesByMetal()) 
                Console.WriteLine($"{m.Key}: {m.Value} шт.");

            Console.WriteLine("\nПо камням:");
            foreach (var st in manager.GetSalesByStone()) 
                Console.WriteLine($"{st.Key}: {st.Value} шт.");

            Console.WriteLine("\nНизкий остаток:");
            foreach (var i in manager.GetCatalog().GetLowStockItems())
                Console.WriteLine($"{i.Name} - {i.StockQuantity} шт.");

            Console.WriteLine("\nНовинки:");
            foreach (var i in manager.GetCatalog().GetNewArrivals())
                Console.WriteLine($"{i.Name} - {i.AddedDate:dd.MM.yyyy}");
        }
    }
}