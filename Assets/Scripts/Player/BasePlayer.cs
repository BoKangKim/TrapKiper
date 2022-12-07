using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum STATE
{
    NONE,
    IDLE_STATE,
    MOVE_STATE,
    JUMP_STATE,
    CAST_STATE,
    SKILL_STATE,
    DIE_STATE,
    MAX
}
[RequireComponent(typeof(AnimationEventReciver))]
public abstract class BasePlayer : MonoBehaviour
{

    #region Members
    //Camera parent
    [Header("Follow CameraArm")]
    private FollowCamera mainCamera = null;
    private Transform cameraArmTr = null;

    //Attack & Skill Set
    [Header("Attack Info")]
    [SerializeField] public Transform attackStartZone        = null;
    [SerializeField] protected SkillData[] gainSkills        = null;

    protected EffectBox myEffectBox = null;
    private GameObject indicator = null;
    private GameObject jumpEffect = null;

    //Player component
    protected Animator playerAnimator = null;
    private Transform playerTr      = null;
    private Rigidbody playerRb      = null;
    private Collider playerCollider = null;

    //Player Speed Value
    [Header("Player Stat Value")]
    [SerializeField] protected float playerSpeed  = 30f;
    [SerializeField] private float runSpeed       = 50f;
    [SerializeField] private float jumpSpeed      = 20f;
    [SerializeField] private float JumpRunSpeed   = 40f;
    [SerializeField] private float stopSpeed      = 5f;
    [SerializeField] private float jumpPower      = 7f;
    protected float originSpeed                   = 0;

    [Header("Player Battle Value")]
    [SerializeField] private float MaxplayerHp       = 50f;
    [SerializeField] private float MaxPlayerMp       = 50f;
    [SerializeField] private float playerAttackPower = 10f;

    //Dir Vector
    private Vector3 lookForward = Vector3.zero;
    private Vector3 lookRight   = Vector3.zero;
    private Vector3 moveDir     = Vector3.zero;

    //Check Value
    private bool isRun       = false;
    private bool jumpCheck   = false;
    public  bool castCheck   = false;
    private bool basicAttackChek = false;
    private bool collisionCheck = false;    


    //axis Value
    private float mouseX = 0;
    private float mouseY = 0;
    protected float axisX  = 0;
    protected float axisZ  = 0;

    //player animation learp Value
    protected float fixedAxisZ = 0f;
    protected float fixedAxisX = 0f;

    //Current Animation State
    private STATE curState;
    //Current Coroutine
    protected Coroutine stateCoroutine;
    #endregion

    //Awake&&Start&&Update method
    #region Awake && Start && Update method
    private void Awake()
    {
        Init();
    }

    private void Start()
    {
        StartImplemented();
    }

    private void Update()
    {
        UpdateImplemented();

    }

    private void FixedUpdate()
    {
        PlyaerMove();
    }

    void Init()
    {
        playerAnimator = GetComponent<Animator>();
        playerTr       = GetComponent<Transform>();
        playerCollider = GetComponent<Collider>();
        playerRb       = GetComponent<Rigidbody>();
        cameraArmTr    = FindObjectOfType<FollowCamera>().transform;
        mainCamera     = FindObjectOfType<FollowCamera>();
        myEffectBox = Resources.Load<EffectBox>("EffectBox");

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;
    }

    void StartImplemented()
    {
        AnimationEventReciver EventReciver = GetComponent<AnimationEventReciver>();
        EventReciver.callJumpEvent = JumpStart;
        EventReciver.callJunpEndEvent = JumpEnd;
        EventReciver.callAttackEvent = BasicAttack;
        EventReciver.callSkillEndEvent = SkillEnd;
        EventReciver.callSkillStartEvent = SkillStart;

        originSpeed = playerSpeed;
        ChageState(STATE.MOVE_STATE);
    }
    void UpdateImplemented()
    {
        GetAxisValue();

        InputKey();

        PlayerRotation();
    }
    #endregion

