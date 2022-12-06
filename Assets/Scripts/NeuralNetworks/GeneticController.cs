using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticController
{
    public List<NeuralNetwork> CurrentPopulation;
    public List<NeuralNetwork> NextPopulation;
    public double PopulationFitness;
    public float AverageFitness;
    private readonly int _populationSize;
    private readonly float _mutationRate;
    private readonly float _crossoverRate;

    public GeneticController(int hiddenLayer, int numberOfNodePerLayer, int numberOfInputNode, int populationSize, float mutationRate, float crossoverRate)
    {
        _populationSize = populationSize;
        _mutationRate = mutationRate;
        _crossoverRate = crossoverRate;
        CurrentPopulation = new List<NeuralNetwork>();
        PopulationFitness = 0f;
        AverageFitness = 0f;

        int[] layerStructure = new int[hiddenLayer + 2];
        layerStructure[0] = numberOfInputNode;
        for (int i = 1; i < hiddenLayer + 1; i++)
            layerStructure[i] = numberOfNodePerLayer;
        layerStructure[hiddenLayer + 1] = 2;

        for (int i = 0; i < _populationSize; ++i)
            CurrentPopulation.Add(new NeuralNetwork(layerStructure));
    }

    public GeneticController(int hiddenLayer, int numberOfNodePerLayer, int numberOfInputNode, int populationSize, float mutationRate, float crossoverRate, string fileName)
    {
        _populationSize = populationSize;
        _mutationRate = mutationRate;
        _crossoverRate = crossoverRate;
        CurrentPopulation = new List<NeuralNetwork>();
        PopulationFitness = 0f;
        AverageFitness = 0f;

        for (int i = 0; i < _populationSize; ++i)
            CurrentPopulation.Add(new NeuralNetwork("./records/" + fileName + ".txt"));
        //Population.Add(new NeuralNetwork(new int[] { 9, 5, 2 }));
    }

    public void Crossover(List<double> mother, List<double> father)
    {
        List<double> tempM = new ();
        List<double> tempF = new ();
        for (int i = 0; i < mother.Count; i++)
        {
            if (UnityEngine.Random.Range(0f, 1f) < _crossoverRate)
            {
                tempM.Add(father[i]);
                tempF.Add(mother[i]);
            }
            else
            {
                tempM.Add(mother[i]);
                tempF.Add(father[i]);
            }
        }
        mother.RemoveRange(0, mother.Count);
        father.RemoveRange(0, mother.Count);

        mother.InsertRange(0, tempF);
        father.InsertRange(0, tempM);
    }

    public void Mutate(NeuralNetwork network) 
    {
        List<double> genes = network.Encode();
        for (int i = 0; i < genes.Count; ++i)
        {
           if (UnityEngine.Random.Range(0f, 1f) < _mutationRate)
                genes[i] += UnityEngine.Random.Range(0f, 1f) * 0.2 - 0.1;
        }

        network.Decode(genes);
    }

    public NeuralNetwork[] Breed(NeuralNetwork mother, NeuralNetwork father)
    {
        NeuralNetwork[] childs = { new NeuralNetwork(mother.LayerStructure), new NeuralNetwork(father.LayerStructure) };
        List<double> motherGenes = mother.Encode();
        List<double> otherGenes = father.Encode();
        Crossover(motherGenes, otherGenes);
        childs[0].Decode(motherGenes);
        childs[1].Decode(otherGenes);
        return childs;
    }

    public void NextGeneration()
    {
        NextPopulation = new List<NeuralNetwork>();
        PopulationFitness = 0f;

        CurrentPopulation.Sort((a, b) => b.Fitness.CompareTo(a.Fitness));

        // 10% of the population is the best of the previous generation
        for (int i = 0; i < _populationSize / 10; ++i)
            NextPopulation.Add(CurrentPopulation[i]);

        // From 50% of the population, we breed the best of the previous generation
        while (NextPopulation.Count < _populationSize)
        {
            NeuralNetwork[] childs = Breed(CurrentPopulation[UnityEngine.Random.Range(0, _populationSize / 2)], CurrentPopulation[UnityEngine.Random.Range(0, _populationSize / 2)]);
            if (UnityEngine.Random.Range(0f, 1f) < _mutationRate)
                Mutate(childs[0]);

            if (UnityEngine.Random.Range(0f, 1f) < _mutationRate)
                Mutate(childs[1]);

            if (UnityEngine.Random.Range(0f, 1f) < 0.5f)
                NextPopulation.Add(childs[0]);
            else
                NextPopulation.Add(childs[1]);
        }

        for (int i = 0; i < _populationSize; ++i)
            CurrentPopulation[i] = NextPopulation[i];
    }

    private int Selection()
    {
        int[] roundIndex = new int[5];
        for (int i = 0; i < 5; ++i)
            roundIndex[i] = UnityEngine.Random.Range(0, _populationSize / 2);

        int minIndex = _populationSize;
        for (int i = 0; i < 5; ++i)
            minIndex = Math.Min(minIndex, roundIndex[i]);

        return minIndex;
    }
}
