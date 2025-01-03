using System.Collections.Generic;

public abstract class CompositeNode : BehaviorNode
{
    protected List<BehaviorNode> children = new List<BehaviorNode>();

    public void AddChild(BehaviorNode node)
    {
        children.Add(node);
    }
}
