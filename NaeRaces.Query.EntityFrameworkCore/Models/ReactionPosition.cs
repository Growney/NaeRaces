using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NaeRaces.Query.EntityFrameworkCore.Models;

public class ReactionPosition
{
    [Key]
    public string ReactionKey { get; set; } = null!;     
    public long GlobalPosition { get; set; }
}
