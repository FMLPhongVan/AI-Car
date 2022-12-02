using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Layer
{
    public List<Neuron> Neurons;
    private int _layerId;

    public object LNeurons { get; internal set; }

    public Layer(int layerId, int neuronCount)
    {
        Neurons = new List<Neuron>(neuronCount);
        _layerId = layerId;
    }

    public double[,] GetWeights(NeuralNetwork network)
    {
        Layer nextLayer = network.Layers[_layerId + 1];
        double[,] weights = new double[Neurons.Count, nextLayer.Neurons.Count];
        for (int i = 0; i < Neurons.Count; ++i)
        {
            for (int j = 0; j < nextLayer.Neurons.Count; ++j)
                weights[i, j] = nextLayer.Neurons[j].Edges[i].Weight;
        }

        return weights;
    }
}
