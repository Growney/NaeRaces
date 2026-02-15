using NaeRaces.Query.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Query.Models;

public record PeerGovernmentDocumentValidation(string Document, Guid PeerPilotId, Guid PeerPilotClubId, bool IsOnClubCommittee, DateTime ValidUntil) : IPeerValidation;
