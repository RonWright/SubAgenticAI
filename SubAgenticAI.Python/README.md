# SubAgenticAI - Python Implementation

This is a Python implementation of the SubAgenticAI architecture with Cloud-Native Resource Governance.

## Overview

SubAgenticAI is a governance-driven architecture for autonomous agent ecosystems. This implementation incorporates cloud-native resource governance principles to ensure SubAgents operate within strict, immutable resource boundaries.

## Architecture

### Core Components

#### CloudResourceAllocationProfile
Defines the maximum allowable resource consumption across multiple categories:
- **Compute Resources**: CPU cores, GPU allocation, execution time limits, compute budgets
- **Memory Resources**: Memory footprint, growth rate limits, container ceilings
- **Network Resources**: Bandwidth limits, message counts, cross-region policies
- **Storage Resources**: State size, log size, temporary storage quotas
- **Trust-Broker Resources**: TL evaluation queries, rate limits, cross-cloud access
- **Cost Controls**: Per-mission and per-SubAgent cost ceilings, budget enforcement

#### FrontLineAgent (FLA)
The authoritative orchestrator responsible for:
- Provisioning compute environments for SubAgents
- Enforcing resource quotas and budgets
- Monitoring real-time resource consumption
- Applying soft and hard enforcement actions
- Maintaining comprehensive audit logs
- Terminating workloads that exceed limits

#### SubAgent
Mission-scoped cognitive units that:
- Operate within strict resource boundaries
- Execute specific missions with defined goals
- Have no direct access to other SubAgents
- Are monitored externally by the FLA
- Can be terminated when limits are exceeded

## Resource Governance

### Soft Enforcement
Triggered when consumption approaches limits (typically 90%):
- Throttling compute resources
- Reducing network throughput
- Delaying trust-broker queries
- Issuing warnings to audit logs

### Hard Enforcement
Triggered when limits are exceeded:
- Immediate termination of the SubAgent
- Revocation of compute resources
- State snapshot and archival
- Comprehensive audit log entry

## Installation

```bash
cd SubAgenticAI.Python
pip install -e .
```

## Usage Example

```python
import asyncio
from datetime import timedelta
from subagentic_ai import (
    CloudResourceAllocationProfile,
    FrontLineAgent,
    SubAgent,
    ResourceUsageMetrics
)

# Define a custom SubAgent
class MyCustomSubAgent(SubAgent):
    async def _execute(self):
        # Implement mission logic here
        await asyncio.sleep(1)
        return {"status": "success"}

# Create a FrontLineAgent
fla = FrontLineAgent()

# Define resource profile for a SubAgent
resource_profile = CloudResourceAllocationProfile(
    max_cpu_cores=2.0,
    max_memory_footprint_bytes=1024 * 1024 * 1024,  # 1 GB
    max_execution_time_per_invocation=timedelta(minutes=10),
    max_cost_per_mission=5.0,
    hard_budget_enforcement=True
)

# Create and provision a SubAgent
sub_agent = MyCustomSubAgent(
    agent_id="sa-001",
    mission_description="Process user data",
    resource_profile=resource_profile
)
fla.provision_sub_agent(sub_agent)

# Monitor and enforce resource limits
metrics = ResourceUsageMetrics(
    sub_agent_id="sa-001",
    current_cpu_usage=1.5,
    current_memory_usage_bytes=512 * 1024 * 1024
)
fla.monitor_and_enforce("sa-001", metrics)

# Get audit log
audit_log = fla.get_audit_log()
for action in audit_log:
    print(f"{action.timestamp}: {action.level} - {action.reason}")
```

## Key Principles

1. **Isolation**: SubAgents operate in isolated environments with no direct access to each other
2. **Immutability**: Resource allocation profiles cannot be changed after SubAgent creation (enforced via frozen dataclasses)
3. **External Monitoring**: SubAgents do not self-report; monitoring is authoritative and external
4. **Predictability**: Strict boundaries ensure predictable behavior and cost control
5. **Auditability**: All actions are logged for compliance and analysis

## Development

### Running Tests
```bash
# Install development dependencies
pip install -e ".[dev]"

# Run tests
pytest tests/
```

### Type Checking
```bash
mypy subagentic_ai/
```

## References

- [Cloud-Native-Resource-Governance.md](../Cloud-Native-Resource-Governance.md)
- [Trust-Based-Autonomous-Agent-Ecosystems.md](../Trust-Based-Autonomous-Agent-Ecosystems.md)
- [README.md](../README.md)
