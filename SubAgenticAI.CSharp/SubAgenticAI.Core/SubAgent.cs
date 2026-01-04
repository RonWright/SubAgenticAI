namespace SubAgenticAI.Core;

/// <summary>
/// SubAgent represents a specialized, mission-scoped cognitive unit.
/// Each SubAgent operates within strict resource boundaries defined by its CloudResourceAllocationProfile.
/// </summary>
public abstract class SubAgent
{
    public string Id { get; }
    public string MissionDescription { get; }
    public CloudResourceAllocationProfile ResourceProfile { get; }
    public DateTime CreatedAt { get; }
    public SubAgentStatus Status { get; private set; }
    
    protected SubAgent(string id, string missionDescription, CloudResourceAllocationProfile resourceProfile)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        MissionDescription = missionDescription ?? throw new ArgumentNullException(nameof(missionDescription));
        ResourceProfile = resourceProfile ?? throw new ArgumentNullException(nameof(resourceProfile));
        CreatedAt = DateTime.UtcNow;
        Status = SubAgentStatus.Created;
    }
    
    /// <summary>
    /// Execute the mission assigned to this SubAgent.
    /// </summary>
    public async Task<object> ExecuteMissionAsync(CancellationToken cancellationToken)
    {
        Status = SubAgentStatus.Running;
        try
        {
            var result = await ExecuteAsync(cancellationToken);
            Status = SubAgentStatus.Completed;
            return result;
        }
        catch (Exception ex)
        {
            Status = SubAgentStatus.Failed;
            throw new SubAgentExecutionException($"SubAgent {Id} failed: {ex.Message}", ex);
        }
    }
    
    /// <summary>
    /// Abstract method to be implemented by specific SubAgent types.
    /// </summary>
    protected abstract Task<object> ExecuteAsync(CancellationToken cancellationToken);
    
    /// <summary>
    /// Terminate this SubAgent immediately.
    /// </summary>
    public void Terminate(string reason)
    {
        Status = SubAgentStatus.Terminated;
        OnTerminated(reason);
    }
    
    protected virtual void OnTerminated(string reason)
    {
        // Override in derived classes to handle cleanup
    }
}

public enum SubAgentStatus
{
    Created,
    Running,
    Completed,
    Failed,
    Terminated
}

public class SubAgentExecutionException : Exception
{
    public SubAgentExecutionException(string message) : base(message) { }
    public SubAgentExecutionException(string message, Exception innerException) : base(message, innerException) { }
}
