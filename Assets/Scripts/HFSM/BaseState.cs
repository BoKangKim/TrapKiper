using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HFSM
{
    public class BaseState
    {
        protected StateMachine stateMachine = null;

        public BaseState(StateMachine machine)
        {
            this.stateMachine = machine;
        }

        public virtual void Enter() { }
        public virtual void UpdateLogic() { }
        public virtual void UpdatePhysics() { }
        public virtual void Exit() { }
    }
}

