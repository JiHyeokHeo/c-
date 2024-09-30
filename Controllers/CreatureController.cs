using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureController : BaseController
{
    int _hp;
    public int HP { set { _hp = value; } get { return _hp; } }
    void Start()
    {
        
    }

    protected override void UpdateController()
    {
        base.UpdateController();
    }
}
