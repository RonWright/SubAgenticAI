"""
Enforcement Actions and Levels

Represents enforcement actions taken by the FLA when resource limits
are approached or exceeded.
"""

from dataclasses import dataclass, field
from datetime import datetime
from enum import Enum, auto


class EnforcementLevel(Enum):
    """Enforcement level for resource governance."""
    NONE = auto()
    SOFT_ENFORCEMENT = auto()
    HARD_ENFORCEMENT = auto()


@dataclass
class EnforcementAction:
    """Record of an enforcement action taken by the FLA."""
    
    sub_agent_id: str
    level: EnforcementLevel
    reason: str
    resource_type: str
    threshold_percentage: float
    terminated: bool
    timestamp: datetime = field(default_factory=datetime.utcnow)
