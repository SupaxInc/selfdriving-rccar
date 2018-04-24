using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {

    public static int numOfPopulation;
    public static float mutationRate;
    public static bool toggleFileIO;
    public static int[] layersInfo;
    public static string filePath;
    public static bool toggleOldPopulations;

    public bool isDebugMode;
    public bool timeMattersAtTheBeginning;
    public bool lessTimeIsBetter;
    public int thresholdGeneration;
    public float MaxTime;
    public double learningRate;
    public double regularizationRate;
    public int lessons;
    public double percentToKeep;
    public double MaxMutationMagnitude;
    public bool manualMode;
    public bool penalizeDeath;

    public bool ToggleOldPopulations
    {
        get { return toggleOldPopulations; }
        set { toggleOldPopulations = value; }
    }


    public string FilePath
    {
        get { return filePath; }
        set { filePath = value; }
    }


    public bool ToggleFileIO
    {
        get { return toggleFileIO; }
        set { toggleFileIO = value; }
    }


    public int[] LayersInfo
    {
        get { return layersInfo; }
        set { layersInfo = value; }
    }


    public void SetMutation(float rate)
    {
        mutationRate = rate;
    }

    public float GetMutation()
    {
        float rate = mutationRate;
        return rate;
    }

    // Use this for initialization


    public void SetPopulation(int population)
    {
        numOfPopulation = population;
    }

    public int GetPopulation()
    {
        int population;
        population = numOfPopulation;
        return population;
    }

	private void Start () {
        isDebugMode = true;

        MaxTime = 60.0f;
        thresholdGeneration = 1000;
        timeMattersAtTheBeginning = false;
        lessTimeIsBetter = true;
        manualMode = false;
        penalizeDeath = true;
        MaxMutationMagnitude = 0.5;
        percentToKeep = 0.5;
        lessons = 1;
        learningRate = 0.1;
        regularizationRate = 0.0;
    }
	
}
