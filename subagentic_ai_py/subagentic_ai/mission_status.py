"""Mission status enumeration for SubAgent lifecycle management."""

from enum import Enum


class MissionStatus(Enum):
    """Represents the lifecycle status of a SubAgent mission."""
    
    CREATED = "created"  # Mission has been created but not yet started
    ACTIVE = "active"  # Mission is currently active
    PAUSED = "paused"  # Mission is paused/suspended
    COMPLETED = "completed"  # Mission completed successfully
    FAILED = "failed"  # Mission failed or was aborted
    RETIRED = "retired"  # Mission has been retired/disposed
