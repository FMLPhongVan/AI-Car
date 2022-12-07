using System;
using System.Collections.Generic;
using UnityEngine;

public class TesterAIController : MonoBehaviour
{
    public NeuralNetwork Network;
    public float TravelDistance = 0;
    public float AverageSpeed = 0;
    public float TimeElapsed;
    public bool Alive = true;

    private TesterController _testerController;

    Vector3 _lastPosition;
    public float _movement;

    // Start is called before the first frame update
    void Awake()
    {
        _testerController = gameObject.GetComponent<TesterController>();

        TimeElapsed = 0;
        _lastPosition = gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!Alive) return;
        List<double> inputs = new ();
        for (int i = 0; i < _testerController.Sensors.Count; ++i)
            inputs.Add(_testerController.Sensors[i].HitDistance);

        inputs.Add(_testerController.Speed);
        double[] output = Network.FeedForward(inputs);
        _testerController.ForceInput = (float)output[0];
        _testerController.TargetSteeringAngle = (float)output[1];
        _testerController.IsBraking = false; //output[2] < 0.5f;

        _movement = Vector3.Distance(gameObject.transform.position, _lastPosition);
        TravelDistance += _movement;
        _lastPosition = gameObject.transform.position;

        TimeElapsed += Time.deltaTime;
        AverageSpeed = TravelDistance / TimeElapsed;

        if (System.Math.Abs(gameObject.transform.rotation.x) > 10 || System.Math.Abs(gameObject.transform.rotation.z) > 10)
            Stop();

        if (_testerController.PlayerHitWall) Stop();
        if (_testerController.PlayerStopped) Stop();
    }

    public void Stop()
    {
        Alive = false;
        _testerController.TargetSteeringAngle = 0;
        _testerController.ForceInput = 0;
        _testerController.IsBraking = false;
        gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        gameObject.GetComponent<Rigidbody>().isKinematic = true;
    }

    public void Reset()
    {
        _testerController.Reset();
        TravelDistance = 0;
        TimeElapsed = 0;
        _lastPosition = gameObject.transform.position;
        Alive = true;
    }
}
