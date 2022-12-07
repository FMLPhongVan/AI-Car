using System;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public NeuralNetwork Network;
    public float TravelDistance = 0;
    public float AverageSpeed = 0;
    public float TimeElapsed;
    public float BestFitness = 0;
    public float BestPopulationFitness = 0;
    public float OverallFitness = 0;
    public float DistanceMultiplier;
    public float SpeedMultiplier;
    public float SensorMultiplier;
    public bool Alive = true;

    private CarCheckpoint _carCheckpoint;
    private CarController _carController;

    Vector3 _lastPosition;
    float _timer = 5f;
    float _averageSensor;
    public float _lastCheckpointDistance;
    public float _movement;

    // Start is called before the first frame update
    void Awake()
    {
        _carCheckpoint = gameObject.GetComponent<CarCheckpoint>();
        _carController = gameObject.GetComponent<CarController>();
        
        TimeElapsed = 0;
        _lastPosition = gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!Alive) return;
        List<double> inputs = new List<double>();
        for (int i = 0; i < _carController.Sensors.Count; ++i)
            inputs.Add(_carController.Sensors[i].HitDistance);

        inputs.Add(_carController.Speed);
        double[] output = Network.FeedForward(inputs);
        _carController.ForceInput = (float)output[0];
        _carController.TargetSteeringAngle = (float)output[1];
        _carController.IsBraking = false; //output[2] < 0.5f;
        CalculateFitness();

        if (BestFitness > OverallFitness)
        {
            _timer -= Time.deltaTime;
            if (_timer <= 0) Stop();
        }
        else
        {
            BestFitness = OverallFitness;
            _timer = 5f;
        }

        if (System.Math.Abs(gameObject.transform.rotation.x) > 8 || System.Math.Abs(gameObject.transform.rotation.z) > 8)
            Stop();

        if (_carController.PlayerHitWall) Stop();
        if (_carController.PlayerStopped) Stop();
    }

    private void CalculateFitness()
    {
        _movement = Vector3.Distance(gameObject.transform.position, _lastPosition);
        Vector3 checkpointVector = _carCheckpoint.GetNextCheckpoint().transform.position - _carCheckpoint.GetLastCheckpoint().transform.position;
        Vector3 moveVector = gameObject.transform.position - _lastPosition;

        if (Math.Abs(Vector3.Angle(checkpointVector, moveVector)) < 90)
            TravelDistance += _movement;
        else TravelDistance -= _movement;

        for (int i = 0; i < _carController.Sensors.Count; ++i)
            _averageSensor += _carController.Sensors[i].HitDistance;

        _averageSensor /= _carController.Sensors.Count;
        _lastPosition = gameObject.transform.position;
        
        TimeElapsed += Time.deltaTime;
        AverageSpeed = TravelDistance / TimeElapsed;
        OverallFitness = TravelDistance * DistanceMultiplier + AverageSpeed * SpeedMultiplier + _averageSensor * SensorMultiplier;
    }

    public void Stop()
    {
        Network.Fitness = OverallFitness;
        if (System.Math.Abs(gameObject.transform.rotation.x) > 8 || System.Math.Abs(gameObject.transform.rotation.z) > 8)
            Network.Fitness /= 2;
        //if (Network.Fitness < 0) Network.Fitness = 0;
        Alive = false;
        _carController.TargetSteeringAngle = 0;
        _carController.ForceInput = 0;
        _carController.IsBraking = false;
        gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        gameObject.GetComponent<Rigidbody>().isKinematic = true;
    }

    public void Reset()
    {
        _carController.Reset();
        TravelDistance = 0;
        TimeElapsed = 0;
        _lastPosition = gameObject.transform.position;
        _timer = 10f;
        _averageSensor = 0;
        if (OverallFitness > BestPopulationFitness) BestPopulationFitness = OverallFitness;
        BestFitness = 0f;
        Network.Fitness = 0;
        OverallFitness = 0;
        Alive = true;
    }

    public int GetLap()
    {
        return _carCheckpoint.CurrentLap;
    }
}
