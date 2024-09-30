using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.EventSystems;

public class GemController : BaseController
{
    Coroutine _swapCoroutine;
    GemInputController _inputController;

    public override void Init()
    {
        BindGemControllEvent(gameObject, ClickEvent, Define.GemControllEvent.Click); 
    }

    protected override void UpdateController()
    {
        base.UpdateController();
    }

    public void DragEvent(PointerEventData data)
    {
        Vector2 currentPos = data.position;
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(currentPos);
        transform.position = mousePos;
    }

    public void ClickEvent(PointerEventData data)
    {
        if (Managers.Map.SetPickObject(gameObject))
        {
            // 위치값을 사전에 저장 후 스왑
            Swap();
        }
    }

    public void Explode()
    {
        Managers.Resource.DestroyAfterTime(gameObject, 2.0f);
    }


    public void Swap()
    {
        StartCoroutine("StartSwap");
    }

    IEnumerator StartSwap()
    {
        Managers.Map.InitSwap();

        while (true)
        {
            if (Managers.Map._swapMoveCoroutine == null)
            {
                StartCoroutine("StartExplodeCheck");
                yield break;       
            }
            yield return null;
        }
    }

    IEnumerator StartExplodeCheck()
    {
        Managers.Map.InitExplodeCheck();
        yield return null;
    }

    protected void BindGemControllEvent(GameObject go, Action<PointerEventData> action, Define.GemControllEvent type = Define.GemControllEvent.Click)
    {
        GemInputController gemController = Util.AddOrGetComponent<GemInputController>(go);

        switch (type)
        {
            case Define.GemControllEvent.Click:
                gemController.OnClickHandler -= action;
                gemController.OnClickHandler += action;
                break;
            case Define.GemControllEvent.Drag:
                gemController.OnDragHandler -= action;
                gemController.OnDragHandler += action;
                break;
        }
    }

}
