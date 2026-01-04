using SubAgenticAI.Core;

Console.WriteLine("=== SubAgenticAI Demo ===");
Console.WriteLine("Demonstrating FLA (Front Line Agent) and SA (SubAgent) management\n");

// Step 1: Set up trust brokers (independent evaluators)
Console.WriteLine("1. Setting up Trust Brokers...");
var broker1 = new SimpleTrustBroker("TB-001", "Trust Broker Alpha", baselineTrust: 0.7);
var broker2 = new SimpleTrustBroker("TB-002", "Trust Broker Beta", baselineTrust: 0.75);
var broker3 = new SimpleTrustBroker("TB-003", "Trust Broker Gamma", baselineTrust: 0.8);

// Configure some sender reputations
broker1.SetSenderReputation("sender-reliable", 0.9);
broker2.SetSenderReputation("sender-reliable", 0.85);
broker3.SetSenderReputation("sender-reliable", 0.88);

broker1.FlagSender("sender-malicious");
Console.WriteLine("   ✓ Three independent trust brokers configured\n");

// Step 2: Create default trust profile
Console.WriteLine("2. Creating Trust Profile...");
var trustProfile = new TrustProfile(
    securityLevel: SecurityLevel.Internal,
    requiredTrustThreshold: new TrustLevel(senderTrust: 0.6, contentTrust: 0.6),
    trustBrokers: new List<ITrustBroker> { broker1, broker2, broker3 },
    minimumBrokerAgreement: 2,
    agreementTolerance: 0.15
);
Console.WriteLine($"   ✓ Security Level: {trustProfile.SecurityLevel}");
Console.WriteLine($"   ✓ Required Trust Threshold: Sender={trustProfile.RequiredTrustThreshold.SenderTrust}, Content={trustProfile.RequiredTrustThreshold.ContentTrust}");
Console.WriteLine($"   ✓ Trust Brokers: {trustProfile.TrustBrokers.Count} (minimum agreement: {trustProfile.MinimumBrokerAgreement})\n");

// Step 3: Create Front Line Agent (FLA)
Console.WriteLine("3. Creating Front Line Agent (FLA)...");
var fla = new FrontLineAgent("FLA-001", trustProfile);
Console.WriteLine($"   ✓ FLA created with ID: {fla.Id}\n");

// Step 4: Process user tasks
Console.WriteLine("4. Processing User Tasks...");
Console.WriteLine("   Task 1: Data Analysis");
var result1 = await fla.ProcessUserTaskAsync(
    "I need to analyze some data", 
    "Analyze customer sentiment from recent reviews");
Console.WriteLine($"   Result: {result1}\n");

Console.WriteLine("   Task 2: Code Generation");
var result2 = await fla.ProcessUserTaskAsync(
    "Write code for authentication", 
    "Generate JWT authentication middleware");
Console.WriteLine($"   Result: {result2}\n");

// Step 5: Demonstrate SubAgent isolation and communication
Console.WriteLine("5. Demonstrating SubAgent Isolation and Communication...");
var researchAgent = await fla.CreateSubAgentAsync("Research");
var analysisAgent = await fla.CreateSubAgentAsync("Analysis");
Console.WriteLine($"   ✓ Created Research SubAgent: {researchAgent.Id}");
Console.WriteLine($"   ✓ Created Analysis SubAgent: {analysisAgent.Id}");

// Try communication without authorization (should fail)
Console.WriteLine("\n   Attempting communication WITHOUT authorization...");
var commResult1 = await fla.MediateCommunicationAsync(
    researchAgent.Id, 
    analysisAgent.Id, 
    "Research findings: market trends are positive");
Console.WriteLine($"   Result: {(commResult1 ? "✓ Allowed" : "✗ Blocked - not authorized")}");

// Authorize communication and try again
Console.WriteLine("\n   Authorizing communication...");
fla.AuthorizeCommunication(
    researchAgent.Id, 
    analysisAgent.Id, 
    isBidirectional: true,
    allowedDataScopes: new List<string> { "research_data", "analysis_results" }
);
Console.WriteLine("   ✓ Communication authorized");

Console.WriteLine("\n   Attempting communication WITH authorization...");
var commResult2 = await fla.MediateCommunicationAsync(
    researchAgent.Id, 
    analysisAgent.Id, 
    "Research findings: market trends are positive");
Console.WriteLine($"   Result: {(commResult2 ? "✓ Allowed and delivered" : "✗ Blocked")}");

// Step 6: Demonstrate trust-based inbound validation
Console.WriteLine("\n6. Demonstrating Trust-Based Inbound Validation...");

// Good sender with good content
Console.WriteLine("\n   Test 1: Reliable sender with verified content");
var acceptResult1 = await researchAgent.ReceiveInformationAsync(
    "sender-reliable", 
    "This is verified research data from a trusted source");
Console.WriteLine($"   Result: {(acceptResult1 ? "✓ Accepted" : "✗ Rejected")}");

// Flagged sender
Console.WriteLine("\n   Test 2: Flagged malicious sender");
var acceptResult2 = await researchAgent.ReceiveInformationAsync(
    "sender-malicious", 
    "This is some data");
Console.WriteLine($"   Result: {(acceptResult2 ? "✓ Accepted" : "✗ Rejected - sender flagged")}");

// Step 7: Clean up - retire SubAgents
Console.WriteLine("\n7. Retiring SubAgents...");
await fla.RetireSubAgentAsync(researchAgent.Id);
await fla.RetireSubAgentAsync(analysisAgent.Id);
Console.WriteLine($"   ✓ Retired {researchAgent.Id}");
Console.WriteLine($"   ✓ Retired {analysisAgent.Id}");

// Step 8: Show audit trail
Console.WriteLine("\n8. Audit Trail (FLA Event Log - last 10 events):");
var eventLog = fla.GetEventLog();
var lastEvents = eventLog.Skip(Math.Max(0, eventLog.Count - 10)).Take(10);
foreach (var evt in lastEvents)
{
    Console.WriteLine($"   {evt}");
}

Console.WriteLine("\n=== Demo Complete ===");
Console.WriteLine($"Active SubAgents: {fla.GetActiveSubAgents().Count}");
Console.WriteLine($"Total Events Logged: {eventLog.Count}");
