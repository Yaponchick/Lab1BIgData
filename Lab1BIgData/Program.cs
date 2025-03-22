using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using HotelsBigDataGenerator.Services;
using Lab1BIgData.Models;

class Program
{
    static async Task Main()
    {
        try
        {
            // Настройка сервисов
            var serviceProvider = new ServiceCollection()
                .AddDbContextFactory<HotelsBigContext>(options =>
                    options.UseSqlServer("Server=KOT; Database=HotelsBig; Trusted_Connection=True; MultipleActiveResultSets=True; TrustServerCertificate=true; Encrypt=False;"))
                .AddScoped<DataGenerator>() // Один экземляр на все запросы
                .AddScoped<RequestGenerator>()
                .AddScoped<DatabaseOperations>()
                .BuildServiceProvider();

            // Получение экземпляров сервисов
            var generator = serviceProvider.GetRequiredService<DataGenerator>();
            var requestGenerator = serviceProvider.GetRequiredService<RequestGenerator>();
            var databaseOperations = serviceProvider.GetRequiredService<DatabaseOperations>();

            bool isRunning = true;

            while (isRunning)
            {
                Console.WriteLine("Выберите действие:");
                Console.WriteLine("1. Генерация");
                Console.WriteLine("2. Имитация клиентов");
                Console.WriteLine("3. Очистка БД");
                Console.WriteLine("q. Выход");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.WriteLine("Начинается генерация данных...");
                        await generator.FillDatabaseAsync(
                            guestsCount: 10000, // Гости
                            hotelsCount: 100, // Отели
                            employeesCount: 2000, // Сотрудники
                            roomsCount: 5000, // Номера
                            bookingsCount: 15000, // Бронирования
                            servicesCount: 1000, // Услуги
                            guestServicesCount: 20000, // Использования услуг
                            roomCleaningsCount: 10000, // Записи об уборке
                            paymentsCount: 15000, // Платежи
                            reviewsCount: 5000 // Отзывы
                        );
                        Console.WriteLine("Генерация данных завершена");
                        break;

                    case "2":
                        await requestGenerator.GenerateRequestsAsync();
                        break;

                    case "3":
                        Console.WriteLine("Очистка...");
                        await generator.ClearDatabaseAsync();
                        break;

                    case "q":
                        Console.WriteLine("Выход");
                        isRunning = false;
                        break;

                    default:
                        Console.WriteLine("Неверный выбор");
                        break;
                }

                Console.WriteLine();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Детали: {ex.InnerException.Message}");
            }
        }
    }
}