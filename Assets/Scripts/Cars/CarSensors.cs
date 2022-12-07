using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSensors : MonoBehaviour
{
    public GameObject Car;
    public GameObject BodyPoint;
    public float HitDistance = 0;

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 direction = gameObject.transform.position - BodyPoint.transform.position;
        HitDistance = 1;
        Physics.SphereCast(BodyPoint.transform.position, 0.1f, direction, out RaycastHit hitInfo, direction.magnitude, ~(1 << 6 | 1 << 2));
        Vector3 sensorPos = BodyPoint.transform.position;
        if (hitInfo.collider != null)
        {
            Debug.Log(hitInfo.collider.tag);
        }
        if (hitInfo.collider != null && !hitInfo.collider.gameObject.CompareTag("Path"))
        {
            Debug.Log(hitInfo.collider.gameObject.name);
            HitDistance = hitInfo.distance / direction.magnitude;
            //Debug.DrawRay(sensorPos, direction, Color.red);
            Debug.DrawRay(sensorPos, hitInfo.point - sensorPos, Color.red);
        } 
        else
        {
            HitDistance = 1;
            Debug.DrawRay(sensorPos, direction, Color.green);
        }
    }
}
