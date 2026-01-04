namespace SubAgenticAI.Core;

/// <summary>
/// Cloud Resource Allocation Profile defines the maximum allowable consumption 
/// across cloud-native resource categories for a SubAgent.
/// This profile is immutable for the lifetime of the SubAgent.
/// </summary>
public class CloudResourceAllocationProfile
{
    // Compute Resources
    public double MaxCpuCores { get; init; }
    public double MaxGpuAllocation { get; init; }
    public TimeSpan MaxExecutionTimePerInvocation { get; init; }
    public double TotalLifetimeComputeBudget { get; init; }

    // Memory Resources
    public long MaxMemoryFootprintBytes { get; init; }
    public long MaxMemoryGrowthRatePerSecond { get; init; }
    public long ContainerMemoryCeilingBytes { get; init; }

    // Network Resources
    public long MaxOutboundBandwidthBytesPerSec { get; init; }
    public long MaxInboundBandwidthBytesPerSec { get; init; }
    public int MaxMessageCount { get; init; }
    public bool AllowCrossRegionCommunication { get; init; }

    // Storage Resources
    public long MaxStateSizeBytes { get; init; }
    public long MaxLogSizeBytes { get; init; }
    public long TempStorageQuotaBytes { get; init; }

    // Trust-Broker Resources
    public int MaxTrustLevelEvaluationQueries { get; init; }
    public int TrustBrokerRateLimitPerMinute { get; init; }
    public bool AllowCrossCloudTrustBrokerAccess { get; init; }

    // Cost Controls
    public decimal MaxCostPerMission { get; init; }
    public decimal MaxCostPerSubAgent { get; init; }
    public bool HardBudgetEnforcement { get; init; }

    public CloudResourceAllocationProfile()
    {
        // Default values for basic profile
        MaxCpuCores = 1.0;
        MaxGpuAllocation = 0.0;
        MaxExecutionTimePerInvocation = TimeSpan.FromMinutes(5);
        TotalLifetimeComputeBudget = 100.0;
        
        MaxMemoryFootprintBytes = 512 * 1024 * 1024; // 512 MB
        MaxMemoryGrowthRatePerSecond = 10 * 1024 * 1024; // 10 MB/s
        ContainerMemoryCeilingBytes = 1024 * 1024 * 1024; // 1 GB
        
        MaxOutboundBandwidthBytesPerSec = 10 * 1024 * 1024; // 10 MB/s
        MaxInboundBandwidthBytesPerSec = 10 * 1024 * 1024; // 10 MB/s
        MaxMessageCount = 1000;
        AllowCrossRegionCommunication = false;
        
        MaxStateSizeBytes = 100 * 1024 * 1024; // 100 MB
        MaxLogSizeBytes = 50 * 1024 * 1024; // 50 MB
        TempStorageQuotaBytes = 200 * 1024 * 1024; // 200 MB
        
        MaxTrustLevelEvaluationQueries = 100;
        TrustBrokerRateLimitPerMinute = 60;
        AllowCrossCloudTrustBrokerAccess = false;
        
        MaxCostPerMission = 10.0m;
        MaxCostPerSubAgent = 50.0m;
        HardBudgetEnforcement = true;
    }
}
