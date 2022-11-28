using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CompositeNode : INode
{
    private Stack<INode> nodes;
    public CompositeNode(params INode[] nodes)
    {
        this.nodes = new Stack<INode>(nodes);
    }

    public abstract bool Run();
}
