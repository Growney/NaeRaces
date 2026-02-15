using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Query.EntityFrameworkCore.Models;

public class TeamMember
{
    public Guid TeamId { get; set; }
    public Guid PilotId { get; set; }
}
