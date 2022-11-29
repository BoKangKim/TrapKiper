using System;

namespace BehaviorTree 
{
    public class NotConditionNode : ILeafNode
    {
        private Func<bool> condition = null;

        public NotConditionNode(Func<bool> condition) => this.condition = condition;

        public bool Run() => !condition();
    }
}

