using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TracksManager : MonoBehaviour
{
    public uint CurrentTrack = 0;
    public string TrackName = "Unknown";
    public List<GameObject> Tracks = new ();

    void Awake()
    {
        GameObject[] tmp = GameObject.FindGameObjectsWithTag("Track");
        for (int i = 0; i < tmp.Length; i++)
            Tracks.Add(tmp[i]);

        CurrentTrack = 0;
        TrackName = Tracks[(int)CurrentTrack].name;
        for (int i = 1; i < Tracks.Count; i++)
            Tracks[i].SetActive(false);
    }

    public List<GameObject> GetCurrentTrackCheckpoints()
    {
        return Tracks[(int)CurrentTrack].GetComponent<Track>().Checkpoints;
    }

    public void NextTrack()
    {
        Tracks[(int)CurrentTrack].SetActive(false);
        CurrentTrack = (CurrentTrack + 1) % (uint)Tracks.Count;
        Debug.Log("Current track: " + CurrentTrack);
        Tracks[(int)CurrentTrack].SetActive(true);
        Tracks[(int)CurrentTrack].transform.position = Vector3.zero;
        TrackName = Tracks[(int)CurrentTrack].name;
    }
}
