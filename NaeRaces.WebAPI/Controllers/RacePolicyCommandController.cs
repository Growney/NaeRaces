using EventDbLite.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NaeRaces.Command.Aggregates;
using NaeRaces.Command.ValueTypes;

namespace NaeRaces.WebAPI.Controllers;

public class RacePolicyCommandController : Controller
{
    private readonly IAggregateRepository _aggregateRepository;

    public RacePolicyCommandController(IAggregateRepository aggregateRepository)
    {
        _aggregateRepository = aggregateRepository;
    }

    [HttpPost("api/racepolicy")]
    public async Task<IActionResult> CreateRacePolicyAsync([FromQuery, BindRequired] Guid racePolicyId,
        [FromQuery, BindRequired] Guid clubId,
        [FromQuery, BindRequired] string name,
        [FromQuery, BindRequired] string description)
    {
        if(racePolicyId == Guid.Empty)
        {
            return BadRequest("RacePolicyId cannot be empty.");
        }

        Name nameValueType = Name.Create(name);

        RacePolicy newRacePolicy = _aggregateRepository.CreateNew<RacePolicy>(() => new RacePolicy(racePolicyId, clubId, nameValueType, description));

        await _aggregateRepository.Save(newRacePolicy);

        return Created($"/api/racepolicy/{newRacePolicy.Id}", new { RacePolicyId = newRacePolicy.Id });
    }

    [HttpPost("api/racepolicy/{racePolicyId}/requirement/minimumage")]
    public async Task<IActionResult> AddMinimumAgeRequirementAsync([FromRoute] Guid racePolicyId,
        [FromQuery, BindRequired] int minimumAge,
        [FromQuery, BindRequired] string validationPolicy)
    {

        ValidationPolicy policy = ValidationPolicy.Create(validationPolicy);

        RacePolicy? racePolicy = await _aggregateRepository.Get<RacePolicy, Guid>(racePolicyId);
        if (racePolicy == null)
        {
            return NotFound();
        }

        int statementId = racePolicy.AddMinimumAgeRequirement(minimumAge, policy);

        await _aggregateRepository.Save(racePolicy);

        return Created($"/api/racepolicy/{racePolicyId}/requirement/{statementId}", new { StatementId = statementId });
    }

    [HttpPost("api/racepolicy/{racePolicyId}/requirement/maximumage")]
    public async Task<IActionResult> AddMaximumAgeRequirementAsync([FromRoute] Guid racePolicyId,
        [FromQuery, BindRequired] int maximumAge,
        [FromQuery, BindRequired] string validationPolicy)
    {
        ValidationPolicy policy = ValidationPolicy.Create(validationPolicy);

        RacePolicy? racePolicy = await _aggregateRepository.Get<RacePolicy, Guid>(racePolicyId);
        if (racePolicy == null)
        {
            return NotFound();
        }

        int statementId = racePolicy.AddMaximumAgeRequirement(maximumAge, policy);

        await _aggregateRepository.Save(racePolicy);

        return Created($"/api/racepolicy/{racePolicyId}/requirement/{statementId}", new { StatementId = statementId });
    }

    [HttpPost("api/racepolicy/{racePolicyId}/requirement/insuranceprovider")]
    public async Task<IActionResult> AddInsuranceProviderRequirementAsync([FromRoute] Guid racePolicyId,
        [FromQuery, BindRequired] string insuranceProvider,
        [FromQuery, BindRequired] string validationPolicy)
    {
        ValidationPolicy policy = ValidationPolicy.Create(validationPolicy);

        RacePolicy? racePolicy = await _aggregateRepository.Get<RacePolicy, Guid>(racePolicyId);
        if (racePolicy == null)
        {
            return NotFound();
        }

        int statementId = racePolicy.AddInsuranceProviderRequirement(insuranceProvider, policy);

        await _aggregateRepository.Save(racePolicy);

        return Created($"/api/racepolicy/{racePolicyId}/requirement/{statementId}", new { StatementId = statementId });
    }

