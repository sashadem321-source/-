using System;

namespace JewelryStore
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== ЮВЕЛИРНЫЙ САЛОН 'БРИЛЛИАНТ' ===\n");
            new StoreMenu().ShowMainMenu();
            Console.WriteLine("\nСпасибо за визит! До новых встреч!");
            Console.ReadKey();
        }
    }
}