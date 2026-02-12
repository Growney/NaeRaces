using EventDbLite.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NaeRaces.Command.Aggregates;
using NaeRaces.Command.ValueTypes;
using NaeRaces.WebAPI.Models.Club;
using NaeRaces.WebAPI.Models.RacePolicy;

namespace NaeRaces.WebAPI.Controllers;

public class RacePolicyCommandController : Controller
{
    private readonly IAggregateRepository _aggregateRepository;

    public RacePolicyCommandController(IAggregateRepository aggregateRepository)
    {
        _aggregateRepository = aggregateRepository;
    }

    [HttpPost("api/racepolicy")]
    public async Task<IActionResult> CreateRacePolicyAsync([FromBody] CreateRacePolicyRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if(request.RacePolicyId == Guid.Empty)
        {
            return BadRequest("RacePolicyId cannot be empty.");
        }

        Name nameValueType = Name.Create(request.Name);

        RacePolicy newRacePolicy = _aggregateRepository.CreateNew<RacePolicy>(() => new RacePolicy(request.RacePolicyId, request.ClubId, nameValueType, request.Description));

        await _aggregateRepository.Save(newRacePolicy);

        return Created($"/api/racepolicy/{newRacePolicy.Id}", new { RacePolicyId = newRacePolicy.Id });
    }

    [HttpPost("api/racepolicy/{racePolicyId}/requirement/minimumage")]
    public async Task<IActionResult> AddMinimumAgeRequirementAsync([FromRoute] Guid racePolicyId,
        [FromBody] AddMinimumAgeRequirementRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        ValidationPolicy policy = ValidationPolicy.Create(request.ValidationPolicy);

        RacePolicy? racePolicy = await _aggregateRepository.Get<RacePolicy, Guid>(racePolicyId);
        if (racePolicy == null)
        {
            return NotFound();
        }

        int statementId = racePolicy.AddMinimumAgeRequirement(request.MinimumAge, policy);

        await _aggregateRepository.Save(racePolicy);

        return Created($"/api/racepolicy/{racePolicyId}/requirement/{statementId}", new { StatementId = statementId });
    }

    [HttpPost("api/racepolicy/{racePolicyId}/requirement/maximumage")]
    public async Task<IActionResult> AddMaximumAgeRequirementAsync([FromRoute] Guid racePolicyId,
        [FromBody] AddMaximumAgeRequirementRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        ValidationPolicy policy = ValidationPolicy.Create(request.ValidationPolicy);

        RacePolicy? racePolicy = await _aggregateRepository.Get<RacePolicy, Guid>(racePolicyId);
        if (racePolicy == null)
        {
            return NotFound();
        }

        int statementId = racePolicy.AddMaximumAgeRequirement(request.MaximumAge, policy);

        await _aggregateRepository.Save(racePolicy);

        return Created($"/api/racepolicy/{racePolicyId}/requirement/{statementId}", new { StatementId = statementId });
    }

    [HttpPost("api/racepolicy/{racePolicyId}/requirement/insuranceprovider")]
    public async Task<IActionResult> AddInsuranceProviderRequirementAsync([FromRoute] Guid racePolicyId,
        [FromBody] AddInsuranceProviderRequirementRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        ValidationPolicy policy = ValidationPolicy.Create(request.ValidationPolicy);

        RacePolicy? racePolicy = await _aggregateRepository.Get<RacePolicy, Guid>(racePolicyId);
        if (racePolicy == null)
        {
            return NotFound();
        }

        int statementId = racePolicy.AddInsuranceProviderRequirement(request.InsuranceProvider, policy);

        await _aggregateRepository.Save(racePolicy);

        return Created($"/api/racepolicy/{racePolicyId}/requirement/{statementId}", new { StatementId = statementId });
    }

    [HttpPost("api/racepolicy/{racePolicyId}/requirement/governmentdocument")]
    public async Task<IActionResult> AddGovernmentDocumentValidationRequirementAsync([FromRoute] Guid racePolicyId,
        [FromBody] AddGovernmentDocumentValidationRequirementRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        ValidationPolicy policy = ValidationPolicy.Create(request.ValidationPolicy);

        RacePolicy? racePolicy = await _aggregateRepository.Get<RacePolicy, Guid>(racePolicyId);
        if (racePolicy == null)
        {
            return NotFound();
        }

        int statementId = racePolicy.AddGovernmentDocumentValidationRequirement(request.GovernmentDocument, policy);

        await _aggregateRepository.Save(racePolicy);

        return Created($"/api/racepolicy/{racePolicyId}/requirement/{statementId}", new { StatementId = statementId });
    }

    [HttpPost("api/racepolicy/{racePolicyId}/requirement/club")]
    public async Task<IActionResult> AddClubRequirementAsync([FromRoute] Guid racePolicyId,
        [FromBody] AddClubRequirementRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        RacePolicy? racePolicy = await _aggregateRepository.Get<RacePolicy, Guid>(racePolicyId);
        if (racePolicy == null)
        {
            return NotFound();
        }

        int statementId = racePolicy.AddClubRequirement(request.ClubId);

        await _aggregateRepository.Save(racePolicy);

        return Created($"/api/racepolicy/{racePolicyId}/requirement/{statementId}", new { StatementId = statementId });
    }

    [HttpPost("api/racepolicy/{racePolicyId}/requirement/clubmembershiplevel")]
    public async Task<IActionResult> AddClubMembershipLevelRequirementAsync([FromRoute] Guid racePolicyId,
        [FromBody] AddClubMembershipLevelRequirementRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        RacePolicy? racePolicy = await _aggregateRepository.Get<RacePolicy, Guid>(racePolicyId);
        if (racePolicy == null)
        {
            return NotFound();
        }

        int statementId = racePolicy.AddClubMembershipLevelRequirement(request.ClubId, request.MembershipLevel);

        await _aggregateRepository.Save(racePolicy);

        return Created($"/api/racepolicy/{racePolicyId}/requirement/{statementId}", new { StatementId = statementId });
    }

    [HttpPost("api/racepolicy/{racePolicyId}/statement")]
    public async Task<IActionResult> AddPolicyStatementAsync([FromRoute] Guid racePolicyId,
        [FromBody] AddPolicyStatementRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        RacePolicy? racePolicy = await _aggregateRepository.Get<RacePolicy, Guid>(racePolicyId);
        if (racePolicy == null)
        {
            return NotFound();
        }

        int statementId = racePolicy.AddPolicyStatement(request.LeftHandStatementId, request.Operand, request.RightHandStatementId, request.IsWithinBrackets);

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
        [FromBody] SetRootStatementRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        RacePolicy? racePolicy = await _aggregateRepository.Get<RacePolicy, Guid>(racePolicyId);
        if (racePolicy == null)
        {
            return NotFound();
        }

        racePolicy.SetRootStatement(request.RootPolicyStatementId);

        await _aggregateRepository.Save(racePolicy);

        return Ok();
    }
}
