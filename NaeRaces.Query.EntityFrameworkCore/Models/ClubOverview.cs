using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace NaeRaces.Query.EntityFrameworkCore.Models;

public class ClubOverview
{
    [Key]
    public Guid ClubId { get; set; }
    public string? Name { get; set; }
    public string? Code { get; set; }
    public string? HomeLocationName { get; set; }
    public string? HomeLocationAddressLine1 { get; set; }
    public string? HomeLocationAddressLine2 { get; set; }
    public string? HomeLocationCity { get; set; }
    public string? HomeLocationPostcode { get; set; }
    public string? HomeLocationCounty { get; set; }
    public int TotalMemberCount { get; set; }
}
