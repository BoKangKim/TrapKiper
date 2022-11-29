using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData
{


}

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
    [SerializeField] Transform cameraArmTr = null;

    //Attack & Skill Set
    [Header("Attack Info")]
    [SerializeField] Transform attackStartZone    = null;
    [SerializeField] GameObject basicAttackPrefab = null;
    [SerializeField] GameObject skillAttackPrefab = null;

    //Player component
    Animator playerAnimator            = null;
    Transform playerTr                 = null;
    Rigidbody playerRb                 = null;
    AnimationEventReciver EventReciver = null;

    //Player Speed Value
    [Header("Player Speed")]
    [SerializeField] float playerSpeed  = 30f;
    [SerializeField] float runSpeed     = 50f;
    [SerializeField] float jumpSpeed    = 20f;
    [SerializeField] float JumpRunSpeed = 40f;
    [SerializeField] float stopSpeed    = 5f;
    float originSpeed                   = 0;

    //Dir Vector
    Vector3 lookForward = Vector3.zero;
    Vector3 lookRight   = Vector3.zero;
    Vector3 moveDir     = Vector3.zero;

    //Check Value
    bool isRun      = false;
    bool jumpCheck  = false;
    bool skillCheck = false;

    //axis Value
    float mouseX = 0;
    float mouseY = 0;
    float axisX  = 0;
    float axisZ  = 0;

    //player animation learp Value
    float fixedAxisZ = 0f;
    float fixedAxisX = 0f;

    //Current Animation State
    STATE curState;
    //Current Coroutine
    Coroutine stateCoroutine;
    Coroutine moveCoroutine;

    //Awake&&Start method
    #region Awake&&Start method
    protected virtual void Init()
    {
        playerAnimator   = GetComponent<Animator>();
        playerTr         = GetComponent<Transform>();
        playerRb         = GetComponent<Rigidbody>();
        EventReciver     = GetComponent<AnimationEventReciver>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;
    }
    protected virtual void StartImplemented()
    {
        EventReciver.callJumpEvent = JumpStart;
        EventReciver.callJunpEndEvent = JumpEnd;
        EventReciver.callAttackEvent = BasicAttack;
        EventReciver.callSkillEvent  = SkillAttack;

        originSpeed = playerSpeed;
        ChageState(STATE.MOVE_STATE);
    }
    #endregion
    //Update method
    #region Update method
    protected virtual void UpdateImplemented()
    {
        GetAxisValue();

        FollowCamera();

        InputKey();

        PlayerRotation();
    }
    void GetAxisValue()
    {
        //Mouse Axis
        mouseX += Input.GetAxis("Mouse X")*2;
        mouseY += Input.GetAxis("Mouse Y")*2;

        //Keyboard Axis
        axisX = Input.GetAxis("Horizontal");
        axisZ = Input.GetAxis("Vertical");
    }
    void InputKey()
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
        if (Input.GetKeyDown(KeyCode.Space) && jumpCheck == false)
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
    void FollowCamera()
    {
        //FollowCam
        cameraArmTr.position = playerTr.transform.position;

        if (-mouseY <= -40f)
        {
            mouseY = 39.5f;
            return;
        }
        else if (-mouseY >= 30f)
        {
            mouseY = -29.5f;
        }

        //Camera Rotation
        cameraArmTr.rotation = Quaternion.Euler(-mouseY, mouseX, 0);
        //Player Rtation
    }
    void PlayerRotation()
    {
        playerTr.rotation = Quaternion.Euler(0, mouseX, 0);
    }
    #endregion
    //delgate method
    #region delgate method
    void BasicAttack()
    {
        Instantiate(basicAttackPrefab, attackStartZone.position, transform.rotation);
    }
    void SkillAttack()
    {
       StartCoroutine(SKILL_STATE());
    }
    void JumpStart()
    {
        playerRb.AddForce(Vector3.up * 7f, ForceMode.VelocityChange);
    }
    void JumpEnd()
    {
        ChageState(STATE.MOVE_STATE);

        jumpCheck = false;
        playerSpeed=originSpeed;
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
                playerSpeed = Mathf.Lerp(playerSpeed, runSpeed, Time.deltaTime*2);
                speed = Mathf.Lerp(speed, 1, Time.deltaTime);

                playerAnimator.SetBool("isMove", false);
                playerAnimator.SetBool("isRun", true);
                playerAnimator.SetFloat("speed", speed);
            }
            else
            {
                playerSpeed = Mathf.Lerp(playerSpeed, originSpeed, Time.deltaTime*2);
                speed = Mathf.Lerp(speed, 0, Time.deltaTime);

                playerAnimator.SetBool("isMove", true);
                playerAnimator.SetBool("isRun", false);
                playerAnimator.SetFloat("speed", speed);
            }

            if (0.3f >= Mathf.Abs(fixedAxisZ) && 0.3f >= Mathf.Abs(fixedAxisX))
                playerSpeed = playerSpeed / 2;
            else
                playerSpeed = originSpeed;

            lookForward = new Vector3(cameraArmTr.transform.forward.x, 0, cameraArmTr.transform.forward.z);
            lookRight = new Vector3(cameraArmTr.transform.right.x, 0, cameraArmTr.transform.right.z);

            if (axisX != 0 || axisZ != 0)
            {
                moveDir = lookForward * axisZ + lookRight * axisX;
                playerRb.MovePosition(this.gameObject.transform.position+moveDir.normalized * playerSpeed * Time.deltaTime);
            }
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
            fixedAxisZ = Mathf.Lerp(fixedAxisZ, axisZ, Time.deltaTime * 6);
            playerAnimator.SetFloat("axisZ", fixedAxisZ);
            fixedAxisX = Mathf.Lerp(fixedAxisX, axisX, Time.deltaTime * 6);
            playerAnimator.SetFloat("axisX", fixedAxisX);

            if (isRun)
            {
                playerSpeed = Mathf.Lerp(playerSpeed, JumpRunSpeed, Time.deltaTime * 2);
                speed = Mathf.Lerp(speed, 0, Time.deltaTime);

                playerAnimator.SetFloat("speed", speed);
            }
           
            lookForward = new Vector3(cameraArmTr.transform.forward.x, 0, cameraArmTr.transform.forward.z);
            lookRight = new Vector3(cameraArmTr.transform.right.x, 0, cameraArmTr.transform.right.z);

            if (axisX != 0 || axisZ != 0)
            {
                moveDir = lookForward * axisZ + lookRight * axisX;
                playerRb.MovePosition(this.gameObject.transform.position + moveDir.normalized * playerSpeed * Time.deltaTime);
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
