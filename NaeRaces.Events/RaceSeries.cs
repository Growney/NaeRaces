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
public record RaceSeriesDiscountAdded(Guid RaceId, int RaceSeriesDiscountId, Guid PilotPolicyId, long PolicyVersion, string Currency, decimal Discount, bool CanBeCombined);
public record RaceSeriesDiscountRemoved(Guid RaceId, int RaceSeriesDiscountId);