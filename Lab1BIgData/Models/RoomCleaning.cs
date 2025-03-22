using System;
using System.Collections.Generic;

namespace Lab1BIgData.Models;

public partial class RoomCleaning
{
    public int Id { get; set; }

    public int RoomId { get; set; }

    public int EmployeeId { get; set; }

    public DateTime CleaningDate { get; set; }

    public string Comments { get; set; } = null!;

    public virtual Employee Employee { get; set; } = null!;

    public virtual Room Room { get; set; } = null!;
}
