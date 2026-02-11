using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Query.EntityFrameworkCore.Models;

public class ClubUniquenessDetails
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Code { get; set; }
}
