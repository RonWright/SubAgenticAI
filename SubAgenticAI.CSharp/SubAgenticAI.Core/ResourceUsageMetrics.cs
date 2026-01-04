namespace SubAgenticAI.Core;

/// <summary>
/// Represents real-time resource consumption metrics for a SubAgent.
/// Used for monitoring and enforcement decisions.
/// </summary>
public class ResourceUsageMetrics
{
    public string SubAgentId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    
    // Compute metrics
    public double CurrentCpuUsage { get; set; }
    public double CurrentGpuUsage { get; set; }
    public TimeSpan TotalExecutionTime { get; set; }
    public double ConsumedComputeBudget { get; set; }
    
    // Memory metrics
    public long CurrentMemoryUsageBytes { get; set; }
    public long PeakMemoryUsageBytes { get; set; }
    
    // Network metrics
    public long TotalOutboundBytes { get; set; }
    public long TotalInboundBytes { get; set; }
    public int MessageCount { get; set; }
    
    // Storage metrics
    public long CurrentStateSizeBytes { get; set; }
    public long CurrentLogSizeBytes { get; set; }
    public long TempStorageUsedBytes { get; set; }
    
    // Trust-broker metrics
    public int TrustEvaluationQueryCount { get; set; }
    
    // Cost metrics
    public decimal CurrentMissionCost { get; set; }
    public decimal TotalSubAgentCost { get; set; }
}
