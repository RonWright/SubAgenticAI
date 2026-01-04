"""
FrontLineAgent (FLA)

The FrontLineAgent is the authoritative orchestrator for SubAgent execution.
It manages resource allocation, monitoring, and enforcement in cloud-native
environments.
"""

from datetime import datetime, timezone
from threading import Lock
from typing import Dict, List, Optional

from .cloud_resource_profile import CloudResourceAllocationProfile
from .enforcement import EnforcementAction, EnforcementLevel
from .resource_metrics import ResourceUsageMetrics
from .sub_agent import SubAgent


class FrontLineAgent:
    """
    FrontLineAgent orchestrates SubAgent execution with cloud-native resource governance.
    
    Responsibilities:
    - Provisioning compute environments for SubAgents
    - Enforcing resource quotas and budgets
    - Monitoring real-time resource consumption
    - Applying soft and hard enforcement actions
    - Maintaining comprehensive audit logs
    - Terminating workloads that exceed limits
    """
    
    def __init__(self):
        self._active_sub_agents: Dict[str, SubAgent] = {}
        self._metrics: Dict[str, ResourceUsageMetrics] = {}
        self._audit_log: List[EnforcementAction] = []
        self._lock = Lock()
        
    def provision_sub_agent(self, sub_agent: SubAgent) -> None:
        """
        Provision a new SubAgent with specified resource profile.
        
        Args:
            sub_agent: The SubAgent to provision.
            
        Raises:
            ValueError: If sub_agent is None.
            RuntimeError: If SubAgent with same ID already exists.
        """
        if sub_agent is None:
            raise ValueError("sub_agent cannot be None")
            
        with self._lock:
            if sub_agent.id in self._active_sub_agents:
                raise RuntimeError(f"SubAgent {sub_agent.id} already exists")
                
            self._active_sub_agents[sub_agent.id] = sub_agent
            self._metrics[sub_agent.id] = ResourceUsageMetrics(
                sub_agent_id=sub_agent.id,
                timestamp=datetime.now(timezone.utc)
            )
            
            self._log_audit(
                sub_agent.id,
                EnforcementLevel.NONE,
                "SubAgent provisioned",
                "System",
                0.0,
                False
            )
            
    def monitor_and_enforce(
        self,
        sub_agent_id: str,
        current_metrics: ResourceUsageMetrics
    ) -> None:
        """
        Monitor resource usage and apply enforcement if needed.
        
        Args:
            sub_agent_id: ID of the SubAgent to monitor.
            current_metrics: Current resource usage metrics.
        """
        if sub_agent_id not in self._active_sub_agents:
            return
            
        sub_agent = self._active_sub_agents[sub_agent_id]
        
        with self._lock:
            self._metrics[sub_agent_id] = current_metrics
            
            profile = sub_agent.resource_profile
            
            # Check for resource limit violations
            self._check_compute_limits(sub_agent, current_metrics, profile)
            self._check_memory_limits(sub_agent, current_metrics, profile)
            self._check_network_limits(sub_agent, current_metrics, profile)
            self._check_storage_limits(sub_agent, current_metrics, profile)
            self._check_cost_limits(sub_agent, current_metrics, profile)
            
    def _check_compute_limits(
        self,
        sub_agent: SubAgent,
        metrics: ResourceUsageMetrics,
        profile: CloudResourceAllocationProfile
    ) -> None:
        """Check compute resource limits."""
        # Check CPU usage
        if metrics.current_cpu_usage >= profile.max_cpu_cores * 0.9:
            self._apply_soft_enforcement(
                sub_agent.id,
                "Compute",
                "CPU approaching limit",
                metrics.current_cpu_usage / profile.max_cpu_cores * 100
            )
            
        if metrics.current_cpu_usage > profile.max_cpu_cores:
            self._apply_hard_enforcement(
                sub_agent,
                "Compute",
                "CPU limit exceeded"
            )
            
        # Check execution time
        exec_time_ratio = (
            metrics.total_execution_time.total_seconds() /
            profile.max_execution_time_per_invocation.total_seconds()
        )
        
        if exec_time_ratio >= 0.9:
            self._apply_soft_enforcement(
                sub_agent.id,
                "Compute",
                "Execution time approaching limit",
                exec_time_ratio * 100
            )
            
        if exec_time_ratio > 1.0:
            self._apply_hard_enforcement(
                sub_agent,
                "Compute",
                "Execution time limit exceeded"
            )
            
    def _check_memory_limits(
        self,
        sub_agent: SubAgent,
        metrics: ResourceUsageMetrics,
        profile: CloudResourceAllocationProfile
    ) -> None:
        """Check memory resource limits."""
        memory_ratio = (
            metrics.current_memory_usage_bytes /
            profile.max_memory_footprint_bytes
        )
        
        if memory_ratio >= 0.9:
            self._apply_soft_enforcement(
                sub_agent.id,
                "Memory",
                "Memory approaching limit",
                memory_ratio * 100
            )
            
        if memory_ratio > 1.0:
            self._apply_hard_enforcement(
                sub_agent,
                "Memory",
                "Memory limit exceeded"
            )
            
    def _check_network_limits(
        self,
        sub_agent: SubAgent,
        metrics: ResourceUsageMetrics,
        profile: CloudResourceAllocationProfile
    ) -> None:
        """Check network resource limits."""
        message_ratio = metrics.message_count / profile.max_message_count
        
        if message_ratio >= 0.9:
            self._apply_soft_enforcement(
                sub_agent.id,
                "Network",
                "Message count approaching limit",
                message_ratio * 100
            )
            
        if message_ratio > 1.0:
            self._apply_hard_enforcement(
                sub_agent,
                "Network",
                "Message count limit exceeded"
            )
            
    def _check_storage_limits(
        self,
        sub_agent: SubAgent,
        metrics: ResourceUsageMetrics,
        profile: CloudResourceAllocationProfile
    ) -> None:
        """Check storage resource limits."""
        state_ratio = (
            metrics.current_state_size_bytes /
            profile.max_state_size_bytes
        )
        
        if state_ratio >= 0.9:
            self._apply_soft_enforcement(
                sub_agent.id,
                "Storage",
                "State size approaching limit",
                state_ratio * 100
            )
            
        if state_ratio > 1.0:
            self._apply_hard_enforcement(
                sub_agent,
                "Storage",
                "State size limit exceeded"
            )
            
    def _check_cost_limits(
        self,
        sub_agent: SubAgent,
        metrics: ResourceUsageMetrics,
        profile: CloudResourceAllocationProfile
    ) -> None:
        """Check cost limits."""
        cost_ratio = metrics.current_mission_cost / profile.max_cost_per_mission
        
        if cost_ratio >= 0.9:
            self._apply_soft_enforcement(
                sub_agent.id,
                "Cost",
                "Mission cost approaching limit",
                cost_ratio * 100
            )
            
        if profile.hard_budget_enforcement and cost_ratio > 1.0:
            self._apply_hard_enforcement(
                sub_agent,
                "Cost",
                "Mission cost limit exceeded"
            )
            
    def _apply_soft_enforcement(
        self,
        sub_agent_id: str,
        resource_type: str,
        reason: str,
        threshold: float
    ) -> None:
        """Apply soft enforcement action."""
        self._log_audit(
            sub_agent_id,
            EnforcementLevel.SOFT_ENFORCEMENT,
            reason,
            resource_type,
            threshold,
            False
        )
        
    def _apply_hard_enforcement(
        self,
        sub_agent: SubAgent,
        resource_type: str,
        reason: str
    ) -> None:
        """Apply hard enforcement action (terminate SubAgent)."""
        sub_agent.terminate(reason)
        self._log_audit(
            sub_agent.id,
            EnforcementLevel.HARD_ENFORCEMENT,
            reason,
            resource_type,
            100.0,
            True
        )
        
        if sub_agent.id in self._active_sub_agents:
            del self._active_sub_agents[sub_agent.id]
            
    def _log_audit(
        self,
        sub_agent_id: str,
        level: EnforcementLevel,
        reason: str,
        resource_type: str,
        threshold: float,
        terminated: bool
    ) -> None:
        """Log an enforcement action to the audit log."""
        action = EnforcementAction(
            sub_agent_id=sub_agent_id,
            level=level,
            reason=reason,
            resource_type=resource_type,
            threshold_percentage=threshold,
            terminated=terminated
        )
        self._audit_log.append(action)
        
    def get_resource_usage(self, sub_agent_id: str) -> Optional[ResourceUsageMetrics]:
        """
        Get current resource usage for a SubAgent.
        
        Args:
            sub_agent_id: ID of the SubAgent.
            
        Returns:
            ResourceUsageMetrics if found, None otherwise.
        """
        with self._lock:
            return self._metrics.get(sub_agent_id)
            
    def get_audit_log(self) -> List[EnforcementAction]:
        """
        Get all enforcement actions for audit purposes.
        
        Returns:
            List of all enforcement actions.
        """
        with self._lock:
            return list(self._audit_log)
            
    def get_active_sub_agents(self) -> List[SubAgent]:
        """
        Get all active SubAgents.
        
        Returns:
            List of active SubAgents.
        """
        with self._lock:
            return list(self._active_sub_agents.values())
            
    def terminate_sub_agent(self, sub_agent_id: str, reason: str) -> None:
        """
        Terminate a specific SubAgent.
        
        Args:
            sub_agent_id: ID of the SubAgent to terminate.
            reason: Reason for termination.
        """
        with self._lock:
            if sub_agent_id in self._active_sub_agents:
                sub_agent = self._active_sub_agents[sub_agent_id]
                sub_agent.terminate(reason)
                self._log_audit(
                    sub_agent_id,
                    EnforcementLevel.HARD_ENFORCEMENT,
                    f"Manual termination: {reason}",
                    "Manual",
                    0.0,
                    True
                )
                del self._active_sub_agents[sub_agent_id]
