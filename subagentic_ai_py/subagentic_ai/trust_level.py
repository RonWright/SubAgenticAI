"""Trust Level (TL) for inbound information flow control."""

from dataclasses import dataclass


@dataclass(frozen=True)
class TrustLevel:
    """
    Trust Level (TL) governs inbound information flow.
    Composed of Sender Trust Level (TLₛ) and Content Trust Level (TL꜀).
    """
    
    sender_trust: float  # Sender Trust Level (TLₛ) - historical reliability and behavioral integrity
    content_trust: float  # Content Trust Level (TL꜀) - factual accuracy and provenance
    
    def __post_init__(self):
        """Validate and clamp trust values to [0.0, 1.0] range."""
        object.__setattr__(self, 'sender_trust', max(0.0, min(1.0, self.sender_trust)))
        object.__setattr__(self, 'content_trust', max(0.0, min(1.0, self.content_trust)))
    
    def meets_threshold(self, threshold: 'TrustLevel') -> bool:
        """
        Check if this trust level meets the required threshold.
        
        Args:
            threshold: Required trust threshold
            
        Returns:
            True if both sender and content trust meet the threshold
        """
        return (self.sender_trust >= threshold.sender_trust and 
                self.content_trust >= threshold.content_trust)
