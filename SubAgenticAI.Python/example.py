"""
Example demonstrating Cloud-Native Resource Governance in Python
"""

import asyncio
from datetime import datetime, timedelta
from subagentic_ai import (
    CloudResourceAllocationProfile,
    FrontLineAgent,
    SubAgent,
    ResourceUsageMetrics
)


class SampleSubAgent(SubAgent):
    """Sample SubAgent implementation."""
    
    async def _execute(self):
        """Execute sample mission."""
        # Simulate some work
        await asyncio.sleep(0.1)
        return {"status": "success", "message": "Sample mission completed"}


def main():
    """Run the example."""
    print("=== SubAgenticAI Python Example ===\n")
    
    # Create a FrontLineAgent
    fla = FrontLineAgent()
    print("1. FrontLineAgent created")
    
    # Define a custom resource profile with stricter limits
    resource_profile = CloudResourceAllocationProfile(
        max_cpu_cores=2.0,
        max_memory_footprint_bytes=1024 * 1024 * 1024,  # 1 GB
        max_execution_time_per_invocation=timedelta(minutes=10),
        max_message_count=500,
        max_cost_per_mission=5.0,
        hard_budget_enforcement=True
    )
    print("2. Resource profile defined")
    
    # Create a sample SubAgent
    sub_agent = SampleSubAgent(
        agent_id="sa-001",
        mission_description="Process sample data",
        resource_profile=resource_profile
    )
    print(f"3. SubAgent created: {sub_agent.id}")
    
    # Provision the SubAgent
    fla.provision_sub_agent(sub_agent)
    print("4. SubAgent provisioned by FLA")
    
    # Simulate resource usage - within limits
    metrics1 = ResourceUsageMetrics(
        sub_agent_id="sa-001",
        current_cpu_usage=1.5,
        current_memory_usage_bytes=512 * 1024 * 1024,  # 512 MB
        message_count=100,
        current_mission_cost=2.0,
        timestamp=datetime.utcnow()
    )
    fla.monitor_and_enforce("sa-001", metrics1)
    print("5. Monitoring: Resources within limits")
    
    # Simulate resource usage - approaching limits (soft enforcement)
    metrics2 = ResourceUsageMetrics(
        sub_agent_id="sa-001",
        current_cpu_usage=1.85,  # 92.5% of limit
        current_memory_usage_bytes=950 * 1024 * 1024,  # 92.8% of limit
        message_count=460,  # 92% of limit
        current_mission_cost=4.7,  # 94% of limit
        timestamp=datetime.utcnow()
    )
    fla.monitor_and_enforce("sa-001", metrics2)
    print("6. Monitoring: Resources approaching limits (soft enforcement triggered)")
    
    # Get audit log
    audit_log = fla.get_audit_log()
    print(f"\n7. Audit Log Entries: {len(audit_log)}")
    for action in audit_log:
        print(f"   - [{action.timestamp.strftime('%H:%M:%S')}] "
              f"{action.level.name}: {action.reason} ({action.resource_type})")
    
    # Get active SubAgents
    active_agents = fla.get_active_sub_agents()
    print(f"\n8. Active SubAgents: {len(active_agents)}")
    
    print("\n=== Example completed successfully ===")


if __name__ == "__main__":
    main()
