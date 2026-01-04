# SubAgenticAI - C# Implementation

This is a first draft C# implementation of the SubAgenticAI architecture for managing Front Line Agents (FLA) and SubAgents (SA) based on the governance-driven model described in the white papers.

## Overview

The SubAgenticAI architecture provides a disciplined, auditable, and scalable foundation for AI agent systems by introducing:

- **Front Line Agent (FLA)**: Orchestrates SubAgents without holding domain knowledge
- **SubAgents (SA)**: Specialized, mission-scoped cognitive units with strict isolation
- **Trust-Based Access Control**: SL/TL substrate for secure information flow
- **Governed Communication**: FLA-mediated exchanges between SubAgents

## Architecture Components

### Core Classes

#### `FrontLineAgent`
The FLA acts as a chief of staff, managing the lifecycle and communication of SubAgents:
- Interprets user intent and classifies domains
- Creates and activates SubAgents
- Enforces isolation boundaries
- Authorizes and mediates cross-agent communication
- Retires SubAgents when missions conclude

#### `SubAgent`
Specialized, mission-scoped cognitive units with:
- Narrow domain focus
- Local memory and isolated state
- Trust-based inbound validation
- Security-governed outbound communication
- Complete auditability through activity logs

#### Trust System

**`TrustProfile`**: Assigned by FLA to each SubAgent, containing:
- `SecurityLevel`: Governs outbound information flow (Public, Internal, Confidential, Secret, TopSecret)
- `TrustLevel`: Required thresholds for inbound acceptance (Sender Trust + Content Trust)
- Content Sharing Policy with multiple trust brokers
- Independent agreement requirements

**`ITrustBroker`**: Interface for independent trust evaluators that assess:
- Sender Trust Level (TLₛ): Historical reliability and behavioral integrity
- Content Trust Level (TL꜀): Factual accuracy and provenance
- Flagging of untrustworthy senders or content

**`SimpleTrustBroker`**: Basic implementation for demonstration purposes

#### Supporting Classes

- `MissionStatus`: Lifecycle states (Created, Active, Paused, Completed, Failed, Retired)
- `CommunicationAuthorization`: Defines allowed exchanges between SubAgents
- `SecurityLevel`: Enum for classification levels
- `TrustLevel`: Composite trust score (sender + content)

## Key Features

### 1. Strict Isolation
SubAgents operate in isolation with their own local state, preventing context bleed and unintended coupling.

### 2. Governed Communication
All inter-agent communication is:
- Explicitly authorized by the FLA
- Mediated through the FLA
- Scoped to specific data types
- Fully logged and auditable
- Can be one-way or bidirectional

### 3. Trust-Based Security

**Outbound (Security Level)**:
- FLA assigns security levels to SubAgents
- Restricts information disclosure based on classification
- Enforced at communication boundaries

**Inbound (Trust Level)**:
- Multiple independent trust brokers evaluate sender and content
- Requires broker agreement within tolerance
- Rejects flagged senders or content
- Validates against required thresholds

### 4. Mission Lifecycle Management
- SubAgents are created for specific missions
- Can be short-lived (created and retired) or long-running
- Status tracking through mission lifecycle
- Automatic cleanup on retirement

### 5. Complete Auditability
- FLA maintains event log of all orchestration activities
- SubAgents maintain activity logs
- Communication authorizations are logged
- All state transitions are traceable

## Building and Running

### Prerequisites
- .NET 8.0 SDK or later

### Build
```bash
dotnet build
```

### Run Demo
```bash
dotnet run --project SubAgenticAI.Demo
```

## Demo Application

The included demo (`SubAgenticAI.Demo`) demonstrates:

1. **Trust Broker Setup**: Three independent brokers with different baseline trust levels
2. **Trust Profile Creation**: Configuring security and trust requirements
3. **FLA Creation**: Initializing the orchestration layer
4. **Task Processing**: Automatic domain classification and SubAgent creation
5. **Isolation Enforcement**: Communication blocked without authorization
6. **Authorized Communication**: FLA-mediated exchanges
7. **Trust Validation**: Accepting trusted sources, rejecting flagged senders
8. **Mission Lifecycle**: SubAgent retirement and cleanup
9. **Audit Trail**: Complete event logging

