using System;
using System.Collections.Generic;

namespace Lab1BIgData.Models;

public partial class Review
{
    public int Id { get; set; }

    public int GuestId { get; set; }

    public int HotelId { get; set; }

    public int Rating { get; set; }

    public string ReviewText { get; set; } = null!;

    public DateTime ReviewDate { get; set; }

    public virtual Guest Guest { get; set; } = null!;

    public virtual Hotel Hotel { get; set; } = null!;
}