    [HttpPost("api/racepolicy/{racePolicyId}/requirement/governmentdocument")]
    public async Task<IActionResult> AddGovernmentDocumentValidationRequirementAsync([FromRoute] Guid racePolicyId,
        [FromQuery, BindRequired] string governmentDocument,
        [FromQuery, BindRequired] string validationPolicy)
    {
        ValidationPolicy policy = ValidationPolicy.Create(validationPolicy);

        RacePolicy? racePolicy = await _aggregateRepository.Get<RacePolicy, Guid>(racePolicyId);
        if (racePolicy == null)
        {
            return NotFound();
        }

        int statementId = racePolicy.AddGovernmentDocumentValidationRequirement(governmentDocument, policy);

        await _aggregateRepository.Save(racePolicy);

        return Created($"/api/racepolicy/{racePolicyId}/requirement/{statementId}", new { StatementId = statementId });
    }

    [HttpPost("api/racepolicy/{racePolicyId}/requirement/club")]
    public async Task<IActionResult> AddClubRequirementAsync([FromRoute] Guid racePolicyId,
        [FromQuery, BindRequired] Guid clubId)
    {
        RacePolicy? racePolicy = await _aggregateRepository.Get<RacePolicy, Guid>(racePolicyId);
        if (racePolicy == null)
        {
            return NotFound();
        }

        int statementId = racePolicy.AddClubRequirement(clubId);

        await _aggregateRepository.Save(racePolicy);

        return Created($"/api/racepolicy/{racePolicyId}/requirement/{statementId}", new { StatementId = statementId });
    }

    [HttpPost("api/racepolicy/{racePolicyId}/requirement/clubmembershiplevel")]
    public async Task<IActionResult> AddClubMembershipLevelRequirementAsync([FromRoute] Guid racePolicyId,
        [FromQuery, BindRequired] Guid clubId,
        [FromQuery, BindRequired] int membershipLevel)
    {
        RacePolicy? racePolicy = await _aggregateRepository.Get<RacePolicy, Guid>(racePolicyId);
        if (racePolicy == null)
        {
            return NotFound();
        }

        int statementId = racePolicy.AddClubMembershipLevelRequirement(clubId, membershipLevel);

        await _aggregateRepository.Save(racePolicy);

        return Created($"/api/racepolicy/{racePolicyId}/requirement/{statementId}", new { StatementId = statementId });
    }

    [HttpPost("api/racepolicy/{racePolicyId}/statement")]
    public async Task<IActionResult> AddPolicyStatementAsync([FromRoute] Guid racePolicyId,
        [FromQuery, BindRequired] int leftHandStatementId,
        [FromQuery, BindRequired] string operand,
        [FromQuery, BindRequired] int rightHandStatementId,
        [FromQuery, BindRequired] bool isWithinBrackets)
    {
        RacePolicy? racePolicy = await _aggregateRepository.Get<RacePolicy, Guid>(racePolicyId);
        if (racePolicy == null)
        {
            return NotFound();
        }

        int statementId = racePolicy.AddPolicyStatement(leftHandStatementId, operand, rightHandStatementId, isWithinBrackets);

        await _aggregateRepository.Save(racePolicy);

        return Created($"/api/racepolicy/{racePolicyId}/statement/{statementId}", new { StatementId = statementId });
    }

    [HttpDelete("api/racepolicy/{racePolicyId}/statement/{policyStatementId}")]
    public async Task<IActionResult> RemovePolicyStatementAsync([FromRoute] Guid racePolicyId,
        [FromRoute] int policyStatementId)
    {
        RacePolicy? racePolicy = await _aggregateRepository.Get<RacePolicy, Guid>(racePolicyId);
        if (racePolicy == null)
        {
            return NotFound();
        }

        racePolicy.RemovePolicyStatement(policyStatementId);

        await _aggregateRepository.Save(racePolicy);

        return Ok();
    }

    [HttpPut("api/racepolicy/{racePolicyId}/rootstatement")]
    public async Task<IActionResult> SetRootStatementAsync([FromRoute] Guid racePolicyId,
        [FromQuery, BindRequired] int rootPolicyStatementId)
    {
        RacePolicy? racePolicy = await _aggregateRepository.Get<RacePolicy, Guid>(racePolicyId);
        if (racePolicy == null)
        {
            return NotFound();
        }

        racePolicy.SetRootStatement(rootPolicyStatementId);

        await _aggregateRepository.Save(racePolicy);

        return Ok();
    }
}
