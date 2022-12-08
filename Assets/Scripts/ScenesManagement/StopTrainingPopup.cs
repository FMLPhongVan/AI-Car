using UnityEngine;
using UnityEngine.UI;

public class StopTrainingPopup : MonoBehaviour
{
    public Button SaveBtn;
    public InputField InputFieldEditor;

    void Start()
    {
        InputFieldEditor.text = "Model" + System.DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss");
        SaveBtn.onClick.AddListener(SaveModel);
    }

    public void SaveModel()
    {
        string name = InputFieldEditor.text;
        // if name if emtpy, name = "Model + Date"
        if (name.Equals(""))
            name = "Model" + System.DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss");

        GameObject.Find("Academy").GetComponent<Academy>().SaveModel(name + ".txt");
        GameObject.Find("SceneChanger").GetComponent<SceneChanger>().MenuScene();
    }
}
