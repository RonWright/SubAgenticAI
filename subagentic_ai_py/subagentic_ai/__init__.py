"""
SubAgenticAI - A governance-driven architecture for agentic systems.

This package provides a Python implementation of the SubAgenticAI architecture
for managing Front Line Agents (FLA) and SubAgents (SA).
"""

from .security_level import SecurityLevel
from .trust_level import TrustLevel
from .mission_status import MissionStatus
from .trust_broker import ITrustBroker, SimpleTrustBroker
from .trust_profile import TrustProfile
from .communication_authorization import CommunicationAuthorization
from .sub_agent import ISubAgent, SubAgent
from .front_line_agent import FrontLineAgent

__version__ = "0.1.0"

__all__ = [
    "SecurityLevel",
    "TrustLevel",
    "MissionStatus",
    "ITrustBroker",
    "SimpleTrustBroker",
    "TrustProfile",
    "CommunicationAuthorization",
    "ISubAgent",
    "SubAgent",
    "FrontLineAgent",
]
