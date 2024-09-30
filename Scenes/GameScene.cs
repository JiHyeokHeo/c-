using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameScene : BaseScene
{

    protected  override void Init()
    {
        base.Init();
        SceneType = Define.SceneType.Play;

        GameObject obj = Managers.Resource.Instantiate("UI/GamePlayUI/PlayUI");
        obj.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
        obj.GetComponent<Canvas>().worldCamera = Camera.main;

        Component[] t =  obj.GetComponentsInChildren<Image>();
        Transform tr = obj.GetComponent<Transform>();
        Managers.Map.Init(obj);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override void Clear()
    {
    
    }
}
