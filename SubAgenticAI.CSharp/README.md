# SubAgenticAI - C# Implementation

This is a C# implementation of the SubAgenticAI architecture with Cloud-Native Resource Governance.

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

## Building the Project

```bash
cd SubAgenticAI.CSharp
dotnet build
```

## Usage Example

```csharp
using SubAgenticAI.Core;

// Create a FrontLineAgent
var fla = new FrontLineAgent();

// Define resource profile for a SubAgent
var resourceProfile = new CloudResourceAllocationProfile
{
    MaxCpuCores = 2.0,
    MaxMemoryFootprintBytes = 1024 * 1024 * 1024, // 1 GB
    MaxExecutionTimePerInvocation = TimeSpan.FromMinutes(10),
    MaxCostPerMission = 5.0m,
    HardBudgetEnforcement = true
};

// Create and provision a SubAgent
var subAgent = new MyCustomSubAgent("sa-001", "Process user data", resourceProfile);
fla.ProvisionSubAgent(subAgent);

// Monitor and enforce resource limits
var metrics = new ResourceUsageMetrics
{
    SubAgentId = "sa-001",
    CurrentCpuUsage = 1.5,
    CurrentMemoryUsageBytes = 512 * 1024 * 1024,
    // ... other metrics
};
fla.MonitorAndEnforce("sa-001", metrics);

// Get audit log
var auditLog = fla.GetAuditLog();
```

## Key Principles

1. **Isolation**: SubAgents operate in isolated environments with no direct access to each other
2. **Immutability**: Resource allocation profiles cannot be changed after SubAgent creation
3. **External Monitoring**: SubAgents do not self-report; monitoring is authoritative and external
4. **Predictability**: Strict boundaries ensure predictable behavior and cost control
5. **Auditability**: All actions are logged for compliance and analysis

## References

- [Cloud-Native-Resource-Governance.md](../Cloud-Native-Resource-Governance.md)
- [Trust-Based-Autonomous-Agent-Ecosystems.md](../Trust-Based-Autonomous-Agent-Ecosystems.md)
- [README.md](../README.md)
