# SubAgenticAI - Python Implementation

This is a Python implementation of the SubAgenticAI architecture for managing Front Line Agents (FLA) and SubAgents (SA) based on the governance-driven model described in the white papers.

## Overview

The SubAgenticAI architecture provides a disciplined, auditable, and scalable foundation for AI agent systems by introducing:

- **Front Line Agent (FLA)**: Orchestrates SubAgents without holding domain knowledge
- **SubAgents (SA)**: Specialized, mission-scoped cognitive units with strict isolation
- **Trust-Based Access Control**: SL/TL substrate for secure information flow
- **Governed Communication**: FLA-mediated exchanges between SubAgents

## Installation

### From Source

```bash
cd subagentic_ai_py
pip install -e .
```

### Development Installation

```bash
cd subagentic_ai_py
pip install -e ".[dev]"
```

## Quick Start

```python
import asyncio
from subagentic_ai import (
    FrontLineAgent,
    SimpleTrustBroker,
    TrustProfile,
    SecurityLevel,
    TrustLevel,
)

async def main():
    # Setup trust brokers
    broker1 = SimpleTrustBroker("TB-001", "Broker Alpha", 0.7)
    broker2 = SimpleTrustBroker("TB-002", "Broker Beta", 0.75)
    
    # Create trust profile
    trust_profile = TrustProfile(
        security_level=SecurityLevel.INTERNAL,
        required_trust_threshold=TrustLevel(0.6, 0.6),
        trust_brokers=(broker1, broker2),
        minimum_broker_agreement=2
    )
    
    # Create FLA
    fla = FrontLineAgent("FLA-001", trust_profile)
    
    # Process user task (automatically creates appropriate SubAgent)
    result = await fla.process_user_task(
        "Analyze data",
        "Perform sentiment analysis"
    )
    print(result)

asyncio.run(main())
```

## Running the Demo

```bash
cd subagentic_ai_py
python demo/demo.py
```

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
- `SecurityLevel`: Governs outbound information flow (PUBLIC, INTERNAL, CONFIDENTIAL, SECRET, TOP_SECRET)
- `TrustLevel`: Required thresholds for inbound acceptance (Sender Trust + Content Trust)
- Content Sharing Policy with multiple trust brokers
- Independent agreement requirements

**`ITrustBroker`**: Interface for independent trust evaluators that assess:
- Sender Trust Level (TLₛ): Historical reliability and behavioral integrity
- Content Trust Level (TL꜀): Factual accuracy and provenance
- Flagging of untrustworthy senders or content

**`SimpleTrustBroker`**: Basic implementation for demonstration purposes

#### Supporting Classes

- `MissionStatus`: Lifecycle states (CREATED, ACTIVE, PAUSED, COMPLETED, FAILED, RETIRED)
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

## Project Structure

```
subagentic_ai_py/
├── pyproject.toml                        # Project configuration
├── README.md                             # This file
├── subagentic_ai/                        # Core library
│   ├── __init__.py                       # Package initialization
│   ├── front_line_agent.py               # FLA orchestration
│   ├── sub_agent.py                      # SubAgent implementation
│   ├── trust_profile.py                  # Trust profile management
│   ├── trust_level.py                    # Trust level composite
│   ├── security_level.py                 # Security classification
│   ├── trust_broker.py                   # Trust broker interface & impl
│   ├── mission_status.py                 # Mission lifecycle states
│   └── communication_authorization.py    # Communication rules
└── demo/                                  # Demo application
    └── demo.py                            # Example usage
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

## Comparison with C# Implementation

This Python implementation is functionally equivalent to the C# implementation (`SubAgenticAI.Core`), providing:

- **Same Architecture**: Identical FLA/SA orchestration model
- **Same Security Model**: SL/TL substrate with independent broker validation
- **Pythonic Design**: Using dataclasses, async/await, type hints
- **Zero Dependencies**: Pure Python 3.8+ implementation (core library)

## References

- [README.md](../../README.md) - SubAgenticAI conceptual architecture
- [Trust-Based-Autonomous-Agent-Ecosystems.md](../../Trust-Based-Autonomous-Agent-Ecosystems.md) - SL/TL substrate specification
- [IMPLEMENTATION.md](../../IMPLEMENTATION.md) - C# implementation details

## License

This is a conceptual implementation for demonstration purposes.
