namespace SubAgenticAI.Core;

/// <summary>
/// Represents enforcement actions taken by the FLA when resource limits are approached or exceeded.
/// </summary>
public enum EnforcementLevel
{
    None,
    SoftEnforcement,
    HardEnforcement
}

public class EnforcementAction
{
    public string SubAgentId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public EnforcementLevel Level { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string ResourceType { get; set; } = string.Empty;
    public double ThresholdPercentage { get; set; }
    public bool Terminated { get; set; }
    
    public EnforcementAction()
    {
        Timestamp = DateTime.UtcNow;
    }
}
