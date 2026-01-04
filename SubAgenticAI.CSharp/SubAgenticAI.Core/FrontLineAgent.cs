namespace SubAgenticAI.Core;

/// <summary>
/// The FrontLineAgent (FLA) is the authoritative orchestrator for SubAgent execution.
/// It manages resource allocation, monitoring, and enforcement in cloud-native environments.
/// </summary>
public class FrontLineAgent
{
    private readonly Dictionary<string, SubAgent> _activeSubAgents;
    private readonly Dictionary<string, ResourceUsageMetrics> _metrics;
    private readonly List<EnforcementAction> _auditLog;
    private readonly object _lock = new object();
    
    public FrontLineAgent()
    {
        _activeSubAgents = new Dictionary<string, SubAgent>();
        _metrics = new Dictionary<string, ResourceUsageMetrics>();
        _auditLog = new List<EnforcementAction>();
    }
    
    /// <summary>
    /// Provision a new SubAgent with specified resource profile.
    /// </summary>
    public void ProvisionSubAgent(SubAgent subAgent)
    {
        if (subAgent == null)
            throw new ArgumentNullException(nameof(subAgent));
            
        lock (_lock)
        {
            if (_activeSubAgents.ContainsKey(subAgent.Id))
                throw new InvalidOperationException($"SubAgent {subAgent.Id} already exists");
                
            _activeSubAgents[subAgent.Id] = subAgent;
            _metrics[subAgent.Id] = new ResourceUsageMetrics 
            { 
                SubAgentId = subAgent.Id,
                Timestamp = DateTime.UtcNow
            };
            
            LogAudit(subAgent.Id, EnforcementLevel.None, "SubAgent provisioned", "System", 0, false);
        }
    }
    
    /// <summary>
    /// Monitor resource usage and apply enforcement if needed.
    /// </summary>
    public void MonitorAndEnforce(string subAgentId, ResourceUsageMetrics currentMetrics)
    {
        if (!_activeSubAgents.TryGetValue(subAgentId, out var subAgent))
            return;
            
        lock (_lock)
        {
            _metrics[subAgentId] = currentMetrics;
            
            var profile = subAgent.ResourceProfile;
            
            // Check for resource limit violations
            CheckComputeLimits(subAgent, currentMetrics, profile);
            CheckMemoryLimits(subAgent, currentMetrics, profile);
            CheckNetworkLimits(subAgent, currentMetrics, profile);
            CheckStorageLimits(subAgent, currentMetrics, profile);
            CheckCostLimits(subAgent, currentMetrics, profile);
        }
    }
    
    private void CheckComputeLimits(SubAgent subAgent, ResourceUsageMetrics metrics, CloudResourceAllocationProfile profile)
    {
        // Check CPU usage
        if (metrics.CurrentCpuUsage >= profile.MaxCpuCores * 0.9)
        {
            ApplySoftEnforcement(subAgent.Id, "Compute", "CPU approaching limit", metrics.CurrentCpuUsage / profile.MaxCpuCores * 100);
        }
        
        if (metrics.CurrentCpuUsage > profile.MaxCpuCores)
        {
            ApplyHardEnforcement(subAgent, "Compute", "CPU limit exceeded");
        }
        
        // Check execution time
        if (metrics.TotalExecutionTime >= profile.MaxExecutionTimePerInvocation * 0.9)
        {
            ApplySoftEnforcement(subAgent.Id, "Compute", "Execution time approaching limit", 
                metrics.TotalExecutionTime.TotalSeconds / profile.MaxExecutionTimePerInvocation.TotalSeconds * 100);
        }
        
        if (metrics.TotalExecutionTime > profile.MaxExecutionTimePerInvocation)
        {
            ApplyHardEnforcement(subAgent, "Compute", "Execution time limit exceeded");
        }
    }
    
    private void CheckMemoryLimits(SubAgent subAgent, ResourceUsageMetrics metrics, CloudResourceAllocationProfile profile)
    {
        if (metrics.CurrentMemoryUsageBytes >= profile.MaxMemoryFootprintBytes * 0.9)
        {
            ApplySoftEnforcement(subAgent.Id, "Memory", "Memory approaching limit", 
                (double)metrics.CurrentMemoryUsageBytes / profile.MaxMemoryFootprintBytes * 100);
        }
        
        if (metrics.CurrentMemoryUsageBytes > profile.MaxMemoryFootprintBytes)
        {
            ApplyHardEnforcement(subAgent, "Memory", "Memory limit exceeded");
        }
    }
    