    //Update method
    #region Update method
    protected virtual void GetAxisValue()
    {
        //Mouse Axis
        mouseX += Input.GetAxis("Mouse X")*0.5f;
        mouseY += Input.GetAxis("Mouse Y") * 0.5f;

        //Keyboard Axis
        axisX = Input.GetAxis("Horizontal");
        axisZ = Input.GetAxis("Vertical");
    }
    protected virtual void InputKey()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            isRun = true;
        }
        else
        {
            playerAnimator.SetBool("isRun", false);
            isRun = false;
        }
        if (Input.GetMouseButtonDown(0) && instSkill == false && basicAttackChek == false)
        {
            basicAttackChek = true;
            indicator = Pool.ObjectInstantiate(myEffectBox.basicAttackIndicator, attackStartZone.position, transform.rotation);
            indicator.transform.SetParent(transform);
            indicator.transform.position = transform.position;// + (transform.forward.normalized *1f)+(Vector3.up);

            playerAnimator.SetTrigger("isAttack");
            Invoke("Invoke_BasicAttack", 1f);

        }
        if (Input.GetKeyDown(KeyCode.Space)&& jumpCheck == false && castCheck == false)
        {
            Debug.Log("input space");
            jumpCheck = true;

            ChageState(STATE.JUMP_STATE);
            Invoke("JumpStart", 0.05f);
        }
        if(Input.GetKeyDown(KeyCode.X) && jumpCheck == false && isRun == false && instSkill == false && castCheck ==false)
        {
           
            castCheck = true;
            playerAnimator.SetTrigger("isSkill");
        }

    }
    protected virtual void PlayerRotation()
    {
        playerTr.rotation = Quaternion.Euler(0, mainCamera.mouseX,0);
        attackStartZone.rotation= Quaternion.Euler(-mainCamera.mouseY,mainCamera.mouseX, 0);
        if(indicator!=null)
        indicator.transform.rotation = Quaternion.Euler(0, mainCamera.mouseX, 0);

    }

    protected virtual void PlyaerMove()
    {
        lookForward = new Vector3(cameraArmTr.transform.forward.x, 0, cameraArmTr.transform.forward.z);
        lookRight = new Vector3(cameraArmTr.transform.right.x, 0, cameraArmTr.transform.right.z);

        if ((axisX != 0 || axisZ != 0 )&&castCheck==false)
        {
            moveDir = lookForward * axisZ + lookRight * axisX;
            playerRb.MovePosition(this.gameObject.transform.position + moveDir.normalized * playerSpeed * Time.fixedDeltaTime);
        }
        else
        {
            moveDir = lookForward * axisZ + lookRight * axisX;
            playerRb.MovePosition(this.gameObject.transform.position + moveDir.normalized * (playerSpeed/10) * Time.fixedDeltaTime);
        }
    }
    #endregion

    //Invoke metsod
    void Invoke_BasicAttack()
    {
        Debug.Log("gd");
        basicAttackChek = false;

        if(indicator != null)
            Pool.ObjectDestroy(indicator);

    }

    //delgate method
    #region delgate method
    void BasicAttack()
    {
        Pool.ObjectInstantiate(myEffectBox.basicAttackPrefab, attackStartZone.position, attackStartZone.rotation);
        Pool.ObjectDestroy(indicator);
        basicAttackChek = false;
    }

    void SkillEnd()
    {
       castCheck = false;
    }

    void SkillStart()
    {
        ChageState(STATE.CAST_STATE);
    }

    void JumpStart()
    {
        playerRb.AddForce(Vector3.up * jumpPower, ForceMode.VelocityChange);
    }

    void JumpEnd()
    {
        ChageState(STATE.MOVE_STATE);
        Pool.ObjectDestroy(jumpEffect);
        jumpCheck = false;
        collisionCheck = false;
    }
    #endregion

    //Coroutine state machine
    protected void ChageState(STATE newState)
    {
        if (newState == curState) return;

        if(stateCoroutine!=null)
            StopAllCoroutines();

        curState = newState;

        stateCoroutine = StartCoroutine(curState.ToString());
    }
    
    IEnumerator MOVE_STATE()
    {
        float speed = 0;
       
        playerAnimator.SetBool("isMove", true);
        fixedAxisZ = 0;
        fixedAxisX = 0;
        playerSpeed = originSpeed;
        while (true)
        {
            fixedAxisZ = Mathf.Lerp(fixedAxisZ, axisZ, Time.fixedDeltaTime * 6);
            playerAnimator.SetFloat("axisZ", fixedAxisZ);
            fixedAxisX = Mathf.Lerp(fixedAxisX, axisX, Time.fixedDeltaTime * 6);
            playerAnimator.SetFloat("axisX", fixedAxisX);

            if (isRun && axisZ >= 0)
            {
                playerSpeed = Mathf.Lerp(playerSpeed, runSpeed, Time.fixedDeltaTime * 2);
                speed = Mathf.Lerp(speed, 1, Time.deltaTime);

                playerAnimator.SetBool("isMove", false);
                playerAnimator.SetBool("isRun", true);
                playerAnimator.SetFloat("speed", speed);
            }
            else
            {
                playerSpeed = Mathf.Lerp(playerSpeed, originSpeed, Time.fixedDeltaTime * 2);
                speed = Mathf.Lerp(speed, 0, Time.deltaTime);

                playerAnimator.SetBool("isMove", true);
                playerAnimator.SetBool("isRun", false);
                playerAnimator.SetFloat("speed", speed);
            }

            if (0.3f >= Mathf.Abs(fixedAxisZ) && 0.3f >= Mathf.Abs(fixedAxisX))
                playerSpeed = playerSpeed / 2;
            else
                playerSpeed = originSpeed;


            yield return null;
        }




    }

    IEnumerator JUMP_STATE()
    {
      
        playerCollider.material.dynamicFriction = 0;
        playerAnimator.SetBool("isJump", true); 
        playerAnimator.SetBool("isMove", false);
        playerAnimator.SetTrigger("isJumpStart");
        playerAnimator.SetBool("isMove", false);

        if (isRun)
            playerSpeed = JumpRunSpeed;
        else
            playerSpeed = jumpSpeed;

        while (true)
        {
            fixedAxisZ = Mathf.Lerp(fixedAxisZ, axisZ, Time.fixedDeltaTime * 6);
            playerAnimator.SetFloat("axisZ", fixedAxisZ);
            fixedAxisX = Mathf.Lerp(fixedAxisX, axisX, Time.fixedDeltaTime * 6);
            playerAnimator.SetFloat("axisX", fixedAxisX);

            yield return null;
        }
    }


    public bool instSkill = false;


    public abstract IEnumerator CAST_STATE();
    public abstract IEnumerator SKILL_STATE();
   

    private void OnCollisionEnter(Collision collision)
    {

        if (jumpCheck && collision.contacts[0].normal.y > 0.7f&& collisionCheck==false)
        {
            jumpEffect = Pool.ObjectInstantiate(myEffectBox.jumpEffect, transform.position, Quaternion.identity);
            jumpEffect.transform.SetParent(transform);
            collisionCheck = true;
            playerCollider.material.dynamicFriction = 10;
            playerSpeed = stopSpeed;
            playerAnimator.SetTrigger("isJumpEnd");
            playerAnimator.SetBool("isJump", false);

            Invoke("JumpEnd", 0.5f);
        }
    }

    
}

