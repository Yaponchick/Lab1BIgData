using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Bogus;
using Lab1BIgData.Models;

namespace HotelsBigDataGenerator.Services
{
    public class DataGenerator
    {
        private readonly IDbContextFactory<HotelsBigContext> _contextFactory;
        private static readonly int batchSize = 1000;

        public DataGenerator(IDbContextFactory<HotelsBigContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        // Генерация отелей
        public async Task<List<Hotel>> GenerateHotels(int count)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var totalHotels = 0;
            var allHotels = new List<Hotel>();

            for (int i = 0; i < count; i += batchSize)
            {
                int currentBatchSize = Math.Min(batchSize, count - i);
                var hotels = new Faker<Hotel>()
                    .RuleFor(h => h.Name, f => f.PickRandom(DataConstants.HotelNames))
                    .RuleFor(h => h.Address, f => f.PickRandom(DataConstants.Addresses))
                    .RuleFor(h => h.City, f => f.PickRandom(DataConstants.Cities))
                    .RuleFor(h => h.Country, f => f.PickRandom(DataConstants.Countries))
                    .RuleFor(h => h.StarRating, f => f.Random.Int(1, 5))
                    .Generate(currentBatchSize);

                await context.Hotels.AddRangeAsync(hotels);
                await context.SaveChangesAsync();

                totalHotels += currentBatchSize;
                Console.WriteLine($"Добавлено {totalHotels} отелей...");
                allHotels.AddRange(hotels);
            }

            return allHotels;
        }

        // Генерация сотрудников
        public async Task<List<Employee>> GenerateEmployees(int count, List<int> hotelIds)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var totalEmployees = 0;
            var allEmployees = new List<Employee>();

            for (int i = 0; i < count; i += batchSize)
            {
                int currentBatchSize = Math.Min(batchSize, count - i);
                var employees = new Faker<Employee>()
                    .RuleFor(e => e.HotelId, f => f.PickRandom(hotelIds))
                    .RuleFor(e => e.FirstName, f => f.PickRandom(DataConstants.FirstNames))
                    .RuleFor(e => e.LastName, f => f.PickRandom(DataConstants.LastNames))
                    .RuleFor(e => e.Position, f => f.PickRandom(DataConstants.Positions))
                    .RuleFor(e => e.Phone, f => f.Phone.PhoneNumber("+7 (###) ###-##-##"))
                    .RuleFor(e => e.HireDate, f => f.Date.Between(new DateTime(2003, 1, 1), DateTime.Now))
                    .RuleFor(e => e.Salary, f => f.Random.Decimal(15000, 100000))
                    .Generate(currentBatchSize);

                await context.Employees.AddRangeAsync(employees);
                await context.SaveChangesAsync();

                totalEmployees += currentBatchSize;
                Console.WriteLine($"Добавлено {totalEmployees} сотрудников...");
                allEmployees.AddRange(employees);
            }

            return allEmployees;
        }

        // Генерация гостей
        public async Task<List<Guest>> GenerateGuests(int count)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var totalGuests = 0;
            var allGuests = new List<Guest>();

            for (int i = 0; i < count; i += batchSize)
            {
                int currentBatchSize = Math.Min(batchSize, count - i);
                var guests = new Faker<Guest>()
                    .RuleFor(g => g.FirstName, f => f.PickRandom(DataConstants.FirstNames))
                    .RuleFor(g => g.LastName, f => f.PickRandom(DataConstants.LastNames))
                    .RuleFor(g => g.Email, (f, g) => $"{g.FirstName}.{g.LastName}.{Guid.NewGuid()}@example.com")
                    .RuleFor(g => g.Phone, f => f.Phone.PhoneNumber("+7 (###) ###-##-##"))
                    .RuleFor(g => g.DateOfBirth, f => DateOnly.FromDateTime(f.Date.Past(50, DateTime.Now.AddYears(-18))))
                    .RuleFor(g => g.DateRegistered, f => f.Date.Past(10))
                    .Generate(currentBatchSize);

                await context.Guests.AddRangeAsync(guests);
                await context.SaveChangesAsync();

                totalGuests += currentBatchSize;
                Console.WriteLine($"Добавлено {totalGuests} гостей...");
                allGuests.AddRange(guests);
            }

