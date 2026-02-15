using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Query.Models;

public record PilotValidationDetails(Guid PilotId, DateTime? DateOfBirth,IEnumerable<string> GovernmentDocuments, IEnumerable<string> InsuranceProviders, IEnumerable<PeerAgeValidation> AgeValidations, IEnumerable<PeerGovernmentDocumentValidation> GovernmentDocumentValidations, IEnumerable<PeerInsuranceProviderValidation> InsuranceProviderValidations, IEnumerable<PilotClub> PilotClubs);
