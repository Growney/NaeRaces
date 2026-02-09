using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Events;

public record RaceSeriesPlanned(Guid RaceSeriesId, string Name);
public record RaceAddedToSeries(Guid RaceSeriesId, Guid RaceId);
public record RaceRemovedFromSeries(Guid RaceSeriesId, Guid RaceId);
public record RaceSeriesRaceApproved(Guid RaceSeriesId, Guid RaceId);
public record RaceSeriesHostClubSet(Guid RaceSeriesId, Guid ClubId);
public record RaceSeriesRacePublishRulesSet(Guid RaceSeriesId, string PublishRules);
public record RaceSeriesRaceRequiredClubMembershipDiscountSet(Guid RaceSeriesId, Guid ClubId, int MembershipLevelId, decimal Discount);
public record RaceSeriesRaceRequiredClubDiscountSet(Guid RaceSeriesId, Guid ClubId, decimal Discount);
