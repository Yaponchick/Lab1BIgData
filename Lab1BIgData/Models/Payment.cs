using System;
using System.Collections.Generic;

namespace Lab1BIgData.Models;

public partial class Payment
{
    public int Id { get; set; }

    public int GuestId { get; set; }

    public int BookingId { get; set; }

    public int? GuestServiceId { get; set; }

    public DateTime PaymentDate { get; set; }

    public decimal Amount { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public virtual Booking Booking { get; set; } = null!;

    public virtual Guest Guest { get; set; } = null!;

    public virtual GuestService? GuestService { get; set; }
}
