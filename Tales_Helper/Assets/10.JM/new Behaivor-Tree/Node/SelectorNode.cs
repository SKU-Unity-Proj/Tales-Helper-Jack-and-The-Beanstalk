using System;

public class SelectorNode : CompositeNode
{
    public override NodeState Execute()
    {
        foreach (var child in children)
        {
            NodeState childState = child.Execute();
            if (childState == NodeState.SUCCESS)
                return NodeState.SUCCESS;
            if (childState == NodeState.RUNNING)
                return NodeState.RUNNING;
        }
        return NodeState.FAILURE;
    }
}
