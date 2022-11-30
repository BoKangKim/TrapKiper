using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : BasePlayer
{
    private void Awake()
    {
        Init();
    }
    private void Start()
    {
        StartImplemented();
    }

    

    protected override void Init()
    {
        base.Init();
    }
    protected override void StartImplemented()
    {
        base.StartImplemented();
    }
    protected override void UpdateImplemented()
    {
        base.UpdateImplemented();
    }
    public override IEnumerator SKILL_STATE()
    {
        return base.SKILL_STATE();

    }
}   
