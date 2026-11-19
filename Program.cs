using System;
using System.Globalization;

namespace Taxi_service
{
    internal class Program
    {
        const int Capacity = 100;
        const int MaxId = 1000000;
        const decimal StandardRate = 18m;
        const int MaxTableRows = 100;      // найбільша кількість рядків у таблиці вартості

        // Опис поїздки через масиви (один індекс — одна поїздка)
        static int[] ids = new int[Capacity];                    // номер поїздки
        static string[] drivers = new string[Capacity];          // водій
        static DateTime[] tripDates = new DateTime[Capacity];    // дата і час
        static string[] pickupAddresses = new string[Capacity];  // адреса де знаходиться пасажир
        static string[] destinations = new string[Capacity];     // адреса призначення
        static double[] distances = new double[Capacity];        // відстань, км
        static decimal[] ratesPerKm = new decimal[Capacity];     // тариф за 1 км
        static bool[] isCash = new bool[Capacity];               // спосіб оплати

        static int tripCount = 0;   // лічильник зайнятих комірок
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
            while (true)
            {
                ShowMenu();
                int choice = ReadInt("Ваш вибір: ", 0, 7);

                switch (choice)
                {
                    case 1: AddTrip(); break;
                    case 2: PrintAll(); break;
                    case 3: FindById(); break;
                    case 4: DeleteById(); break;
                    case 5: CostCalculator(); break;
                    case 6: RunTabulation(); break;
                    case 7: ShowAbout(); break;
                    case 0: Goodbye(); return;
                }

                Pause();
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
            Console.WriteLine("5: порахувати вартість поїздки");
            Console.WriteLine("6: таблиця вартості за відстанню");
            Console.WriteLine("7: про програму");
            Console.WriteLine("0: вихід");
        }

        static void Pause()
        {
            Console.WriteLine();
            Console.Write("Натисніть Enter, щоб продовжити...");
            Console.ReadLine();
        }

        static void Goodbye()
        {
            Console.WriteLine("Роботу завершено.");
        }


        static void PrintCounter()
        {
            Console.WriteLine($"Зайнято комірок: {tripCount} зі {Capacity}.");
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
            if (tripCount >= Capacity) return false;

            ids[tripCount] = nextId;
            drivers[tripCount] = driver;
            tripDates[tripCount] = date;
            pickupAddresses[tripCount] = pickup;
            destinations[tripCount] = destination;
            distances[tripCount] = distance;
            ratesPerKm[tripCount] = rate;
            isCash[tripCount] = cash;

            tripCount++;
            nextId++;
            return true;
        }


        static void Seed()
        {
            Store("Ковальчук І.", new DateTime(2026, 11, 12, 8, 30, 0), "вул. Станційна, 2", "Автовокзал", 1.8, 18m, true);
            Store("Грига Н.", new DateTime(2026, 11, 12, 9, 15, 0), "вул. Українська 19", "ПГФК коледж", 18.2, 20m, false);
            Store("Кополовець П.", new DateTime(2026, 11, 12, 11, 40, 0), "вул. Легоцького, 19а", "ТРЦ Токіо", 4.0, 18m, false);
        }


        // Вартість поїздки: відстань × тариф. Нічого не виводить, лише повертає результат
        static decimal CalcCost(double distanceKm, decimal ratePerKm)
        {
            return Math.Round((decimal)distanceKm * ratePerKm, 2);
        }


        static int IndexOfId(int id)
        {
            for (int i = 0; i < tripCount; i++)
            {
                if (ids[i] == id) return i;
            }
            return -1;
        }


        static void AddTrip()
        {
            if (tripCount >= Capacity)
            {
                Console.WriteLine($"Сховище заповнене: більше за {Capacity} поїздок не вміщується.");
                return;
            }

            Console.WriteLine("Нова поїздка");

            string driver = ReadText("Водій (прізвище та ініціал): ");
            DateTime date = ReadDate("Дата й час (дд.мм.рррр гг:хх): ");
            string pickup = ReadText("Адреса подачі: ");
            string destination = ReadText("Адреса призначення: ");
            double distance = ReadDouble("Відстань, км: ", 0.1, 1000);
            decimal rate = (decimal)ReadDouble("Тариф за 1 км, грн: ", 0.1, 1000);
            bool cash = ReadInt("Спосіб оплати (1 — готівка, 2 — картка): ", 1, 2) == 1;

            if (!Store(driver, date, pickup, destination, distance, rate, cash))
            {
                Console.WriteLine($"Сховище заповнене: більше за {Capacity} поїздок не вміщується.");
                return;
            }

            Console.WriteLine($"Додано поїздку №{ids[tripCount - 1]} у позицію {tripCount - 1}.");
            PrintCounter();
        }


