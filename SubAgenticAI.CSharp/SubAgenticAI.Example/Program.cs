using SubAgenticAI.Core;

// Example demonstrating Cloud-Native Resource Governance in C#

Console.WriteLine("=== SubAgenticAI C# Example ===\n");

// Create a FrontLineAgent
var fla = new FrontLineAgent();
Console.WriteLine("1. FrontLineAgent created");

// Define a custom resource profile with stricter limits
var resourceProfile = new CloudResourceAllocationProfile
{
    MaxCpuCores = 2.0,
    MaxMemoryFootprintBytes = 1024 * 1024 * 1024, // 1 GB
    MaxExecutionTimePerInvocation = TimeSpan.FromMinutes(10),
    MaxMessageCount = 500,
    MaxCostPerMission = 5.0m,
    HardBudgetEnforcement = true
};
Console.WriteLine("2. Resource profile defined");

// Create a sample SubAgent
var subAgent = new SampleSubAgent("sa-001", "Process sample data", resourceProfile);
Console.WriteLine($"3. SubAgent created: {subAgent.Id}");

// Provision the SubAgent
fla.ProvisionSubAgent(subAgent);
Console.WriteLine("4. SubAgent provisioned by FLA");

// Simulate resource usage - within limits
var metrics1 = new ResourceUsageMetrics
{
    SubAgentId = "sa-001",
    CurrentCpuUsage = 1.5,
    CurrentMemoryUsageBytes = 512 * 1024 * 1024, // 512 MB
    MessageCount = 100,
    CurrentMissionCost = 2.0m,
    Timestamp = DateTime.UtcNow
};
fla.MonitorAndEnforce("sa-001", metrics1);
Console.WriteLine("5. Monitoring: Resources within limits");

// Simulate resource usage - approaching limits (soft enforcement)
var metrics2 = new ResourceUsageMetrics
{
    SubAgentId = "sa-001",
    CurrentCpuUsage = 1.85, // 92.5% of limit
    CurrentMemoryUsageBytes = 950 * 1024 * 1024, // 92.8% of limit
    MessageCount = 460, // 92% of limit
    CurrentMissionCost = 4.7m, // 94% of limit
    Timestamp = DateTime.UtcNow
};
fla.MonitorAndEnforce("sa-001", metrics2);
Console.WriteLine("6. Monitoring: Resources approaching limits (soft enforcement triggered)");

// Get audit log
var auditLog = fla.GetAuditLog();
Console.WriteLine($"\n7. Audit Log Entries: {auditLog.Count}");
foreach (var action in auditLog)
{
    Console.WriteLine($"   - [{action.Timestamp:HH:mm:ss}] {action.Level}: {action.Reason} ({action.ResourceType})");
}

// Get active SubAgents
var activeAgents = fla.GetActiveSubAgents();
Console.WriteLine($"\n8. Active SubAgents: {activeAgents.Count}");

Console.WriteLine("\n=== Example completed successfully ===");

// Sample SubAgent implementation
public class SampleSubAgent : SubAgent
{
    public SampleSubAgent(string id, string missionDescription, CloudResourceAllocationProfile resourceProfile)
        : base(id, missionDescription, resourceProfile)
    {
    }

    protected override async Task<object> ExecuteAsync(CancellationToken cancellationToken)
    {
        // Simulate some work
        await Task.Delay(100, cancellationToken);
        return new { Status = "Success", Message = "Sample mission completed" };
    }
}
