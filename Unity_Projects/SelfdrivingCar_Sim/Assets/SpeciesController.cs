using System.Threading;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MachineLearning;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.AccessControl;
using System.Security.Principal;

public class SpeciesController : MonoBehaviour
{
    public GameObject carSpeciesPrefab;             // able to get component from the scene
    public GameObject[] carSpecies;                 // uses the carSpeciesPrefab component from the scene and creates a list of multiple of them
    public GameObject bestCar;

    public Population<Species> population;

    public GUIManager guiManager;

    private LevelManager level;
    private Respawn respawn;

    public Transform spawnPoint;                    // able to get the component of the Respawn Point from the scene
    public Rigidbody2D[] rbCar;

    public int popSize;
    public double mutationRate;
    public bool fileIO_toggled;
    public float elapsedTime;
    public bool carsHaveRespawned;

    public GameObject[] GetCarSpecies()
    {
        return carSpecies;
    }

    // Use this for initialization
    void Start()
    {
        carsHaveRespawned = false;
        guiManager = FindObjectOfType<GUIManager>();

        elapsedTime = 0.0f;

        level = FindObjectOfType<LevelManager>();           // makes it so were able to access the LevelManager script
        respawn = FindObjectOfType<Respawn>();

        //Loading populatio from the file.
        //Debug.Log(level.ToggleFileIO);
        fileIO_toggled = level.ToggleFileIO;
        if (level.ToggleFileIO && level.ToggleOldPopulations)
        {
            using (Stream stream = File.Open(Path.Combine(level.FilePath, "population.bin"), FileMode.Open))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                population = binaryFormatter.Deserialize(stream) as Population<Species>;
            }
        }
        else
        {
            population = new Population<Species>(level.GetPopulation(), Functions.ActivationFunction.Tanh, level.LayersInfo.ToArray());
        }
        popSize = population.Size;
        population.Crossover = Functions.CrossoverType.Uniform;
        population.Mutation = Functions.MutationType.Gaussian;
        population.MutationRate = mutationRate = level.GetMutation();
        population.MaxMutationMagnitude = level.MaxMutationMagnitude;
        population.PercentToKeep = level.percentToKeep;
        spawnPoint = GameObject.Find("Respawn Point").transform;            // finds the game object called Respawn Point and gets its transform
        carSpecies = new GameObject[popSize];
        rbCar = new Rigidbody2D[popSize];
        for (int i = 0; i < popSize; i++)         // clones how many species specified by the user from the main menu
        {
            carSpecies[i] = Instantiate(carSpeciesPrefab, spawnPoint.position, spawnPoint.rotation);
            carSpecies[i].GetComponent<Respawn>().respawnPoint = spawnPoint;
            Physics2D.IgnoreLayerCollision(9, 9);           // ignores layer 9 which is "Car" layer
            rbCar[i] = carSpecies[i].GetComponent<Rigidbody2D>();               // Gets each cloned carSpecies' Rigidbody2D
        }
        bestCar = Instantiate(carSpeciesPrefab, spawnPoint.position, spawnPoint.rotation);
        bestCar.GetComponent<Respawn>().respawnPoint = spawnPoint;
        bestCar.GetComponent<SpriteRenderer>().color = new Color32(0, 255, 0, 255);
        AssignBrains();
    }

    // Update is called once per frame
    void Update()
    {
        //    level.MaxTime = (float)Functions.Map(population.CurrentGeneration, 0, 1000, 60, 300);
        population.MutationRate = mutationRate;
        population.MaxMutationMagnitude = level.MaxMutationMagnitude;
        population.PercentToKeep = level.percentToKeep;
        carsHaveRespawned = false;
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= level.MaxTime)
        {
            RespawnAllCars();
            // population.NumberOfGenerations;                    // gets number of generations
            //Thread t = new Thread(() =>
            //{
            //    OracleConnection conn = new OracleConnection(String.Format("Data Source=Neptune; User Id={0}; Password={1}", "ols655_181b10", "27934299"));
            //    //int num;
            //    //OracleCommand cmd = new OracleCommand("SELECT COUNT(*) FROM DEPT", conn);
            //    OracleCommand cmd = new OracleCommand("INSERT INTO CAMP VALUES(:gen, :camp)", conn);
            //    cmd.Parameters.Add(new OracleParameter(":gen", population.CurrentGeneration));
            //    cmd.Parameters.Add(new OracleParameter(":camp", "SelfDriving"));

            //    conn.Open();
            //    try
            //    {
            //        cmd.ExecuteNonQuery();
            //        //object total = cmd.ExecuteScalar();
            //        //if (Convert.IsDBNull(total))
            //        //    num = 0;
            //        //else
            //        //    num = Convert.ToInt32(total);
            //    }
            //    finally
            //    {
            //        conn.Close();
            //    }
            //    //Debug.Log(num);
            //});
            //t.Start();
        }
        else  //If generation had not finished yet.
        {
            bool allDead = true;
            for (int carIndex = 0; carIndex < popSize; carIndex++)
            {
                GameObject car2D = carSpecies[carIndex];
                if (car2D.GetComponent<Respawn>().isAlive)
                {
                    allDead = false;

                    //Species brain = population.Members[carIndex];
                    //List<double> data = car2D.GetComponent<Car2DController>().GetSensorData().ToList();       // list could be able to be appended, and data needs to be appended
                    //data.Add(car2D.GetComponent<Car2DController>().Speed);

                    //float userSpeed = Input.GetAxis("Vertical");
                    //float userDirection = Input.GetAxis("Horizontal");
                    //if ((userDirection != 0.0 || userSpeed != 0.0) || manualMode)
                    //{
                    //    //Debug.Log(userSpeed + " " + userDirection);
                    //    double[] userInput = new double[] { userSpeed == 0.0f ? car2D.GetComponent<Car2DController>().Speed : userSpeed, userDirection == 0 ? rbCar[carIndex].angularVelocity / 100.0 : userDirection};
                    //    for (int i = 0; i < lessons; i++)
                    //    {
                    //        brain.Train(data.ToArray(), userInput, learningRate, regularizationRate);
                    //    }
                    //    car2D.GetComponent<Car2DController>().changeSpeed(userSpeed);
                    //    car2D.GetComponent<Car2DController>().changeAngle(userDirection);
                    //}
                    //else
                    //{
                    //    double[] newVelAngle = brain.Guess(data.ToArray());
                    //    float nnSpeed = (float)newVelAngle[0];
                    //    float nnDirection = (float)newVelAngle[1];
                    //    //Debug.Log(userSpeed + " " + userDirection);
                    //    car2D.GetComponent<Car2DController>().changeSpeed(nnSpeed);           // Thinking that index 0 is speed for NN
                    //    car2D.GetComponent<Car2DController>().changeAngle(nnDirection);           // Thinking that index 1 is angle for NN
                    //}
                    //brain.Fitness += speed;     // change this based on travelled distance / time with emphasis on positive speed.
                    car2D.GetComponent<Car2DController>().TimeAlive += Time.deltaTime;
                }
            }

            if (allDead)
            {
                RespawnAllCars();
            }
        }
    }

    private void RespawnAllCars()
    {
        for (int carIndex = 0; carIndex < popSize; carIndex++)
        {
            GameObject carS = carSpecies[carIndex];
            Respawn r = carS.GetComponent<Respawn>();
            Car2DController c2Dc = carS.GetComponent<Car2DController>();
            //float distanceFitness = Mathf.Pow(1.1f, c2Dc.TravelledDistance) + Mathf.Pow(1.1f, Vector2.Distance(rbCar[carIndex].position, r.respawnPoint.position));
            float distanceFitness = c2Dc.TravelledDistance + Vector2.Distance(rbCar[carIndex].position, r.respawnPoint.position);
            population.Members[carIndex].Fitness += distanceFitness;
            if (!r.isAlive && level.penalizeDeath)
            {
                //double timeFactor = c2Dc.TimeAlive / (1.05 * level.MaxTime);
                //population.Members[carIndex].Fitness *= timeFactor;

                //double timeFactor = Functions.Map(c2Dc.TimeAlive, 0, level.MaxTime, 0.9, 0.95);
                //population.Members[carIndex].Fitness *= timeFactor;

                //double linearCoefficientOfTime = Functions.Line(c2Dc.TimeAlive, 10.0 / level.MaxTime, -5.0);
                //double constraintlinearCoefficientOfTime = Functions.Logistic(linearCoefficientOfTime);
                //population.Members[carIndex].Fitness *= constraintlinearCoefficientOfTime;

                population.Members[carIndex].Fitness *= TimeEffectWithRespectToCurrentGeneration(c2Dc.TimeAlive, population.CurrentGeneration, level.MaxTime, level.thresholdGeneration, level.timeMattersAtTheBeginning, level.lessTimeIsBetter);  //Currently works!

                //double sensorCoefficient = 1 / c2Dc.CumulativeSensoryData;
            }
            //Debug.Log(population.Members[carIndex].Fitness + " of car #" + carIndex.ToString());
            carS.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);
            r.RespawnCar();
        }
        bestCar.GetComponent<Respawn>().RespawnCar();
        bestCar.GetComponent<SpriteRenderer>().color = new Color32(0, 255, 0, 255);
        carsHaveRespawned = true;
        population.SortSpeciesByFitness();
        population.NormalizeFitness();
        population.FilterByPerformance();
        double bestFitness = population.ToBreed();       // make mutation rate dynamic
        guiManager.startTime = Time.time;
        guiManager.numOfGenerations = population.CurrentGeneration;
        guiManager.bestFitness = bestFitness;
        elapsedTime = 0.0f;
        AssignBrains();

        //Every 50 generation saves progress
        if (level.ToggleFileIO)
        {
            if (population.CurrentGeneration % 50 == 0)
            {
                SaveBestBrain("bestBrain.txt", population.BestMember);
                SavePopulationToFile("population.bin");
            }
            Debug.Log(bestFitness);
        }
    }

    public float TimeEffectWithRespectToCurrentGeneration(float time, int generation, float maxTime, int thresholdGeneration, bool timeMattersAtTheBeginning = false, bool lessTimeIsBetter = true)
    {
        double linearCoefficiantOfGeneration = Functions.Line(generation, 14.0 / thresholdGeneration, -7.0);
        if (timeMattersAtTheBeginning)
        {
            linearCoefficiantOfGeneration *= -1.0;
        }
        double constraintGeneration = Functions.Logistic(linearCoefficiantOfGeneration);
        double linearCoefficientOfTime = Functions.Line(time * constraintGeneration, 14.0 / maxTime, -7.0);
        if (lessTimeIsBetter)
        {
            linearCoefficientOfTime *= -1.0;
        }
        double constraintTime = Functions.Logistic(linearCoefficientOfTime);
        return (float)constraintTime;
    }

    public void AssignBrains()
    {
        for (int carIndex = 0; carIndex < popSize; carIndex++)
        {
            carSpecies[carIndex].GetComponent<Car2DController>().brain = population.Members[carIndex];
        }

        bestCar.GetComponent<Car2DController>().brain = population.BestMember;
    }

    public void SaveBestBrain(string fileName, Species brain)
    {
        String dnaStr = "";
        foreach (int layer in population.LayersInfo)
        {
            dnaStr += layer + " ";
        }
        dnaStr += ";";
        List<double> dna = brain.DNA;
        foreach (double gene in dna)
        {
            dnaStr += gene + " ";
        }
        string fullPath = Path.Combine(level.FilePath, fileName);
        File.WriteAllText(fullPath, dnaStr);
    }

    public void SavePopulationToFile(string fileName)
    {
        try
        {
            string fullPath = Path.Combine(level.FilePath, fileName);
            using (Stream stream = File.Open(fullPath, FileMode.Create, FileAccess.Write, FileShare.Write))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(stream, population);
            }
            GrantAccess(fullPath);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            throw e;
        }
    }

    private void GrantAccess(string fullPath)
    {
        DirectoryInfo dInfo = new DirectoryInfo(fullPath);
        DirectorySecurity dSecurity = dInfo.GetAccessControl();
        dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
        dInfo.SetAccessControl(dSecurity);
    }

    void OnApplicationQuit()
    {
        if (level.ToggleFileIO)
        {
            SavePopulationToFile("population.bin");
            SaveBestBrain("bestBrain.txt", population.BestMember);
        }
    }
}