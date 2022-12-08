using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseTrap : MonoBehaviour
{
    [SerializeField] protected GameObject activeEff = null;
    private bool isCollocate = false;

    private void Awake()
    {
        Camera camera = Camera.main;
    }

    private void Update()
    {
        if(isCollocate == false)
        {
            PreView();
            return;
        }

        if (ActiveCondition() == false)
        {
            return;
        }

        TrapLogic();
    }

    protected abstract void TrapLogic();
    protected abstract bool ActiveCondition();

    public void PreView()
    {
        RaycastHit hitinfo;
        Vector3 startVec = GameManager.Inst.GetPlayer.transform.position + (Vector3.up * 1.5f) + (GameManager.Inst.GetPlayer.transform.forward * 2f);
        Vector3 y = new Vector3(0,-5f,0f);
        Vector3 dirVec = ((startVec + y) - startVec).normalized;

        Debug.DrawRay(startVec, dirVec * 5,Color.red);

        if(Physics.Raycast(startVec, dirVec, out hitinfo,5f))
        {
            this.transform.position = hitinfo.point + (Vector3.up * 0.2f);
            this.gameObject.transform.rotation = Quaternion.LookRotation(hitinfo.normal);
        }
    }

    public virtual void Collocate()
    {
        this.gameObject.transform.SetParent(GameManager.Inst.GetTrapManager.transform,true);
        isCollocate = true;
    }
}
