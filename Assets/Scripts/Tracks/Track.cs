using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Track : MonoBehaviour
{
    public List<GameObject> Checkpoints;

    // Start is called before the first frame update
    void Awake()
    {
        Component[] tmp = gameObject.GetComponentsInChildren(typeof(Transform));
        for (int i = 0; i < tmp.Length; i++)
        {
            if (tmp[i].gameObject.tag.Equals("Checkpoint"))
                Checkpoints.Add(tmp[i].gameObject);
        }
    }
}
