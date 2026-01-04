namespace SubAgenticAI.Core;

/// <summary>
/// Defines the contract for a SubAgent (SA) - a specialized, mission-scoped cognitive unit
/// </summary>
public interface ISubAgent
{
    /// <summary>
    /// Unique identifier for this SubAgent
    /// </summary>
    string Id { get; }
    
    /// <summary>
    /// Domain or mission focus of this SubAgent
    /// </summary>
    string Domain { get; }
    
    /// <summary>
    /// Current mission status
    /// </summary>
    MissionStatus Status { get; }
    
    /// <summary>
    /// Trust profile assigned by the FLA
    /// </summary>
    TrustProfile TrustProfile { get; }
    
    /// <summary>
    /// Starts the SubAgent's mission
    /// </summary>
    Task StartMissionAsync();
    
    /// <summary>
    /// Processes a task within the SubAgent's domain
    /// </summary>
    Task<string> ProcessTaskAsync(string task);
    
    /// <summary>
    /// Receives information from external source (validated by trust profile)
    /// </summary>
    Task<bool> ReceiveInformationAsync(string senderId, string information);
    
    /// <summary>
    /// Sends information outbound (governed by security level)
    /// </summary>
    Task<bool> SendInformationAsync(string recipientId, string information);
    
    /// <summary>
    /// Completes the mission
    /// </summary>
    Task CompleteMissionAsync();
    
    /// <summary>
    /// Retires/disposes the SubAgent
    /// </summary>
    Task RetireAsync();
}
