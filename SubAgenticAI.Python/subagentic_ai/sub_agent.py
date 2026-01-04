"""
SubAgent Base Class

SubAgent represents a specialized, mission-scoped cognitive unit.
Each SubAgent operates within strict resource boundaries defined by its
CloudResourceAllocationProfile.
"""

from abc import ABC, abstractmethod
from datetime import datetime
from enum import Enum, auto
from typing import Any

from .cloud_resource_profile import CloudResourceAllocationProfile


class SubAgentStatus(Enum):
    """Status of a SubAgent."""
    CREATED = auto()
    RUNNING = auto()
    COMPLETED = auto()
    FAILED = auto()
    TERMINATED = auto()


class SubAgentExecutionException(Exception):
    """Exception raised when SubAgent execution fails."""
    pass


class SubAgent(ABC):
    """
    Base class for SubAgents.
    
    SubAgents are specialized, mission-scoped cognitive units that operate
    within strict resource boundaries.
    """
    
    def __init__(
        self,
        agent_id: str,
        mission_description: str,
        resource_profile: CloudResourceAllocationProfile
    ):
        if not agent_id:
            raise ValueError("agent_id cannot be empty")
        if not mission_description:
            raise ValueError("mission_description cannot be empty")
        if resource_profile is None:
            raise ValueError("resource_profile cannot be None")
            
        self._id = agent_id
        self._mission_description = mission_description
        self._resource_profile = resource_profile
        self._created_at = datetime.utcnow()
        self._status = SubAgentStatus.CREATED
        
    @property
    def id(self) -> str:
        """Unique identifier for this SubAgent."""
        return self._id
        
    @property
    def mission_description(self) -> str:
        """Description of the mission assigned to this SubAgent."""
        return self._mission_description
        
    @property
    def resource_profile(self) -> CloudResourceAllocationProfile:
        """Immutable resource allocation profile."""
        return self._resource_profile
        
    @property
    def created_at(self) -> datetime:
        """Timestamp when this SubAgent was created."""
        return self._created_at
        
    @property
    def status(self) -> SubAgentStatus:
        """Current status of this SubAgent."""
        return self._status
        
    async def execute_mission(self) -> Any:
        """
        Execute the mission assigned to this SubAgent.
        
        Returns:
            Result of the mission execution.
            
        Raises:
            SubAgentExecutionException: If execution fails.
        """
        self._status = SubAgentStatus.RUNNING
        try:
            result = await self._execute()
            self._status = SubAgentStatus.COMPLETED
            return result
        except Exception as ex:
            self._status = SubAgentStatus.FAILED
            raise SubAgentExecutionException(
                f"SubAgent {self._id} failed: {str(ex)}"
            ) from ex
            
    @abstractmethod
    async def _execute(self) -> Any:
        """
        Abstract method to be implemented by specific SubAgent types.
        
        Returns:
            Result of the execution.
        """
        pass
        
    def terminate(self, reason: str) -> None:
        """
        Terminate this SubAgent immediately.
        
        Args:
            reason: Reason for termination.
        """
        self._status = SubAgentStatus.TERMINATED
        self._on_terminated(reason)
        
    def _on_terminated(self, reason: str) -> None:
        """
        Override in derived classes to handle cleanup.
        
        Args:
            reason: Reason for termination.
        """
        pass
