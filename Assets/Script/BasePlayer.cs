using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum STATE
{
    NONE,
    IDLE_STATE,
    MOVE_STATE,
    JUMP_STATE,
    SKILL_STATE,
    DIE_STATE,
    MAX
}
[RequireComponent(typeof(AnimationEventReciver))]
public abstract class BasePlayer : MonoBehaviour
{
    //Camera parent
    [Header("Follow CameraArm")]
    private FollowCamera mainCamera = null;
    private Transform cameraArmTr = null;

    //Attack & Skill Set
    [Header("Attack Info")]
    [SerializeField] private Transform attackStartZone    = null;
    [SerializeField] private GameObject basicAttackPrefab = null;
    [SerializeField] private GameObject skillAttackPrefab = null;

    //Player component
    private Animator playerAnimator = null;
    private Transform playerTr      = null;
    private Rigidbody playerRb      = null;

    //Player Body
    [Header("Player Body")]
    [SerializeField] RectTransform playerCanvas  = null;

    //Player Speed Value
    [Header("Player Stat Value")]
    [SerializeField] private float playerSpeed  = 30f;
    [SerializeField] private float runSpeed     = 50f;
    [SerializeField] private float jumpSpeed    = 20f;
    [SerializeField] private float JumpRunSpeed = 40f;
    [SerializeField] private float stopSpeed    = 5f;
    [SerializeField] private float jumpPower    = 7f;
    private float originSpeed                   = 0;

    [Header("Player Battle Value")]
    [SerializeField] private float MaxplayerHp       = 50f;
    [SerializeField] private float MaxPlayerMp       = 50f;
    [SerializeField] private float playerAttackPower = 10f;

    //Dir Vector
    private Vector3 lookForward = Vector3.zero;
    private Vector3 lookRight   = Vector3.zero;
    private Vector3 moveDir     = Vector3.zero;

    //Check Value
    private bool isRun      = false;
    private bool jumpCheck  = false;
    private bool skillCheck = false;

    //axis Value
    private float mouseX = 0;
    private float mouseY = 0;
    private float axisX  = 0;
    private float axisZ  = 0;

    //player animation learp Value
    private float fixedAxisZ = 0f;
    private float fixedAxisX = 0f;

    //Current Animation State
    private STATE curState;
    //Current Coroutine
    private Coroutine stateCoroutine;

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

    //Awake&&Start&&Update method
    #region Awake && Start && Update method
    void Init()
    {
        playerAnimator   = GetComponent<Animator>();
        playerTr         = GetComponent<Transform>();
        playerRb         = GetComponent<Rigidbody>();
        cameraArmTr      = FindObjectOfType<FollowCamera>().transform;
        mainCamera       = FindObjectOfType<FollowCamera>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;
    }

    void StartImplemented()
    {
        AnimationEventReciver EventReciver = GetComponent<AnimationEventReciver>();
        EventReciver.callJumpEvent = JumpStart;
        EventReciver.callJunpEndEvent = JumpEnd;
        EventReciver.callAttackEvent = BasicAttack;
        EventReciver.callSkillEvent = SkillAttack;

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
        mouseX += Input.GetAxis("Mouse X")*6;
        mouseY += Input.GetAxis("Mouse Y")*2;

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

        if (Input.GetMouseButtonDown(0))
        {
            playerAnimator.SetTrigger("isAttack");
        }

        if (Input.GetKeyDown(KeyCode.Space)&& jumpCheck == false)
        {
            jumpCheck = true;
            ChageState(STATE.JUMP_STATE);
        }

        if(Input.GetKeyDown(KeyCode.X) && jumpCheck == false && isRun == false && skillCheck == false)
        {
            skillCheck = true;
            playerAnimator.SetTrigger("isSkill");
        }
    }
    protected virtual void PlayerRotation()
    {
        playerTr.rotation = Quaternion.Euler(0, mainCamera.mouseX,0);
        attackStartZone.rotation= Quaternion.Euler(-mainCamera.mouseY,mainCamera.mouseX, 0);
        playerCanvas.rotation = Quaternion.Euler(-mainCamera.mouseY, mainCamera.mouseX, 0);
    }

    protected virtual void PlyaerMove()
    {
        lookForward = new Vector3(cameraArmTr.transform.forward.x, 0, cameraArmTr.transform.forward.z);
        lookRight = new Vector3(cameraArmTr.transform.right.x, 0, cameraArmTr.transform.right.z);

        if (axisX != 0 || axisZ != 0)
        {
            moveDir = lookForward * axisZ + lookRight * axisX;
            playerRb.MovePosition(this.gameObject.transform.position + moveDir.normalized * playerSpeed * Time.fixedDeltaTime);
        }
    }
    #endregion

    //delgate method
    #region delgate method
    void BasicAttack()
    {
        Instantiate(basicAttackPrefab, attackStartZone.position, attackStartZone.rotation);
    }

    void SkillAttack()
    {
       StartCoroutine(SKILL_STATE());
    }

    void JumpStart()
    {
        playerRb.AddForce(Vector3.up * jumpPower, ForceMode.VelocityChange);
    }

    void JumpEnd()
    {
        jumpCheck = false;
        

        ChageState(STATE.MOVE_STATE);
        
        playerSpeed = originSpeed;
    }
    #endregion

    //Coroutine state machine
    void ChageState(STATE newState)
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
            fixedAxisZ = Mathf.Lerp(fixedAxisZ, axisZ, 2f);
            playerAnimator.SetFloat("axisZ", fixedAxisZ);
            fixedAxisX = Mathf.Lerp(fixedAxisX, axisX, 2f);
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
        float speed = 0;
        playerAnimator.SetBool("isJump", true);
        playerAnimator.SetBool("isMove", false);
        playerAnimator.SetTrigger("isJumpStart");
        playerAnimator.SetBool("isMove", false);
        playerSpeed = jumpSpeed;

        while (true)
        {
            fixedAxisZ = Mathf.Lerp(fixedAxisZ, axisZ, Time.fixedDeltaTime * 6);
            playerAnimator.SetFloat("axisZ", fixedAxisZ);
            fixedAxisX = Mathf.Lerp(fixedAxisX, axisX, Time.fixedDeltaTime * 6);
            playerAnimator.SetFloat("axisX", fixedAxisX);

            if (isRun)
            {
                playerSpeed = Mathf.Lerp(playerSpeed, JumpRunSpeed, Time.fixedDeltaTime * 2);
                speed = Mathf.Lerp(speed, 0, Time.deltaTime);

                playerAnimator.SetFloat("speed", speed);
            }

            yield return null;
        }
    }

    public virtual IEnumerator SKILL_STATE()
    {
        GameObject skill = Instantiate(basicAttackPrefab, attackStartZone.position, transform.rotation);
        skill.transform.localScale = new Vector3(2f,2f,2f);
        skillCheck = false;
        yield return null;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (jumpCheck && collision.contacts[0].normal.y > 0.7f)
        {
            playerSpeed = stopSpeed;
            playerAnimator.SetTrigger("isJumpEnd");
            playerAnimator.SetBool("isJump", false);
        }
    }


}