        static void PrintAll()
        {
            if (tripCount == 0)
            {
                Console.WriteLine("Поїздок поки немає.");
                return;
            }

            Console.WriteLine();
            Console.WriteLine($"{"№",3} | {"Водій",-12} | {"Дата й час",-16} | {"Звідки",-20} | " +
                              $"{"Куди",-20} | {"Км",7} | {"Тариф",7} | {"Оплата",-7} | {"Вартість",9}");
            Console.WriteLine(new string('-', 125));

            for (int i = 0; i < tripCount; i++)
            {
                PrintRow(i);
            }

            Console.WriteLine(new string('-', 125));
            Console.WriteLine($"Усього поїздок: {tripCount}, загальний виторг: {TotalRevenue():F2} грн.");
            PrintCounter();

            PrintDriverRevenue();
        }


        // Виводить один рядок таблиці для поїздки
        static void PrintRow(int index)
        {
            string pay = isCash[index] ? "готівка" : "картка";
            decimal cost = CalcCost(distances[index], ratesPerKm[index]);

            Console.WriteLine($"{ids[index],3} | {drivers[index],-12} | {tripDates[index]:dd.MM.yyyy HH:mm} | " +
                              $"{pickupAddresses[index],-20} | {destinations[index],-20} | " +
                              $"{distances[index],7:F1} | {ratesPerKm[index],7:F2} | {pay,-7} | {cost,9:F2}");
        }


        // Загальний виторг за всіма поїздками
        static decimal TotalRevenue()
        {
            decimal total = 0;
            for (int i = 0; i < tripCount; i++)
            {
                total += CalcCost(distances[i], ratesPerKm[i]);
            }
            return total;
        }


        // Заповнює масиви та виторг по водіях
        static int CollectDriverRevenue(string[] driverNames, decimal[] driverSums)
        {
            int n = 0;

            for (int i = 0; i < tripCount; i++)
            {
                int k = -1;
                for (int j = 0; j < n; j++)
                {
                    if (driverNames[j] == drivers[i]) { k = j; break; }
                }
                if (k == -1)
                {
                    k = n;
                    driverNames[n] = drivers[i];
                    n++;
                }
                driverSums[k] += CalcCost(distances[i], ratesPerKm[i]);
            }

            return n;
        }


        static void PrintDriverRevenue()
        {
            string[] names = new string[tripCount];
            decimal[] sums = new decimal[tripCount];
            int n = CollectDriverRevenue(names, sums);

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
            PrintTrip(index);
        }


        // Виводить усі поля однієї поїздки
        static void PrintTrip(int index)
        {
            decimal cost = CalcCost(distances[index], ratesPerKm[index]);

            Console.WriteLine($"  номер:      {ids[index]}");
            Console.WriteLine($"  водій:      {drivers[index]}");
            Console.WriteLine($"  дата й час: {tripDates[index]:dd.MM.yyyy HH:mm}");
            Console.WriteLine($"  звідки:     {pickupAddresses[index]}");
            Console.WriteLine($"  куди:       {destinations[index]}");
            Console.WriteLine($"  відстань:   {distances[index]:F1} км");
            Console.WriteLine($"  тариф:      {ratesPerKm[index]:F2} грн/км");
            Console.WriteLine($"  оплата:     {(isCash[index] ? "готівка" : "картка")}");
            Console.WriteLine($"  вартість:   {cost:F2} грн");
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

            RemoveTripAt(index);
            PrintCounter();
        }

        static void RemoveTripAt(int index)
        {
            for (int i = index; i < tripCount - 1; i++)
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

            tripCount--;

            ids[tripCount] = 0;
            drivers[tripCount] = "";
            tripDates[tripCount] = default;
            pickupAddresses[tripCount] = "";
            destinations[tripCount] = "";
            distances[tripCount] = 0;
            ratesPerKm[tripCount] = 0m;
            isCash[tripCount] = false;
        }
        static void CostCalculator()
        {
            double distance = ReadDouble("Відстань, км: ", 0.1, 1000);
            decimal rate = (decimal)ReadDouble("Тариф за 1 км, грн: ", 0.1, 1000);

            decimal cost = CalcCost(distance, rate);
            Console.WriteLine($"Вартість поїздки: {cost:F2} грн.");
        }


        // Скільки рядків видасть табулювання
        static int CountRows(double start, double end, double step)
        {
            return (int)Math.Floor((end - start) / step + 1e-9) + 1;
        }


        // Виводить таблицю вартості поїздки за стандартним тарифом
        static void Tabulate(double start, double end, double step)
        {
            int rows = CountRows(start, end, step);

            Console.WriteLine();
            Console.WriteLine($"Вартість поїздки за тарифом {StandardRate:F2} грн/км");
            Console.WriteLine($"{"Відстань, км",14} | {"Вартість, грн",14}");
            Console.WriteLine(new string('-', 31));

            for (int i = 0; i < rows; i++)
            {
                double distance = start + i * step;
                decimal cost = CalcCost(distance, StandardRate);
                Console.WriteLine($"{distance,14:F2} | {cost,14:F2}");
            }
        }


