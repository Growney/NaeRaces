using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Query.Models;

public record PilotSelectionPolicyDetails(Guid Id, Guid ClubId, string Name, string Description, long LatestVersion);
