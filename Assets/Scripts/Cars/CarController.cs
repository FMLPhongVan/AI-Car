using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CarController : MonoBehaviour
{
    // Car & sensors;
    public GameObject Car;
    public AIController ai;
    public Rigidbody _carRigidBody;
    public List<CarSensors> Sensors;
    public CarCheckpoint CarCheckpoint;

/*    public float Acceleration = 5f;
    public float Deacceleration = 3f;
    public float TurnSpeed = 100f;*/
    public float Speed;

    // Car flags
    public bool PlayerStopped;
    public bool PlayerHitWall;
    public bool HitCheckPoint;
    public bool TimerStarted;
    
    // Car physics properties
    float _torqueForce = 0;
    Vector3 _startPostion;
    Quaternion _carRotation;
    float _idleTime = 10f;
    float _timeLeft = 0;
    int _firstCheckpoint;

    public float ForceInput;
    public float SteeringAngle;
    public bool IsBraking = false;
    private float _currentSteerAngle;
    private float _currentBrakeForce;
    [SerializeField] private float _motorForce;
    [SerializeField] private float _brakeForce;
    [SerializeField] public float MaxSteeringAngle = 50f;

    [SerializeField] private WheelCollider _frontLeftWheelCollider;
    [SerializeField] private WheelCollider _frontRightWheelCollider;
    [SerializeField] private WheelCollider _rearLeftWheelCollider;
    [SerializeField] private WheelCollider _rearRightWheelCollider;

    [SerializeField] private Transform _frontLeftWheelTransform;
    [SerializeField] private Transform _frontRightWheeTransform;
    [SerializeField] private Transform _rearLeftWheelTransform;
    [SerializeField] private Transform _rearRightWheelTransform;


    // Start is called before the first frame update
    void Start()
    {
        CarCheckpoint = GetComponent<CarCheckpoint>();
        _carRigidBody = GetComponent<Rigidbody>();
        ai = GetComponent<AIController>();
        PlayerStopped = false;
        PlayerHitWall = false;
        HitCheckPoint = false;
        _startPostion = Car.transform.position;
        //_startPostion = Vector3.zero;
        _carRotation = Car.transform.rotation;
        _firstCheckpoint = CarCheckpoint.NextCheckPoint;
        TimerStarted = false;
    }

    /*void FixedUpdate()
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

        if (ForceInput > 0)
            _carRigidBody.AddForce(transform.forward * Acceleration);
        else
            _carRigidBody.AddForce(transform.forward * Deacceleration);
        _carRigidBody.velocity.Set(_carRigidBody.velocity.x, 0, _carRigidBody.velocity.z);

        _torqueForce = Mathf.Lerp(0, TurnSpeed, _carRigidBody.velocity.magnitude / 2);
        _carRigidBody.angularVelocity = SteeringAngle * _torqueForce * Vector3.up;
        Car.transform.position.Set(Car.transform.position.x, 0, Car.transform.position.z);

    }*/

    private void FixedUpdate()
    {
        if (!ai.Alive) return;
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
        
        HandleMotor();
        UpdateAllWheel();
        _carRigidBody.velocity.Set(_carRigidBody.velocity.x, 0, _carRigidBody.velocity.z);
        Car.transform.position.Set(Car.transform.position.x, 0, Car.transform.position.z);
    }

    private void HandleMotor()
    {
        _frontLeftWheelCollider.motorTorque = ForceInput * _motorForce;
        _frontRightWheelCollider.motorTorque = ForceInput * _motorForce;
        _currentBrakeForce = IsBraking ? _brakeForce : 0f;

        _frontLeftWheelCollider.brakeTorque = _currentBrakeForce;
        _frontRightWheelCollider.brakeTorque = _currentBrakeForce;
        _rearLeftWheelCollider.brakeTorque = _currentBrakeForce;
        _rearRightWheelCollider.brakeTorque = _currentBrakeForce;

        _currentSteerAngle = MaxSteeringAngle * SteeringAngle;
        _frontLeftWheelCollider.steerAngle = _currentSteerAngle;
        _frontRightWheelCollider.steerAngle = _currentSteerAngle;
    }

    private void UpdateAllWheel()
    {
        UpdateWheelState(_frontLeftWheelCollider, _frontLeftWheelTransform);
        UpdateWheelState(_frontRightWheelCollider, _frontRightWheeTransform);
        UpdateWheelState(_rearLeftWheelCollider, _rearLeftWheelTransform);
        UpdateWheelState(_rearRightWheelCollider, _rearRightWheelTransform);
    }

    private void UpdateWheelState(WheelCollider collider, Transform wheel)
    {
        Vector3 pos;
        Quaternion rot;
        collider.GetWorldPose(out pos, out rot);
        wheel.rotation = rot;
        wheel.position = pos;
    }

    /*    Vector3 ForwardVelocity()
        {
            return transform.forward * Vector3.Dot(_carRigidBody.velocity, transform.forward);
        }

        Vector3 SideVelocity()
        {
            return transform.right * Vector3.Dot(_carRigidBody.velocity, transform.right);
        }*/

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
        else if (other.gameObject.tag == "Path")
        {
            _carRigidBody.velocity.Set(_carRigidBody.velocity.x, 0, _carRigidBody.velocity.z);
            Car.transform.position.Set(Car.transform.position.x, 0, Car.transform.position.z);
            return;
        }
        else if (other.gameObject.tag == "Player")
        {
            //Physics.IgnoreCollision(GetComponent<Collider>(), other);
            return;
        }

        PlayerHitWall = true;
    }

    public void Reset()
    {
        _carRigidBody.velocity = Vector3.zero;
        _carRigidBody.position = _startPostion;
        Car.transform.rotation = _carRotation;
        
        _currentBrakeForce = 0f;
        _currentSteerAngle = 0f;

        ForceInput = 0;
        SteeringAngle = 0;
        this.HandleMotor();
        UpdateAllWheel();

        CarCheckpoint.NextCheckPoint = _firstCheckpoint;
        _timeLeft = 0;

        PlayerStopped = false;
        PlayerHitWall = false;
        HitCheckPoint = false;
        TimerStarted = false;
    }
}
