using Bogus;
using Lab1BIgData.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelsBigDataGenerator.Services
{
    public class DatabaseOperations
    {
        private readonly IDbContextFactory<HotelsBigContext> _contextFactory;

        public DatabaseOperations(IDbContextFactory<HotelsBigContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        private void Log(string message)
        {
            Console.WriteLine(message);
        }

        // Hotels
        public async Task CreateHotelAsync(Hotel hotel)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            context.Hotels.Add(hotel);
            await context.SaveChangesAsync();
            Log($"[CREATE] Создан отель: \"{hotel.Name}\", ID: {hotel.Id}.");
        }

        public async Task<List<Hotel>> GetHotelsAsync()
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Hotels.ToListAsync();
        }

        public async Task UpdateHotelAsync(int hotelId, string newName)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var hotel = await context.Hotels.FindAsync(hotelId);
            if (hotel != null)
            {
                string oldName = hotel.Name;
                hotel.Name = newName;
                await context.SaveChangesAsync();
                Log($"[UPDATE] Обновлен отель с ID {hotelId}. Старое название: \"{oldName}\", новое название: \"{newName}\".");
            }
        }

        public async Task DeleteHotelAsync(int hotelId)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var hotel = await context.Hotels
                .Include(h => h.Rooms) // Связанные номера
                .Include(h => h.Services) // Связанные услуги
                .Include(h => h.Employees) // Связанные сотрудники
                .FirstOrDefaultAsync(h => h.Id == hotelId);
            if (hotel != null)
            {
                string hotelName = hotel.Name;
                // Удаление номеров
                context.Rooms.RemoveRange(hotel.Rooms);
                // Удаление услуг
                context.Services.RemoveRange(hotel.Services);
                // Удаление сотрудников
                context.Employees.RemoveRange(hotel.Employees);
                // Удаление отеля
                context.Hotels.Remove(hotel);
                await context.SaveChangesAsync();
                Log($"[DELETE] Удален отель с ID {hotelId}. Название: \"{hotelName}\".");
            }
        }

        // Employees
        public async Task<List<Employee>> GetEmployeesAsync()
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Employees.ToListAsync();
        }

        public async Task UpdateEmployeeAsync(int employeeId, decimal newSalary)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var employee = await context.Employees.FindAsync(employeeId);
            if (employee != null)
            {
                decimal oldSalary = employee.Salary;
                employee.Salary = newSalary;
                await context.SaveChangesAsync();
                Log($"[UPDATE] Обновлен сотрудник с ID {employeeId}. Старая зарплата: {oldSalary}, новая зарплата: {newSalary}.");
            }
        }

        public async Task DeleteEmployeeAsync(int employeeId)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var employee = await context.Employees
                .Include(e => e.RoomCleanings) // Записи об уборке
                .FirstOrDefaultAsync(e => e.Id == employeeId);
            if (employee != null)
            {
                string employeeName = $"{employee.FirstName} {employee.LastName}";

                // Удаление записей об уборке
                context.RoomCleanings.RemoveRange(employee.RoomCleanings);

                // Удаление сотрудника
                context.Employees.Remove(employee);

                await context.SaveChangesAsync();
                Log($"[DELETE] Удален сотрудник с ID {employeeId}. Имя: \"{employeeName}\".");
            }
        }

        // Guests
        public async Task<List<Guest>> GetGuestsAsync()
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Guests.ToListAsync();
        }

        public async Task UpdateGuestAsync(int guestId, string newEmail)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var guest = await context.Guests.FindAsync(guestId);

            if (guest == null)
            {
                Log($"[ERROR] Гость с ID {guestId} не найден.");
                return;
            }

            // Проверяем, существует ли уже другой гость с таким же Email
            var existingGuestWithEmail = await context.Guests
                .FirstOrDefaultAsync(g => g.Email == newEmail && g.Id != guestId);

            if (existingGuestWithEmail != null)
            {
                Log($"[ERROR] Невозможно обновить Email для гостя с ID {guestId}. Email \"{newEmail}\" уже используется другим гостем.");
                return;
            }

            // Обновляем Email
            string oldEmail = guest.Email;
            guest.Email = newEmail;

            await context.SaveChangesAsync();
            Log($"[UPDATE] Обновлен гость с ID {guestId}. Старый email: \"{oldEmail}\", новый email: \"{newEmail}\".");
        }

        public async Task DeleteGuestAsync(int guestId)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var guest = await context.Guests
                .Include(g => g.Bookings) // Включаем связанные бронирования
                .Include(g => g.GuestServices) // Включаем связанные услуги
                .Include(g => g.Payments) // Включаем связанные платежи
                .Include(g => g.Reviews) // Включаем связанные отзывы
                .FirstOrDefaultAsync(g => g.Id == guestId);

            if (guest != null)
            {
                string guestName = $"{guest.FirstName} {guest.LastName}";

                // Удаляем связанные платежи
                foreach (var booking in guest.Bookings)
                {
                    var relatedPayments = await context.Payments
                        .Where(p => p.BookingId == booking.Id)
                        .ToListAsync();
                    context.Payments.RemoveRange(relatedPayments);
                }

                // Удаляем связанные бронирования
                context.Bookings.RemoveRange(guest.Bookings);

                // Удаляем связанные услуги
                foreach (var guestService in guest.GuestServices)
                {
                    var relatedPayments = await context.Payments
                        .Where(p => p.GuestServiceId == guestService.Id)
                        .ToListAsync();
                    context.Payments.RemoveRange(relatedPayments);
                }
                context.GuestServices.RemoveRange(guest.GuestServices);

                // Удаляем связанные платежи (напрямую связанные с гостем)
                context.Payments.RemoveRange(guest.Payments);

                // Удаляем связанные отзывы
                context.Reviews.RemoveRange(guest.Reviews);

                // Удаляем гостя
                context.Guests.Remove(guest);

                await context.SaveChangesAsync();
                Log($"[DELETE] Удален гость с ID {guestId}. Имя: \"{guestName}\".");
            }
        }

        // Bookings
        public async Task<List<Booking>> GetBookingsByClientIdAsync(int clientId)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Bookings.Where(b => b.GuestId == clientId).ToListAsync();
        }

        public async Task<List<Booking>> GetBookingsAsync()
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Bookings.ToListAsync();
        }

        public async Task UpdateBookingAsync(int bookingId, DateTime newCheckInDate)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var booking = await context.Bookings.FindAsync(bookingId);
            if (booking != null)
            {
                DateTime oldCheckInDate = booking.CheckInDate;
                booking.CheckInDate = newCheckInDate;
                await context.SaveChangesAsync();
                Log($"[UPDATE] Обновлено бронирование с ID {bookingId}. Старая дата заезда: {oldCheckInDate}, новая дата заезда: {newCheckInDate}.");
            }
        }

        public async Task DeleteBookingAsync(int bookingId)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var booking = await context.Bookings.FindAsync(bookingId);
            if (booking != null)
            {
                // Находим связанные платежи
                var relatedPayments = await context.Payments.Where(p => p.BookingId == bookingId).ToListAsync();
                // Удаляем связанные платежи
                context.Payments.RemoveRange(relatedPayments);
                // Удаляем бронирование
                context.Bookings.Remove(booking);
                await context.SaveChangesAsync();
                Log($"[DELETE] Удалено бронирование с ID {bookingId}.");
            }
        }

        // Services
        public async Task<List<Service>> GetServicesAsync()
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Services.ToListAsync();
        }

        public async Task UpdateServiceAsync(int serviceId, decimal newPrice)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var service = await context.Services.FindAsync(serviceId);
            if (service != null)
            {
                decimal oldPrice = service.Price;
                service.Price = newPrice;
                await context.SaveChangesAsync();
                Log($"[UPDATE] Обновлена услуга с ID {serviceId}. Старая цена: {oldPrice}, новая цена: {newPrice}.");
            }
        }

        public async Task DeleteServiceAsync(int serviceId)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var service = await context.Services.FindAsync(serviceId);
            if (service != null)
            {
                string serviceName = service.Name;

                // Находим связанные записи использования услуг
                var relatedGuestServices = await context.GuestServices
                    .Where(gs => gs.ServiceId == serviceId)
                    .ToListAsync();

                // Удаляем связанные платежи
                foreach (var guestService in relatedGuestServices)
                {
                    var relatedPayments = await context.Payments
                        .Where(p => p.GuestServiceId == guestService.Id)
                        .ToListAsync();
                    context.Payments.RemoveRange(relatedPayments);
                }

                // Удаляем связанные записи использования услуг
                context.GuestServices.RemoveRange(relatedGuestServices);

                // Удаляем услугу
                context.Services.Remove(service);

                await context.SaveChangesAsync();
                Log($"[DELETE] Удалена услуга с ID {serviceId}. Название: \"{serviceName}\".");
            }
        }

        // Reviews
        public async Task<List<Review>> GetReviewsAsync()
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Reviews.ToListAsync();
        }

        public async Task UpdateReviewAsync(int reviewId, int newRating)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var review = await context.Reviews.FindAsync(reviewId);
            if (review != null)
            {
                int oldRating = review.Rating;
                review.Rating = newRating;
                await context.SaveChangesAsync();
                Log($"[UPDATE] Обновлен отзыв с ID {reviewId}. Старый рейтинг: {oldRating}, новый рейтинг: {newRating}.");
            }
        }

        public async Task DeleteReviewAsync(int reviewId)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var review = await context.Reviews.FindAsync(reviewId);
            if (review != null)
            {
                string reviewText = review.ReviewText;
                // Удаляем отзыв
                context.Reviews.Remove(review);
                await context.SaveChangesAsync();
                Log($"[DELETE] Удален отзыв с ID {reviewId}. Текст: \"{reviewText}\".");
            }
        }

        // RoomCleaning
        public async Task<List<RoomCleaning>> GetRoomCleaningsAsync()
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.RoomCleanings.ToListAsync();
        }

        public async Task UpdateRoomCleaningAsync(int roomCleaningId, DateTime newCleaningDate)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var roomCleaning = await context.RoomCleanings.FindAsync(roomCleaningId);
            if (roomCleaning != null)
            {
                DateTime oldCleaningDate = roomCleaning.CleaningDate;
                roomCleaning.CleaningDate = newCleaningDate;
                await context.SaveChangesAsync();
                Log($"[UPDATE] Обновлена запись об уборке с ID {roomCleaningId}. Старая дата: {oldCleaningDate}, новая дата: {newCleaningDate}.");
            }
        }

        public async Task DeleteRoomCleaningAsync(int roomCleaningId)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var roomCleaning = await context.RoomCleanings.FindAsync(roomCleaningId);
            if (roomCleaning != null)
            {
                // Удаляем запись об уборке
                context.RoomCleanings.Remove(roomCleaning);
                await context.SaveChangesAsync();
                Log($"[DELETE] Удалена запись об уборке с ID {roomCleaningId}.");
            }
        }
    }
}


