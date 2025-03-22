using System;
using System.Collections.Generic;

namespace Lab1BIgData.Models;

public partial class GuestService
{
    public int Id { get; set; }

    public int GuestId { get; set; }

    public int ServiceId { get; set; }

    public DateTime DateUsed { get; set; }

    public decimal TotalPrice { get; set; }

    public virtual Guest Guest { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Service Service { get; set; } = null!;
}
