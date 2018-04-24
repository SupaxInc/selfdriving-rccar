using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour {

    private LevelManager level;

    private float time;
    public float distance = 0;
    public float startTime;
    public double bestFitness;
    public long numOfGenerations;
    public float currDistance;
    public float maxDistance;
    public float maxTime;

    public Transform carSpecies;
    public Text txtLearningRate;
    public Text txtPopulation;
    public Text timerText;
    public Text generationText;
    public Text bestFitnessText;
    public Text currDistanceText;
    public Text maxDistanceText;
    public Text maxTimeText;
    public Text mutationRateText;

    public Toggle enableMinimap;

    public GameObject Minimap;

    // Use this for initialization

    private void Awake()
    {
        level = FindObjectOfType<LevelManager>();
        txtPopulation.text = string.Format("Population: {0}", level.GetPopulation());
        mutationRateText.text = string.Format("Mutation Rate: {0:0.00}", level.GetMutation());
        txtLearningRate.text = string.Format("Learning Rate: {0}", level.learningRate);
    }

    void Start () {
        startTime = Time.time;
        numOfGenerations = 1;
        bestFitness = 0;
        currDistance = 0.0f;
        maxDistance = 0.0f;
        maxTime = 0.0f;
        enableMinimap.onValueChanged.AddListener(toggle_miniMap);
        toggle_miniMap(enableMinimap.isOn);
    }
	
	// Update is called once per frame
	void Update () {
        time = Time.time - startTime;
        string minutes = ((int)time / 60).ToString();
        string seconds = (time % 60).ToString("f2");
        timerText.text = minutes + ":" + seconds;
        generationText.text = String.Format("Generation: {0}", numOfGenerations);
        bestFitnessText.text = String.Format("Best Fitness: {0:0.00}", bestFitness);
        currDistanceText.text = String.Format("Curr. Distance: {0:0.00}", currDistance);
        maxDistanceText.text = String.Format("Max Distance: {0:0.00}", maxDistance);
        level = FindObjectOfType<LevelManager>();
        txtPopulation.text = string.Format("Population: {0}", level.GetPopulation());
        mutationRateText.text = string.Format("Mutation Rate: {0:0.00}", level.GetMutation());
        txtLearningRate.text = string.Format("Learning Rate: {0}", level.learningRate);
    }

    public void toggle_miniMap(bool isOn)
    {
        if(isOn)
        {
            Minimap.gameObject.SetActive(true);
        }
        else
        {
            Minimap.gameObject.SetActive(false);
        }
    }
}
