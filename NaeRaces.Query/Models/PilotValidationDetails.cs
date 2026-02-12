using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Query.Models;

public record PilotValidationDetails(Guid PilotId, DateTime? DateOfBirth, IEnumerable<PeerAgeValidation> AgeValidations, IEnumerable<PeerGovernmentDocumentValidation> GovernmentDocumentValidations, IEnumerable<PeerInsuranceProviderValidation> InsuranceProviderValidations);
