using System;
using System.Globalization;

namespace Taxi_service
{
    internal class Program
    {
        const int Capacity = 100;

        // Опис поїздки 
        static int[] ids = new int[Capacity];                    // номер поїздки
        static string[] drivers = new string[Capacity];          // водій
        static DateTime[] tripDates = new DateTime[Capacity];    // дата і час
        static string[] pickupAddresses = new string[Capacity];  // адреса де знаходиться пасажир
        static string[] destinations = new string[Capacity];     // адреса призначення
        static double[] distances = new double[Capacity];        // відстань, км
        static decimal[] ratesPerKm = new decimal[Capacity];     // тариф за 1 км
        static bool[] isCash = new bool[Capacity];               // спосіб оплати

        static int count = 0;
        static int nextId = 1;


        static Program()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8; // для укр мови
            Seed();
        }

        static void Main(string[] args)
        {
            PrintCounter();
        }


        static void PrintCounter()
        {
            Console.WriteLine($"Зайнято комірок: {count} зі {Capacity}.");
        }


        // Записує поїздку у сховище
        static bool Store(string driver, DateTime date, string pickup, string destination,
                          double distance, decimal rate, bool cash)
        {
            if (count >= Capacity) return false;

            ids[count] = nextId;
            drivers[count] = driver;
            tripDates[count] = date;
            pickupAddresses[count] = pickup;
            destinations[count] = destination;
            distances[count] = distance;
            ratesPerKm[count] = rate;
            isCash[count] = cash;

            count++;
            nextId++;
            return true;
        }

        // Маршрути поїздок
        static void Seed()
        {
            Store("Ковальчук І.", new DateTime(2026, 11, 12, 8, 30, 0), "вул. Станційна, 2", "ПГФК коледж", 1.8, 18m, true);
            Store("Грига Н.", new DateTime(2026, 11, 12, 9, 15, 0), "вул. Українська 19", "ТРЦ Токіо", 18.2, 20m, false);
            Store("Кополовець П.", new DateTime(2026, 11, 12, 11, 40, 0), "вул. Легоцького, 19а", "Автовокзал", 4.0, 18m, false);
        }


        static decimal Cost(int index)
        {
            return Math.Round((decimal)distances[index] * ratesPerKm[index], 2);
        }


        static int IndexOfId(int id)
        {
            for (int i = 0; i < count; i++)
            {
                if (ids[i] == id) return i;
            }
            return -1;
        }
        static void PrintAll()
        {
            if (count == 0)
            {
                Console.WriteLine("Поїздок поки немає.");
                return;
            }

            Console.WriteLine();
            Console.WriteLine($"{"№",3} | {"Водій",-12} | {"Дата й час",-16} | {"Звідки",-20} | " +
                              $"{"Куди",-20} | {"Км",7} | {"Тариф",7} | {"Оплата",-7} | {"Вартість",9}");
            Console.WriteLine(new string('-', 125));

            decimal total = 0;
            for (int i = 0; i < count; i++)
            {
                string pay = isCash[i] ? "готівка" : "картка";
                Console.WriteLine($"{ids[i],3} | {drivers[i],-12} | {tripDates[i]:dd.MM.yyyy HH:mm} | " +
                                  $"{pickupAddresses[i],-20} | {destinations[i],-20} | " +
                                  $"{distances[i],7:F1} | {ratesPerKm[i],7:F2} | {pay,-7} | {Cost(i),9:F2}");
                total += Cost(i);
            }

            Console.WriteLine(new string('-', 125));
            Console.WriteLine($"Усього поїздок: {count}, загальний виторг: {total:F2} грн.");
            PrintCounter();

            PrintDriverRevenue();
        }


        static void PrintDriverRevenue()
        {
            string[] names = new string[count];
            decimal[] sums = new decimal[count];
            int n = 0;

            for (int i = 0; i < count; i++)
            {
                int k = -1;
                for (int j = 0; j < n; j++)
                {
                    if (names[j] == drivers[i]) { k = j; break; }
                }
                if (k == -1)
                {
                    k = n;
                    names[n] = drivers[i];
                    n++;
                }
                sums[k] += Cost(i);
            }

            Console.WriteLine("Виторг по водіях:");
            for (int j = 0; j < n; j++)
            {
                Console.WriteLine($"  {names[j],-12} {sums[j],10:F2} грн");
            }

            static void FindById()
            {
                Console.Write("Номер поїздки для пошуку: ");
                if (!int.TryParse(Console.ReadLine(), out int id))
                {
                    Console.WriteLine("Номер має бути цілим числом.");
                    return;
                }

                int index = IndexOfId(id);
                if (index == -1)
                {
                    Console.WriteLine($"Поїздки №{id} у списку немає.");
                    return;
                }

                Console.WriteLine($"Знайдено в позиції {index}:");
                Console.WriteLine($"  номер:      {ids[index]}");
                Console.WriteLine($"  водій:      {drivers[index]}");
                Console.WriteLine($"  дата й час: {tripDates[index]:dd.MM.yyyy HH:mm}");
                Console.WriteLine($"  звідки:     {pickupAddresses[index]}");
                Console.WriteLine($"  куди:       {destinations[index]}");
                Console.WriteLine($"  відстань:   {distances[index]:F1} км");
                Console.WriteLine($"  тариф:      {ratesPerKm[index]:F2} грн/км");
                Console.WriteLine($"  оплата:     {(isCash[index] ? "готівка" : "картка")}");
                Console.WriteLine($"  вартість:   {Cost(index):F2} грн");
            }


            static void DeleteById()
            {
                Console.Write("Номер поїздки для видалення: ");
                if (!int.TryParse(Console.ReadLine(), out int id))
                {
                    Console.WriteLine("Номер має бути цілим числом.");
                    return;
                }

                int index = IndexOfId(id);
                if (index == -1)
                {
                    Console.WriteLine($"Поїздки №{id} у списку немає.");
                    return;
                }

                Console.WriteLine($"Видалено поїздку №{ids[index]} ({drivers[index]}, " +
                                  $"{pickupAddresses[index]} — {destinations[index]}).");

                for (int i = index; i < count - 1; i++)
                {
                    ids[i] = ids[i + 1];
                    drivers[i] = drivers[i + 1];
                    tripDates[i] = tripDates[i + 1];
                    pickupAddresses[i] = pickupAddresses[i + 1];
                    destinations[i] = destinations[i + 1];
                    distances[i] = distances[i + 1];
                    ratesPerKm[i] = ratesPerKm[i + 1];
                    isCash[i] = isCash[i + 1];
                }

                count--;

                // Очищаємо звільнену останню комірку
                ids[count] = 0;
                drivers[count] = "";
                tripDates[count] = default;
                pickupAddresses[count] = "";
                destinations[count] = "";
                distances[count] = 0;
                ratesPerKm[count] = 0m;
                isCash[count] = false;

                PrintCounter();
            }
        }
    }
}