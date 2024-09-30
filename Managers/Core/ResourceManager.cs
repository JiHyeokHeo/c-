using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor;
using UnityEngine;

public class ResourceManager 
{
    public T Load<T>(string path) where T : Object
    {
        if (typeof(T) == typeof(GameObject))
        {
            string name = path;
            int index = name.LastIndexOf('/');
            if (index > 0)
                name = name.Substring(index + 1);
        }

        return Resources.Load<T>(path);
    }

    public GameObject Instantiate(string path, Transform parent = null)
    {
        GameObject origin = Load<GameObject>($"Prefabs/{path}");
        if (origin == null)
        {
            Debug.Log($" 프리팹 로드 실패 : {path}");
            return null;
        }

        GameObject obj = Object.Instantiate(origin, parent);
        int index = obj.name.IndexOf("(Clone)");
        if (index > 0)
            obj.name = obj.name.Substring(0, index);

        return obj;
    }

    public void Destroy(GameObject obj)
    {
        DestroyAfterTime(obj, 0);
    }

    public void DestroyAfterTime(GameObject obj, float time)
    {
        if (obj == null)
            return;
        obj.SetActive(false);

        Object.Destroy(obj, time);
    }
}
