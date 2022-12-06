using System;
using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using Unity.VisualScripting;
using UnityEngine;

public class Academy : MonoBehaviour
{
    public string FileName = "";
    public GameObject CarVariant;
    public int HiddenLayer = 1;
    public int NumberOfNodePerLayer = 8;
    public int NumberOfInputNode = 13;
    public int Generation = 1;
    public int NumberOfCarsPerGeneration = 20;
    public float MutationRate = 0.05f;
    public float CrossoverRate = 0.5f;
    public Vector3 StartPosition;
    public float BestGenomeFitness = -100000;
    public AIController BestCar;

    public GeneticController _species;
    TracksManager _tracksManager;
    public GameObject[] _cars;
    public AIController[] _aiControllers;

    // Start is called before the first frame update
    void Awake()
    {
        _tracksManager = gameObject.GetComponent<TracksManager>();

        Time.timeScale = 2f;
        if (FileName == "")
            _species = new GeneticController(HiddenLayer, NumberOfNodePerLayer, NumberOfInputNode, NumberOfCarsPerGeneration, MutationRate, CrossoverRate);
        else
            _species = new GeneticController(HiddenLayer, NumberOfNodePerLayer, NumberOfInputNode, NumberOfCarsPerGeneration, MutationRate, CrossoverRate, FileName);
        //_species = new GeneticController(HiddenLayer, NumberOfNodePerLayer, NumberOfInputNode, NumberOfCarsPerGeneration, MutationRate, CrossoverRate);
        _cars = new GameObject[NumberOfCarsPerGeneration];
        _aiControllers = new AIController[NumberOfCarsPerGeneration];

        for (int i = 0; i < NumberOfCarsPerGeneration; i++)
        {
            _cars[i] = Instantiate(CarVariant, StartPosition, Quaternion.identity);
            _aiControllers[i] = _cars[i].GetComponent<AIController>();
            _aiControllers[i].Network = _species.CurrentPopulation[i];
        }


        for (int i = 0; i < NumberOfCarsPerGeneration; ++i)
            for (int j = 0; j < NumberOfCarsPerGeneration; ++j)
                if (i != j)
                {
                    Physics.IgnoreCollision(_cars[i].GetComponentInChildren<BoxCollider>(), _cars[j].GetComponentInChildren<BoxCollider>(), true);
                    Physics.IgnoreCollision(_cars[i].GetComponentInChildren<BoxCollider>(), _cars[j].GetComponentInChildren<WheelCollider>(), true);
                    Physics.IgnoreCollision(_cars[i].GetComponentInChildren<WheelCollider>(), _cars[j].GetComponentInChildren<BoxCollider>(), true);
                    Physics.IgnoreCollision(_cars[i].GetComponentInChildren<WheelCollider>(), _cars[j].GetComponentInChildren<WheelCollider>(), true);
                }
    }

    void Start()
    {
        BestCar = _aiControllers[0];

        for (int i = 0; i < NumberOfCarsPerGeneration; i++)
        {
            _cars[i].GetComponent<CarCheckpoint>().SetTracksManager(_tracksManager);
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < _species.CurrentPopulation.Count; ++i)
        {
            if (_aiControllers[i].Alive && BestGenomeFitness < _aiControllers[i].OverallFitness)
            {
                BestGenomeFitness = _aiControllers[i].OverallFitness;
                BestCar = _aiControllers[i];
            }
        }
        
        if (MeetStopCondition())
        {
            for (int i = 0; i < _aiControllers.Length; ++i) _aiControllers[i].Stop();
            Debug.Log("Meet stop condition. Start a new generation.");
            SetUpNewGeneration();
            Generation++;
            // Sleep for 2 second before next gen
            
        }
    }

    private bool MeetStopCondition()
    {
        bool stop = true;
        for (int i = 0; i < NumberOfCarsPerGeneration; ++i)
        {
            if (_aiControllers[i] == null || (_aiControllers[i].Alive && _aiControllers[i].GetLap() < 2))
                stop = false;
        }
        return stop;
    }

    private void SetUpNewGeneration()
    {
        _tracksManager.NextTrack();
        _species.NextGeneration();
        for (int i = 0; i < _cars.Length; ++i)
        {
            _aiControllers[i].Reset();
            _aiControllers[i].Network = _species.CurrentPopulation[i];
        }
    }
}
