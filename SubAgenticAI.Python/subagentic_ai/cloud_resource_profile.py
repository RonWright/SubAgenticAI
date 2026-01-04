"""
Cloud Resource Allocation Profile

Defines the maximum allowable consumption across cloud-native resource categories
for a SubAgent. This profile is immutable for the lifetime of the SubAgent.
"""

from dataclasses import dataclass
from datetime import timedelta


@dataclass(frozen=True)
class CloudResourceAllocationProfile:
    """
    Immutable resource allocation profile for a SubAgent.
    Defines limits across compute, memory, network, storage, trust-broker, and cost categories.
    """
    
    # Compute Resources
    max_cpu_cores: float = 1.0
    max_gpu_allocation: float = 0.0
    max_execution_time_per_invocation: timedelta = timedelta(minutes=5)
    total_lifetime_compute_budget: float = 100.0
    
    # Memory Resources (in bytes)
    max_memory_footprint_bytes: int = 512 * 1024 * 1024  # 512 MB
    max_memory_growth_rate_per_second: int = 10 * 1024 * 1024  # 10 MB/s
    container_memory_ceiling_bytes: int = 1024 * 1024 * 1024  # 1 GB
    
    # Network Resources
    max_outbound_bandwidth_bytes_per_sec: int = 10 * 1024 * 1024  # 10 MB/s
    max_inbound_bandwidth_bytes_per_sec: int = 10 * 1024 * 1024  # 10 MB/s
    max_message_count: int = 1000
    allow_cross_region_communication: bool = False
    
    # Storage Resources (in bytes)
    max_state_size_bytes: int = 100 * 1024 * 1024  # 100 MB
    max_log_size_bytes: int = 50 * 1024 * 1024  # 50 MB
    temp_storage_quota_bytes: int = 200 * 1024 * 1024  # 200 MB
    
    # Trust-Broker Resources
    max_trust_level_evaluation_queries: int = 100
    trust_broker_rate_limit_per_minute: int = 60
    allow_cross_cloud_trust_broker_access: bool = False
    
    # Cost Controls
    max_cost_per_mission: float = 10.0
    max_cost_per_sub_agent: float = 50.0
    hard_budget_enforcement: bool = True
