using UnityEngine;
using System.Collections;
using System;
using System.Reflection;
using HarmonyLib;
using System.Collections.Generic;

namespace Utils{
    class MiscUtils{
        public static bool IsOffBounds(GameObject uiObject, float xMargin, float yMargin){
        Vector3 uiObjectPosition = uiObject.transform.position;
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(uiObjectPosition);
        return screenPosition.x < 0 - Screen.width/100 * xMargin|| screenPosition.x > Screen.width + Screen.width/100 * xMargin|| screenPosition.y < 0 - Screen.height/100 * yMargin || screenPosition.y > Screen.height + Screen.height/100 * yMargin;
    }
    public static string GetGameObjectPath(Transform obj)
    {
     string path = obj.name;
     while (obj.parent != null)
     {
         obj = obj.parent;
         path = obj.name + "/" + path;
     }
     return path;
    }
    public static int ReverseBits(int num, int bitCount)
    {
        int result = 0;
        for (int i = 0; i < bitCount; i++)
        {
            result <<= 1;
            result |= (num & 1);
            num >>= 1;
        }
        return result;
    }
    
    }

public class PauseScript : MonoBehaviour
{
    public static PauseScript Instance;
    private PauseScript() {}
    public static PauseScript GetInstance()
    {
        if (Instance == null)
        {
            GameObject go = new GameObject("PauseScript");
            Instance = go.AddComponent<PauseScript>();
            DontDestroyOnLoad(go);
        }
        return Instance;
    }
    private bool isPaused = false;
    public void Update()
    {
        if(!Input.GetKeyDown(KeyCode.Space)) return;
        if (!isPaused)
        {
            Time.timeScale = 0f;
            isPaused = true;
        }
        else
        {
            Time.timeScale = 1f;
            isPaused = false;
        }
    }
}
public class SoundpacksDebugger : MonoBehaviour {
    public Type targetType = typeof(SoundpackUtils);
    public List<object> objects;
    public static SoundpacksDebugger Instance;
    public static SoundpacksDebugger GetInstance()
    {
        if (Instance == null)
        {
            GameObject go = new GameObject("PauseScript");
            Instance = go.AddComponent<SoundpacksDebugger>();
            DontDestroyOnLoad(go);
        }
        return Instance;
    }
    public void Start()
    {
        AddFieldsAsComponents();
    }

    public void AddFieldsAsComponents()
    {
        // Get all the public fields from the target type
        objects = [];
        
        FieldInfo[] fields = targetType.GetFields(BindingFlags.Public | BindingFlags.Static);

        foreach (FieldInfo field in fields)
        {
            objects.Add(field.GetValue(null));
        }

        
    }
    public void Update()
    {
        if(!Input.GetKeyDown(KeyCode.Space)) return;
        AddFieldsAsComponents();
    }
}


public class Pair<T, U> {
    public Pair() {
    }

    public Pair(T key, U value) {
        this.Key = key;
        this.Value = value;
    }
    public void SetValues(T key, U value){
        this.Key = key;
        this.Value = value;
    }
    public T Key { get; set; }
    public U Value { get; set; }
};
}