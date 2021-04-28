using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class JsBridge : MonoBehaviour
{
    #if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static void extern CreateImage(byte[] bytes)
    #endif
    
    public void SendImageToJs(byte[] bytes)
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
        CreateImage(bytes);
        #endif
    }
}
