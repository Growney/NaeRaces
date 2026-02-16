using NaeRaces.Query.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Query.Models;

public record PilotSelectionPolicyStatementTree(Guid PilotSelectionPolicyId, Guid ClubId, IPilotSelectionPolicyStatement Statement);
