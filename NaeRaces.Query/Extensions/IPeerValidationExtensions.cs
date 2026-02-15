using NaeRaces.Query.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Query.Abstractions;

public static class IPeerValidationExtensions
{
    public static bool IsValidForPolicy<T>(this IEnumerable<T> peerValidations, string validationPolicy, Guid clubId, IEnumerable<Guid> pilotClubs) where T : IPeerValidation
    {
        IEnumerable<T> validPeerValidations = peerValidations.GetValidPeerValidations(validationPolicy, clubId, pilotClubs);

        return Constants.ValidationPolicy.IsValidForPeerValidations(validationPolicy, validPeerValidations);
    }
    public static IEnumerable<T> GetValidPeerValidations<T>(this IEnumerable<T> peerValidations, string validationPolicy, Guid clubId, IEnumerable<Guid> pilotClubs) where T : IPeerValidation
    {
        return validationPolicy switch
        {
            Constants.ValidationPolicy.None => peerValidations, // No requirement to have any so they are all valid. This allows the other methods to work without needing to check for the validation policy first.
            Constants.ValidationPolicy.Any => peerValidations, // Any validation is acceptable
            Constants.ValidationPolicy.AnyClubMember => peerValidations, // At present only club memebers can validate age
            Constants.ValidationPolicy.AnyClubCommitteeMember => peerValidations.Where(x => x.IsOnClubCommittee), // At present only club memebers can validate age
            Constants.ValidationPolicy.PolicyClubMember => peerValidations.Where(x => x.PeerPilotClubId == clubId),
            Constants.ValidationPolicy.PolicyClubCommitteeMember => peerValidations.Where(x => x.PeerPilotClubId == clubId && x.IsOnClubCommittee),
            Constants.ValidationPolicy.PilotClubMember => peerValidations.Where(x => pilotClubs.Contains(x.PeerPilotClubId)),
            Constants.ValidationPolicy.PilotClubCommitteeMember => peerValidations.Where(x => pilotClubs.Contains(x.PeerPilotClubId) && x.IsOnClubCommittee),
            _ => Enumerable.Empty<T>() // Invalid validation policy
        };
    }

    extension(Constants.ValidationPolicy src)
    {
        public static bool IsValidForPeerValidations<T>(string policy, IEnumerable<T> peerValidations) where T : IPeerValidation
        {
            return policy switch
            {
                Constants.ValidationPolicy.None => true, // No additional validation required

                Constants.ValidationPolicy.Any or
                Constants.ValidationPolicy.AnyClubMember or
                Constants.ValidationPolicy.AnyClubCommitteeMember or // At present only club memebers can validate age
                Constants.ValidationPolicy.PolicyClubMember or
                Constants.ValidationPolicy.PolicyClubCommitteeMember or
                Constants.ValidationPolicy.PilotClubMember or
                Constants.ValidationPolicy.PilotClubCommitteeMember => peerValidations.Any(),
                _ => false // Invalid validation policy
            };
        }
    }
}
