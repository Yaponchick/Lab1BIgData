using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Bogus;
using Bogus.DataSets;
using Lab1BIgData.Models;

namespace HotelsBigDataGenerator.Services
{
    public class RequestGenerator
    {
        private readonly IDbContextFactory<HotelsBigContext> _contextFactory;
        private readonly DatabaseOperations _databaseOperations;
        private readonly Random _random;

        public RequestGenerator(IDbContextFactory<HotelsBigContext> contextFactory, DatabaseOperations databaseOperations)
        {
            _contextFactory = contextFactory;
            _databaseOperations = databaseOperations;
            _random = new Random();
        }

        public async Task GenerateRequestsAsync(int requestCount = 10000, int selectChance = 80, int updateChance = 95)
        {
            // Проверка корректности входных данных
            if (selectChance < 0 || updateChance < selectChance || updateChance > 100)
            {
                throw new ArgumentException("Некорректные значения вероятностей для операций.");
            }

            using var context = await _contextFactory.CreateDbContextAsync();

            for (int i = 0; i < requestCount; i++)
            {
                // Выбор типа вероятностей
                OperationType operationType = GetRandomOperationType(selectChance, updateChance);

                try
                {
                    switch (operationType)
                    {
                        case OperationType.Select:
                            await GenerateSelectQueryAsync(context);
                            break;

                        case OperationType.Update:
                            await GenerateUpdateQueryAsync(context);
                            break;

                        case OperationType.Delete:
                            await GenerateDeleteQueryAsync(context);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при выполнении операции {operationType}: {ex.Message}");
                }
            }
        }

        private OperationType GetRandomOperationType(int selectChance, int updateChance)
        {
            int randomValue = _random.Next(1, 100);

            if (randomValue <= selectChance)
            {
                return OperationType.Select;
            }
            else if (randomValue <= updateChance)
            {
                return OperationType.Update;
            }
            else
            {
                return OperationType.Delete;
            }
        }

        // Типы операций
        private enum OperationType
        {
            Select,
            Update,
            Delete
        }

        private async Task GenerateSelectQueryAsync(HotelsBigContext context)
        {
            // Случайный выбор гостя
            var guests = await _databaseOperations.GetGuestsAsync();
            if (guests.Count == 0) return;
            var randomGuest = guests[_random.Next(guests.Count)];

            // Случайный выбор бронирования гостя
            var bookings = await _databaseOperations.GetBookingsByClientIdAsync(randomGuest.Id);
            if (bookings.Count == 0) return;
            var randomBooking = bookings[_random.Next(bookings.Count)];

            // Выбор случайного сценария
            int scenarioType = _random.Next(1, 6);
            switch (scenarioType)
            {
                case 1:
                    // Вывод общей стоимости бронирования
                    var payments = await context.Payments.Where(p => p.BookingId == randomBooking.Id).ToListAsync();
                    decimal totalCost = payments.Sum(p => p.Amount);
                    Console.WriteLine($"Общая стоимость бронирования ID {randomBooking.Id} для гостя \"{randomGuest.FirstName} {randomGuest.LastName}\":");
                    Console.WriteLine($"- Итоговая стоимость: {totalCost}");
                    break;

                case 2:
                    // Вывод информации о номере, связанном с бронированием
                    var room = await context.Rooms.FindAsync(randomBooking.RoomId);
                    if (room == null)
                    {
                        Console.WriteLine("Комната не найдена.");
                        return;
                    }
                    Console.WriteLine($"Информация о номере, связанном с бронированием ID {randomBooking.Id}:");
                    Console.WriteLine($"- Номер комнаты: {room.RoomNumber}");
                    Console.WriteLine($"- Тип комнаты: {room.RoomType}");
                    Console.WriteLine($"- Цена за ночь: {room.PricePerNight}");
                    break;

                case 3:
                    // Вывод списка услуг, использованных гостем
                    var guestServices = await context.GuestServices.Where(gs => gs.GuestId == randomGuest.Id).ToListAsync(); // Список услуг
                    Console.WriteLine($"Список услуг, использованных гостем \"{randomGuest.FirstName} {randomGuest.LastName}\":");
                    foreach (var service in guestServices)
                    {
                        var serviceDetails = await context.Services.FindAsync(service.ServiceId);
                        if (serviceDetails != null)
                        {
                            Console.WriteLine($"- Услуга: {serviceDetails.Name}, Стоимость: {service.TotalPrice}");
                        }
                    }
                    break;

                case 4:
                    // Вывод отзывов гостя об отелях
                    var reviews = await context.Reviews.Where(r => r.GuestId == randomGuest.Id).ToListAsync();
                    Console.WriteLine($"Отзывы гостя \"{randomGuest.FirstName} {randomGuest.LastName}\":");
                    foreach (var review in reviews)
                    {
                        var hotel = await context.Hotels.FindAsync(review.HotelId);
                        if (hotel != null)
                        {
                            Console.WriteLine($"- Отель: {hotel.Name}, Рейтинг: {review.Rating}, Комментарий: {review.ReviewText}");
                        }
                    }
                    break;

                case 5:
                    // Вывод информации об уборке номера
                    var roomCleanings = await context.RoomCleanings.Where(rc => rc.RoomId == randomBooking.RoomId).ToListAsync();
                    Console.WriteLine($"Информация об уборке номера ID {randomBooking.RoomId}:");
                    foreach (var cleaning in roomCleanings)
                    {
                        var employee = await context.Employees.FindAsync(cleaning.EmployeeId);
                        if (employee != null)
                        {
                            Console.WriteLine($"- Сотрудник: {employee.FirstName} {employee.LastName}, Дата уборки: {cleaning.CleaningDate}");
                        }
                    }
                    break;
            }
        }

        private async Task GenerateUpdateQueryAsync(HotelsBigContext context)
        {
            int updateType = _random.Next(1, 7);
            switch (updateType)
            {
                case 1:
                    // Обновление почты гостя
                    var guests = await _databaseOperations.GetGuestsAsync();
                    if (guests.Count == 0) return;
                    var randomGuest = guests[_random.Next(guests.Count)];
                    string newName = new Name().FirstName(); // Генерация нового имени
                    Console.WriteLine($"Обновление имени гостя с ID {randomGuest.Id}. Старое имя: \"{randomGuest.FirstName}\", новое имя: \"{newName}\".");
                    await _databaseOperations.UpdateGuestAsync(randomGuest.Id, newName + "@example.com"); // Также обновляем email
                    break;

                case 2:
                    // Обновление цены услуги
                    var services = await _databaseOperations.GetServicesAsync();
                    if (services.Count == 0) return;
                    var randomService = services[_random.Next(services.Count)];
                    decimal newServicePrice = _random.Next(100, 1000);
                    Console.WriteLine($"Обновление цены услуги ID {randomService.Id}:");
                    Console.WriteLine($"- Старая цена: {randomService.Price}");
                    Console.WriteLine($"- Новая цена: {newServicePrice}");
                    await _databaseOperations.UpdateServiceAsync(randomService.Id, newServicePrice);
                    break;

                case 3:
                    // Обновление статуса бронирования
                    var bookings = await _databaseOperations.GetBookingsAsync();
                    if (bookings.Count == 0) return;
                    var randomBooking = bookings[_random.Next(bookings.Count)];
                    string newStatus = _random.Next(0, 2) == 0 ? "Confirmed" : "Cancelled";
                    Console.WriteLine($"Обновление статуса бронирования ID {randomBooking.Id}:");
                    Console.WriteLine($"- Старый статус: {randomBooking.Status}");
                    Console.WriteLine($"- Новый статус: {newStatus}");
                    await _databaseOperations.UpdateBookingAsync(randomBooking.Id, DateTime.UtcNow.AddDays(_random.Next(1, 30)));
                    break;

                case 4:
                    // Обновление зарплаты сотрудника
                    var employees = await _databaseOperations.GetEmployeesAsync();
                    if (employees.Count == 0) return;
                    var randomEmployee = employees[_random.Next(employees.Count)];
                    decimal newSalary = _random.Next(20000, 100000);
                    Console.WriteLine($"Обновление зарплаты сотрудника ID {randomEmployee.Id}:");
                    Console.WriteLine($"- Старая зарплата: {randomEmployee.Salary}");
                    Console.WriteLine($"- Новая зарплата: {newSalary}");
                    await _databaseOperations.UpdateEmployeeAsync(randomEmployee.Id, newSalary);
                    break;

                case 5:
                    // Обновление даты уборки
                    var roomCleanings = await _databaseOperations.GetRoomCleaningsAsync();
                    if (roomCleanings.Count == 0) return;
                    var randomCleaning = roomCleanings[_random.Next(roomCleanings.Count)];
                    DateTime newCleaningDate = DateTime.UtcNow.AddDays(_random.Next(1, 30));
                    Console.WriteLine($"Обновление даты уборки ID {randomCleaning.Id}:");
                    Console.WriteLine($"- Старая дата: {randomCleaning.CleaningDate}");
                    Console.WriteLine($"- Новая дата: {newCleaningDate}");
                    await _databaseOperations.UpdateRoomCleaningAsync(randomCleaning.Id, newCleaningDate);
                    break;

                case 6:
                    // Обновление рейтинга отзыва
                    var reviews = await _databaseOperations.GetReviewsAsync();
                    if (reviews.Count == 0) return;
                    var randomReview = reviews[_random.Next(reviews.Count)];
                    int newRating = _random.Next(1, 6);
                    Console.WriteLine($"Обновление рейтинга отзыва ID {randomReview.Id}:");
                    Console.WriteLine($"- Старый рейтинг: {randomReview.Rating}");
                    Console.WriteLine($"- Новый рейтинг: {newRating}");
                    await _databaseOperations.UpdateReviewAsync(randomReview.Id, newRating);
                    break;
            }
            Console.WriteLine();
        }

        private async Task GenerateDeleteQueryAsync(HotelsBigContext context)
        {
            int deleteType = _random.Next(1, 7);
            switch (deleteType)
            {
                case 1:
                    // Удаление случайного гостя
                    var guests = await _databaseOperations.GetGuestsAsync();
                    if (guests.Count == 0) return;
                    var randomGuest = guests[_random.Next(guests.Count)];
                    Console.WriteLine($"Удаление гостя с ID {randomGuest.Id}. Имя: \"{randomGuest.FirstName} {randomGuest.LastName}\".");
                    await _databaseOperations.DeleteGuestAsync(randomGuest.Id);
                    break;

                case 2:
                    // Удаление случайного бронирования
                    var bookings = await _databaseOperations.GetBookingsAsync();
                    if (bookings.Count == 0) return;
                    var randomBooking = bookings[_random.Next(bookings.Count)];
                    Console.WriteLine($"Удаление бронирования с ID {randomBooking.Id}.");
                    await _databaseOperations.DeleteBookingAsync(randomBooking.Id);
                    break;

                case 3:
                    // Удаление случайной услуги
                    var services = await _databaseOperations.GetServicesAsync();
                    if (services.Count == 0) return;
                    var randomService = services[_random.Next(services.Count)];
                    Console.WriteLine($"Удаление услуги с ID {randomService.Id}. Название: \"{randomService.Name}\".");
                    await _databaseOperations.DeleteServiceAsync(randomService.Id);
                    break;

                case 4:
                    // Удаление случайного отзыва
                    var reviews = await _databaseOperations.GetReviewsAsync();
                    if (reviews.Count == 0) return;
                    var randomReview = reviews[_random.Next(reviews.Count)];
                    Console.WriteLine($"Удаление отзыва с ID {randomReview.Id}.");
                    await _databaseOperations.DeleteReviewAsync(randomReview.Id);
                    break;

                case 5:
                    // Удаление случайной записи об уборке
                    var roomCleanings = await _databaseOperations.GetRoomCleaningsAsync();
                    if (roomCleanings.Count == 0) return;
                    var randomCleaning = roomCleanings[_random.Next(roomCleanings.Count)];
                    Console.WriteLine($"Удаление записи об уборке с ID {randomCleaning.Id}.");
                    await _databaseOperations.DeleteRoomCleaningAsync(randomCleaning.Id);
                    break;

                case 6:
                    // Удаление случайного сотрудника
                    var employees = await _databaseOperations.GetEmployeesAsync();
                    if (employees.Count == 0) return;
                    var randomEmployee = employees[_random.Next(employees.Count)];
                    Console.WriteLine($"Удаление сотрудника с ID {randomEmployee.Id}. Имя: \"{randomEmployee.FirstName} {randomEmployee.LastName}\".");
                    await _databaseOperations.DeleteEmployeeAsync(randomEmployee.Id);
                    break;
            }
            Console.WriteLine();
        }
    }
}