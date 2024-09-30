using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{ 
    public enum PlayerState
    {
        Die,
        Play,
        End,
    }

    public enum SceneType
    {
        Unknown,
        Play,
        Lobby,
    }
    public enum MouseEvent
    {
        Press,
        Click,
        End,
    }

    public enum GemControllEvent
    {
        Click,
        Drag,
        End,
    }

    public enum MapData
    {
        None,
        Stage1,
        Stage2,
    }

    public enum GemType
    {
        Start = 0,
        Amethyst,
        Diamond,
        Emerald,
        Garnet,
        Ruby,
        Sapphire,
        Topaz,
        Tourmaline,
        End,
    }
}
