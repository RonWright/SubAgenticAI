namespace SubAgenticAI.Core;

/// <summary>
/// Implementation of a SubAgent - specialized, mission-scoped cognitive unit with strict isolation
/// </summary>
public class SubAgent : ISubAgent
{
    private readonly Dictionary<string, object> _localState;
    private readonly List<string> _activityLog;
    
    public string Id { get; }
    public string Domain { get; }
    public MissionStatus Status { get; private set; }
    public TrustProfile TrustProfile { get; }
    
    /// <summary>
    /// Creates a new SubAgent
    /// </summary>
    public SubAgent(string id, string domain, TrustProfile trustProfile)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        Domain = domain ?? throw new ArgumentNullException(nameof(domain));
        TrustProfile = trustProfile ?? throw new ArgumentNullException(nameof(trustProfile));
        
        _localState = new Dictionary<string, object>();
        _activityLog = new List<string>();
        Status = MissionStatus.Created;
        
        LogActivity($"SubAgent created: {Id} for domain: {Domain}");
    }
    
    public Task StartMissionAsync()
    {
        if (Status != MissionStatus.Created)
        {
            throw new InvalidOperationException($"Cannot start mission from status: {Status}");
        }
        
        Status = MissionStatus.Active;
        LogActivity("Mission started");
        return Task.CompletedTask;
    }
    
    public async Task<string> ProcessTaskAsync(string task)
    {
        if (Status != MissionStatus.Active)
        {
            throw new InvalidOperationException($"Cannot process task in status: {Status}");
        }
        
        LogActivity($"Processing task: {task}");
        
        // This is a placeholder for actual task processing logic
        // In a real implementation, this would involve domain-specific processing
        await Task.Delay(10); // Simulate processing
        
        var result = $"Processed: {task} in domain: {Domain}";
        LogActivity($"Task completed: {result}");
        
        return result;
    }
    
    public async Task<bool> ReceiveInformationAsync(string senderId, string information)
    {
        if (Status != MissionStatus.Active)
        {
            LogActivity($"Rejected inbound information - status: {Status}");
            return false;
        }
        
        // Validate inbound information using trust profile
        var isValid = await TrustProfile.ValidateInboundAsync(senderId, information);
        
        if (isValid)
        {
            _localState[$"received_{DateTime.UtcNow.Ticks}"] = information;
            LogActivity($"Accepted information from: {senderId}");
            return true;
        }
        
        LogActivity($"Rejected information from: {senderId} - trust validation failed");
        return false;
    }
    
    public Task<bool> SendInformationAsync(string recipientId, string information)
    {
        if (Status != MissionStatus.Active)
        {
            LogActivity($"Cannot send information - status: {Status}");
            return Task.FromResult(false);
        }
        
        // Security level check for outbound information
        // In a real implementation, this would check against recipient's clearance
        LogActivity($"Sending information to: {recipientId} (SL: {TrustProfile.SecurityLevel})");
        return Task.FromResult(true);
    }
    
    public Task CompleteMissionAsync()
    {
        if (Status != MissionStatus.Active && Status != MissionStatus.Paused)
        {
            throw new InvalidOperationException($"Cannot complete mission from status: {Status}");
        }
        
        Status = MissionStatus.Completed;
        LogActivity("Mission completed");
        return Task.CompletedTask;
    }
    
    public Task RetireAsync()
    {
        if (Status == MissionStatus.Retired)
        {
            return Task.CompletedTask;
        }
        
        Status = MissionStatus.Retired;
        LogActivity("SubAgent retired");
        
        // Clear local state for garbage collection
        _localState.Clear();
        
        return Task.CompletedTask;
    }
    
    /// <summary>
    /// Gets the activity log for auditing purposes
    /// </summary>
    public IReadOnlyList<string> GetActivityLog() => _activityLog.AsReadOnly();
    
    /// <summary>
    /// Gets a snapshot of local state (for debugging/auditing)
    /// </summary>
    public IReadOnlyDictionary<string, object> GetStateSnapshot() => 
        new Dictionary<string, object>(_localState);
    
    private void LogActivity(string activity)
    {
        var logEntry = $"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff}] {activity}";
        _activityLog.Add(logEntry);
    }
}
