using System;

public class Edge
{
    public double Weight = 0;
    public Edge()
    {
        Weight = UnityEngine.Random.Range(-0.5f, 0.5f);
    }
}
