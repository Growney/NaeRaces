using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Query.EntityFrameworkCore.Models;

public class PilotDetails
{
    public Guid Id { get; set; }
    public string? Callsign { get; set; }
    public string? Name { get; set; }
    public string? Nationality { get; set; }
}
