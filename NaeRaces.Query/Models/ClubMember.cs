using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Query.Models;

public record ClubMember(Guid Id, Guid ClubId, Guid PilotId, int? MembershipLevelId, int? PaymentOptionId, bool IsOnCommittee, bool IsRegistrationConfirmed, Guid? RegistrationValidatedBy, DateTime? RegistrationValidUntil);
