namespace SubAgenticAI.Core;

/// <summary>
/// Front Line Agent (FLA) - orchestrates SubAgents, enforces isolation, and manages communication.
/// Acts as chief of staff without holding domain knowledge.
/// </summary>
public class FrontLineAgent
{
    private readonly Dictionary<string, ISubAgent> _activeSubAgents;
    private readonly List<CommunicationAuthorization> _communicationAuthorizations;
    private readonly List<string> _eventLog;
    private readonly TrustProfile _defaultTrustProfile;
    
    public string Id { get; }
    
    /// <summary>
    /// Creates a new Front Line Agent
    /// </summary>
    public FrontLineAgent(string id, TrustProfile defaultTrustProfile)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        _defaultTrustProfile = defaultTrustProfile ?? throw new ArgumentNullException(nameof(defaultTrustProfile));
        
        _activeSubAgents = new Dictionary<string, ISubAgent>();
        _communicationAuthorizations = new List<CommunicationAuthorization>();
        _eventLog = new List<string>();
        
        LogEvent($"FLA created: {Id}");
    }
    
    /// <summary>
    /// Interprets user intent and classifies the domain
    /// </summary>
    public string ClassifyDomain(string userIntent)
    {
        LogEvent($"Classifying domain for intent: {userIntent}");
        
        // This is a simplified classification - in real implementation would use ML/NLP
        var intent = userIntent.ToLowerInvariant();
        
        if (intent.Contains("data") || intent.Contains("analyze"))
            return "DataAnalysis";
        if (intent.Contains("code") || intent.Contains("program"))
            return "CodeGeneration";
        if (intent.Contains("security") || intent.Contains("audit"))
            return "SecurityAudit";
        if (intent.Contains("research") || intent.Contains("find"))
            return "Research";
        
        return "General";
    }
    
    /// <summary>
    /// Spins up a new SubAgent for a specific domain/mission
    /// </summary>
    public async Task<ISubAgent> CreateSubAgentAsync(string domain, TrustProfile? trustProfile = null)
    {
        var agentId = $"SA-{domain}-{Guid.NewGuid():N}";
        var profile = trustProfile ?? _defaultTrustProfile;
        
        var subAgent = new SubAgent(agentId, domain, profile);
        _activeSubAgents[agentId] = subAgent;
        
        await subAgent.StartMissionAsync();
        
        LogEvent($"Created and started SubAgent: {agentId} for domain: {domain}");
        
        return subAgent;
    }
    
    /// <summary>
    /// Reactivates an existing SubAgent (if it was previously created)
    /// </summary>
    public Task<ISubAgent?> ReactivateSubAgentAsync(string agentId)
    {
        if (_activeSubAgents.TryGetValue(agentId, out var agent))
        {
            LogEvent($"Reactivated SubAgent: {agentId}");
            return Task.FromResult<ISubAgent?>(agent);
        }
        
        LogEvent($"Failed to reactivate SubAgent: {agentId} - not found");
        return Task.FromResult<ISubAgent?>(null);
    }
    
    /// <summary>
    /// Authorizes communication between two SubAgents
    /// </summary>
    public void AuthorizeCommunication(
        string fromAgentId, 
        string toAgentId, 
        bool isBidirectional = false,
        List<string>? allowedDataScopes = null,
        TimeSpan? ttl = null)
    {
        var authorization = new CommunicationAuthorization(
            fromAgentId, 
            toAgentId, 
            isBidirectional, 
            allowedDataScopes, 
            ttl);
        
        _communicationAuthorizations.Add(authorization);
        
        LogEvent($"Authorized communication: {fromAgentId} -> {toAgentId} " +
                $"(bidirectional: {isBidirectional})");
    }
    
    /// <summary>
    /// Mediates communication between SubAgents (FLA-mediated communication)
    /// </summary>
    public async Task<bool> MediateCommunicationAsync(
        string fromAgentId, 
        string toAgentId, 
        string information)
    {
        // Check if communication is authorized
        var authorization = _communicationAuthorizations
            .FirstOrDefault(a => a.IsAuthorized(fromAgentId, toAgentId));
        
        if (authorization == null)
        {
            LogEvent($"Communication blocked: {fromAgentId} -> {toAgentId} - not authorized");
            return false;
        }
        
        if (!_activeSubAgents.TryGetValue(fromAgentId, out var sender))
        {
            LogEvent($"Communication failed: sender {fromAgentId} not found");
            return false;
        }
        
        if (!_activeSubAgents.TryGetValue(toAgentId, out var receiver))
        {
            LogEvent($"Communication failed: receiver {toAgentId} not found");
            return false;
        }
        
        // Send through sender's outbound (security level check)
        var sentSuccess = await sender.SendInformationAsync(toAgentId, information);
        if (!sentSuccess)
        {
            LogEvent($"Communication failed: sender rejected outbound");
            return false;
        }
        
        // Receive at receiver's inbound (trust level check)
        var receiveSuccess = await receiver.ReceiveInformationAsync(fromAgentId, information);
        
        LogEvent($"Communication mediated: {fromAgentId} -> {toAgentId} - success: {receiveSuccess}");
        
        return receiveSuccess;
    }
    
    /// <summary>
    /// Retires a SubAgent when its mission concludes
    /// </summary>
    public async Task RetireSubAgentAsync(string agentId)
    {
        if (_activeSubAgents.TryGetValue(agentId, out var agent))
        {
            await agent.RetireAsync();
            _activeSubAgents.Remove(agentId);
            
            // Remove any communication authorizations involving this agent
            _communicationAuthorizations.RemoveAll(a => 
                a.FromAgentId == agentId || a.ToAgentId == agentId);
            
            LogEvent($"Retired SubAgent: {agentId}");
        }
    }
    
    /// <summary>
    /// Processes a user task by creating appropriate SubAgent(s)
    /// </summary>
    public async Task<string> ProcessUserTaskAsync(string userIntent, string taskDetails)
    {
        LogEvent($"Processing user task: {userIntent}");
        
        var domain = ClassifyDomain(userIntent);
        var subAgent = await CreateSubAgentAsync(domain);
        
        try
        {
            var result = await subAgent.ProcessTaskAsync(taskDetails);
            await subAgent.CompleteMissionAsync();
            
            return result;
        }
        catch (Exception ex)
        {
            LogEvent($"Task processing failed: {ex.Message}");
            throw;
        }
    }
    
    /// <summary>
    /// Gets all active SubAgents
    /// </summary>
    public IReadOnlyDictionary<string, ISubAgent> GetActiveSubAgents() => 
        new Dictionary<string, ISubAgent>(_activeSubAgents);
    
    /// <summary>
    /// Gets the event log for auditing
    /// </summary>
    public IReadOnlyList<string> GetEventLog() => _eventLog.AsReadOnly();
    
    private void LogEvent(string eventDescription)
    {
        var logEntry = $"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff}] [FLA-{Id}] {eventDescription}";
        _eventLog.Add(logEntry);
    }
}
