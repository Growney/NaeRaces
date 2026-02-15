using NaeRaces.Query.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Query.Models;

public record RacePolicyStatementTree(Guid RacePolicyId, Guid ClubId, IRacePolicyStatement Statement);