using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ScreenShotMaker is made by SkorpionX. The source can be found in https://forum.unity.com/threads/export-full-scene-as-image.496232/#post-4299523 .
/// </summary>
public class ScreenShotMaker : MonoBehaviour
{
    [HideInInspector] public static ScreenShotMaker instance;
    
    public GameObject target;
 
    private RenderTexture renderTexture;
    private Camera renderCamera;
    private Vector4 bounds;
 
    private int resolution = 160;
    private float cameraDistance = -2.0f;

    private void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(this);
    }

    public void SaveScene()
    {
        gameObject.AddComponent(typeof(Camera));
 
        renderCamera = GetComponent<Camera>();
        renderCamera.enabled = true;
        renderCamera.cameraType = CameraType.Game;
        renderCamera.forceIntoRenderTexture = true;
        renderCamera.orthographic = true;
        renderCamera.orthographicSize = 5;
        renderCamera.aspect = 1.0f;
        renderCamera.targetDisplay = 2;
        renderCamera.backgroundColor = new Color((float)36/256, (float)36/256, (float)36/256);

        renderTexture = new RenderTexture(resolution, resolution, 24);
 
        renderCamera.targetTexture = renderTexture;
 
        bounds = new Vector4();
 
        // Debug.Log("Initialized successfully!");
        // Debug.Log("Computing level boundaries...");
 
        if (target != null)
        {
            Bounds boundaries;
 
            if (target.GetComponentInChildren<Renderer>() != null)
            {
                boundaries = target.GetComponentInChildren<Renderer>().bounds;
            }
            else if(target.GetComponentInChildren<Collider2D>() != null)
            {
                boundaries = target.GetComponentInChildren<Collider2D>().bounds;
            }
            else
            {
                Debug.Log("Unfortunately no boundaries could be found :/");
 
                return;
            }
         
            bounds.w = boundaries.min.x;
            bounds.x = boundaries.max.x;
            bounds.y = boundaries.min.y;
            bounds.z = boundaries.max.y;
        }
        else
        {
            object[] gameObjects = FindObjectsOfType(typeof(GameObject));
 
            foreach (object gameObj in gameObjects)
            {
                GameObject levelElement = (GameObject)gameObj;
                Bounds boundaries = new Bounds();
 
                if (levelElement.GetComponentInChildren<Renderer>() != null)
                {
                    boundaries = levelElement.GetComponentInChildren<Renderer>().bounds;
                }
                else if (levelElement.GetComponentInChildren<Collider2D>() != null)
                {
                    boundaries = levelElement.GetComponentInChildren<Collider2D>().bounds;
                }
                else
                {
                    continue;
                }
 
                if (boundaries != null)
                {
                    bounds.w = Mathf.Min(bounds.w, boundaries.min.x);
                    bounds.x = Mathf.Max(bounds.x, boundaries.max.x);
                    bounds.y = Mathf.Min(bounds.y, boundaries.min.y);
                    bounds.z = Mathf.Max(bounds.z, boundaries.max.y);
                }
            }
        }
     
        // Debug.Log("Boundaries computed successfuly! The computed boundaries are " + bounds);
        // Debug.Log("Computing target image resolution and final setup...");
 
        int xRes = Mathf.RoundToInt(resolution * ((bounds.x - bounds.w) / (renderCamera.aspect * renderCamera.orthographicSize * 2 * renderCamera.aspect)));
        int yRes = Mathf.RoundToInt(resolution * ((bounds.z - bounds.y) / (renderCamera.aspect * renderCamera.orthographicSize * 2 / renderCamera.aspect)));
 
        Texture2D virtualPhoto = new Texture2D(xRes, yRes, TextureFormat.RGB24, false);
        RenderTexture.active = renderTexture;
 
        // Debug.Log("Success! Everything seems ready to render!");
 
        for (float i = bounds.w, xPos = 0; i < bounds.x; i += renderCamera.aspect * renderCamera.orthographicSize * 2, xPos++)
        {
            for (float j = bounds.y, yPos = 0; j < bounds.z; j += renderCamera.aspect * renderCamera.orthographicSize * 2, yPos++)
            {
                gameObject.transform.position = new Vector3(i + renderCamera.aspect * renderCamera.orthographicSize, j + renderCamera.aspect * renderCamera.orthographicSize, cameraDistance);
 
                renderCamera.Render();
 
                virtualPhoto.ReadPixels(new Rect(0, 0, resolution, resolution), (int)xPos * resolution, (int)yPos * resolution);
 
                // Debug.Log("Rendered and copied chunk " + (xPos + 1) + ":" + (yPos + 1));
            }
        }
 
        Debug.Log("All chunks rendered! Some final adjustments and picture should be saved!");
 
        RenderTexture.active = null;
        renderCamera.targetTexture = null;
     
        byte[] bytes = virtualPhoto.EncodeToPNG();
        JsBridge.SendImageToJs(bytes);

        // Save dungeon to the application path!
        // string file_path = Application.persistentDataPath + "/" + "myDungeon.png";
        // Debug.Log(file_path);
        // System.IO.File.WriteAllBytes(file_path, bytes);
    }
}