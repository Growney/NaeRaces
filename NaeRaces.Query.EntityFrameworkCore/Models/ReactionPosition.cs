using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NaeRaces.Query.EntityFrameworkCore.Models;

public class ReactionPosition
{
    [Key]
    public required string ReactionKey { get; init; }     
    public ulong PreparePosition { get; set; }
    public ulong CommitPosition { get; set; }
}
