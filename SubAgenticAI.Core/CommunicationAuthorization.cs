namespace SubAgenticAI.Core;

/// <summary>
/// Represents a communication authorization between SubAgents
/// </summary>
public class CommunicationAuthorization
{
    public string FromAgentId { get; set; }
    public string ToAgentId { get; set; }
    public bool IsBidirectional { get; set; }
    public List<string> AllowedDataScopes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    
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
        AllowedDataScopes = allowedDataScopes ?? new List<string> { "*" };
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
