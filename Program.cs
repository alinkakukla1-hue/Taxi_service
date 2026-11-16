using System;
using System.Globalization;

namespace Taxi_service
{
    internal class Program
    {
        const int Capacity = 100;
        const int MaxId = 1000000;

        // Опис поїздки через масиви (один індекс — одна поїздка)
        static int[] ids = new int[Capacity];                    // номер поїздки
        static string[] drivers = new string[Capacity];          // водій
        static DateTime[] tripDates = new DateTime[Capacity];    // дата і час
        static string[] pickupAddresses = new string[Capacity];  // адреса де знаходиться пасажир
        static string[] destinations = new string[Capacity];     // адреса призначення
        static double[] distances = new double[Capacity];        // відстань, км
        static decimal[] ratesPerKm = new decimal[Capacity];     // тариф за 1 км
        static bool[] isCash = new bool[Capacity];               // спосіб оплати

        static int count = 0;   // лічильник зайнятих комірок
        static int nextId = 1;

        // Початкове налаштування виконується до Main,
        // щоб у Main лишився тільки цикл меню
        static Program()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8; // для укр мови
            Seed();
        }

        static void Main(string[] args)
        {
            bool running = true;
            while (running)
            {
                ShowMenu();

                string choice = (Console.ReadLine() ?? "").Trim();

                switch (choice)
                {
                    case "1": AddTrip(); break;
                    case "2": PrintAll(); break;
                    case "3": FindById(); break;
                    case "4": DeleteById(); break;
                    case "0": Goodbye(); running = false; break;
                    default: UnknownChoice(); break;
                }
            }
        }


        static void ShowMenu()
        {
            Console.WriteLine();
            Console.WriteLine("Служба таксі (поїздки)");
            PrintCounter();
            Console.WriteLine("1: додати поїздку");
            Console.WriteLine("2: показати всі поїздки");
            Console.WriteLine("3: знайти поїздку за номером");
            Console.WriteLine("4: видалити поїздку за номером");
            Console.WriteLine("0: вихід");
            Console.Write("Ваш вибір: ");
        }


        static void UnknownChoice()
        {
            Console.WriteLine("Такого пункту в меню немає");
        }


        static void Goodbye()
        {
            Console.WriteLine("Роботу завершено.");
        }


