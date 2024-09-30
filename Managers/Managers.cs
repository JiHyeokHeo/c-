using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Managers : MonoBehaviour
{
    static Managers _instnace;

    public static Managers Instance { get { Init();  return _instnace; } }

    #region Contents

    #endregion

    #region Core
    InputManager _input = new InputManager();
    ResourceManager _resource = new ResourceManager();
    GameManager _game = new GameManager();
    #endregion 
    public static InputManager Input { get { return Instance._input; } }
    public static MapManager Map
    {
        get
        {
            if (_instnace.GetComponent<MapManager>() == null)
                _instnace.AddComponent<MapManager>();

            return _instnace.GetComponent<MapManager>();
        }
    }
    public static GameManager Game { get { return Instance._game; } }
    public static ResourceManager Resource { get { return Instance._resource; } }

    void Start()
    {
        Init();
    }

    static void Init()
    {
        if (_instnace == null)
        {
            GameObject obj = GameObject.Find("@Managers");
            if (obj == null)
            {
                obj = new GameObject { name = "@Managers" };
                obj.AddComponent<Managers>();
            }

            DontDestroyOnLoad(obj);
            _instnace = obj.GetComponent<Managers>();
        }
    }

    public void Update()
    {
        
    }

    public static void Clear()
    {
        Input.Clear();
    }
}
