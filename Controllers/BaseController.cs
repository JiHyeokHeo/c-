using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class BaseController : MonoBehaviour
{
    void Start()
    {
        Init();
    }

    public virtual void Init()
    {

    }

    void Update()
    {
        UpdateController();
    }

    protected virtual void UpdateController()
    {

    }

  

}
