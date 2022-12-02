using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.Types;

public class Academy : MonoBehaviour
{
    public int NumGens;
    public int NumSimulate;
    public float MutationRate;
    public Vector3 StartPosition;
    public GameObject Car;
    public TracksManager TrackManager;
    public GeneticController species;
    public int CurrentGenome;
    public float BestGenomeFitness;
    public int BatchSimulate;
    public int Generation = 1;
    public AIController BestCar;

    GameObject[] _cars;
    AIController[] _aiControllers;

    // Start is called before the first frame update
    void Start()
    {
        species = new GeneticController(NumGens, MutationRate);
        _cars = new GameObject[NumSimulate];
        _aiControllers = new AIController[NumSimulate];
        BestCar = _aiControllers[0];

        CarCheckpoint checkpoint = Car.GetComponent<CarCheckpoint>();
        checkpoint.TrackManager = TrackManager;
        
        for (int i = 0; i < NumSimulate; i++)
        {
            _cars[i] = Instantiate(Car, StartPosition, Car.transform.rotation);
            _aiControllers[i] = _cars[i].GetComponent<AIController>();
            _aiControllers[i].network = species.Population[i];
        }
    }

    // Update is called once per frame
    void Update()
    {
        float bestCarFitness = 0;
        bool allCarsDead = true;
        foreach (AIController car in _aiControllers)
        {
            if (car.Alive)
            {
                allCarsDead = false;
                if (car.OverallFitness > bestCarFitness)
                {
                    bestCarFitness = car.OverallFitness;
                    BestCar = car;  
                    if (BestCar.OverallFitness > BestGenomeFitness)
                    {
                        BestGenomeFitness = BestCar.OverallFitness;
                    }
                }

            }
        }

        if (allCarsDead)
        { 
            Debug.Log("All Cars Dead");
            // If we have simualted all genomes, reset and get next gen
            if (CurrentGenome == NumGens)
            {
                Debug.Log("New Gen");
                species.NextGen();
                for (int i = 0; i < NumSimulate; i++)
                {
                    _aiControllers[i].network = species.Population[i];
                    _aiControllers[i].Reset();
                }
                Generation++;
                CurrentGenome = NumSimulate;
            }
            else 
            {
                if (CurrentGenome + NumSimulate <= NumGens)
                {
                    Debug.Log("Full Sim");
                    BatchSimulate = NumSimulate;
                }
                else
                {
                    Debug.Log("Partial Sim");
                    BatchSimulate = NumGens - CurrentGenome;
                }

                for (int i = 0; i < BatchSimulate; i++)
                {
                    _aiControllers[i].network = species.Population[CurrentGenome + i];
                    _aiControllers[i].Reset();
                }
                CurrentGenome += BatchSimulate;
            }
        }
    }
}
