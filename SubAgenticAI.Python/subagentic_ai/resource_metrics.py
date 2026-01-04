"""
Resource Usage Metrics

Represents real-time resource consumption metrics for a SubAgent.
Used for monitoring and enforcement decisions.
"""

from dataclasses import dataclass, field
from datetime import datetime, timedelta


@dataclass
class ResourceUsageMetrics:
    """Real-time resource consumption metrics for a SubAgent."""
    
    sub_agent_id: str
    timestamp: datetime = field(default_factory=datetime.utcnow)
    
    # Compute metrics
    current_cpu_usage: float = 0.0
    current_gpu_usage: float = 0.0
    total_execution_time: timedelta = timedelta()
    consumed_compute_budget: float = 0.0
    
    # Memory metrics (in bytes)
    current_memory_usage_bytes: int = 0
    peak_memory_usage_bytes: int = 0
    
    # Network metrics
    total_outbound_bytes: int = 0
    total_inbound_bytes: int = 0
    message_count: int = 0
    
    # Storage metrics (in bytes)
    current_state_size_bytes: int = 0
    current_log_size_bytes: int = 0
    temp_storage_used_bytes: int = 0
    
    # Trust-broker metrics
    trust_evaluation_query_count: int = 0
    
    # Cost metrics
    current_mission_cost: float = 0.0
    total_sub_agent_cost: float = 0.0
