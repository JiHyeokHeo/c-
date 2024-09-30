using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : CreatureController
{
    int _exp;
    public int EXP { set { _exp = value; } get { return _exp; } }

    void Start()
    {
        
    }

    // Update is called once per frame
    protected override void UpdateController()
    {
        base.UpdateController();
    }
}
