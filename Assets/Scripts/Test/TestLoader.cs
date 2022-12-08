using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLoader : MonoBehaviour
{
    public string Model = "";
    public GameObject TestCarVariant;
    public Vector3 StartPosition;

    void Awake()
    {
        Time.timeScale = 2f;
        GameObject car = Instantiate(TestCarVariant, StartPosition, Quaternion.identity);
        car.GetComponent<TesterAIController>().Network = new NeuralNetwork("./records/" + Model);
    }
}
