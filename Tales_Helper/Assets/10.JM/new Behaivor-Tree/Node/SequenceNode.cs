using System.Collections.Generic;

public class SequenceNode : BehaviorNode
{
    protected List<BehaviorNode> children = new List<BehaviorNode>();

    public void AddChild(BehaviorNode node)
    {
        children.Add(node);
    }

    public override NodeState Execute()
    {
        foreach (var child in children)
        {
            NodeState state = child.Execute();

            if (state == NodeState.FAILURE) return NodeState.FAILURE;
            if (state == NodeState.RUNNING) return NodeState.RUNNING;
            if (state == NodeState.SUCCESS) return NodeState.SUCCESS; // ���� ��ȯ �� ���� ���� �̵�
        }

        return NodeState.SUCCESS;
    }
}
