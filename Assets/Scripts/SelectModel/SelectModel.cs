using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class SelectModel : MonoBehaviour
{
    private FileInfo[] info;

    public TMP_Dropdown TMPDropdown;
    public TMP_Text text;

    public void Start()
    {
        DirectoryInfo dir = new DirectoryInfo("./records/");
        FileInfo[] info = dir.GetFiles("*.*");
        TMPDropdown.options.Clear();
        foreach (FileInfo f in info)
        {
            TMPDropdown.options.Add(new TMP_Dropdown.OptionData() { text = f.Name });
        }
    }
    public void Update()
    {
        PlayerPrefs.SetString("model", TMPDropdown.options[TMPDropdown.value].text);
    }
}

