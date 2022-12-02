using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Edge
{
    public double Weight = 0;
    public Edge()
    {
        Weight = UnityEngine.Random.Range(-1f, 1f);
    }
}
