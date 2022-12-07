using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

[Serializable]
public class NeuralNetwork
{
    public List<Layer> Layers;
    public int[] LayerStructure;
    public float Fitness;
    public double FitnessRatio;

    public NeuralNetwork(int[] layerStructure)
    {
        if (layerStructure.Length < 2) return;
        LayerStructure = layerStructure;
        Layers = new List<Layer>();
        Fitness = 0f;
        FitnessRatio = 0f;

        for (int i = 0; i < layerStructure.Length; i++)
        {
            Layer layer = new (i, LayerStructure[i]);
            Layers.Add(layer);

            for (int j = 0; j < LayerStructure[i]; j++)
                layer.Neurons.Add(new Neuron());
            

            foreach (Neuron neuron in layer.Neurons)
            {
                if (i == 0)
                    neuron.Bias = 0f;
                else
                {
                    for (int k = 0; k < LayerStructure[i - 1]; k++)
                        neuron.Edges.Add(new Edge());
                }
            }
        }
    }

    public NeuralNetwork(String fileName)
    {
        string[] lines = System.IO.File.ReadAllLines(fileName);
        string[] structure = lines[0].Split(new char[] { ',' });
        int[] numStrucutre = new int[structure.Length];
        for (int i = 0; i < structure.Length; i++)
        {
            numStrucutre[i] = System.Convert.ToInt32(structure[i]);
        }

        // Make a Neural Net with those specifications
        NeuralNetwork NN = new (numStrucutre);

        // Get the encoded value
        string[] element = lines[1].Split(", ");

        List<double> encoded = new List<double>();
        for (int i = 0; i < element.Length; i++)
        {
            Debug.Log(element[i]);
            float num = (float)(Convert.ToDouble(element[i], CultureInfo.InvariantCulture));
            if (num < -1f) num = -1f;
            if (num > 1f) num = 1f;
            encoded.Add(num);
            Debug.Log(num);
        }

        //Update our NN with the value
        NN.Decode(encoded);
        Layers = NN.Layers;
        LayerStructure = NN.LayerStructure;
        Fitness = 0f;
        FitnessRatio = 0f;
    }

    public List<double> Encode()
    {
        List<double> encoded = new ();

        foreach (Layer layer in Layers)
        {
            foreach (Neuron neuron in layer.Neurons)
            {
                encoded.Add(neuron.Bias);
                foreach (Edge edge in neuron.Edges)
                    encoded.Add(edge.Weight);
            }
        }

        return encoded;
    }

    public void Decode(List<double> encoded)
    {
        int index = 0;
        foreach (Layer layer in Layers)
        {
            foreach (Neuron neuron in layer.Neurons)
            {
                neuron.Bias = encoded[index];
                index++;
                foreach (Edge edge in neuron.Edges)
                {
                    edge.Weight = encoded[index];
                    index++;
                }
            }
        }
    }

    public double[] FeedForward(List<double> inputs)
    {
        if (inputs.Count != Layers[0].Neurons.Count) return null;
        
        for (int i = 0; i < Layers.Count; ++i)
        {
            Layer layer = Layers[i];
            for (int j = 0; j < layer.Neurons.Count; ++j)
            {
                Neuron neuron = layer.Neurons[j];
                if (i == 0) neuron.Value = inputs[j];
                else
                {
                    double sum = 0;
                    for (int k = 0; k < Layers[i - 1].Neurons.Count; ++k)
                        sum += Layers[i - 1].Neurons[k].Value * neuron.Edges[k].Weight;

                    neuron.Value = 2 * activation(sum + neuron.Bias) - 1;
                }
            }
        }

        Layer lastLayer = Layers[Layers.Count - 1];
        int num = lastLayer.Neurons.Count;
        double[] outputs = new double[num];
        for (int i = 0; i < num; ++i)
            outputs[i] = lastLayer.Neurons[i].Value;


        outputs[0] = activation(outputs[0]);
        outputs[1] = Math.Tanh(2 * outputs[1]);
        outputs[2] = activation(outputs[2]);
        Debug.Log("Output: " + outputs[0] + ", " + outputs[1] + ", " + outputs[2]);
        return outputs;
    }

    public void Save()
    {
        StreamWriter write = new StreamWriter("./records/trackC/neuralnet/nn" + Fitness + ".txt", true);

        for (int i = 0; i < LayerStructure.Length - 1; ++i)
            write.Write(LayerStructure[i] + ", ");

        write.Write(LayerStructure[LayerStructure.Length - 1] + "\n");
        List<double> encoded = Encode();
        for (int i = 0; i < encoded.Count - 1; i++)
        {
            if (encoded[i] > 1f || encoded[i] < -1f) Debug.Log("Error: " + encoded[i]);
            write.Write(encoded[i]);
            write.Write(", ");
        }
        write.Write(encoded[encoded.Count - 1]);
        write.Write("\n");

        write.Close();
    }

    public void Save(String fileName)
    {
        StreamWriter write = new StreamWriter("./records/" + fileName + "/bestcar/bc" + (int)Fitness + ".txt", true);

        for (int i = 0; i < LayerStructure.Length - 1; ++i)
            write.Write(LayerStructure[i] + ", ");

        write.Write(LayerStructure[LayerStructure.Length - 1] + "\n");
        List<double> encoded = Encode();
        for (int i = 0; i < encoded.Count - 1; i++)
        {
            if (encoded[i] > 1f || encoded[i] < -1f) Debug.Log("Error: " + encoded[i]);
            write.Write(encoded[i]);
            write.Write(", ");
        }
        write.Write(encoded[encoded.Count - 1]);
        write.Write("\n");

        write.Close();
    }

    private double sigmoid(double x)
    {
        return 1 / (1 + Math.Exp(-x));
    }

    private double activation(double x)
    {
        return 1 / (1 + Math.Pow(Math.E, -4 * x));
    }
}
