namespace SubAgenticAI.Core;

/// <summary>
/// Trust profile assigned by the FLA to each SubAgent.
/// Contains SL (Security Level) for outbound and TL (Trust Level) thresholds for inbound communication.
/// </summary>
public class TrustProfile
{
    private const int DefaultMinimumBrokerAgreement = 2;
    
    /// <summary>
    /// Security Level for outbound information flow
    /// </summary>
    public SecurityLevel SecurityLevel { get; init; }
    
    /// <summary>
    /// Required trust threshold for accepting inbound information
    /// </summary>
    public TrustLevel RequiredTrustThreshold { get; init; }
    
    /// <summary>
    /// Content Sharing Policy - set of trust brokers to use for evaluation
    /// </summary>
    public IReadOnlyList<ITrustBroker> TrustBrokers { get; init; }
    
    /// <summary>
    /// Minimum number of brokers that must agree for independent agreement
    /// </summary>
    public int MinimumBrokerAgreement { get; init; }
    
    /// <summary>
    /// Tolerance for convergence of broker evaluations (0.0 to 1.0)
    /// </summary>
    public double AgreementTolerance { get; init; }
    
    /// <summary>
    /// Creates a new trust profile
    /// </summary>
    public TrustProfile(
        SecurityLevel securityLevel, 
        TrustLevel requiredTrustThreshold,
        List<ITrustBroker> trustBrokers,
        int minimumBrokerAgreement = 2,
        double agreementTolerance = 0.1)
    {
        SecurityLevel = securityLevel;
        RequiredTrustThreshold = requiredTrustThreshold;
        TrustBrokers = (trustBrokers ?? new List<ITrustBroker>()).AsReadOnly();
        MinimumBrokerAgreement = Math.Max(DefaultMinimumBrokerAgreement, minimumBrokerAgreement);
        AgreementTolerance = Math.Clamp(agreementTolerance, 0.0, 1.0);
    }
    
    /// <summary>
    /// Evaluates if inbound content meets trust requirements with independent broker agreement
    /// </summary>
    public async Task<bool> ValidateInboundAsync(string senderId, string content)
    {
        if (TrustBrokers.Count < MinimumBrokerAgreement)
        {
            return false;
        }
        
        var senderTrusts = new List<double>();
        var contentTrusts = new List<double>();
        
        // Evaluate with all brokers
        foreach (var broker in TrustBrokers)
        {
            // Check if any broker flags the content
            if (await broker.IsFlaggedAsync(senderId, content))
            {
                return false;
            }
            
            var senderTrust = await broker.EvaluateSenderTrustAsync(senderId);
            var contentTrust = await broker.EvaluateContentTrustAsync(content);
            
            senderTrusts.Add(senderTrust);
            contentTrusts.Add(contentTrust);
        }
        
        // Check for independent agreement (convergence within tolerance)
        if (!HasIndependentAgreement(senderTrusts) || !HasIndependentAgreement(contentTrusts))
        {
            return false;
        }
        
        // Check if average trust levels meet threshold
        var avgSenderTrust = senderTrusts.Average();
        var avgContentTrust = contentTrusts.Average();
        var evaluatedTrust = new TrustLevel(avgSenderTrust, avgContentTrust);
        
        return evaluatedTrust.MeetsThreshold(RequiredTrustThreshold);
    }
    
    /// <summary>
    /// Checks if broker evaluations converge within tolerance
    /// </summary>
    private bool HasIndependentAgreement(List<double> evaluations)
    {
        if (evaluations.Count < MinimumBrokerAgreement)
        {
            return false;
        }
        
        var avg = evaluations.Average();
        var withinTolerance = evaluations.Count(e => Math.Abs(e - avg) <= AgreementTolerance);
        
        return withinTolerance >= MinimumBrokerAgreement;
    }
}
