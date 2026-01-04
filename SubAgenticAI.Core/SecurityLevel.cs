namespace SubAgenticAI.Core;

/// <summary>
/// Security Level (SL) governs outbound information flow.
/// Specifies the conditions under which a SubAgent may disclose information to external entities.
/// </summary>
public enum SecurityLevel
{
    /// <summary>
    /// Public information - no restrictions on disclosure
    /// </summary>
    Public = 0,
    
    /// <summary>
    /// Internal use only - restricted to organization
    /// </summary>
    Internal = 1,
    
    /// <summary>
    /// Confidential - limited disclosure
    /// </summary>
    Confidential = 2,
    
    /// <summary>
    /// Secret - highly restricted disclosure
    /// </summary>
    Secret = 3,
    
    /// <summary>
    /// Top Secret - maximum restrictions
    /// </summary>
    TopSecret = 4
}
