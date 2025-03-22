using System;
using System.Collections.Generic;

namespace Lab1BIgData.Models;

public partial class Service
{
    public int Id { get; set; }

    public int HotelId { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public decimal Price { get; set; }

    public virtual ICollection<GuestService> GuestServices { get; set; } = new List<GuestService>();

    public virtual Hotel Hotel { get; set; } = null!;
}
