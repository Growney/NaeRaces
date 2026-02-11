using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Query.EntityFrameworkCore.Models;

public class ClubMember
{
    public Guid Id { get; set; }
    public Guid ClubId { get; set; }
    public Guid PilotId { get; set; }
    public int? MembershipLevelId { get; set; }
    public int? PaymentOptionId { get; set; }
    public bool IsOnCommittee { get; set; }
    public bool IsRegistrationConfirmed { get; set; }
    public Guid? RegistrationValidatedBy { get; set; }
    public DateTime? RegistrationValidUntil { get; set; }
}