    private void CheckNetworkLimits(SubAgent subAgent, ResourceUsageMetrics metrics, CloudResourceAllocationProfile profile)
    {
        if (metrics.MessageCount >= profile.MaxMessageCount * 0.9)
        {
            ApplySoftEnforcement(subAgent.Id, "Network", "Message count approaching limit", 
                (double)metrics.MessageCount / profile.MaxMessageCount * 100);
        }
        
        if (metrics.MessageCount > profile.MaxMessageCount)
        {
            ApplyHardEnforcement(subAgent, "Network", "Message count limit exceeded");
        }
    }
    
    private void CheckStorageLimits(SubAgent subAgent, ResourceUsageMetrics metrics, CloudResourceAllocationProfile profile)
    {
        if (metrics.CurrentStateSizeBytes >= profile.MaxStateSizeBytes * 0.9)
        {
            ApplySoftEnforcement(subAgent.Id, "Storage", "State size approaching limit", 
                (double)metrics.CurrentStateSizeBytes / profile.MaxStateSizeBytes * 100);
        }
        
        if (metrics.CurrentStateSizeBytes > profile.MaxStateSizeBytes)
        {
            ApplyHardEnforcement(subAgent, "Storage", "State size limit exceeded");
        }
    }
    
    private void CheckCostLimits(SubAgent subAgent, ResourceUsageMetrics metrics, CloudResourceAllocationProfile profile)
    {
        if (metrics.CurrentMissionCost >= profile.MaxCostPerMission * 0.9m)
        {
            ApplySoftEnforcement(subAgent.Id, "Cost", "Mission cost approaching limit", 
                (double)(metrics.CurrentMissionCost / profile.MaxCostPerMission) * 100);
        }
        
        if (profile.HardBudgetEnforcement && metrics.CurrentMissionCost > profile.MaxCostPerMission)
        {
            ApplyHardEnforcement(subAgent, "Cost", "Mission cost limit exceeded");
        }
    }
    
    private void ApplySoftEnforcement(string subAgentId, string resourceType, string reason, double threshold)
    {
        LogAudit(subAgentId, EnforcementLevel.SoftEnforcement, reason, resourceType, threshold, false);
    }
    
    private void ApplyHardEnforcement(SubAgent subAgent, string resourceType, string reason)
    {
        subAgent.Terminate(reason);
        LogAudit(subAgent.Id, EnforcementLevel.HardEnforcement, reason, resourceType, 100, true);
        
        lock (_lock)
        {
            _activeSubAgents.Remove(subAgent.Id);
        }
    }
    
    private void LogAudit(string subAgentId, EnforcementLevel level, string reason, string resourceType, double threshold, bool terminated)
    {
        var action = new EnforcementAction
        {
            SubAgentId = subAgentId,
            Level = level,
            Reason = reason,
            ResourceType = resourceType,
            ThresholdPercentage = threshold,
            Terminated = terminated
        };
        
        _auditLog.Add(action);
    }
    
    /// <summary>
    /// Get current resource usage for a SubAgent.
    /// </summary>
    public ResourceUsageMetrics? GetResourceUsage(string subAgentId)
    {
        lock (_lock)
        {
            return _metrics.TryGetValue(subAgentId, out var metrics) ? metrics : null;
        }
    }
    
    /// <summary>
    /// Get all enforcement actions for audit purposes.
    /// </summary>
    public IReadOnlyList<EnforcementAction> GetAuditLog()
    {
        lock (_lock)
        {
            return _auditLog.AsReadOnly();
        }
    }
    
    /// <summary>
    /// Get all active SubAgents.
    /// </summary>
    public IReadOnlyCollection<SubAgent> GetActiveSubAgents()
    {
        lock (_lock)
        {
            return _activeSubAgents.Values.ToList().AsReadOnly();
        }
    }
    
    /// <summary>
    /// Terminate a specific SubAgent.
    /// </summary>
    public void TerminateSubAgent(string subAgentId, string reason)
    {
        lock (_lock)
        {
            if (_activeSubAgents.TryGetValue(subAgentId, out var subAgent))
            {
                subAgent.Terminate(reason);
                LogAudit(subAgentId, EnforcementLevel.HardEnforcement, $"Manual termination: {reason}", "Manual", 0, true);
                _activeSubAgents.Remove(subAgentId);
            }
        }
    }
}
