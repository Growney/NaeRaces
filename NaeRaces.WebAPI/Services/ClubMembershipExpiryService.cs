using EventDbLite.Abstractions;
using NaeRaces.Command.Aggregates;
using NaeRaces.Query.Abstractions;

namespace NaeRaces.WebAPI.Services;

public class ClubMembershipExpiryService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ClubMembershipExpiryService> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(15);

    public ClubMembershipExpiryService(IServiceScopeFactory scopeFactory, ILogger<ClubMembershipExpiryService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessExpiringMembershipsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing expiring club memberships");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }
    }

    private async Task ProcessExpiringMembershipsAsync(CancellationToken stoppingToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var queryContext = scope.ServiceProvider.GetRequiredService<INaeRacesQueryContext>();
        var aggregateRepository = scope.ServiceProvider.GetRequiredService<IAggregateRepository>();
        var clock = scope.ServiceProvider.GetRequiredService<IClock>();

        var now = clock.UtcNow;

        await foreach (var member in queryContext.ClubMember.GetMembershipsExpiringBefore(now).WithCancellation(stoppingToken))
        {
            try
            {
                ClubMembership? membership = await aggregateRepository.Get<ClubMembership, Guid>(member.Id);
                if (membership == null)
                    continue;

                if (member.AutoRenew)
                {
                    // TODO: Integrate with payment provider to process renewal payment.
                    // On success call membership.Renew(newValidUntil);
                    // On failure call membership.FailRenewal();
                    _logger.LogInformation("Membership for pilot {PilotId} in club {ClubId} is due for auto-renewal", member.PilotId, member.ClubId);
                }
                else
                {
                    membership.Expire();
                    await aggregateRepository.Save(membership);
                    _logger.LogInformation("Expired membership for pilot {PilotId} in club {ClubId}", member.PilotId, member.ClubId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing membership expiry for pilot {PilotId} in club {ClubId}", member.PilotId, member.ClubId);
            }
        }
    }
}