## Usage Example

```csharp
// Create trust brokers
var broker1 = new SimpleTrustBroker("TB-001", "Broker Alpha", 0.7);
var broker2 = new SimpleTrustBroker("TB-002", "Broker Beta", 0.75);

// Create trust profile
var trustProfile = new TrustProfile(
    securityLevel: SecurityLevel.Internal,
    requiredTrustThreshold: new TrustLevel(0.6, 0.6),
    trustBrokers: new List<ITrustBroker> { broker1, broker2 },
    minimumBrokerAgreement: 2
);

// Create FLA
var fla = new FrontLineAgent("FLA-001", trustProfile);

// Process user task (automatically creates appropriate SubAgent)
var result = await fla.ProcessUserTaskAsync(
    "Analyze data", 
    "Perform sentiment analysis"
);

// Create SubAgents manually for complex scenarios
var agent1 = await fla.CreateSubAgentAsync("Research");
var agent2 = await fla.CreateSubAgentAsync("Analysis");

// Authorize communication
fla.AuthorizeCommunication(agent1.Id, agent2.Id, isBidirectional: true);

// Mediate communication
await fla.MediateCommunicationAsync(
    agent1.Id, 
    agent2.Id, 
    "Research findings..."
);

// Clean up
await fla.RetireSubAgentAsync(agent1.Id);
```

## Project Structure

```
SubAgenticAI/
├── SubAgenticAI.sln                     # Solution file
├── SubAgenticAI.Core/                   # Core library
│   ├── FrontLineAgent.cs                # FLA orchestration
│   ├── SubAgent.cs                      # SubAgent implementation
│   ├── ISubAgent.cs                     # SubAgent interface
│   ├── TrustProfile.cs                  # Trust profile management
│   ├── TrustLevel.cs                    # Trust level composite
│   ├── SecurityLevel.cs                 # Security classification
│   ├── ITrustBroker.cs                  # Trust broker interface
│   ├── SimpleTrustBroker.cs            # Basic trust broker
│   ├── MissionStatus.cs                 # Mission lifecycle states
│   └── CommunicationAuthorization.cs    # Communication rules
└── SubAgenticAI.Demo/                   # Demo application
    └── Program.cs                        # Example usage
```

## Design Principles

This implementation follows the governance principles outlined in the white papers:

### Military-Inspired
- Clear chain of command (FLA → SA)
- Mission-scoped operations
- Rules of engagement (trust profiles)

### Government-Inspired
- Separation of powers (FLA orchestrates, SA executes)
- Checks and balances (trust broker agreement)
- Auditability (complete event logging)

### Corporate-Inspired
- Domain-specific teams (specialized SubAgents)
- Project-based task forces (mission lifecycle)

### Scientific-Inspired
- Independent verification (multiple trust brokers)
- Reproducibility (audit trails)
- Peer review (broker consensus)

## Future Enhancements

This is a first draft. Future versions could include:

1. **Persistence**: Save/restore SubAgent state and FLA configuration
2. **Advanced Trust Brokers**: Integration with external trust services
3. **Communication Protocols**: Standardized message formats
4. **Scaling**: Distributed FLA and SubAgent deployment
5. **Monitoring**: Real-time dashboards and metrics
6. **Policy Engine**: More sophisticated authorization rules
7. **Learning**: Trust broker models that improve over time
8. **Integration**: APIs for external system connectivity

## References

- [README.md](../README.md) - SubAgenticAI conceptual architecture
- [Trust-Based-Autonomous-Agent-Ecosystems.md](../Trust-Based-Autonomous-Agent-Ecosystems.md) - SL/TL substrate specification

## License

This is a conceptual implementation for demonstration purposes.
