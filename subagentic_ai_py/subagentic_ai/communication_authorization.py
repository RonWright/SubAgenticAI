"""Communication authorization between SubAgents."""

from dataclasses import dataclass, field
from datetime import datetime, timedelta
from typing import Optional, Tuple


@dataclass(frozen=True)
class CommunicationAuthorization:
    """Represents a communication authorization between SubAgents."""
    
    from_agent_id: str
    to_agent_id: str
    is_bidirectional: bool = False
    allowed_data_scopes: Tuple[str, ...] = ("*",)
    created_at: datetime = field(default_factory=datetime.utcnow)
    expires_at: Optional[datetime] = None
    
    @staticmethod
    def create(from_agent_id: str, to_agent_id: str, is_bidirectional: bool = False,
               allowed_data_scopes: Optional[Tuple[str, ...]] = None,
               ttl: Optional[timedelta] = None) -> 'CommunicationAuthorization':
        """
        Create a new communication authorization.
        
        Args:
            from_agent_id: Source agent ID
            to_agent_id: Target agent ID
            is_bidirectional: Whether communication is two-way
            allowed_data_scopes: Scopes of allowed data
            ttl: Time to live for this authorization
            
        Returns:
            New CommunicationAuthorization instance
        """
        created_at = datetime.utcnow()
        expires_at = created_at + ttl if ttl else None
        scopes = allowed_data_scopes if allowed_data_scopes else ("*",)
        
        return CommunicationAuthorization(
            from_agent_id=from_agent_id,
            to_agent_id=to_agent_id,
            is_bidirectional=is_bidirectional,
            allowed_data_scopes=scopes,
            created_at=created_at,
            expires_at=expires_at
        )
    
    def is_expired(self) -> bool:
        """Check if this authorization has expired."""
        return self.expires_at is not None and datetime.utcnow() > self.expires_at
    
    def is_authorized(self, from_id: str, to_id: str) -> bool:
        """
        Check if communication is authorized.
        
        Args:
            from_id: Source agent ID
            to_id: Target agent ID
            
        Returns:
            True if communication is authorized
        """
        if self.is_expired():
            return False
        
        if self.from_agent_id == from_id and self.to_agent_id == to_id:
            return True
        
        if self.is_bidirectional and self.from_agent_id == to_id and self.to_agent_id == from_id:
            return True
        
        return False
