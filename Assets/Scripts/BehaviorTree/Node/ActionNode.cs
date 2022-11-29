using System;

namespace BehaviorTree 
{
    public class ActionNode : ILeafNode
    {
        private Action action = null;

        public ActionNode(Action action) => this.action = action;

        public bool Run()
        {
            action();
            return true;
        }
    }
}

