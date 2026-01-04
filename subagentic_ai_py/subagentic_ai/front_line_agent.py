"""Front Line Agent (FLA) - orchestration layer."""

from datetime import datetime, timedelta
from typing import Dict, List, Optional
from uuid import uuid4

from .sub_agent import ISubAgent, SubAgent
from .communication_authorization import CommunicationAuthorization
from .trust_profile import TrustProfile


class FrontLineAgent:
    """
    Front Line Agent (FLA) - orchestrates SubAgents, enforces isolation, and manages communication.
    Acts as chief of staff without holding domain knowledge.
    """
    
    def __init__(self, agent_id: str, default_trust_profile: TrustProfile):
        """
        Create a new Front Line Agent.
        
        Args:
            agent_id: Unique identifier for this FLA
            default_trust_profile: Default trust profile for SubAgents
        """
        self._id = agent_id
        self._default_trust_profile = default_trust_profile
        self._active_sub_agents: Dict[str, ISubAgent] = {}
        self._communication_authorizations: List[CommunicationAuthorization] = []
        self._event_log: List[str] = []
        
        self._log_event(f"FLA created: {agent_id}")
    
    @property
    def id(self) -> str:
        return self._id
    
    def classify_domain(self, user_intent: str) -> str:
        """
        Interpret user intent and classify the domain.
        
        Args:
            user_intent: User's stated intent
            
        Returns:
            Classified domain
        """
        self._log_event(f"Classifying domain for intent: {user_intent}")
        
        # Simplified classification - in real implementation would use ML/NLP
        intent = user_intent.lower()
        
        if "data" in intent or "analyze" in intent:
            return "DataAnalysis"
        elif "code" in intent or "program" in intent:
            return "CodeGeneration"
        elif "security" in intent or "audit" in intent:
            return "SecurityAudit"
        elif "research" in intent or "find" in intent:
            return "Research"
        
        return "General"
    
    async def create_sub_agent(self, domain: str, 
                               trust_profile: Optional[TrustProfile] = None) -> ISubAgent:
        """
        Spin up a new SubAgent for a specific domain/mission.
        
        Args:
            domain: Domain or mission focus
            trust_profile: Optional custom trust profile
            
        Returns:
            Created SubAgent instance
        """
        agent_id = f"SA-{domain}-{uuid4().hex}"
        profile = trust_profile or self._default_trust_profile
        
        sub_agent = SubAgent(agent_id, domain, profile)
        self._active_sub_agents[agent_id] = sub_agent
        
        await sub_agent.start_mission()
        
        self._log_event(f"Created and started SubAgent: {agent_id} for domain: {domain}")
        
        return sub_agent
    
    async def reactivate_sub_agent(self, agent_id: str) -> Optional[ISubAgent]:
        """
        Reactivate an existing SubAgent (if it was previously created).
        
        Args:
            agent_id: SubAgent identifier
            
        Returns:
            SubAgent instance or None if not found
        """
        agent = self._active_sub_agents.get(agent_id)
        if agent:
            self._log_event(f"Reactivated SubAgent: {agent_id}")
        else:
            self._log_event(f"Failed to reactivate SubAgent: {agent_id} - not found")
        
        return agent
    
    def authorize_communication(self, from_agent_id: str, to_agent_id: str,
                               is_bidirectional: bool = False,
                               allowed_data_scopes: Optional[List[str]] = None,
                               ttl: Optional[timedelta] = None) -> None:
        """
        Authorize communication between two SubAgents.
        
        Args:
            from_agent_id: Source agent ID
            to_agent_id: Target agent ID
            is_bidirectional: Whether communication is two-way
            allowed_data_scopes: List of allowed data scopes
            ttl: Time to live for this authorization
        """
        scopes = tuple(allowed_data_scopes) if allowed_data_scopes else None
        authorization = CommunicationAuthorization.create(
            from_agent_id, to_agent_id, is_bidirectional, scopes, ttl
        )
        
        self._communication_authorizations.append(authorization)
        
        self._log_event(
            f"Authorized communication: {from_agent_id} -> {to_agent_id} "
            f"(bidirectional: {is_bidirectional})"
        )
    
    async def mediate_communication(self, from_agent_id: str, to_agent_id: str,
                                   information: str) -> bool:
        """
        Mediate communication between SubAgents (FLA-mediated communication).
        
        Args:
            from_agent_id: Source agent ID
            to_agent_id: Target agent ID
            information: Information to transmit
            
        Returns:
            True if communication succeeded
        """
        # Check if communication is authorized
        authorization = next(
            (auth for auth in self._communication_authorizations
             if auth.is_authorized(from_agent_id, to_agent_id)),
            None
        )
        
        if not authorization:
            self._log_event(
                f"Communication blocked: {from_agent_id} -> {to_agent_id} - not authorized"
            )
            return False
        
        sender = self._active_sub_agents.get(from_agent_id)
        if not sender:
            self._log_event(f"Communication failed: sender {from_agent_id} not found")
            return False
        
        receiver = self._active_sub_agents.get(to_agent_id)
        if not receiver:
            self._log_event(f"Communication failed: receiver {to_agent_id} not found")
            return False
        
        # Send through sender's outbound (security level check)
        sent_success = await sender.send_information(to_agent_id, information)
        if not sent_success:
            self._log_event("Communication failed: sender rejected outbound")
            return False
        
        # Receive at receiver's inbound (trust level check)
        receive_success = await receiver.receive_information(from_agent_id, information)
        
        self._log_event(
            f"Communication mediated: {from_agent_id} -> {to_agent_id} - success: {receive_success}"
        )
        
        return receive_success
    
    async def retire_sub_agent(self, agent_id: str) -> None:
        """
        Retire a SubAgent when its mission concludes.
        
        Args:
            agent_id: SubAgent identifier
        """
        agent = self._active_sub_agents.get(agent_id)
        if agent:
            await agent.retire()
            del self._active_sub_agents[agent_id]
            
            # Remove any communication authorizations involving this agent
            self._communication_authorizations = [
                auth for auth in self._communication_authorizations
                if auth.from_agent_id != agent_id and auth.to_agent_id != agent_id
            ]
            
            self._log_event(f"Retired SubAgent: {agent_id}")
    
    async def process_user_task(self, user_intent: str, task_details: str) -> str:
        """
        Process a user task by creating appropriate SubAgent(s).
        
        Args:
            user_intent: User's stated intent
            task_details: Details of the task
            
        Returns:
            Task result
        """
        self._log_event(f"Processing user task: {user_intent}")
        
        domain = self.classify_domain(user_intent)
        sub_agent = await self.create_sub_agent(domain)
        
        try:
            result = await sub_agent.process_task(task_details)
            await sub_agent.complete_mission()
            return result
        except Exception as e:
            self._log_event(f"Task processing failed: {str(e)}")
            raise
    
    def get_active_sub_agents(self) -> Dict[str, ISubAgent]:
        """Get all active SubAgents."""
        return self._active_sub_agents.copy()
    
    def get_event_log(self) -> List[str]:
        """Get the event log for auditing."""
        return self._event_log.copy()
    
    def _log_event(self, event_description: str) -> None:
        """Log an event."""
        log_entry = (
            f"[{datetime.utcnow().strftime('%Y-%m-%d %H:%M:%S.%f')[:-3]}] "
            f"[FLA-{self._id}] {event_description}"
        )
        self._event_log.append(log_entry)
