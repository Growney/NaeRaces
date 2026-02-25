using EventDbLite.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NaeRaces.Command.Aggregates;
using NaeRaces.Command.ValueTypes;
using NaeRaces.WebAPI.Models.Club;
using NaeRaces.WebAPI.Shared.PilotSelectionPolicy;

namespace NaeRaces.WebAPI.Controllers;

public class PilotSelectionPolicyCommandController : Controller
{
    private readonly IAggregateRepository _aggregateRepository;

    public PilotSelectionPolicyCommandController(IAggregateRepository aggregateRepository)
    {
        _aggregateRepository = aggregateRepository;
    }

    [HttpPost("api/pilotselectionpolicy")]
    public async Task<IActionResult> CreatePilotSelectionPolicyAsync([FromBody] CreatePilotSelectionPolicyRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if(request.PilotSelectionPolicyId == Guid.Empty)
        {
            return BadRequest("PilotSelectionPolicyId cannot be empty.");
        }

        Name nameValueType = Name.Create(request.Name);

        PilotSelectionPolicy newPilotSelectionPolicy = _aggregateRepository.CreateNew<PilotSelectionPolicy>(() => new PilotSelectionPolicy(request.PilotSelectionPolicyId, request.ClubId, nameValueType, request.Description));

        await _aggregateRepository.Save(newPilotSelectionPolicy);

        return Created($"/api/pilotselectionpolicy/{newPilotSelectionPolicy.Id}", new { PilotSelectionPolicyId = newPilotSelectionPolicy.Id });
    }

    [HttpPost("api/pilotselectionpolicy/{pilotSelectionPolicyId}/requirement/minimumage")]
    public async Task<IActionResult> AddMinimumAgeRequirementAsync([FromRoute] Guid pilotSelectionPolicyId,
        [FromBody] AddMinimumAgeRequirementRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        ValidationPolicy policy = ValidationPolicy.Create(request.ValidationPolicy);

        PilotSelectionPolicy? pilotSelectionPolicy = await _aggregateRepository.Get<PilotSelectionPolicy, Guid>(pilotSelectionPolicyId);
        if (pilotSelectionPolicy == null)
        {
            return NotFound();
        }

        int statementId = pilotSelectionPolicy.AddMinimumAgeRequirement(request.MinimumAge, policy);

        await _aggregateRepository.Save(pilotSelectionPolicy);

        return Created($"/api/pilotselectionpolicy/{pilotSelectionPolicyId}/requirement/{statementId}", new { StatementId = statementId });
    }

    [HttpPost("api/pilotselectionpolicy/{pilotSelectionPolicyId}/requirement/maximumage")]
    public async Task<IActionResult> AddMaximumAgeRequirementAsync([FromRoute] Guid pilotSelectionPolicyId,
        [FromBody] AddMaximumAgeRequirementRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        ValidationPolicy policy = ValidationPolicy.Create(request.ValidationPolicy);

        PilotSelectionPolicy? pilotSelectionPolicy = await _aggregateRepository.Get<PilotSelectionPolicy, Guid>(pilotSelectionPolicyId);
        if (pilotSelectionPolicy == null)
        {
            return NotFound();
        }

        int statementId = pilotSelectionPolicy.AddMaximumAgeRequirement(request.MaximumAge, policy);

        await _aggregateRepository.Save(pilotSelectionPolicy);

        return Created($"/api/pilotselectionpolicy/{pilotSelectionPolicyId}/requirement/{statementId}", new { StatementId = statementId });
    }

    [HttpPost("api/pilotselectionpolicy/{pilotSelectionPolicyId}/requirement/insuranceprovider")]
    public async Task<IActionResult> AddInsuranceProviderRequirementAsync([FromRoute] Guid pilotSelectionPolicyId,
        [FromBody] AddInsuranceProviderRequirementRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        ValidationPolicy policy = ValidationPolicy.Create(request.ValidationPolicy);

        PilotSelectionPolicy? pilotSelectionPolicy = await _aggregateRepository.Get<PilotSelectionPolicy, Guid>(pilotSelectionPolicyId);
        if (pilotSelectionPolicy == null)
        {
            return NotFound();
        }

        int statementId = pilotSelectionPolicy.AddInsuranceProviderRequirement(request.InsuranceProvider, policy);

        await _aggregateRepository.Save(pilotSelectionPolicy);

        return Created($"/api/pilotselectionpolicy/{pilotSelectionPolicyId}/requirement/{statementId}", new { StatementId = statementId });
    }

