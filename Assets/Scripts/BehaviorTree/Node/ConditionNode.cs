using System;

namespace BehaviorTree 
{
    public class ConditionNode : ILeafNode
    {
        private Func<bool> condition = null;

        public ConditionNode(Func<bool> condition) => this.condition = condition;

        public bool Run()
        {
            return condition();
        }
    }
}

