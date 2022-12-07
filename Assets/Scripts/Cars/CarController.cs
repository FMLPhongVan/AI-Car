using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CarController : MonoBehaviour
{
    // Car & sensors;
    private AIController _ai;
    private Rigidbody _carRigidBody;
    public List<CarSensors> Sensors;
    private CarCheckpoint _carCheckpoint;

    public float Speed;

    // Car flags
    public bool PlayerStopped;
    public bool PlayerHitWall;
    public bool TimerStarted;
    
    // Car physics properties
    Vector3 _startPostion;
    Quaternion _carRotation;
    readonly float _idleTime = 10f;
    float _timeLeft = 0;

    public float ForceInput;
    public float TargetSteeringAngle;
    public bool IsBraking = false;
    
    private float _currentSteerAngle;
    private float _currentBrakeForce;
    
    [SerializeField] private float _motorForce = 5000;
    [SerializeField] private float _brakeForce = 3000;
    [SerializeField] public float MaxSteeringAngle = 50f;
    private float _maxSteerAnglePerDeltaTime;

    [SerializeField] private WheelCollider _frontLeftWheelCollider;
    [SerializeField] private WheelCollider _frontRightWheelCollider;
    [SerializeField] private WheelCollider _rearLeftWheelCollider;
    [SerializeField] private WheelCollider _rearRightWheelCollider;

    [SerializeField] private Transform _frontLeftWheelTransform;
    [SerializeField] private Transform _frontRightWheeTransform;
    [SerializeField] private Transform _rearLeftWheelTransform;
    [SerializeField] private Transform _rearRightWheelTransform;


    // Start is called before the first frame update
    void Awake()
    {
        _carCheckpoint = GetComponent<CarCheckpoint>();
        _carRigidBody = GetComponent<Rigidbody>();
        PlayerStopped = false;
        PlayerHitWall = false;
        TimerStarted = false;
        _startPostion =gameObject.transform.position;
        _carRotation = gameObject.transform.rotation;
    }

    void Start()
    {
        _ai = gameObject.GetComponent<AIController>();
    }

    private void FixedUpdate()
    {
        if (!_ai.Alive) return;
        _carRigidBody.velocity.Set(_carRigidBody.velocity.x, 0, _carRigidBody.velocity.z);
        Speed = _carRigidBody.velocity.magnitude;
        if (Speed < 0.1f)
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
        //_carRigidBody.velocity.Set(_carRigidBody.velocity.x, 0, _carRigidBody.velocity.z);
        //gameObject.transform.position.Set(gameObject.transform.position.x, 0, gameObject.transform.position.z);
    }

    private void HandleMotor()
    {
        _rearLeftWheelCollider.motorTorque = ForceInput * _motorForce;
        _rearRightWheelCollider.motorTorque = ForceInput * _motorForce;
        _currentBrakeForce = IsBraking ? _brakeForce : 0f;

        _frontLeftWheelCollider.brakeTorque = _currentBrakeForce;
        _frontRightWheelCollider.brakeTorque = _currentBrakeForce;
        _rearLeftWheelCollider.brakeTorque = _currentBrakeForce;
        _rearRightWheelCollider.brakeTorque = _currentBrakeForce;


        _maxSteerAnglePerDeltaTime = 0.05f / Time.deltaTime;
        if (TargetSteeringAngle * MaxSteeringAngle > _currentSteerAngle)
        {
            _currentSteerAngle += _maxSteerAnglePerDeltaTime;
            if (TargetSteeringAngle * MaxSteeringAngle < _currentSteerAngle)
                _currentSteerAngle = TargetSteeringAngle * MaxSteeringAngle;
        }
        else if (TargetSteeringAngle * MaxSteeringAngle < _currentSteerAngle)
        {
            _currentSteerAngle -= _maxSteerAnglePerDeltaTime;
            if (TargetSteeringAngle * MaxSteeringAngle > _currentSteerAngle)
                _currentSteerAngle = TargetSteeringAngle * MaxSteeringAngle;
        }
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
        collider.GetWorldPose(out Vector3 pos, out Quaternion rot);
        wheel.SetPositionAndRotation(pos, rot);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Checkpoint"))
        {
            _carCheckpoint.CheckHitCheckpoint(other.gameObject);
            return;
        }
        else if (other.gameObject.CompareTag("Path"))
        {
            _carRigidBody.velocity.Set(_carRigidBody.velocity.x, 0, _carRigidBody.velocity.z);
            gameObject.transform.position.Set(gameObject.transform.position.x, 0, gameObject.transform.position.z);
            return;
        }
        else if (other.gameObject.CompareTag("Player"))
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
        _carRigidBody.isKinematic = false;
        gameObject.transform.position = _startPostion;
        gameObject.transform.rotation = _carRotation;
        
        _currentBrakeForce = 0f;
        _currentSteerAngle = 0f;

        ForceInput = 0;
        TargetSteeringAngle = 0;
        HandleMotor();
        _frontLeftWheelCollider.steerAngle = 0;
        _frontRightWheelCollider.steerAngle = 0;
        UpdateAllWheel();

        _timeLeft = 0;

        PlayerStopped = false;
        PlayerHitWall = false;
        TimerStarted = false;

        _carCheckpoint.SetUpTrackCheckpoints();
    }
}
