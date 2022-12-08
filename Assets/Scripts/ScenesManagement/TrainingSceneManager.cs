using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrainingSceneManager : MonoBehaviour
{
    public Button StopTrainingBtn;
    public Button ExitBtn;
    public GameObject SavePanel;
    
    // Start is called before the first frame update
    void Start()
    {
        StopTrainingBtn.onClick.AddListener(StopTrainingAndSave);
        ExitBtn.onClick.AddListener(() => GameObject.Find("SceneChanger").GetComponent<SceneChanger>().MenuScene());
    }

    private void StopTrainingAndSave()
    {
        Time.timeScale = 0;
        SavePanel.SetActive(true);
    }
}
