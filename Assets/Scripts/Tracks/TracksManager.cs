using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TracksManager : MonoBehaviour
{
    public List<GameObject> Checkpoints;

    // Start is called before the first frame update
    void Start()
    {
        GameObject obj = GameObject.Find("Checkpoints");
        foreach (Transform child in obj.transform)
        {
            Debug.Log(child.name);
            Checkpoints.Add(child.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