            return allGuests;
        }

        // Генерация номеров
        public async Task<List<Room>> GenerateRooms(int count, List<int> hotelIds)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var totalRooms = 0;
            var allRooms = new List<Room>();

            for (int i = 0; i < count; i += batchSize)
            {
                int currentBatchSize = Math.Min(batchSize, count - i);
                var rooms = new Faker<Room>()
                    .RuleFor(r => r.HotelId, f => f.PickRandom(hotelIds))
                    .RuleFor(r => r.RoomNumber, f => f.Random.ReplaceNumbers("###"))
                    .RuleFor(r => r.RoomType, f => f.PickRandom(DataConstants.RoomTypes))
                    .RuleFor(r => r.PricePerNight, f => f.Random.Decimal(500, 5000))
                    .RuleFor(r => r.IsAvailable, f => f.Random.Bool(0.8f)) // 80%
                    .Generate(currentBatchSize);

                await context.Rooms.AddRangeAsync(rooms);
                await context.SaveChangesAsync();

                totalRooms += currentBatchSize;
                Console.WriteLine($"Добавлено {totalRooms} номеров...");
                allRooms.AddRange(rooms);
            }

            return allRooms;
        }

        // Генерация бронирований
        public async Task<List<Booking>> GenerateBookings(int count, List<int> guestIds, List<int> roomIds)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var totalBookings = 0;
            var allBookings = new List<Booking>();

            for (int i = 0; i < count; i += batchSize)
            {
                int currentBatchSize = Math.Min(batchSize, count - i);
                var bookings = new Faker<Booking>()
                    .RuleFor(b => b.GuestId, f => f.PickRandom(guestIds))
                    .RuleFor(b => b.RoomId, f => f.PickRandom(roomIds))
                    .RuleFor(b => b.CheckInDate, f => f.Date.Between(new DateTime(2003, 1, 1), DateTime.Now))
                    .RuleFor(b => b.CheckOutDate, (f, b) => b.CheckInDate.AddDays(f.Random.Int(1, 14))) // Дата выезда через 1-14 дней
                    .RuleFor(b => b.TotalPrice, (f, b) => f.Random.Decimal(1000, 10000))
                    .RuleFor(b => b.Status, f => f.PickRandom(DataConstants.BookingStatuses))
                    .Generate(currentBatchSize);

                await context.Bookings.AddRangeAsync(bookings);
                await context.SaveChangesAsync();

                totalBookings += currentBatchSize;
                Console.WriteLine($"Добавлено {totalBookings} бронирований...");
                allBookings.AddRange(bookings);
            }
            return allBookings;
        }

        // Генерация услуг
        public async Task<List<Service>> GenerateServices(int count, List<int> hotelIds)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var totalServices = 0;
            var allServices = new List<Service>();

            for (int i = 0; i < count; i += batchSize)
            {
                int currentBatchSize = Math.Min(batchSize, count - i);
                var services = new Faker<Service>()
                    .RuleFor(s => s.HotelId, f => f.PickRandom(hotelIds))
                    .RuleFor(s => s.Name, f => f.PickRandom(DataConstants.ServiceNames))
                    .RuleFor(s => s.Description, f => f.PickRandom(DataConstants.ServiceDescriptions))
                    .RuleFor(s => s.Price, f => f.Random.Decimal(100, 5000))
                    .Generate(currentBatchSize);

                await context.Services.AddRangeAsync(services);
                await context.SaveChangesAsync();

                totalServices += currentBatchSize;
                Console.WriteLine($"Добавлено {totalServices} услуг...");
                allServices.AddRange(services);
            }

            return allServices;
        }

        // Генерация использования услуг
        public async Task<List<GuestService>> GenerateGuestServices(int count, List<int> guestIds, List<int> serviceIds)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var totalGuestServices = 0;
            var allGuestServices = new List<GuestService>();

            for (int i = 0; i < count; i += batchSize)
            {
                int currentBatchSize = Math.Min(batchSize, count - i);
                var guestServices = new Faker<GuestService>()
                    .RuleFor(gs => gs.GuestId, f => f.PickRandom(guestIds))
                    .RuleFor(gs => gs.ServiceId, f => f.PickRandom(serviceIds))
                    .RuleFor(gs => gs.DateUsed, f => f.Date.Between(new DateTime(2003, 1, 1), DateTime.Now)).RuleFor(gs => gs.TotalPrice, f => f.Random.Decimal(100, 5000))
                    .Generate(currentBatchSize);

                await context.GuestServices.AddRangeAsync(guestServices);
                await context.SaveChangesAsync();

                totalGuestServices += currentBatchSize;
                Console.WriteLine($"Добавлено {totalGuestServices} использований услуг...");
                allGuestServices.AddRange(guestServices);
            }

            return allGuestServices;
        }

        // Генерация уборки номеров
        public async Task<List<RoomCleaning>> GenerateRoomCleanings(int count, List<int> roomIds, List<int> employeeIds)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var totalRoomCleanings = 0;
            var allRoomCleanings = new List<RoomCleaning>();

            for (int i = 0; i < count; i += batchSize)
            {
                int currentBatchSize = Math.Min(batchSize, count - i);
                var roomCleanings = new Faker<RoomCleaning>()
                    .RuleFor(rc => rc.RoomId, f => f.PickRandom(roomIds))
                    .RuleFor(rc => rc.EmployeeId, f => f.PickRandom(employeeIds))
                    .RuleFor(rc => rc.CleaningDate, f => f.Date.Between(new DateTime(2003, 1, 1), DateTime.Now)).RuleFor(rc => rc.Comments, f => f.Lorem.Sentence())
                    .Generate(currentBatchSize);

                await context.RoomCleanings.AddRangeAsync(roomCleanings);
                await context.SaveChangesAsync();

                totalRoomCleanings += currentBatchSize;
                Console.WriteLine($"Добавлено {totalRoomCleanings} записей об уборке...");
                allRoomCleanings.AddRange(roomCleanings);
            }

            return allRoomCleanings;
        }

        // Генерация платежей
        public async Task<List<Payment>> GeneratePayments(int count, List<int> bookingIds, List<int> guestIds, List<int> guestServiceIds)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var totalPayments = 0;
            var allPayments = new List<Payment>();

            for (int i = 0; i < count; i += batchSize)
            {
                int currentBatchSize = Math.Min(batchSize, count - i);
                var payments = new Faker<Payment>()
                    .RuleFor(p => p.BookingId, f => f.PickRandom(bookingIds))
                    .RuleFor(p => p.GuestId, f => f.PickRandom(guestIds))
                    .RuleFor(p => p.GuestServiceId, f => f.PickRandom(guestServiceIds))
                    .RuleFor(rc => rc.PaymentDate, f => f.Date.Between(new DateTime(2003, 1, 1), DateTime.Now))
                    .RuleFor(p => p.Amount, f => f.Random.Decimal(500, 10000))
                    .RuleFor(p => p.PaymentMethod, f => f.PickRandom(DataConstants.PaymentMethods))
                    .Generate(currentBatchSize);

                await context.Payments.AddRangeAsync(payments);
                await context.SaveChangesAsync();

                totalPayments += currentBatchSize;
                Console.WriteLine($"Добавлено {totalPayments} платежей...");
                allPayments.AddRange(payments);
            }

            return allPayments;
        }

        // Генерация отзывов
        public async Task<List<Review>> GenerateReviews(int count, List<int> guestIds, List<int> hotelIds)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var totalReviews = 0;
            var allReviews = new List<Review>();

            // Разделение отзывов на положительные и негативные
            var positiveReviewComments = new[]
            {
                "Отличное обслуживание!", "Вкусная еда в ресторане.", "Персонал очень вежливый.",
                "Удобное расположение.", "Завтрак просто великолепный.", "Все понравилось, спасибо!"
            };

            var negativeReviewComments = new[]
            {
                "Номер был грязным.", "Цены слишком высокие.", "Проблемы с кондиционером.",
                "Не рекомендую этот отель."
            };

            for (int i = 0; i < count; i += batchSize)
            {
                int currentBatchSize = Math.Min(batchSize, count - i);
                var reviews = new Faker<Review>("ru") 
                    .RuleFor(r => r.GuestId, f => f.PickRandom(guestIds))
                    .RuleFor(r => r.HotelId, f => f.PickRandom(hotelIds))
                    .RuleFor(r => r.Rating, f => f.Random.Int(1, 5)) // Рейтинг от 1 до 5
                    .RuleFor(r => r.ReviewDate, f => f.Date.Between(DateTime.Now.AddYears(-20), DateTime.Now))
                    .RuleFor(r => r.ReviewText, (f, r) =>
                    {
                        if (r.Rating >= 4)
                        {
                            return f.PickRandom(positiveReviewComments);
                        }
                        else
                        {
                            return f.PickRandom(negativeReviewComments);
                        }
                    })
                    .Generate(currentBatchSize);

                await context.Reviews.AddRangeAsync(reviews);
                await context.SaveChangesAsync();
                totalReviews += currentBatchSize;
                Console.WriteLine($"Добавлено {totalReviews} отзывов...");
                allReviews.AddRange(reviews);
            }

            return allReviews;
        }

        // Заполнение базы данных
        public async Task FillDatabaseAsync(
            int guestsCount,
            int hotelsCount,
            int employeesCount,
            int roomsCount,
            int bookingsCount,
            int servicesCount,
            int guestServicesCount,
            int roomCleaningsCount,
            int paymentsCount,
            int reviewsCount)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            await context.Database.BeginTransactionAsync();
            try
            {
                // Генерация данных
                var hotels = await GenerateHotels(hotelsCount);
                var hotelIds = hotels.Select(h => h.Id).ToList();

                var guests = await GenerateGuests(guestsCount);
                var guestIds = guests.Select(g => g.Id).ToList();

                var employees = await GenerateEmployees(employeesCount, hotelIds);
                var employeeIds = employees.Select(e => e.Id).ToList();

                var rooms = await GenerateRooms(roomsCount, hotelIds);
                var roomIds = rooms.Select(r => r.Id).ToList();

                var bookings = await GenerateBookings(bookingsCount, guestIds, roomIds);
                var bookingIds = bookings.Select(b => b.Id).ToList();

                var services = await GenerateServices(servicesCount, hotelIds);
                var serviceIds = services.Select(s => s.Id).ToList();

                var guestServices = await GenerateGuestServices(guestServicesCount, guestIds, serviceIds);
                var guestServiceIds = guestServices.Select(gs => gs.Id).ToList();

                var roomCleanings = await GenerateRoomCleanings(roomCleaningsCount, roomIds, employeeIds);

                var payments = await GeneratePayments(paymentsCount, bookingIds, guestIds, guestServiceIds);

                var reviews = await GenerateReviews(reviewsCount, guestIds, hotelIds);

                await context.SaveChangesAsync();
                await context.Database.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await context.Database.RollbackTransactionAsync();
                Console.WriteLine($"Ошибка: {ex.Message}");
                Console.WriteLine($"Детали: {ex.InnerException?.Message}");
                throw;
            }
        }

        // Очистка базы данных
        public async Task ClearDatabaseAsync()
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync();

                await context.Payments.ExecuteDeleteAsync(); // Удаление платежей
                await context.Reviews.ExecuteDeleteAsync(); // Удаление отзывов
                await context.GuestServices.ExecuteDeleteAsync(); // Удаление использований услуг
                await context.RoomCleanings.ExecuteDeleteAsync(); // Удаление записей об уборке
                await context.Bookings.ExecuteDeleteAsync(); // Удаление бронирований
                await context.Services.ExecuteDeleteAsync(); // Удаление услуг
                await context.Rooms.ExecuteDeleteAsync(); // Удаление номеров
                await context.Employees.ExecuteDeleteAsync(); // Удаление сотрудников
                await context.Guests.ExecuteDeleteAsync(); // Удаление гостей
                await context.Hotels.ExecuteDeleteAsync(); // Удаление отелей

                var entityTypes = context.Model.GetEntityTypes();
                foreach (var entityType in entityTypes)
                {
                    var tableName = entityType.GetTableName();
                    var identityColumn = entityType.GetProperties()
                        .FirstOrDefault(p => p.ValueGenerated == Microsoft.EntityFrameworkCore.Metadata.ValueGenerated.OnAdd);
                    if (identityColumn != null && !string.IsNullOrEmpty(tableName))
                    {
                        await context.Database.ExecuteSqlRawAsync($"DBCC CHECKIDENT ('{tableName}', RESEED, 0)");
                    }
                }

                Console.WriteLine("База данных успешно очищена.");
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
}