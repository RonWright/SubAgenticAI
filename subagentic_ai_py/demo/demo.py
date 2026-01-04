"""Demo application for SubAgenticAI Python implementation."""

import asyncio
from subagentic_ai import (
    FrontLineAgent,
    SimpleTrustBroker,
    TrustProfile,
    SecurityLevel,
    TrustLevel,
)


async def main():
    """Run the SubAgenticAI demo."""
    print("=== SubAgenticAI Python Demo ===")
    print("Demonstrating FLA (Front Line Agent) and SA (SubAgent) management\n")
    
    # Step 1: Set up trust brokers (independent evaluators)
    print("1. Setting up Trust Brokers...")
    broker1 = SimpleTrustBroker("TB-001", "Trust Broker Alpha", baseline_trust=0.7)
    broker2 = SimpleTrustBroker("TB-002", "Trust Broker Beta", baseline_trust=0.75)
    broker3 = SimpleTrustBroker("TB-003", "Trust Broker Gamma", baseline_trust=0.8)
    
    # Configure some sender reputations
    broker1.set_sender_reputation("sender-reliable", 0.9)
    broker2.set_sender_reputation("sender-reliable", 0.85)
    broker3.set_sender_reputation("sender-reliable", 0.88)
    
    broker1.flag_sender("sender-malicious")
    print("   ✓ Three independent trust brokers configured\n")
    
    # Step 2: Create default trust profile
    print("2. Creating Trust Profile...")
    trust_profile = TrustProfile(
        security_level=SecurityLevel.INTERNAL,
        required_trust_threshold=TrustLevel(sender_trust=0.6, content_trust=0.6),
        trust_brokers=(broker1, broker2, broker3),
        minimum_broker_agreement=2,
        agreement_tolerance=0.15
    )
    print(f"   ✓ Security Level: {trust_profile.security_level.name}")
    print(f"   ✓ Required Trust Threshold: Sender={trust_profile.required_trust_threshold.sender_trust}, "
          f"Content={trust_profile.required_trust_threshold.content_trust}")
    print(f"   ✓ Trust Brokers: {len(trust_profile.trust_brokers)} "
          f"(minimum agreement: {trust_profile.minimum_broker_agreement})\n")
    
    # Step 3: Create Front Line Agent (FLA)
    print("3. Creating Front Line Agent (FLA)...")
    fla = FrontLineAgent("FLA-001", trust_profile)
    print(f"   ✓ FLA created with ID: {fla.id}\n")
    
    # Step 4: Process user tasks
    print("4. Processing User Tasks...")
    print("   Task 1: Data Analysis")
    result1 = await fla.process_user_task(
        "I need to analyze some data",
        "Analyze customer sentiment from recent reviews"
    )
    print(f"   Result: {result1}\n")
    
    print("   Task 2: Code Generation")
    result2 = await fla.process_user_task(
        "Write code for authentication",
        "Generate JWT authentication middleware"
    )
    print(f"   Result: {result2}\n")
    
    # Step 5: Demonstrate SubAgent isolation and communication
    print("5. Demonstrating SubAgent Isolation and Communication...")
    research_agent = await fla.create_sub_agent("Research")
    analysis_agent = await fla.create_sub_agent("Analysis")
    print(f"   ✓ Created Research SubAgent: {research_agent.id}")
    print(f"   ✓ Created Analysis SubAgent: {analysis_agent.id}")
    
    # Try communication without authorization (should fail)
    print("\n   Attempting communication WITHOUT authorization...")
    comm_result1 = await fla.mediate_communication(
        research_agent.id,
        analysis_agent.id,
        "Research findings: market trends are positive"
    )
    print(f"   Result: {'✓ Allowed' if comm_result1 else '✗ Blocked - not authorized'}")
    
    # Authorize communication and try again
    print("\n   Authorizing communication...")
    fla.authorize_communication(
        research_agent.id,
        analysis_agent.id,
        is_bidirectional=True,
        allowed_data_scopes=["research_data", "analysis_results"]
    )
    print("   ✓ Communication authorized")
    
    print("\n   Attempting communication WITH authorization...")
    comm_result2 = await fla.mediate_communication(
        research_agent.id,
        analysis_agent.id,
        "Research findings: market trends are positive"
    )
    print(f"   Result: {'✓ Allowed and delivered' if comm_result2 else '✗ Blocked'}")
    
    # Step 6: Demonstrate trust-based inbound validation
    print("\n6. Demonstrating Trust-Based Inbound Validation...")
    
    # Good sender with good content
    print("\n   Test 1: Reliable sender with verified content")
    accept_result1 = await research_agent.receive_information(
        "sender-reliable",
        "This is verified research data from a trusted source"
    )
    print(f"   Result: {'✓ Accepted' if accept_result1 else '✗ Rejected'}")
    
    # Flagged sender
    print("\n   Test 2: Flagged malicious sender")
    accept_result2 = await research_agent.receive_information(
        "sender-malicious",
        "This is some data"
    )
    print(f"   Result: {'✓ Accepted' if accept_result2 else '✗ Rejected - sender flagged'}")
    
    # Step 7: Clean up - retire SubAgents
    print("\n7. Retiring SubAgents...")
    await fla.retire_sub_agent(research_agent.id)
    await fla.retire_sub_agent(analysis_agent.id)
    print(f"   ✓ Retired {research_agent.id}")
    print(f"   ✓ Retired {analysis_agent.id}")
    
    # Step 8: Show audit trail
    print("\n8. Audit Trail (FLA Event Log - last 10 events):")
    event_log = fla.get_event_log()
    last_events = event_log[-10:] if len(event_log) > 10 else event_log
    for evt in last_events:
        print(f"   {evt}")
    
    print("\n=== Demo Complete ===")
    print(f"Active SubAgents: {len(fla.get_active_sub_agents())}")
    print(f"Total Events Logged: {len(event_log)}")


if __name__ == "__main__":
    asyncio.run(main())
