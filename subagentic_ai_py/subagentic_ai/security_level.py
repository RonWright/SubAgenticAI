"""Security Level (SL) enumeration for outbound information flow control."""

from enum import Enum


class SecurityLevel(Enum):
    """
    Security Level (SL) governs outbound information flow.
    Specifies the conditions under which a SubAgent may disclose information to external entities.
    """
    
    PUBLIC = 0  # Public information - no restrictions on disclosure
    INTERNAL = 1  # Internal use only - restricted to organization
    CONFIDENTIAL = 2  # Confidential - limited disclosure
    SECRET = 3  # Secret - highly restricted disclosure
    TOP_SECRET = 4  # Top Secret - maximum restrictions
