using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class JsBridge : MonoBehaviour
{
    #if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void CreateImage(byte[] bytes, int size, string fileName);
    #endif
    
    public static void SendImageToJs(byte[] bytes)
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
        CreateImage(bytes, bytes.Length, "myDungeon.png");
        #endif
    }
}