    [HttpPost("api/pilotselectionpolicy/{pilotSelectionPolicyId}/requirement/governmentdocument")]
    public async Task<IActionResult> AddGovernmentDocumentValidationRequirementAsync([FromRoute] Guid pilotSelectionPolicyId,
        [FromBody] AddGovernmentDocumentValidationRequirementRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        ValidationPolicy policy = ValidationPolicy.Create(request.ValidationPolicy);

        PilotSelectionPolicy? pilotSelectionPolicy = await _aggregateRepository.Get<PilotSelectionPolicy, Guid>(pilotSelectionPolicyId);
        if (pilotSelectionPolicy == null)
        {
            return NotFound();
        }

        int statementId = pilotSelectionPolicy.AddGovernmentDocumentValidationRequirement(request.GovernmentDocument, policy);

        await _aggregateRepository.Save(pilotSelectionPolicy);

        return Created($"/api/pilotselectionpolicy/{pilotSelectionPolicyId}/requirement/{statementId}", new { StatementId = statementId });
    }

    [HttpPost("api/pilotselectionpolicy/{pilotSelectionPolicyId}/requirement/club")]
    public async Task<IActionResult> AddClubRequirementAsync([FromRoute] Guid pilotSelectionPolicyId,
        [FromBody] AddClubRequirementRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        PilotSelectionPolicy? pilotSelectionPolicy = await _aggregateRepository.Get<PilotSelectionPolicy, Guid>(pilotSelectionPolicyId);
        if (pilotSelectionPolicy == null)
        {
            return NotFound();
        }

        int statementId = pilotSelectionPolicy.AddClubRequirement(request.ClubId);

        await _aggregateRepository.Save(pilotSelectionPolicy);

        return Created($"/api/pilotselectionpolicy/{pilotSelectionPolicyId}/requirement/{statementId}", new { StatementId = statementId });
    }

    [HttpPost("api/pilotselectionpolicy/{pilotSelectionPolicyId}/requirement/clubmembershiplevel")]
    public async Task<IActionResult> AddClubMembershipLevelRequirementAsync([FromRoute] Guid pilotSelectionPolicyId,
        [FromBody] AddClubMembershipLevelRequirementRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        PilotSelectionPolicy? pilotSelectionPolicy = await _aggregateRepository.Get<PilotSelectionPolicy, Guid>(pilotSelectionPolicyId);
        if (pilotSelectionPolicy == null)
        {
            return NotFound();
        }

        int statementId = pilotSelectionPolicy.AddClubMembershipLevelRequirement(request.ClubId, request.MembershipLevel);

        await _aggregateRepository.Save(pilotSelectionPolicy);

        return Created($"/api/pilotselectionpolicy/{pilotSelectionPolicyId}/requirement/{statementId}", new { StatementId = statementId });
    }

    [HttpPost("api/pilotselectionpolicy/{pilotSelectionPolicyId}/statement")]
    public async Task<IActionResult> AddPolicyStatementAsync([FromRoute] Guid pilotSelectionPolicyId,
        [FromBody] AddPolicyStatementRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        PilotSelectionPolicy? pilotSelectionPolicy = await _aggregateRepository.Get<PilotSelectionPolicy, Guid>(pilotSelectionPolicyId);
        if (pilotSelectionPolicy == null)
        {
            return NotFound();
        }

        int statementId = pilotSelectionPolicy.AddPolicyStatement(request.LeftHandStatementId, request.Operand, request.RightHandStatementId, request.IsWithinBrackets);

        await _aggregateRepository.Save(pilotSelectionPolicy);

        return Created($"/api/pilotselectionpolicy/{pilotSelectionPolicyId}/statement/{statementId}", new { StatementId = statementId });
    }

    [HttpDelete("api/pilotselectionpolicy/{pilotSelectionPolicyId}/statement/{policyStatementId}")]
    public async Task<IActionResult> RemovePolicyStatementAsync([FromRoute] Guid pilotSelectionPolicyId,
        [FromRoute] int policyStatementId)
    {
        PilotSelectionPolicy? pilotSelectionPolicy = await _aggregateRepository.Get<PilotSelectionPolicy, Guid>(pilotSelectionPolicyId);
        if (pilotSelectionPolicy == null)
        {
            return NotFound();
        }

        pilotSelectionPolicy.RemovePolicyStatement(policyStatementId);

        await _aggregateRepository.Save(pilotSelectionPolicy);

        return Ok();
    }

    [HttpPut("api/pilotselectionpolicy/{pilotSelectionPolicyId}/rootstatement")]
    public async Task<IActionResult> SetRootStatementAsync([FromRoute] Guid pilotSelectionPolicyId,
        [FromBody] SetRootStatementRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        PilotSelectionPolicy? pilotSelectionPolicy = await _aggregateRepository.Get<PilotSelectionPolicy, Guid>(pilotSelectionPolicyId);
        if (pilotSelectionPolicy == null)
        {
            return NotFound();
        }

        pilotSelectionPolicy.SetRootStatement(request.RootPolicyStatementId);

        await _aggregateRepository.Save(pilotSelectionPolicy);

        return Ok();
    }
}
