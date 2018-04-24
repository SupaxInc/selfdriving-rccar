using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Globalization;
using System.Threading;
using System;
using System.IO;

public class MainMenu : MonoBehaviour
{

    [Header("Simulation Settings")]
    public InputField populationField;
    public InputField mutationField;
    public InputField layersInfoField;
    public InputField filePathField;
    public TMP_Text txtFilePath;
    public GameObject startSimulationPanel;
    public Toggle toggleFileIO;
    public Toggle toggleOldPopulation;
    public Button btnBegin;

    private LevelManager level;
    public SpeciesController speciesController;

    public Button btnOptionsBack;
    public Button btnChangeAI;
    public Button btnChangeCar;

    public GameObject optionsMenu;

    [Header("AI Settings")]
    public GameObject pauseMenuAISettings;
    public TMP_Text txtTresholdGen;
    public TMP_Text txtMaxTime;
    public TMP_Text txtLearningRate;
    public TMP_Text txtPercentToKeep;
    public TMP_Text txtPenalizeDeath;
    public TMP_Text txtTimeMatters;
    public TMP_InputField inputChangeThresholdGen;
    public TMP_InputField inputChangeMaxTime;
    public TMP_InputField inputChangeLearningRate;
    public TMP_InputField inputChangePercentToKeep;
    public Toggle togglePenalizeDeath;
    public Toggle toggleTimeMatters;
    public Button btnApplyAI;

    [Header("Car Settings")]
    public GameObject pauseMenuCarSettings;
    public TMP_Text txtSlipperyForce;
    public TMP_Text txtStickyForce;
    public TMP_Text txtStickyVelocity;
    public TMP_Text txtSpeedForce;
    public TMP_Text txtTorqueForce;
    public TMP_Text txtSensorLength;
    public TMP_InputField inputChangeMaxSlipperyForce;
    public TMP_InputField inputChangeMaxStickyForce;
    public TMP_InputField inputChangeMaxStickyVelocity;
    public TMP_InputField inputChangeSpeedForce;
    public TMP_InputField inputChangeTorqueForce;
    public TMP_InputField inputChangeSensorLength;
    public Button btnApplyCar;


    private void Awake()
    {
        level = FindObjectOfType<LevelManager>();
    }

    public void Start()
    {
        setSimInactive();
        pauseMenuAISettings.SetActive(false);
        pauseMenuCarSettings.SetActive(false);
        optionsMenu.SetActive(false);
        btnOptionsBack.gameObject.SetActive(false);
        toggleFileIO.onValueChanged.AddListener(toggle_fileIO);
        toggle_fileIO(toggleFileIO.isOn);
    }

    public void toggle_fileIO(bool isOn)
    {
        if(isOn)
        {
            txtFilePath.gameObject.SetActive(true);
            filePathField.gameObject.SetActive(true);
            toggleOldPopulation.gameObject.SetActive(true);
        }
        else
        {
            txtFilePath.gameObject.SetActive(false);
            filePathField.gameObject.SetActive(false);
            toggleOldPopulation.gameObject.SetActive(false);
        }
    }

