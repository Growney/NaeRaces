# NaeRaces Project Summary

## Overview
NaeRaces is an event-sourced racing management system built using CQRS (Command Query Responsibility Segregation) architecture. It manages racing clubs, pilots, teams, races, and race series for competitive drone/RC racing events.

## Architecture

### Event Sourcing Foundation (EventDbLite)
- Custom event sourcing framework that stores all state changes as events
- Aggregates are reconstituted from event streams
- Supports projections for read models
- Includes reaction system for event processing
- SignalR integration for real-time updates

### CQRS Pattern
- **Commands**: Handled through aggregates in NaeRaces.Command
- **Queries**: Handled through Entity Framework Core projections in NaeRaces.Query
- **API Layer**: ASP.NET Core WebAPI controllers expose HTTP endpoints

---

## Domain Aggregates

### 1. Club Aggregate
**Purpose**: Manages racing club organizations

**Key Responsibilities**:
- Club formation with code, name, and founder pilot
- Club details management (code, name, description, contact details)
- **Location Management**:
  - Add/remove/rename club locations
  - Track addresses and information for each location
  - Set home location
- **Membership Management**:
  - Define membership levels with age requirements
  - Multiple payment options (annual, monthly, subscription)
  - Pilot registration and confirmation
  - Membership discounts
- **Committee Management**:
  - Add/remove committee members
  - Requires confirmed membership for committee roles
- **Age & Insurance Requirements**:
  - Minimum/maximum age with validation policies
  - Insurance provider requirements
  - Government document validation requirements
- **Race Tags**: Club-specific race categorization

**Key Value Types**:
- Code, Name, Address (value objects with validation)

---

### 2. Pilot Aggregate
**Purpose**: Manages individual racer profiles

**Key Responsibilities**:
- Pilot registration with unique call sign
- Profile management (call sign, nationality, date of birth)
- **Peer Validation System**:
  - Government document validation by club committee members
  - Insurance validation by club committee members
  - Date of birth validation by club committee members
  - Tracks validation expiry dates
  - Links validations to specific clubs

**Validation Model**:
- Validations are peer-based (pilot validates another pilot)
- Tracks if validator is on club committee
- Time-limited validations (valid until date)

---

### 3. Race Aggregate
**Purpose**: Manages individual race events

**Key Responsibilities**:
- **Race Planning**:
  - Regular races (linked to club and location)
  - Team races (with team size requirements)
  - Race description
- **Validation Policy**:
  - Set/migrate/remove validation policies
  - Version tracking for policy changes
  - Cannot change once published
- **Schedule Management**:
  - Multiple race dates with start/end times
  - Reschedule dates
  - Cancel specific dates
  - Registration open date
  - Payment deadline
  - Go/No-Go decision date
- **Cost Management**:
  - Multi-currency support
  - Cost increases/reductions tracked separately
  - Club membership level discounts
  - Unpaid registration permissions
- **Attendee Management**:
  - Minimum/maximum attendees
  - Team attendance permissions
  - Team substitution permissions
  - Individual pilot attendance permissions
- **Team Race Settings**:
  - Fixed/flexible team sizes (min/max)
  - Team count limits (min/max teams)
- **Registration**:
  - Team roster registration
  - Individual pilot registration
  - Confirmation workflow
  - Cancellation support
  - Tracks base price and discounts
- **Publishing Workflow**:
  - Publish details (partial)
  - Publish race (full)
  - Many settings locked after publishing
- **Tagging**:
  - Global tags
  - Club-specific tags

**Business Rules**:
- Cannot modify many settings after publishing
- Race dates cannot overlap
- Registration must open before race dates
- Validation policy versions require migration

---

### 4. RacePolicy Aggregate
**Purpose**: Defines complex validation requirements for races

**Key Responsibilities**:
- Create named validation policies for clubs
- **Requirement Types**:
  - Minimum age with validation policy
  - Maximum age with validation policy
  - Insurance provider with validation policy
  - Government document with validation policy
  - Club membership requirement
  - Club membership level requirement
- **Composite Statements**:
  - Combine requirements using operands (AND/OR)
  - Support bracketed expressions
  - Prevent recursive loops
  - Track statement references
- **Root Statement**: Defines the entry point for policy evaluation

**Statement Model**:
- Each requirement generates a statement ID
- Composite statements combine two statements with an operand
- Tree structure with root statement
- Cannot remove statements that are referenced

---

### 5. RaceSeries Aggregate
**Purpose**: Groups related races into a series

**Key Responsibilities**:
- Plan race series with name
- Add races to series
- Remove races from series

**Use Cases**:
- Championship series
- Seasonal competitions
- League play

---

### 6. Team Aggregate
**Purpose**: Manages racing teams and rosters

**Key Responsibilities**:
- Team formation with captain pilot
- **Team Membership**:
  - Add/remove pilots from team
  - Captain automatically added on formation
- **Roster Management**:
  - Plan race-specific rosters
  - Add pilots to rosters (must be team members)
  - Pilot substitutions for races
  - Remove pilots from rosters
  - Multiple rosters per team for different races

