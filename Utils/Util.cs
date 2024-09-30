using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Util  
{
    public static T AddOrGetComponent<T>(GameObject go) where T : UnityEngine.Component
    {
        T component = go.GetComponent<T>();
        if (component == null)
            component = go.AddComponent<T>();

        return component;
    }

    public static string EnumToString<T>(T value) where T : Enum
    {
        return value.ToString();
    }

    public static T StringToEnum<T>(string name) where T : Enum
    {
        return (T)Enum.Parse(typeof(T), name);
    }



}
