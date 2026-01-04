"""Trust broker interface and implementation."""

from abc import ABC, abstractmethod
from typing import Dict, Set


class ITrustBroker(ABC):
    """Interface for independent trust brokers that evaluate sender and content trustworthiness."""
    
    @property
    @abstractmethod
    def broker_id(self) -> str:
        """Unique identifier for this trust broker."""
        pass
    
    @property
    @abstractmethod
    def name(self) -> str:
        """Name of the trust broker."""
        pass
    
    @abstractmethod
    async def evaluate_sender_trust(self, sender_id: str) -> float:
        """
        Evaluate the sender's trustworthiness.
        
        Args:
            sender_id: Identifier of the sender
            
        Returns:
            Trust level between 0.0 and 1.0
        """
        pass
    
    @abstractmethod
    async def evaluate_content_trust(self, content: str) -> float:
        """
        Evaluate the content's trustworthiness.
        
        Args:
            content: Content to evaluate
            
        Returns:
            Trust level between 0.0 and 1.0
        """
        pass
    
    @abstractmethod
    async def is_flagged(self, sender_id: str, content: str) -> bool:
        """
        Check if sender or content is flagged as untrustworthy.
        
        Args:
            sender_id: Sender identifier
            content: Content
            
        Returns:
            True if flagged as untrustworthy
        """
        pass


class SimpleTrustBroker(ITrustBroker):
    """
    Simple implementation of a trust broker for demonstration purposes.
    In production, this would connect to external trust evaluation services.
    """
    
    MALICIOUS_CONTENT_PENALTY = 0.3
    SUSPICIOUS_CONTENT_PENALTY = 0.6
    VERIFIED_CONTENT_BONUS = 1.2
    
    def __init__(self, broker_id: str, name: str, baseline_trust: float = 0.7):
        """
        Initialize a simple trust broker.
        
        Args:
            broker_id: Unique identifier
            name: Broker name
            baseline_trust: Default trust level (0.0 to 1.0)
        """
        self._broker_id = broker_id
        self._name = name
        self._baseline_trust = max(0.0, min(1.0, baseline_trust))
        self._sender_reputations: Dict[str, float] = {}
        self._flagged_senders: Set[str] = set()
    
    @property
    def broker_id(self) -> str:
        return self._broker_id
    
    @property
    def name(self) -> str:
        return self._name
    
    async def evaluate_sender_trust(self, sender_id: str) -> float:
        """Return stored reputation or baseline."""
        return self._sender_reputations.get(sender_id, self._baseline_trust)
    
    async def evaluate_content_trust(self, content: str) -> float:
        """Simple heuristic - in production would use ML/NLP."""
        trust_score = self._baseline_trust
        
        # Reduce trust for suspicious patterns
        if "malicious" in content.lower():
            trust_score *= self.MALICIOUS_CONTENT_PENALTY
        elif "suspicious" in content.lower():
            trust_score *= self.SUSPICIOUS_CONTENT_PENALTY
        
        # Increase trust for verified markers
        if "verified" in content.lower():
            trust_score = min(1.0, trust_score * self.VERIFIED_CONTENT_BONUS)
        
        return max(0.0, min(1.0, trust_score))
    
    async def is_flagged(self, sender_id: str, content: str) -> bool:
        """Check if sender is flagged or content is malicious."""
        if sender_id in self._flagged_senders:
            return True
        
        if "malicious" in content.lower():
            return True
        
        return False
    
    def set_sender_reputation(self, sender_id: str, reputation: float) -> None:
        """Set reputation for a sender (for testing/configuration)."""
        self._sender_reputations[sender_id] = max(0.0, min(1.0, reputation))
    
    def flag_sender(self, sender_id: str) -> None:
        """Flag a sender as untrustworthy."""
        self._flagged_senders.add(sender_id)
    
    def unflag_sender(self, sender_id: str) -> None:
        """Remove flag from a sender."""
        self._flagged_senders.discard(sender_id)
