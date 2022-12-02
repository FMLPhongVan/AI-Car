using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Neuron
{
    public List<Edge> Edges;
    public double Value;
    public double Delta;
    public double Bias;

    public Neuron()
    {
        Bias = UnityEngine.Random.Range(-1f, 1f);
        Edges = new List<Edge>();
    }

    public Neuron(float bias)
    {
        Bias = bias;
        Edges = new List<Edge>();
    }
}
