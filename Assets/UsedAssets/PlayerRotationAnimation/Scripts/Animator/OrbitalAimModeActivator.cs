using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitalAimModeActivator : StateMachineBehaviour
{
    [Tooltip("The speed at which IK is enabled when the animation state transition has begun.")]
    public float iKFadeSpeed = 1f;
    [Tooltip("The amount of time that a soft aim value calculation is used while transitioning states. This timer prevents the arms from moving through the characters body when the target is behind the character.")]
    public float aimCalcDelay = 0.1f;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    //Activates aim mode and sets the aim calculation delay when the animation state transitions
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.GetComponent<OrbitalRemote>() == null) return;
        animator.GetComponent<OrbitalRemote>().ActivateAimMode(iKFadeSpeed);
        animator.GetComponent<OrbitalRemote>().SetStartupDelay(aimCalcDelay);
    }
}
