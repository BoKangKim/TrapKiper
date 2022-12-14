using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HFSM;

public class MoveMentSM : StateMachine
{
    private BaseState initState = null;
    public Animator animator = null;
    public Rigidbody playerRb = null;

    [Header("Follow CameraArm")]
    [HideInInspector] public FollowCamera mainCamera = null;
    [HideInInspector] public Transform cameraArmTr = null;

    [HideInInspector] public bool castCheck = false;
    [HideInInspector] public bool isRun = false;

    [HideInInspector] public IdelState idle = null;
    [HideInInspector] public MoveState move = null;

    [Header("Player Stat Value")]
    [SerializeField] public float playerSpeed = 30f;
    [SerializeField] public float runSpeed = 50f;
    [HideInInspector] public float originSpeed = 0;

    private void Awake()
    {
        idle = new IdelState(this);
        move = new MoveState(this);

        cameraArmTr = FindObjectOfType<FollowCamera>().transform;
        mainCamera = FindObjectOfType<FollowCamera>();

        initState = idle;
    }

    protected override BaseState InitializingState()
    {
        return initState;
    }

}
