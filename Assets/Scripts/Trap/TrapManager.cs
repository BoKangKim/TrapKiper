using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapManager : MonoBehaviour
{
    [SerializeField] private GameObject[] traps = null;
    private Vector3 forward = Vector3.zero;
    private BaseTrap nowObj = null;
    private int showIndex = -1;

    private void Awake()
    {
        Pool.ListCaching(traps);
    }

    private void Start()
    {
        forward = GameManager.Inst.GetPlayer.transform.forward.normalized * 5f;
    }

    public void StartPreView(int index)
    {
        if (showIndex == index)
        {
            Pool.ObjectDestroy(nowObj.gameObject);
            nowObj = null;
            showIndex = -1;
            return;
        }
        else
        {
            if (nowObj != null)
            {
                Pool.ObjectDestroy(nowObj.gameObject);
                nowObj = null;
            }

            showIndex = index;

            if (traps.Length <= index)
            {
                return;
            }
            
            if(Pool.ObjectInstantiate(traps[index], GameManager.Inst.GetPlayer.transform, false).TryGetComponent<BaseTrap>(out nowObj))
            {
                nowObj.transform.localPosition = forward;
                nowObj.transform.rotation = Quaternion.identity;
            }
        }
    }


    public void TrapCollocate()
    {
        if (nowObj == null)
        {
            return;
        }

        nowObj.Collocate();
        nowObj = null;
        showIndex = -1;
    }
}
