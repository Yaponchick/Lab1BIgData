using System;
using System.Collections.Generic;

namespace Lab1BIgData.Models;

public partial class Room
{
    public int Id { get; set; }

    public int HotelId { get; set; }

    public string RoomNumber { get; set; } = null!;

    public string RoomType { get; set; } = null!;

    public decimal PricePerNight { get; set; }

    public bool IsAvailable { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual Hotel Hotel { get; set; } = null!;

    public virtual ICollection<RoomCleaning> RoomCleanings { get; set; } = new List<RoomCleaning>();
}
