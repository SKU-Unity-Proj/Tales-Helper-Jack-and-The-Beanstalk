using System;

public abstract class BehaviorNode
{
    public abstract NodeState Execute();
}

public enum NodeState
{
    RUNNING,
    SUCCESS,
    FAILURE
}
