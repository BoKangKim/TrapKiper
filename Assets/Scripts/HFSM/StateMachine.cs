using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HFSM 
{
    public abstract class StateMachine : MonoBehaviour
    {
        private BaseState state = null;

        private void Start()
        {
            state = InitializingState();
        }

        private void Update()
        {
            if(state != null)
            {
                state.UpdateLogic();
            }
        }

        private void FixedUpdate()
        {
            if(state != null)
            {
                state.UpdatePhysics();
            }
        }

        public void ChangeState(BaseState nextState)
        {
            state.Exit();

            state = nextState;
            state.Enter();
        }


        protected abstract BaseState InitializingState();
    }
}

