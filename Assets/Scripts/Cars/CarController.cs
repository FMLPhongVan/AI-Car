using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CarController : MonoBehaviour
{
    // Car & sensors;
    public GameObject Car;
    public Rigidbody _carRigidBody;
    public List<CarSensors> Sensors;
    public CarCheckpoint CarCheckpoint;

    public float Acceleration = 5f;
    public float Deacceleration = 3f;
    public float TurnSpeed = 100f;
    public float Speed;

    // Car flags
    public bool PlayerStopped;
    public bool PlayerHitWall;
    public bool HitCheckPoint;
    public bool TimerStarted;

    public float CarDrive;
    public float CarTurn;

    // Car physics properties
    float _torqueForce = 0;
    Vector3 _startPostion;
    Quaternion _carRotation;
    float _idleTime = 10f;
    float _timeLeft = 0;
    int _firstCheckpoint;
    float _driftSpeedMoving = 0.9f;
    float _driftSpeedStatic = 0.9f;
    float _maxSideways = 0.5f;


    // Start is called before the first frame update
    void Start()
    {
        CarCheckpoint = GetComponent<CarCheckpoint>();
        _carRigidBody = GetComponent<Rigidbody>();
        PlayerStopped = false;
        PlayerHitWall = false;
        HitCheckPoint = false;
        _startPostion = Car.transform.position;
        _startPostion = Vector3.zero;
        _carRotation = Car.transform.rotation;
        _firstCheckpoint = CarCheckpoint.NextCheckPoint;
        TimerStarted = false;
    }

    void FixedUpdate()
    {
        _carRigidBody.velocity.Set(_carRigidBody.velocity.x, 0, _carRigidBody.velocity.z);
        Speed = _carRigidBody.velocity.magnitude;

        if (Speed < 0.05f)
        {
            if (TimerStarted)
            {
                _timeLeft += Time.deltaTime;
                if (_timeLeft > _idleTime)
                {
                    PlayerStopped = true;
                    TimerStarted = false;
                    _timeLeft = 0;
                }
            }
            else TimerStarted = true;
        }

        float driftFactor = _driftSpeedStatic;
        if (ForwardVelocity().magnitude > _maxSideways)
            driftFactor = _driftSpeedMoving;

        _carRigidBody.velocity = ForwardVelocity() + SideVelocity() * driftFactor;

        if (CarDrive > 0)
            _carRigidBody.AddForce(transform.forward * Acceleration);
        else
            _carRigidBody.AddForce(transform.forward * Deacceleration);
        _carRigidBody.velocity.Set(_carRigidBody.velocity.x, 0, _carRigidBody.velocity.z);

        _torqueForce = Mathf.Lerp(0, TurnSpeed, _carRigidBody.velocity.magnitude / 2);
        _carRigidBody.angularVelocity = CarTurn * _torqueForce * Vector3.up;
        Car.transform.position.Set(Car.transform.position.x, 0, Car.transform.position.z);

    }

    Vector3 ForwardVelocity()
    {
        return transform.forward * Vector3.Dot(_carRigidBody.velocity, transform.forward);
    }

    Vector3 SideVelocity()
    {
        return transform.right * Vector3.Dot(_carRigidBody.velocity, transform.right);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Checkpoint")
        {
            if (other.gameObject == CarCheckpoint.GetNextCheckpoint())
            {
                if (CarCheckpoint.NextCheckPoint == CarCheckpoint.Checkpoints.Count - 1)
                {
                    CarCheckpoint.CurrentLap++;
                    CarCheckpoint.NextCheckPoint = 0;
                }
                else
                {
                    CarCheckpoint.NextCheckPoint++;
                    HitCheckPoint = true;
                }
            }
            return;
        }
        else if (other.gameObject.tag == "Player")
            return;

        PlayerHitWall = true;
    }

    public void Reset()
    {
        _carRigidBody.velocity = Vector3.zero;
        _carRigidBody.position = _startPostion;
        Car.transform.rotation = _carRotation;
        CarCheckpoint.NextCheckPoint = _firstCheckpoint;
        _timeLeft = 0;

        PlayerStopped = false;
        PlayerHitWall = false;
        HitCheckPoint = false;
        TimerStarted = false;
    }
}
