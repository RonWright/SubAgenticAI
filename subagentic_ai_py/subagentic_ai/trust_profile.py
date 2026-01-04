"""Trust profile for SubAgent security and trust management."""

from dataclasses import dataclass, field
from typing import List, Tuple
from statistics import mean

from .security_level import SecurityLevel
from .trust_level import TrustLevel
from .trust_broker import ITrustBroker


@dataclass(frozen=True)
class TrustProfile:
    """
    Trust profile assigned by the FLA to each SubAgent.
    Contains SL (Security Level) for outbound and TL (Trust Level) thresholds for inbound communication.
    """
    
    DEFAULT_MINIMUM_BROKER_AGREEMENT = 2
    
    security_level: SecurityLevel  # Security Level for outbound information flow
    required_trust_threshold: TrustLevel  # Required trust threshold for accepting inbound information
    trust_brokers: Tuple[ITrustBroker, ...]  # Content Sharing Policy - set of trust brokers
    minimum_broker_agreement: int = DEFAULT_MINIMUM_BROKER_AGREEMENT  # Minimum brokers that must agree
    agreement_tolerance: float = 0.1  # Tolerance for convergence of broker evaluations
    
    def __post_init__(self):
        """Validate and adjust configuration."""
        # Ensure minimum broker agreement is at least 2
        if self.minimum_broker_agreement < self.DEFAULT_MINIMUM_BROKER_AGREEMENT:
            object.__setattr__(self, 'minimum_broker_agreement', self.DEFAULT_MINIMUM_BROKER_AGREEMENT)
        
        # Clamp agreement tolerance
        object.__setattr__(self, 'agreement_tolerance', max(0.0, min(1.0, self.agreement_tolerance)))
    
    async def validate_inbound(self, sender_id: str, content: str) -> bool:
        """
        Evaluate if inbound content meets trust requirements with independent broker agreement.
        
        Args:
            sender_id: Identifier of the sender
            content: Content to validate
            
        Returns:
            True if content passes trust validation
        """
        if len(self.trust_brokers) < self.minimum_broker_agreement:
            return False
        
        sender_trusts = []
        content_trusts = []
        
        # Evaluate with all brokers
        for broker in self.trust_brokers:
            # Check if any broker flags the content
            if await broker.is_flagged(sender_id, content):
                return False
            
            sender_trust = await broker.evaluate_sender_trust(sender_id)
            content_trust = await broker.evaluate_content_trust(content)
            
            sender_trusts.append(sender_trust)
            content_trusts.append(content_trust)
        
        # Check for independent agreement (convergence within tolerance)
        if not self._has_independent_agreement(sender_trusts):
            return False
        if not self._has_independent_agreement(content_trusts):
            return False
        
        # Check if average trust levels meet threshold
        avg_sender_trust = mean(sender_trusts)
        avg_content_trust = mean(content_trusts)
        evaluated_trust = TrustLevel(avg_sender_trust, avg_content_trust)
        
        return evaluated_trust.meets_threshold(self.required_trust_threshold)
    
    def _has_independent_agreement(self, evaluations: List[float]) -> bool:
        """Check if broker evaluations converge within tolerance."""
        if len(evaluations) < self.minimum_broker_agreement:
            return False
        
        avg = mean(evaluations)
        within_tolerance = sum(1 for e in evaluations if abs(e - avg) <= self.agreement_tolerance)
        
        return within_tolerance >= self.minimum_broker_agreement