**Business Rules**:
- Captain is automatically a team member
- Only team members can be added to rosters
- Substitutions require both pilots to be team members

---

## API Layer (WebAPI Controllers)

All controllers follow consistent patterns:

### Controller Pattern
```
1. Dependency injection of IAggregateRepository
2. HTTP route attributes with RESTful paths
3. [FromRoute] for entity IDs in path
4. [FromQuery, BindRequired] for required parameters
5. [FromQuery] for optional parameters
6. Consistent responses:
   - POST: Created with resource URI and ID
   - PUT: Ok
   - DELETE: Ok
   - NotFound when aggregate doesn't exist
7. Save aggregate after operations
```

### Available Controllers
- **ClubCommandController**: 30+ endpoints for club management
- **PilotCommandController**: 7 endpoints for pilot management
- **RaceCommandController**: 40+ endpoints for race management
- **RacePolicyCommandController**: 10 endpoints for policy management
- **RaceSeriesCommandController**: 3 endpoints for series management
- **TeamCommandController**: 7 endpoints for team management

---

## Query Layer (NaeRaces.Query)

### Purpose
Provides optimized read models separate from command aggregates

### Implementation
- **NaeRacesQueryDbContext**: Entity Framework Core DbContext
- **Query Handlers**: Interface-based handlers (e.g., IClubUniquenessQueryHandler)
- **Models**: Read-optimized data models (e.g., ClubBasicDetails)

### Pattern
- Event handlers update read models from domain events
- Queries read from optimized database schema
- Eventual consistency model

---

## Key Design Patterns & Principles

### 1. Event Sourcing
- All state changes captured as immutable events
- Aggregates rebuilt by replaying events
- Complete audit trail
- Event handlers (When methods) apply events to state

### 2. CQRS
- Commands modify state through aggregates
- Queries read from separate optimized models
- Different scalability characteristics

### 3. Domain-Driven Design
- Aggregates enforce business rules
- Value objects (Code, Name, Address)
- Ubiquitous language in code
- Bounded contexts

### 4. Validation Strategies
- **Pre-validation**: Controller parameter validation
- **Aggregate validation**: Business rule enforcement
- **Value object validation**: Type-level constraints

### 5. Idempotency
- Many operations check current state before raising events
- Prevents duplicate events for same state change

### 6. Encapsulation
- Private nested classes for internal state (Location, Registration, etc.)
- Public methods expose only valid operations
- ThrowIfIdNotSet() guards against invalid state

---

## Business Domain: Competitive Racing

### Workflow Example: Hosting a Race

1. **Club Setup**:
   - Form club with founder
   - Add locations
   - Set up membership levels and payment options
   - Define age/insurance requirements

2. **Pilot Registration**:
   - Pilots register with call signs
   - Add personal details
   - Peer validations by club committee

3. **Race Policy Creation**:
   - Define validation requirements
   - Combine requirements into policy statements
   - Set root statement

4. **Race Planning**:
   - Plan race with club and location
   - Set validation policy
   - Schedule race dates
   - Set costs and payment deadlines
   - Configure team/individual attendance rules
   - Set attendee limits

5. **Race Publishing**:
   - Publish details for registration
   - Publish race fully

6. **Registration**:
   - Teams/pilots register
   - Apply membership discounts
   - Confirm registrations

7. **Race Execution**:
   - Go/No-Go decision
   - Handle substitutions
   - Cancel if needed

---

## Technical Considerations

### Eventual Consistency
- Read models updated asynchronously via event handlers
- Commands may succeed before queries reflect changes

### Concurrency
- Event store handles concurrent modifications
- Optimistic concurrency through version tracking

### Validation Policy Versioning
- Policies can be migrated to new versions
- Prevents breaking changes to published races

### Multi-Currency Support
- Race costs tracked per currency
- Separate events for cost increases vs. reductions

### Peer Validation Model
- Distributed trust system
- Committee members can validate
- Time-limited validations
- Club-specific validations

---

## Extension Points

### Adding New Aggregates
1. Create aggregate class inheriting from AggregateRoot<TId>
2. Define domain events
3. Implement business methods that Raise events
4. Implement When(Event) handlers
5. Create controller with command endpoints
6. Add query handlers for read models

### Adding New Requirements
- Extend RacePolicy with new requirement types
- Add corresponding validation logic
- Update policy evaluation

### Integration Points
- SignalR for real-time event notifications
- External validation systems
- Payment gateways (via registration confirmation)
- Scoring systems (race results)

---

## Summary

NaeRaces is a sophisticated event-sourced system for managing competitive racing organizations. It provides:

- **Complete club management** with locations, memberships, and committees
- **Flexible race planning** with validation policies and registration
- **Team and individual** competition support
- **Peer-based validation** system for compliance
- **Rich business rules** enforced at aggregate level
- **Full audit trail** through event sourcing
- **Scalable architecture** with CQRS pattern

The system is designed for eventual multi-tenancy (club-based), supports complex validation scenarios, and maintains strong consistency within aggregates while providing eventual consistency across bounded contexts.