        static void PrintCounter()
        {
            Console.WriteLine($"Зайнято комірок: {count} зі {Capacity}.");
        }
        // Просить ціле число. Якщо введе неправильне, повертає його.
        static int ReadInt(string prompt, int min, int max)
        {
            while (true)
            {
                Console.Write(prompt);
                string text = (Console.ReadLine() ?? "").Trim();

                if (!int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int value))
                {
                    Console.WriteLine("Потрібно ввести ціле число. Спробуйте ще раз.");
                    continue;
                }

                if (value < min || value > max)
                {
                    Console.WriteLine($"Число має бути від {min} до {max}. Спробуйте ще раз.");
                    continue;
                }

                return value;
            }
        }


        // Просить дійсне число.
        static double ReadDouble(string prompt, double min, double max)
        {
            while (true)
            {
                Console.Write(prompt);
                string text = (Console.ReadLine() ?? "").Trim().Replace(',', '.');

                // NaN і нескінченність теж розпізнаються як числа, тому відсіюємо їх окремо
                if (!double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out double value)
                    || double.IsNaN(value) || double.IsInfinity(value))
                {
                    Console.WriteLine("Потрібно ввести число. Спробуйте ще раз.");
                    continue;
                }

                if (value < min || value > max)
                {
                    Console.WriteLine($"Число має бути від {min} до {max}. Спробуйте ще раз.");
                    continue;
                }

                return value;
            }
        }

        static string ReadText(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string text = (Console.ReadLine() ?? "").Trim();

                if (text.Length > 0)
                {
                    return text;
                }

                Console.WriteLine("Значення не може бути порожнім. Спробуйте ще раз.");
            }
        }


        // Питає дату й час у форматі дд.мм.рррр гг:хх, доки користувач не введе коректні. Повертає їх.
        static DateTime ReadDate(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string text = (Console.ReadLine() ?? "").Trim();

                if (DateTime.TryParseExact(text, "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture,
                                           DateTimeStyles.None, out DateTime date))
                {
                    return date;
                }

                Console.WriteLine("Дату не розпізнано. Потрібен формат 12.11.2026 14:30. Спробуйте ще раз.");
            }
        }

        // Записує поїздку у сховище. Повертає false, якщо вільних комірок немає
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


        static void Seed()
        {
            Store("Ковальчук І.", new DateTime(2026, 11, 12, 8, 30, 0), "вул. Станційна, 2", "Автовокзал", 1.8, 18m, true);
            Store("Грига Н.", new DateTime(2026, 11, 12, 9, 15, 0), "вул. Українська 19", "ПГФК коледж", 18.2, 20m, false);
            Store("Кополовець П.", new DateTime(2026, 11, 12, 11, 40, 0), "вул. Легоцького, 19а", "ТРЦ Токіо", 4.0, 18m, false);
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


        static void AddTrip()
        {
            if (count >= Capacity)
            {
                Console.WriteLine($"Сховище заповнене: більше за {Capacity} поїздок не вміщується.");
                return;
            }

            Console.WriteLine("Нова поїздка");

            Console.Write("Водій (прізвище та ініціал): ");
            string driver = (Console.ReadLine() ?? "").Trim();
            if (driver.Length == 0)
            {
                Console.WriteLine("Ім'я водія не може бути порожнім. Поїздку не додано.");
                return;
            }

            Console.Write("Дата й час (дд.мм.рррр гг:хх): ");
            string dateText = (Console.ReadLine() ?? "").Trim();
            bool dateOk = DateTime.TryParseExact(dateText, "dd.MM.yyyy HH:mm",
                                                 CultureInfo.InvariantCulture,
                                                 DateTimeStyles.None, out DateTime date);
            if (!dateOk)
            {
                Console.WriteLine("Дату не розпізнано. Потрібен формат 12.11.2026 14:30. Поїздку не додано.");
                return;
            }

            Console.Write("Адреса подачі: ");
            string pickup = (Console.ReadLine() ?? "").Trim();
            if (pickup.Length == 0)
            {
                Console.WriteLine("Адреса подачі не може бути порожньою. Поїздку не додано.");
                return;
            }

            Console.Write("Адреса призначення: ");
            string destination = (Console.ReadLine() ?? "").Trim();
            if (destination.Length == 0)
            {
                Console.WriteLine("Адреса призначення не може бути порожньою. Поїздку не додано.");
                return;
            }

            Console.Write("Відстань, км: ");
            string distText = (Console.ReadLine() ?? "").Trim().Replace(',', '.');
            bool distOk = double.TryParse(distText, NumberStyles.Float,
                                          CultureInfo.InvariantCulture, out double distance);
            
            if (!distOk || double.IsNaN(distance) || double.IsInfinity(distance)
                || distance <= 0 || distance > 1000)
            {
                Console.WriteLine("Відстань має бути числом понад 0 і не більше 1000. Поїздку не додано.");
                return;
            }

            Console.Write("Тариф за 1 км, грн: ");
            string rateText = (Console.ReadLine() ?? "").Trim().Replace(',', '.');
            bool rateOk = decimal.TryParse(rateText, NumberStyles.Number,
                                           CultureInfo.InvariantCulture, out decimal rate);
            if (!rateOk || rate <= 0 || rate > 1000)
            {
                Console.WriteLine("Тариф має бути числом понад 0 і не більше 1000. Поїздку не додано.");
                return;
            }

            Console.Write("Спосіб оплати (1 — готівка, 2 — картка): ");
            string payText = (Console.ReadLine() ?? "").Trim();
            if (payText != "1" && payText != "2")
            {
                Console.WriteLine("Потрібно ввести 1 або 2. Поїздку не додано.");
                return;
            }
            bool cash = payText == "1";

            if (!Store(driver, date, pickup, destination, distance, rate, cash))
            {
                Console.WriteLine($"Сховище заповнене: більше за {Capacity} поїздок не вміщується.");
                return;
            }

            Console.WriteLine($"Додано поїздку №{ids[count - 1]} у позицію {count - 1}.");
            PrintCounter();
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
        }


        static void FindById()
        {
            int id = ReadInt("Номер поїздки для пошуку: ", 1, MaxId);

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
            int id = ReadInt("Номер поїздки для видалення: ", 1, MaxId);

            int index = IndexOfId(id);
            if (index == -1)
            {
                Console.WriteLine($"Поїздки №{id} у списку немає.");
                return;
            }

            Console.WriteLine($"Видалено поїздку №{ids[index]} ({drivers[index]}, " +
                              $"{pickupAddresses[index]} — {destinations[index]}).");

            // Зсуваємо наступні записи вліво
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

            // Очищаємо комірку що звільнилася
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