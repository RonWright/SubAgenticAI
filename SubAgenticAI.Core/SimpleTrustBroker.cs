namespace SubAgenticAI.Core;

/// <summary>
/// Simple implementation of a trust broker for demonstration purposes
/// In production, this would connect to external trust evaluation services
/// </summary>
public class SimpleTrustBroker : ITrustBroker
{
    public string BrokerId { get; }
    public string Name { get; }
    
    private readonly Dictionary<string, double> _senderReputations;
    private readonly HashSet<string> _flaggedSenders;
    private readonly double _baselineTrust;
    
    public SimpleTrustBroker(string brokerId, string name, double baselineTrust = 0.7)
    {
        BrokerId = brokerId;
        Name = name;
        _baselineTrust = Math.Clamp(baselineTrust, 0.0, 1.0);
        _senderReputations = new Dictionary<string, double>();
        _flaggedSenders = new HashSet<string>();
    }
    
    public Task<double> EvaluateSenderTrustAsync(string senderId)
    {
        // Return stored reputation or baseline
        if (_senderReputations.TryGetValue(senderId, out var reputation))
        {
            return Task.FromResult(reputation);
        }
        
        return Task.FromResult(_baselineTrust);
    }
    
    public Task<double> EvaluateContentTrustAsync(string content)
    {
        // Simple heuristic - in production would use ML/NLP
        var trustScore = _baselineTrust;
        
        // Reduce trust for suspicious patterns
        if (content.Contains("malicious", StringComparison.OrdinalIgnoreCase))
            trustScore *= 0.3;
        else if (content.Contains("suspicious", StringComparison.OrdinalIgnoreCase))
            trustScore *= 0.6;
        
        // Increase trust for verified markers
        if (content.Contains("verified", StringComparison.OrdinalIgnoreCase))
            trustScore = Math.Min(1.0, trustScore * 1.2);
        
        return Task.FromResult(Math.Clamp(trustScore, 0.0, 1.0));
    }
    
    public Task<bool> IsFlaggedAsync(string senderId, string content)
    {
        // Check if sender is flagged
        if (_flaggedSenders.Contains(senderId))
        {
            return Task.FromResult(true);
        }
        
        // Check for obviously malicious content
        if (content.Contains("malicious", StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(true);
        }
        
        return Task.FromResult(false);
    }
    
    /// <summary>
    /// Sets reputation for a sender (for testing/configuration)
    /// </summary>
    public void SetSenderReputation(string senderId, double reputation)
    {
        _senderReputations[senderId] = Math.Clamp(reputation, 0.0, 1.0);
    }
    
    /// <summary>
    /// Flags a sender as untrustworthy
    /// </summary>
    public void FlagSender(string senderId)
    {
        _flaggedSenders.Add(senderId);
    }
    
    /// <summary>
    /// Removes flag from a sender
    /// </summary>
    public void UnflagSender(string senderId)
    {
        _flaggedSenders.Remove(senderId);
    }
}
