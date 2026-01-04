namespace SubAgenticAI.Core;

/// <summary>
/// Defines the contract for independent trust brokers that evaluate sender and content trustworthiness
/// </summary>
public interface ITrustBroker
{
    /// <summary>
    /// Unique identifier for this trust broker
    /// </summary>
    string BrokerId { get; }
    
    /// <summary>
    /// Name of the trust broker
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// Evaluates the sender's trustworthiness
    /// </summary>
    /// <param name="senderId">Identifier of the sender</param>
    /// <returns>Trust level between 0.0 and 1.0</returns>
    Task<double> EvaluateSenderTrustAsync(string senderId);
    
    /// <summary>
    /// Evaluates the content's trustworthiness
    /// </summary>
    /// <param name="content">Content to evaluate</param>
    /// <returns>Trust level between 0.0 and 1.0</returns>
    Task<double> EvaluateContentTrustAsync(string content);
    
    /// <summary>
    /// Flags sender or content as untrustworthy
    /// </summary>
    /// <param name="senderId">Sender identifier</param>
    /// <param name="content">Content</param>
    /// <returns>True if flagged as untrustworthy</returns>
    Task<bool> IsFlaggedAsync(string senderId, string content);
}
