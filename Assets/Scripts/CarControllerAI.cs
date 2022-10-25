using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarControllerAI : MonoBehaviour {
    public List<GameObject> cars;
    public List<GameObject> carRanking;
    public int population = 20;
    public int generation = 0;
    public GameObject car;
    [HideInInspector]
    public DNA winner;
    public DNA secWinner;
    private int carsCreated = 0;
    // Use this for initialization
    void Start () {
        newPopulation();
		
	}
	
	// Update is called once per frame
	void Update () {
		if (cars.Count == 0)
        {
            winner = carRanking[carRanking.Count - 1].GetComponent<Car>().getDNA();
            secWinner = carRanking[carRanking.Count - 2].GetComponent<Car>().getDNA();
            for (int i = 0; i < carRanking.Count; ++i)
            {
                Destroy(carRanking[i]);
            }
            carRanking.Clear();
            newPopulation(true);
        }
	}
    public List<GameObject> getCars()
    {
        return cars;
    }

    public List<GameObject> getCarRanking()
    {
        return carRanking;
    }

    public void newPopulation()
    {
        cars = new List<GameObject>();
        for(int i = 0; i < population; i++)
        {
            GameObject carObj = (Instantiate(car));
            cars.Add(carObj);
            carObj.GetComponent<Car>().Initialize();
        }
        generation++;
        Debug.Log(generation);
    }
    public void newPopulation(bool geneticManipulation)
    {
        if (geneticManipulation)
        {
            cars = new List<GameObject>();
            for(int i = 0; i < population; i++)
            {
                DNA dna = winner.crossover(secWinner);
                DNA mutated = dna.mutate();
                GameObject carObj = Instantiate(car);
                cars.Add(carObj);
                carObj.GetComponent<Car>().Initialize(mutated);
            }
        }
        generation++;
        carsCreated = 0;
    }
    public void restartGeneration()
    {
        cars.Clear();
        newPopulation();
    }
}
