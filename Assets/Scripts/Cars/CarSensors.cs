using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSensors : MonoBehaviour
{
    public GameObject Car;
    public float HitDistance = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 direction = gameObject.transform.position - Car.transform.position;
        HitDistance = 1;
        Physics.Raycast(Car.transform.position, direction, out RaycastHit hitInfo, direction.magnitude, ~(1 << 6 | 1 << 2));
        Vector3 sensorPos = Car.transform.position;
        sensorPos.Set(sensorPos.x, sensorPos.y + 0.5f, sensorPos.z);
        if (hitInfo.collider != null)
        {
            HitDistance = hitInfo.distance / direction.magnitude;
            Debug.DrawRay(sensorPos, direction, Color.red);
        } 
        else
        {
            HitDistance = 1;
            Debug.DrawRay(sensorPos, direction, Color.green);
        }
    }
}
