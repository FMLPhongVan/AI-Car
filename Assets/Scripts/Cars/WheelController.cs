using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelController : MonoBehaviour
{
    public CarController CarController;
    public Rigidbody CarRigidBody;

    Vector3 _wheelAngle;
    float _steerAngle;
    float _maxSteerAngle = 30f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _steerAngle = CarController.CarTurn * _maxSteerAngle + CarRigidBody.rotation.y;
    }

    void LateUpdate()
    {
        _wheelAngle = transform.eulerAngles;
        _wheelAngle.y = _steerAngle;
        transform.eulerAngles = _wheelAngle;
    }
}
