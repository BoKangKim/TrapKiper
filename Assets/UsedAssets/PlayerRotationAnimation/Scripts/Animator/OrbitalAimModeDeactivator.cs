using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitalAimModeDeactivator : StateMachineBehaviour
{
    [Tooltip("The speed at which IK is disabled when the animation state transition has begun.")]
    public float iKFadeSpeed = 15f;
    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    //Deactivates aim mode when the animation state transitions
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.GetComponent<OrbitalRemote>() == null) return;
        animator.GetComponent<OrbitalRemote>().DeactivateAimMode(iKFadeSpeed);
    }
}
