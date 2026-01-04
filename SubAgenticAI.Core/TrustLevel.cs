namespace SubAgenticAI.Core;

/// <summary>
/// Trust Level (TL) governs inbound information flow.
/// Composed of Sender Trust Level (TLₛ) and Content Trust Level (TL꜀).
/// </summary>
public class TrustLevel
{
    /// <summary>
    /// Sender Trust Level (TLₛ) - historical reliability and behavioral integrity of the sender
    /// </summary>
    public double SenderTrust { get; init; }
    
    /// <summary>
    /// Content Trust Level (TL꜀) - factual accuracy and provenance of the content
    /// </summary>
    public double ContentTrust { get; init; }
    
    /// <summary>
    /// Creates a new TrustLevel with specified sender and content trust values
    /// </summary>
    /// <param name="senderTrust">Sender trust level (0.0 to 1.0)</param>
    /// <param name="contentTrust">Content trust level (0.0 to 1.0)</param>
    public TrustLevel(double senderTrust, double contentTrust)
    {
        SenderTrust = Math.Clamp(senderTrust, 0.0, 1.0);
        ContentTrust = Math.Clamp(contentTrust, 0.0, 1.0);
    }
    
    /// <summary>
    /// Checks if this trust level meets the required threshold
    /// </summary>
    /// <param name="threshold">Required trust threshold</param>
    /// <returns>True if both sender and content trust meet the threshold</returns>
    public bool MeetsThreshold(TrustLevel threshold)
    {
        return SenderTrust >= threshold.SenderTrust && 
               ContentTrust >= threshold.ContentTrust;
    }
}
