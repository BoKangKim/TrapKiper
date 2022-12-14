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

[RequireComponent(typeof(AnimationEventReciver),typeof(PlayerData))]
public abstract class BasePlayer : MonoBehaviour
{

    #region Members
    //Camera parent
    [Header("Follow CameraArm")]
    private FollowCamera mainCamera = null;
    private Transform cameraArmTr = null;

    //Attack & Skill Set
    [Header("Attack Info")]
    public Transform attackStartZone        = null;
    //private List<SkillData> gainSkills      = null;
    private SkillData gainSkill = null;

    protected PlayerEffect myEffectBox = null;
    private GameObject indicator = null;
    private GameObject jumpEffect = null;

    //Player component
    [Header("Player component")]
    protected Animator playerAnimator = null;
    private Transform playerTr        = null;
    private Rigidbody playerRb        = null;
    private Collider playerCollider   = null;
    private PlayerData playerData     = null;

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
    private Vector3 jumpPos     = Vector3.zero;

    //Check Value
    private bool isRun       = false;
    private bool jumpCheck   = false;
    public  bool castCheck   = false;
    private bool basicAttackChek = false;
    private bool collisionCheck = false;
    private bool downCheck = false;
    public bool instSkill = false;

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
        playerRb       = GetComponent<Rigidbody>();
        cameraArmTr    = FindObjectOfType<FollowCamera>().transform;
        mainCamera     = FindObjectOfType<FollowCamera>();
        playerCollider = GetComponent<Collider>();
        playerData = GetComponent<PlayerData>();
        myEffectBox = Resources.Load<PlayerEffect>("ScriptableObject/" + "PlayerEffectContainer");
        //gainSkills = new List<SkillData>();
        GameManager.Inst.GetPlayer.GetPlayerData().info.curHp = GameManager.Inst.GetPlayer.GetPlayerData().info.maxHp;

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
        if (Input.GetMouseButtonDown(0) && instSkill == false && basicAttackChek == false && castCheck == false )
        {
            basicAttackChek = true;
            indicator = Pool.ObjectInstantiate(myEffectBox.basicAttackIndicator, attackStartZone.position, transform.rotation);
            indicator.transform.SetParent(transform);
            indicator.transform.position = transform.position;

            playerAnimator.SetTrigger("isAttack");
            Invoke("Invoke_BasicAttack", 1f);

        }
        if (Input.GetKeyDown(KeyCode.Space)&& jumpCheck == false && castCheck == false)
        {
            jumpCheck = true;
            StartCoroutine(DownCheck());

            ChageState(STATE.JUMP_STATE);
            Invoke("JumpStart", 0.05f);
        }
        if(Input.GetKeyDown(KeyCode.X) && jumpCheck == false && isRun == false && instSkill == false && castCheck ==false&& gainSkill!=null)
        {
            castCheck = true;
            playerAnimator.SetTrigger("isSkill");
        }
        if(Input.GetKeyDown(KeyCode.I) == true)
        {
            GameManager.Inst.GetTrapManager.StartPreView(0);
        }
        if(Input.GetKeyDown(KeyCode.O) == true)
        {
            GameManager.Inst.GetTrapManager.TrapCollocate();
        }

    }

    private IEnumerator DownCheck()
    {
        while(true)
        {
            jumpPos = transform.position;
            yield return new WaitForSeconds(0.1f);

            if (transform.position.y < jumpPos.y)
            {
                playerRb.AddForce(Vector3.down * (jumpPower*0.3f), ForceMode.VelocityChange);
                downCheck = true;
                yield break;
            }
            downCheck = false;
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
    #region Invoke metsod
    void Invoke_BasicAttack()
    {
            basicAttackChek = false;

        if(indicator != null)
            Pool.ObjectDestroy(indicator);
    }
    #endregion

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
    #region Coroutine state machine
    protected void ChageState(STATE newState)
    {
        if (newState == curState) return;

        if(stateCoroutine!=null)
            StopCoroutine(stateCoroutine);

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
      
        playerAnimator.SetBool("isJump", true); 
        playerAnimator.SetBool("isMove", false);
        playerAnimator.SetTrigger("isJumpStart");
        playerAnimator.SetBool("isMove", false);
        playerCollider.material.dynamicFriction = 0;
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

    public abstract IEnumerator CAST_STATE();
    public abstract IEnumerator SKILL_STATE();
    #endregion

    //Skill Inst Mesod
    protected virtual void InstSkill(int index)
    {
        if (gainSkill != null)
            Pool.ObjectInstantiate(gainSkill.gameObject, transform.position, Quaternion.identity);
        else Debug.Log("you dont nave skill");

        //if (gainSkills[index] != null)
        //    Pool.ObjectInstantiate(gainSkills[index].gameObject, transform.position, Quaternion.identity);
        //else Debug.Log("you dont nave skill");
    }

    public void AddGainSkillList(SkillData getSkill)
    {

        gainSkill = getSkill;

        //if(gainSkills.Count>0)
        //{
        //    for (int i = 0; i < gainSkills.Count; i++)
        //    {
        //        if (gainSkills[i] != getSkill)
        //        {
        //            gainSkills.Add(getSkill);
        //            return;
        //        }
        //        else
        //        {
        //            return;
        //        }
        //    }
        //}
        //gainSkills.Add(getSkill);   

    }

    private void OnCollisionEnter(Collision collision)
    {

        if (downCheck&& jumpCheck && collision.contacts[0].normal.y > 0.7f&& collisionCheck==false)
        {
            downCheck = false;
            jumpEffect = Pool.ObjectInstantiate(myEffectBox.jumpEffect, transform.position, Quaternion.identity);
            jumpEffect.transform.SetParent(transform);
            collisionCheck = true;
            playerCollider.material.dynamicFriction = 1;
            playerSpeed = stopSpeed;
            playerAnimator.SetTrigger("isJumpEnd");
            playerAnimator.SetBool("isJump", false);

            Invoke("JumpEnd", 0.5f);
        }
    }
    
    public PlayerData GetPlayerData()
    {
        return playerData;
    }

    public void TransferDamage(float damamge)
    {
        GameManager.Inst.GetUiManager.GetPlayerHpBar.value = GameManager.Inst.GetPlayer.GetPlayerData().info.curHp;

        if(GameManager.Inst.GetPlayer.GetPlayerData().info.curHp<=0)
        {
            //Destroy(this.gameObject);
        }
    }



}

