using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static System.Net.Mime.MediaTypeNames;

public class SceneChanger : MonoBehaviour
{
    public void MenuScene()
    {
        SceneManager.LoadScene("MenuScene");
    }

    public void TestTerrainA()
    {
        SceneManager.LoadScene("TestTerrain_A");
    }

    public void TestTerrainB()
    {
        SceneManager.LoadScene("TestTerrain_B");
    }

    public void SelectTerrainScene()
    {
        SceneManager.LoadScene("SelectTerrainScene");
    }

    public void TrainingScene()
    {
        SceneManager.LoadScene("TrainingScene");
    }
}