    public void OnSubmit()
    {
        try
        {
            string inputString = layersInfoField.text;
            if (!string.IsNullOrEmpty(inputString))
            {
                try
                {
                    string[] inputData = inputString.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    level.LayersInfo = new int[inputData.Length];
                    for (int inputStr = 0; inputStr < inputData.Length; inputStr++)
                    {
                        level.LayersInfo[inputStr] = int.Parse(inputData[inputStr]);
                    }
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                    level.LayersInfo = new int[4];
                    level.LayersInfo[0] = 7;
                    level.LayersInfo[1] = 15;
                    level.LayersInfo[2] = 5;
                    level.LayersInfo[3] = 2;
                }
            }
            else
            {
                level.LayersInfo = new int[4];
                level.LayersInfo[0] = 7;
                level.LayersInfo[1] = 15;
                level.LayersInfo[2] = 5;
                level.LayersInfo[3] = 2;
            }
            level.ToggleFileIO = toggleFileIO.isOn;
            level.ToggleOldPopulations = toggleOldPopulation.isOn;
            if (!string.IsNullOrEmpty(filePathField.text))
            {
                level.FilePath = filePathField.text;
                if (!Directory.Exists(level.FilePath))
                {
                    throw new Exception(string.Format("Directory '{0}' does not exist.", level.FilePath));
                }
            }
            //Debug.Log(string.Format("{0}, {1}, {2}, {3}", level.LayersInfo[0], level.LayersInfo[1], level.LayersInfo[2], level.LayersInfo[3]));
            level.SetPopulation(int.Parse(populationField.text));
            level.SetMutation(float.Parse(mutationField.text));
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);       // loads index 1 of the Scene List in Build settings
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    public void btn_changeAISettings()
    {
        btnOptionsBack.gameObject.SetActive(false);
        btnChangeAI.gameObject.SetActive(false);
        btnChangeCar.gameObject.SetActive(false);
        pauseMenuAISettings.SetActive(true);
        txtTresholdGen.text = string.Format("Max Threshold Gen: {0}", level.thresholdGeneration);
        txtMaxTime.text = string.Format("Max Time: {0}", level.MaxTime);
        txtLearningRate.text = string.Format("Learning Rate: {0}", level.learningRate);
        txtPercentToKeep.text = string.Format("Precent to Keep: {0}", level.percentToKeep);
        txtPenalizeDeath.text = string.Format("Penalize Death: {0}", level.penalizeDeath);
        txtTimeMatters.text = string.Format("Time Matters @ Beginning: {0}", level.timeMattersAtTheBeginning);
    }

    public void btn_applyAI()
    {
        try
        {
            if (!string.IsNullOrEmpty(inputChangeThresholdGen.text))
                level.thresholdGeneration = int.Parse(inputChangeThresholdGen.text);
            if (!string.IsNullOrEmpty(inputChangeMaxTime.text))
                level.MaxTime = float.Parse(inputChangeMaxTime.text);
            if (!string.IsNullOrEmpty(inputChangeLearningRate.text))
                level.learningRate = double.Parse(inputChangeLearningRate.text);
            if (!string.IsNullOrEmpty(inputChangePercentToKeep.text))
                level.percentToKeep = double.Parse(inputChangePercentToKeep.text);
            level.penalizeDeath = togglePenalizeDeath.isOn;
            level.timeMattersAtTheBeginning = toggleTimeMatters.isOn;
            txtTresholdGen.text = string.Format("Max Threshold Gen: {0}", level.thresholdGeneration);
            txtMaxTime.text = string.Format("Max Time: {0}", level.MaxTime);
            txtLearningRate.text = string.Format("Learning Rate: {0}", level.learningRate);
            txtPercentToKeep.text = string.Format("Precent to Keep: {0}", level.percentToKeep);
            txtPenalizeDeath.text = string.Format("Penalize Death: {0}", level.penalizeDeath);
            txtTimeMatters.text = string.Format("Time Matters @ Beginning: {0}", level.timeMattersAtTheBeginning);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    public void btn_AIBack()
    {
        btnChangeAI.gameObject.SetActive(true);
        btnChangeCar.gameObject.SetActive(true);
        btnOptionsBack.gameObject.SetActive(true);
        pauseMenuAISettings.SetActive(false);
    }

    public void btn_Options()
    {
        btnOptionsBack.gameObject.SetActive(true);
        optionsMenu.SetActive(true);
    }

    public void btn_backOptions()
    {
        optionsMenu.SetActive(false);
    }

    public void setStartSimActive()
    {
        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        startSimulationPanel.SetActive(true);
        populationField.gameObject.SetActive(true);
        mutationField.gameObject.SetActive(true);
        layersInfoField.gameObject.SetActive(true);
        toggleFileIO.gameObject.SetActive(true);
        btnBegin.gameObject.SetActive(true);
    }

    public void setSimInactive()
    {
        startSimulationPanel.SetActive(false);
        populationField.gameObject.SetActive(false);
        mutationField.gameObject.SetActive(false);
        layersInfoField.gameObject.SetActive(false);
        toggleFileIO.gameObject.SetActive(false);
        btnBegin.gameObject.SetActive(false);
    }



    public void quitSimulation()
    {
        Application.Quit();
    }



}
