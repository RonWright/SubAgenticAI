"""SubAgent interface and implementation."""

from abc import ABC, abstractmethod
from datetime import datetime
from typing import Dict, List, Any

from .mission_status import MissionStatus
from .trust_profile import TrustProfile


class ISubAgent(ABC):
    """Interface for a SubAgent (SA) - a specialized, mission-scoped cognitive unit."""
    
    @property
    @abstractmethod
    def id(self) -> str:
        """Unique identifier for this SubAgent."""
        pass
    
    @property
    @abstractmethod
    def domain(self) -> str:
        """Domain or mission focus of this SubAgent."""
        pass
    
    @property
    @abstractmethod
    def status(self) -> MissionStatus:
        """Current mission status."""
        pass
    
    @property
    @abstractmethod
    def trust_profile(self) -> TrustProfile:
        """Trust profile assigned by the FLA."""
        pass
    
    @abstractmethod
    async def start_mission(self) -> None:
        """Start the SubAgent's mission."""
        pass
    
    @abstractmethod
    async def process_task(self, task: str) -> str:
        """Process a task within the SubAgent's domain."""
        pass
    
    @abstractmethod
    async def receive_information(self, sender_id: str, information: str) -> bool:
        """Receive information from external source (validated by trust profile)."""
        pass
    
    @abstractmethod
    async def send_information(self, recipient_id: str, information: str) -> bool:
        """Send information outbound (governed by security level)."""
        pass
    
    @abstractmethod
    async def complete_mission(self) -> None:
        """Complete the mission."""
        pass
    
    @abstractmethod
    async def retire(self) -> None:
        """Retire/dispose the SubAgent."""
        pass


class SubAgent(ISubAgent):
    """Implementation of a SubAgent - specialized, mission-scoped cognitive unit with strict isolation."""
    
    def __init__(self, agent_id: str, domain: str, trust_profile: TrustProfile):
        """
        Create a new SubAgent.
        
        Args:
            agent_id: Unique identifier
            domain: Domain or mission focus
            trust_profile: Trust profile assigned by FLA
        """
        self._id = agent_id
        self._domain = domain
        self._trust_profile = trust_profile
        self._status = MissionStatus.CREATED
        self._local_state: Dict[str, Any] = {}
        self._activity_log: List[str] = []
        
        self._log_activity(f"SubAgent created: {agent_id} for domain: {domain}")
    
    @property
    def id(self) -> str:
        return self._id
    
    @property
    def domain(self) -> str:
        return self._domain
    
    @property
    def status(self) -> MissionStatus:
        return self._status
    
    @property
    def trust_profile(self) -> TrustProfile:
        return self._trust_profile
    
    async def start_mission(self) -> None:
        """Start the mission."""
        if self._status != MissionStatus.CREATED:
            raise RuntimeError(f"Cannot start mission from status: {self._status}")
        
        self._status = MissionStatus.ACTIVE
        self._log_activity("Mission started")
    
    async def process_task(self, task: str) -> str:
        """Process a task."""
        if self._status != MissionStatus.ACTIVE:
            raise RuntimeError(f"Cannot process task in status: {self._status}")
        
        self._log_activity(f"Processing task: {task}")
        
        # Placeholder for actual task processing logic
        # In a real implementation, this would involve domain-specific processing
        result = f"Processed: {task} in domain: {self._domain}"
        self._log_activity(f"Task completed: {result}")
        
        return result
    
    async def receive_information(self, sender_id: str, information: str) -> bool:
        """Receive information with trust validation."""
        if self._status != MissionStatus.ACTIVE:
            self._log_activity(f"Rejected inbound information - status: {self._status}")
            return False
        
        # Validate inbound information using trust profile
        is_valid = await self._trust_profile.validate_inbound(sender_id, information)
        
        if is_valid:
            self._local_state[f"received_{datetime.utcnow().timestamp()}"] = information
            self._log_activity(f"Accepted information from: {sender_id}")
            return True
        
        self._log_activity(f"Rejected information from: {sender_id} - trust validation failed")
        return False
    
    async def send_information(self, recipient_id: str, information: str) -> bool:
        """Send information outbound."""
        if self._status != MissionStatus.ACTIVE:
            self._log_activity(f"Cannot send information - status: {self._status}")
            return False
        
        # Security level check for outbound information
        self._log_activity(f"Sending information to: {recipient_id} (SL: {self._trust_profile.security_level})")
        return True
    
    async def complete_mission(self) -> None:
        """Complete the mission."""
        if self._status not in (MissionStatus.ACTIVE, MissionStatus.PAUSED):
            raise RuntimeError(f"Cannot complete mission from status: {self._status}")
        
        self._status = MissionStatus.COMPLETED
        self._log_activity("Mission completed")
    
    async def retire(self) -> None:
        """Retire the SubAgent."""
        if self._status == MissionStatus.RETIRED:
            return
        
        self._status = MissionStatus.RETIRED
        self._log_activity("SubAgent retired")
        
        # Clear local state for garbage collection
        self._local_state.clear()
    
    def get_activity_log(self) -> List[str]:
        """Get the activity log for auditing purposes."""
        return self._activity_log.copy()
    
    def get_state_snapshot(self) -> Dict[str, Any]:
        """Get a snapshot of local state (for debugging/auditing)."""
        return self._local_state.copy()
    
    def _log_activity(self, activity: str) -> None:
        """Log an activity."""
        log_entry = f"[{datetime.utcnow().strftime('%Y-%m-%d %H:%M:%S.%f')[:-3]}] {activity}"
        self._activity_log.append(log_entry)
