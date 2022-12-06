using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCheckpoint : MonoBehaviour
{
    public int CurrentLap = 0;
    public int NextCheckPoint = 1;
    public float DistanceToNextCheckpoint = 0;

    private TracksManager _trackManager;
    private Queue<GameObject> _checkpoints = new();
    private GameObject _lastCheckpoint;

    // Start is called before the first frame update
    void Start()
    {
        //SetUpTrackCheckpoints();
    }

    // Update is called once per frame
    void Update()
    {
        DistanceToNextCheckpoint =
            Vector3.Distance(gameObject.transform.position, _checkpoints.Peek().transform.position);
    }

    public void SetUpTrackCheckpoints()
    {
        var tmp = _trackManager.GetCurrentTrackCheckpoints();
        _checkpoints.Clear();
        for (int i = 1; i < tmp.Count; ++i) _checkpoints.Enqueue(tmp[i]);
        _checkpoints.Enqueue(tmp[0]);
        _lastCheckpoint = tmp[0];
        DistanceToNextCheckpoint =
            Vector3.Distance(gameObject.transform.position, _checkpoints.Peek().transform.position);
        NextCheckPoint = 1;
        CurrentLap = 0;
    }

    public void CheckHitCheckpoint(GameObject HitCheckpoint)
    {
        if (HitCheckpoint != _checkpoints.Peek()) return;
        _lastCheckpoint = _checkpoints.Dequeue();
        NextCheckPoint++;
        if (_checkpoints.Count == 0)
        {
            var tmp = _trackManager.GetCurrentTrackCheckpoints();
            for (int i = 1; i < tmp.Count; ++i) _checkpoints.Enqueue(tmp[i]);
            _checkpoints.Enqueue(tmp[0]);
            NextCheckPoint = 1;
            CurrentLap++;
        } 
    }

    public GameObject GetNextCheckpoint()
    {
        return _checkpoints.Peek();
    }

    public GameObject GetLastCheckpoint()
    {
        return _lastCheckpoint;
    }

    public float GetDistanceToNextCheckpoint()
    {
        return Vector3.Distance(gameObject.transform.position, _checkpoints.Peek().transform.position);
    }

    public void SetTracksManager(TracksManager tracksManager)
    {
        _trackManager = tracksManager;
        SetUpTrackCheckpoints();
    }
}
