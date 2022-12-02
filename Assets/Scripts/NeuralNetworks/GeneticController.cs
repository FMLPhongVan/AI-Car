using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticController
{
    public List<NeuralNetwork> Population;
    public List<NeuralNetwork> NextGeneration;
    public double PopulationFitness;
    public float MutationRate;
    public float AverageFitness;
    private int _populationSize;

    public GeneticController(int populationSize, float mutationRate)
    {
        _populationSize = populationSize;
        MutationRate = mutationRate;
        Population = new List<NeuralNetwork>();
        PopulationFitness = 0f;
        AverageFitness = 0f;

        for (int i = 0; i < _populationSize; ++i)
            //Population.Add(new NeuralNetwork("./records/nn87.txt"));
            Population.Add(new NeuralNetwork(new int[] { 9, 5, 2 }));
    }

    public GeneticController(int populationSize, float mutationRate, string fileName)
    {
        _populationSize = populationSize;
        MutationRate = mutationRate;
        Population = new List<NeuralNetwork>();
        PopulationFitness = 0f;
        AverageFitness = 0f;

        for (int i = 0; i < _populationSize; ++i)
            Population.Add(new NeuralNetwork("./records/" + fileName + ".txt"));
        //Population.Add(new NeuralNetwork(new int[] { 9, 5, 2 }));
    }

    public void Crossover(List<double> mother, List<double> father)
    {
        List<double> tempM = new List<double>();
        List<double> tempF = new List<double>();
        for (int i = 0; i < mother.Count; i++)
        {
            if (UnityEngine.Random.Range(0, 1f) > .5)
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
            if (MutationRate > UnityEngine.Random.Range(0f, 1f))
                genes[i] = UnityEngine.Random.Range(-1f, 1f);
        }

        network.Decode(genes);
    }

    public NeuralNetwork[] Breed(NeuralNetwork mother, NeuralNetwork father)
    {
        NeuralNetwork[] childs = { new NeuralNetwork(mother.LayerStructure), new NeuralNetwork(mother.LayerStructure) };
        List<double> motherGenes = mother.Encode();
        List<double> otherGenes = father.Encode();
        childs[0].Decode(motherGenes);
        childs[1].Decode(otherGenes);
        return childs;
    }

    public void NextGen()
    {
        NextGeneration = new List<NeuralNetwork>();
        PopulationFitness = 0f;

        for (int i = 0; i < Population.Count; ++i)
            PopulationFitness += Population[i].Fitness;

        for (int i = 0; i < Population.Count; ++i)
            Population[i].FitnessRatio = (float)(Population[i].Fitness / PopulationFitness);

        AverageFitness = (float)(PopulationFitness / Population.Count);
        Population.Sort((a, b) => b.Fitness.CompareTo(a.Fitness));
        Population[0].Save();
        NextGeneration.Add(Population[0]);

        for (int i = 0; i < Population.Count / 2; ++i)
        {
            int firstParentId = -1;
            int secondParentId = -1;
            float firstChance = UnityEngine.Random.Range(0f, 1f);
            float secondChance = UnityEngine.Random.Range(0f, 1f);
            double range = 0;
            
            for (int j = 0; j < Population.Count; ++j)
            {
                range += Population[i].FitnessRatio;
                if (firstChance > range && secondChance > range) continue;
                if (firstChance <= range && firstParentId == -1)
                    firstParentId = j;

                if (secondChance <= range && secondParentId == -1)
                    secondParentId = (firstParentId == j) ? (j + 1) % Population.Count : j;

                if (firstParentId >= 0 && secondParentId >= 0) break;
            }

            if (firstParentId == -1) firstParentId = Population.Count - 1;
            if (secondParentId == -1) secondParentId = Population.Count - 1;

            NeuralNetwork[] childs = Breed(Population[firstParentId], Population[secondParentId]);
            Mutate(childs[0]);
            Mutate(childs[1]);
            NextGeneration.Add(childs[0]);
            NextGeneration.Add(childs[1]);
        }

        for (int i = 0; i < _populationSize; ++i)
            Population[i] = NextGeneration[i];
    }
}
