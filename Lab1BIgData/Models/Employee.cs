using System;
using System.Collections.Generic;

namespace Lab1BIgData.Models;

public partial class Employee
{
    public int Id { get; set; }

    public int HotelId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Position { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public DateTime HireDate { get; set; }

    public decimal Salary { get; set; }

    public virtual Hotel Hotel { get; set; } = null!;

    public virtual ICollection<RoomCleaning> RoomCleanings { get; set; } = new List<RoomCleaning>();
}
