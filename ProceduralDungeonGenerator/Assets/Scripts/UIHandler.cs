using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    public InputField numberOfRoomsMin_IF;
    public InputField numberOfRoomsMax_IF;
    public InputField avgRoomSize_IF;
    public InputField corridorWidth_IF;
    public InputField dungeonOldnessLevel_IF;

    public Toggle use_entrance_Toggle;

    public GameObject dungeonSettings;

    public Text error_text;

    public DungeonGeneratorData GeneratorData;
    public DungeonGenerator Generator;

    private Vector2Int nRoomsRange = new Vector2Int(2, 100);
    private Vector2Int aRoomSRange = new Vector2Int(6, 50);
    private Vector2Int cWidthRange = new Vector2Int(3, 5);
    private Vector2Int oldLvlRange = new Vector2Int(0, 10);

    private bool dungeonSettingsEnabled = true;
    private bool use_entrance = true;

    private void Awake()
    {
        numberOfRoomsMin_IF.text = GeneratorData.numberOfRoomsMin.ToString();
        numberOfRoomsMax_IF.text = GeneratorData.numberOfRoomsMax.ToString();
        avgRoomSize_IF.text = GeneratorData.averageRoomSize.ToString();
        corridorWidth_IF.text = GeneratorData.corridorWidth.ToString();
        dungeonOldnessLevel_IF.text = GeneratorData.oldnessLevel.ToString();
    }

    private void FixedUpdate()
    {
        if (use_entrance != use_entrance_Toggle.isOn)
        {
            use_entrance = use_entrance_Toggle.isOn;
            DungeonGenerator.instance.SetDungeonEntrance(use_entrance);
        }
    }

    public void HideButton()
    {
        if (dungeonSettingsEnabled)
        {
            dungeonSettingsEnabled = false;
        }
        else
        {
            dungeonSettingsEnabled = true;
        }
        dungeonSettings.SetActive(dungeonSettingsEnabled);
    }

    public void ExportButton()
    {
        ScreenShotMaker.instance.SaveScene();
    }

    public void Generate_Button()
    {
        if (numberOfRoomsMin_IF.text == "" || numberOfRoomsMax_IF.text == "" || avgRoomSize_IF.text == "" ||
            corridorWidth_IF.text == "" || dungeonOldnessLevel_IF.text == "")
        {
            SetErrorText("ERROR: Some fields are empty!!");
            return;
        }

        if (InputChecker(StringToInt(numberOfRoomsMin_IF.text), StringToInt(numberOfRoomsMax_IF.text),
            StringToInt(avgRoomSize_IF.text), StringToInt(corridorWidth_IF.text),
            StringToInt(dungeonOldnessLevel_IF.text)))
        {
            error_text.gameObject.SetActive(false);
            Generator.GenerateDungeon();
        }
    }

    public void ChangeBgColorButton()
    {
        Color color = EventSystem.current.currentSelectedGameObject.GetComponent<Image>().color;
        Camera.main.backgroundColor = color;
    }

    private bool InputChecker(int nRoomsMin, int nRoomsMax, int aRoomS, int cWidth, int oldLvl)
    {
        if (nRoomsMin == -1 || nRoomsMax == -1 || aRoomS == -1 || cWidth == -1 || oldLvl == -1) return false;
        
        if (nRoomsMin >= nRoomsRange.x && nRoomsMin <= nRoomsRange.y && nRoomsMax >= nRoomsRange.x && nRoomsMax <= nRoomsRange.y)
        {
            if (nRoomsMin <= nRoomsMax)
            {
                GeneratorData.numberOfRoomsMin = nRoomsMin;
                GeneratorData.numberOfRoomsMax = nRoomsMax;
            }
            else
            {
                SetErrorText("ERROR: Number of Rooms min cannot be higher than number rooms max!!");
                return false; 
            }
        }
        else
        {
            SetErrorText("ERROR: Number of Rooms is not in the range!!");
            return false; 
        }

        if (aRoomS >= aRoomSRange.x && aRoomS <= aRoomSRange.y)
        {
            GeneratorData.averageRoomSize = aRoomS;
        }
        else
        {
            SetErrorText("ERROR: Avg. room size is not in the range!!");
            return false; 
        }

        if (cWidth >= cWidthRange.x && cWidth <= cWidthRange.y)
        {
            GeneratorData.corridorWidth = cWidth;
        }
        else
        {
            SetErrorText("ERROR: Corridor width is not in the range!!");
            return false; 
        }

        if (oldLvl >= oldLvlRange.x && oldLvl <= oldLvlRange.y)
        {
            GeneratorData.oldnessLevel = oldLvl;
        }
        else
        {
            SetErrorText("ERROR: Oldness Level is not in the range!!");
            return false; 
        }

        return true;
    }

    private void SetErrorText(string message)
    {
        error_text.text = message;
        error_text.gameObject.SetActive(true);
    }

    private int StringToInt(string s)
    {
        int i;
        if (Int32.TryParse(s, out i))
        {
            return i;
        }
        
        SetErrorText("ERROR: Some inputs are not integers!!");
        return -1;
    }
}