        static void RunTabulation()
        {
            double startKm = ReadDouble("Початкова відстань, км: ", 0.1, 1000);
            double endKm = ReadDouble("Кінцева відстань, км: ", startKm, 1000);
            double stepKm = ReadDouble("Крок, км: ", 0.1, 1000);

            int rows = CountRows(startKm, endKm, stepKm);
            if (rows > MaxTableRows)
            {
                Console.WriteLine($"Таблиця матиме {rows} рядків, а дозволено не більше {MaxTableRows}. Збільште крок.");
                return;
            }

            Tabulate(startKm, endKm, stepKm);
        }


        static void ShowAbout()
        {
            Console.WriteLine("Тема: служба таксі — облік поїздок.");
            Console.WriteLine("Сутність: поїздка (один запис у сховищі, не більше 100 записів).");
            Console.WriteLine("Дані зберігаються в паралельних масивах, один індекс — одна поїздка.");
            Console.WriteLine("Поля поїздки:");
            Console.WriteLine("  номер поїздки       — int");
            Console.WriteLine("  водій               — string");
            Console.WriteLine("  дата й час          — DateTime");
            Console.WriteLine("  адреса подачі       — string");
            Console.WriteLine("  адреса призначення  — string");
            Console.WriteLine("  відстань, км        — double");
            Console.WriteLine("  тариф за 1 км, грн  — decimal");
            Console.WriteLine("  спосіб оплати       — bool (true — готівка, false — картка)");
        }
        
        static void AddValue(double[] values, ref int count, double newValue, int capacity) // Додає число в масив. Лічильник з ref, щоб метод змінив саму змінну з Main
        {
            if (count == capacity)
            {
                Console.WriteLine("Місця більше немає.");
                return;
            }

            values[count] = newValue;
            count++;
        }


        // Виводить масив до count таблицею з номерами
        static void PrintValues(double[] values, int count)
        {
            if (count == 0)
            {
                Console.WriteLine("Список відстаней поки порожній.");
                return;
            }

            Console.WriteLine($"{"№",3} | {"Відстань, км",12}");
            Console.WriteLine(new string('-', 18));

            for (int i = 0; i < count; i++)
            {
                Console.WriteLine($"{i + 1,3} | {values[i],12:F2}");
            }
        }


        static void PrintValuesCounter(int count, int capacity)
        {
            Console.WriteLine($"Відстаней у списку: {count} зі {capacity}.");
        }
        
        static void Statistics(double[] values, int count, out double sum, out double average)
        {
            sum = 0;

            for (int i = 0; i < count; i++)
            {
                sum += values[i];
            }

            average = count > 0 ? sum / count : 0;
        }

        static bool FindMax(double[] values, int count, out double max, out int maxIndex) // Повертає через out найбільше значення індеса
        {
            if (count == 0)
            {
                max = 0;
                maxIndex = -1;
                return false;
            }

            max = values[0];
            maxIndex = 0;

            for (int i = 1; i < count; i++)
            {
                if (values[i] > max)
                {
                    max = values[i];
                    maxIndex = i;
                }
            }

            return true;
        }


        static void ShowStatistics(double[] values, int count)
        {
            PrintValues(values, count);

            Statistics(values, count, out double sum, out double average);
            Console.WriteLine($"Сума відстаней: {sum:F2} км, середня: {average:F2} км.");

            if (FindMax(values, count, out double max, out int maxIndex))
            {
                Console.WriteLine($"Найбільша відстань: {max:F2} км (номер {maxIndex + 1}).");
            }
            else
            {
                Console.WriteLine("Найбільшої відстані немає: список порожній.");
            }
        }
       
        static void RemoveAt(double[] values, ref int count, int index) // Видаляє елемент за індексом і зсуває решту вліво. Неправильний індекс: повідомлення без змін
        {
            if (index < 0 || index >= count)
            {
                Console.WriteLine($"Неправильний номер: {index + 1}. Нічого не змінено.");
                return;
            }

            for (int i = index; i < count - 1; i++)
            {
                values[i] = values[i + 1];
            }

            count--;
            values[count] = 0;
        }

        static void TryChangeInt(int number)
        {
            number += 100;
            Console.WriteLine($"  у методі TryChangeInt: number = {number}");
        }


        static void ChangeElement(double[] array)
        {
            array[0] += 100;
            Console.WriteLine($"  у методі ChangeElement: array[0] = {array[0]}");
        }


        static void ShowValueVsReference()
        {
            int number = 5;
            double[] array = { 5, 6, 7 };

            Console.WriteLine("Значимий тип (int без ref):");
            Console.WriteLine($"  до виклику: number = {number}");
            TryChangeInt(number);
            Console.WriteLine($"  після виклику: number = {number} (не змінився)");

            Console.WriteLine();
            Console.WriteLine("Посилальний тип (масив):");
            Console.WriteLine($"  до виклику: array[0] = {array[0]}");
            ChangeElement(array);
            Console.WriteLine($"  після виклику: array[0] = {array[0]} (змінився)");
        }
    }
}