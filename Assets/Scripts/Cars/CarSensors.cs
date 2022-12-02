using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSensors : MonoBehaviour
{
    public GameObject Car;
    public float Distance = 0;
    public float HitNormal = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 direction = gameObject.transform.position - Car.transform.position;
        HitNormal = 1;
        RaycastHit hitInfo;
        bool hit = Physics.Raycast(Car.transform.position, direction, out hitInfo, direction.magnitude, ~(1 << 6 | 1 << 2));
        if (hitInfo.collider != null)
        {
            HitNormal = hitInfo.distance / direction.magnitude;
            Debug.DrawRay(Car.transform.position, direction, Color.red);
        } 
        else
        {
            Distance = 1;
            Debug.DrawRay(Car.transform.position, direction, Color.green);
        }
    }
}
