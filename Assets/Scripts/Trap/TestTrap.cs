using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTrap : BaseTrap
{
    private bool wait = false;
    private float time = 0f;
    protected override bool ActiveCondition()
    {
        time += Time.deltaTime;
        
        if(time > 5f)
        {
            wait = true;
        }

        return wait;
    }

    protected override void TrapLogic()
    {
        Pool.ObjectInstantiate(activeEff,this.transform.position,Quaternion.identity);
        Pool.ObjectDestroy(gameObject);
    }
    
}
