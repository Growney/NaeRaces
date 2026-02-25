using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Query.Models;

public record RaceRegistrationDetails(DateTime FirstRaceDate, DateTime LastRaceDate, bool IsCancelled, Guid? PolicyId, long? PolicyVersion);