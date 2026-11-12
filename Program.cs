using System;
using System.Globalization;

namespace Lab06
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
    }
}