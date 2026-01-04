namespace SubAgenticAI.Core;

/// <summary>
/// Represents the lifecycle status of a SubAgent mission
/// </summary>
public enum MissionStatus
{
    /// <summary>
    /// Mission has been created but not yet started
    /// </summary>
    Created,
    
    /// <summary>
    /// Mission is currently active
    /// </summary>
    Active,
    
    /// <summary>
    /// Mission is paused/suspended
    /// </summary>
    Paused,
    
    /// <summary>
    /// Mission completed successfully
    /// </summary>
    Completed,
    
    /// <summary>
    /// Mission failed or was aborted
    /// </summary>
    Failed,
    
    /// <summary>
    /// Mission has been retired/disposed
    /// </summary>
    Retired
}
