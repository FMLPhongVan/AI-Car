using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.EditorUtilities;
using Unity.VisualScripting;
using UnityEngine;

public class Academy : MonoBehaviour
{
    public Canvas Canvas;
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
    public NeuralNetwork[] BestCarEachTrack;
    public float CurrentBestFitness = -100000;
    public float BestGenomeFitness = -100000;

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
        BestCarEachTrack = new NeuralNetwork[_tracksManager.Tracks.Count];

        for (int i = 0; i < NumberOfCarsPerGeneration; i++)
        {
            _cars[i].GetComponent<CarCheckpoint>().SetTracksManager(_tracksManager);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // pause
            if (Time.timeScale == 0)
                Time.timeScale = 2f;
            else
                Time.timeScale = 0;
        }

        for (int i = 0; i < _species.CurrentPopulation.Count; ++i)
        {
            if (_aiControllers[i].Alive && CurrentBestFitness < _aiControllers[i].OverallFitness)
            {
                CurrentBestFitness = _aiControllers[i].OverallFitness;
            }
        }
        
        if (MeetStopCondition())
        {
            bool triggerSaveGene = false;
            for (int i = 0; i < _aiControllers.Length; ++i) _aiControllers[i].Stop();
            uint currentTrack = _tracksManager.CurrentTrack;
            for (int i = 0; i < _aiControllers.Length; ++i)
            {
                if (BestCarEachTrack[currentTrack] == null || BestCarEachTrack[currentTrack].Fitness < _aiControllers[i].Network.Fitness)
                {
                    //if (BestCarEachTrack[currentTrack] == null)
                    BestCarEachTrack[currentTrack] = new NeuralNetwork(_aiControllers[i].Network.LayerStructure);
                    BestCarEachTrack[currentTrack].Decode(_aiControllers[i].Network.Encode());
                    BestCarEachTrack[currentTrack].Fitness = _aiControllers[i].Network.Fitness;
                    triggerSaveGene = true;
                }
            }
            for (int i = 0; i < BestCarEachTrack.Length; ++i)
                if (BestCarEachTrack[i] != null && BestGenomeFitness < BestCarEachTrack[i].Fitness)
                    BestGenomeFitness = BestCarEachTrack[i].Fitness;
                
            if (triggerSaveGene)
            {
                BestCarEachTrack[currentTrack].SaveGene("tmp/" + _tracksManager.Tracks[(int)currentTrack].name + ".txt");
            }
            Debug.Log("Meet stop condition. Start a new generation.");
            SetUpNewGeneration();
            Generation++;
            // Sleep for 2 second before next gen
            
        }

        TextMeshProUGUI[] textMeshProUGUIs = Canvas.GetComponentsInChildren<TextMeshProUGUI>();
        for (int i = 0; i < textMeshProUGUIs.Length; ++i)
        {
            if (textMeshProUGUIs[i].name == "Info")
                textMeshProUGUIs[i].text = "Generation: " + Generation + "\nTrack: " + _tracksManager.TrackName + "\nCTBF: " + CurrentBestFitness + "\nBest Fitness: " + BestGenomeFitness;
        }
    }

    public void SaveModel(string fileName)
    {
        float tmp = -10000;
        int diff = -1;
        int choose = -1;
        for (int i = 0; i < BestCarEachTrack.Length; ++i)
        {
            if (BestCarEachTrack[i] != null)
            {
                if (tmp < BestCarEachTrack[i].Fitness)
                {
                    tmp = BestCarEachTrack[i].Fitness;
                    diff = _tracksManager.Tracks[i].GetComponent<Track>().Difficulty;
                    choose = i;
                }
                else if (tmp == BestCarEachTrack[i].Fitness && _tracksManager.Tracks[i].GetComponent<Track>().Difficulty > diff)
                {
                    tmp = BestCarEachTrack[i].Fitness;
                    diff = _tracksManager.Tracks[i].GetComponent<Track>().Difficulty;
                    choose = i;
                }
            }
        }

        if (choose == -1)
            _aiControllers[0].Network.SaveGene(fileName);
        else BestCarEachTrack[choose].SaveGene(fileName);
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

        CurrentBestFitness = -100000;
    }
}
