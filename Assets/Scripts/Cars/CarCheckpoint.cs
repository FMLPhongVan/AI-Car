using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCheckpoint : MonoBehaviour
{
    public CarController CarController;
    public TracksManager TrackManager;
    public List<GameObject> Checkpoints;
    public int NextCheckPoint = 1;
    public int CurrentLap = 0;
    public float DistanceToNextCheckpoint = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        Checkpoints = TrackManager.Checkpoints;
        DistanceToNextCheckpoint = 
            Vector3.Distance(CarController.Car.transform.position, Checkpoints[NextCheckPoint].transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        DistanceToNextCheckpoint =
            Vector3.Distance(CarController.Car.transform.position, Checkpoints[NextCheckPoint].transform.position);
    }

    public GameObject GetNextCheckpoint()
    {
        return Checkpoints[NextCheckPoint];
    }

    public float GetDistanceToNextCheckpoint()
    {
        return Vector3.Distance(CarController.Car.transform.position, Checkpoints[NextCheckPoint].transform.position);
    }
}
