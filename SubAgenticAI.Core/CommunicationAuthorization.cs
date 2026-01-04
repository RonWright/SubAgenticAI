namespace SubAgenticAI.Core;

/// <summary>
/// Represents a communication authorization between SubAgents
/// </summary>
public class CommunicationAuthorization
{
    public string FromAgentId { get; init; }
    public string ToAgentId { get; init; }
    public bool IsBidirectional { get; init; }
    public IReadOnlyList<string> AllowedDataScopes { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? ExpiresAt { get; init; }
    
    public CommunicationAuthorization(
        string fromAgentId, 
        string toAgentId, 
        bool isBidirectional = false,
        List<string>? allowedDataScopes = null,
        TimeSpan? ttl = null)
    {
        FromAgentId = fromAgentId;
        ToAgentId = toAgentId;
        IsBidirectional = isBidirectional;
        AllowedDataScopes = (allowedDataScopes ?? new List<string> { "*" }).AsReadOnly();
        CreatedAt = DateTime.UtcNow;
        ExpiresAt = ttl.HasValue ? CreatedAt + ttl.Value : null;
    }
    
    public bool IsExpired() => ExpiresAt.HasValue && DateTime.UtcNow > ExpiresAt.Value;
    
    public bool IsAuthorized(string fromId, string toId)
    {
        if (IsExpired()) return false;
        
        if (FromAgentId == fromId && ToAgentId == toId) return true;
        if (IsBidirectional && FromAgentId == toId && ToAgentId == fromId) return true;
        
        return false;
    }
}
