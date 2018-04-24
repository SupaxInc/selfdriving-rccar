using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    private SpeciesController speciesController;
    private LevelManager level;
    private GUIManager guiManager;

    public Transform target;
    public GameObject[] carSpecies;

    public int popSize;
    public float distance;
    public static float maxDistance;

	// Use this for initialization
	void Start () {
        guiManager = FindObjectOfType<GUIManager>();
        level = FindObjectOfType<LevelManager>();
        popSize = level.GetPopulation();
        speciesController = FindObjectOfType<SpeciesController>();
        carSpecies = speciesController.GetCarSpecies();
        distance = 0.0f;
        maxDistance = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
        guiManager.currDistance = distance;
        if(speciesController.carsHaveRespawned)
        {
            if (maxDistance < distance)
            {
                maxDistance = distance;
                guiManager.maxDistance = distance;
            }
            distance = 0.0f;
        }
        for (int i = 0; i < popSize; i++)
        {
            if(carSpecies[i].GetComponent<Car2DController>().TravelledDistance > distance)
            {
                distance = carSpecies[i].GetComponent<Car2DController>().TravelledDistance;
                target = carSpecies[i].transform;
                transform.position = new Vector3(target.position.x, target.position.y, -10);
            }
        }
        transform.position = new Vector3(target.position.x, target.position.y, -10);           // Camera follows the objects x,y,z axis
    }
}
