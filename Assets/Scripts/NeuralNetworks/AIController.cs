using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public NeuralNetwork network = null;
    public CarController CarController;
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

    Vector3 _lastPosition;
    float _timer = 0;
    float _averageSensor;
    public float _lastCheckpointDistance;
    public float _movement;

    // Start is called before the first frame update
    void Start()
    {
        TimeElapsed = 0;
        _lastPosition = CarController.Car.transform.position;
        _lastCheckpointDistance = CarController.CarCheckpoint.DistanceToNextCheckpoint;
    }

    // Update is called once per frame
    void Update()
    {
        if (!Alive) return;
        List<double> inputs = new List<double>();
        for (int i = 0; i < CarController.Sensors.Count; ++i)
            inputs.Add(CarController.Sensors[i].HitNormal);

        inputs.Add(CarController.Speed);
        double[] output = network.FeedForward(inputs);
        CarController.ForceInput = (float)output[0];
        CarController.SteeringAngle = (float)output[1];
        //CarController.IsBraking = output[2] < 0.5f;
        CalculateFitness();

        if (BestFitness > OverallFitness)
        {
            _timer -= Time.deltaTime;
            if (_timer <= 0) Stop();
        }
        else
        {
            BestFitness = OverallFitness;
            _timer = 10f;
        }

        if (CarController.PlayerHitWall) Stop();
        if (CarController.PlayerStopped) Stop();
    }

    private void CalculateFitness()
    {
        _movement = Vector3.Distance(CarController.Car.transform.position, _lastPosition);
        if (CarController.CarCheckpoint.GetDistanceToNextCheckpoint() >= _lastCheckpointDistance)
        {
            _movement *= -2;
        }

        TravelDistance += _movement;
        for (int i = 0; i < CarController.Sensors.Count; ++i)
            _averageSensor += CarController.Sensors[i].HitNormal;

        _averageSensor /= CarController.Sensors.Count;
        _lastCheckpointDistance = CarController.CarCheckpoint.GetDistanceToNextCheckpoint();
        _lastPosition = CarController.Car.transform.position;
        TimeElapsed += Time.deltaTime;
        AverageSpeed = TravelDistance / TimeElapsed;
        OverallFitness = (TravelDistance * DistanceMultiplier) + AverageSpeed * SpeedMultiplier + _averageSensor * SensorMultiplier;
    }

    public void Stop()
    {
        network.Fitness = OverallFitness;
        if (System.Math.Abs(CarController.Car.transform.rotation.x) > 8 || System.Math.Abs(CarController.Car.transform.rotation.z) > 8)
            network.Fitness = OverallFitness / 2;
        if (network.Fitness < 0) network.Fitness = 0;
        Alive = false;
        CarController.SteeringAngle = 0;
        CarController.ForceInput = 0;
        CarController.IsBraking = false;
        CarController._carRigidBody.velocity = Vector3.zero;
        CarController._carRigidBody.isKinematic = true;
    }

    public void Reset()
    {
        CarController.Reset();
        TravelDistance = 0;
        TimeElapsed = 0;
        _lastPosition = CarController.Car.transform.position;
        _lastCheckpointDistance = CarController.CarCheckpoint.GetDistanceToNextCheckpoint();
        _timer = 10f;
        _averageSensor = 0;
        if (OverallFitness > BestPopulationFitness) BestPopulationFitness = OverallFitness;
        BestFitness = 0f;
        Alive = true;
        CarController._carRigidBody.isKinematic = false;
    }
}
