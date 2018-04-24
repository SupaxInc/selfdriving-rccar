using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System;
using System.IO;
using System.Threading;
using System.Globalization;

public class PauseMenu : MonoBehaviour
{

    public static bool isGamePaused = false;

    public LevelManager level;
    public SpeciesController speciesController;

    public Button btnResume;
    public Button btnPause;
    public Button btnChangeAI;
    public Button btnChangeCar;
    public Button btnQuit;

    [Header("AI Settings")]
    public GameObject pauseMenuAISettings;
    public TMP_Text txtTresholdGen;
    public TMP_Text txtMaxTime;
    public TMP_Text txtMutationRate;
    public TMP_Text txtLearningRate;
    public TMP_Text txtPercentToKeep;
    public TMP_Text txtPenalizeDeath;
    public TMP_Text txtTimeMatters;
    public TMP_Text txtFileIO;
    public TMP_Text txtChangeFilePath;
    public TMP_InputField inputChangeThresholdGen;
    public TMP_InputField inputChangeMaxTime;
    public TMP_InputField inputChangeMutationRate;
    public TMP_InputField inputChangeLearningRate;
    public TMP_InputField inputChangePercentToKeep;
    public TMP_InputField inputChangeFilePath;
    public Toggle togglePenalizeDeath;
    public Toggle toggleTimeMatters;
    public Toggle toggleFileIO;
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

    public GameObject pauseMenu;
    public GameObject[] carSpecies;

    public int popSize;

    private void Start()
    {
        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        level = FindObjectOfType<LevelManager>();
        speciesController = FindObjectOfType<SpeciesController>();
        popSize = level.GetPopulation();
        carSpecies = speciesController.GetCarSpecies();
        pauseMenuAISettings.SetActive(false);
        pauseMenuCarSettings.SetActive(false);
        toggleFileIO.onValueChanged.AddListener(toggle_fileIO);
        toggle_fileIO(toggleFileIO.isOn);
    }

    // Use this for initialization
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isGamePaused)
            {
                btn_ResumeGame();
            }
            else
            {
                btn_PauseGame();
            }
        }
    }

    // Update is called once per frame
    public void btn_ResumeGame()
    {
        Time.timeScale = 1.0f;
        pauseMenu.SetActive(false);
        isGamePaused = false;
    }

    public void toggle_fileIO(bool isOn)
    {
        if (isOn)
        {
            txtChangeFilePath.gameObject.SetActive(true);
            inputChangeFilePath.gameObject.SetActive(true);
            txtChangeFilePath.text = string.Format("File Path: {0}", level.FilePath);
        }
        else
        {
            txtChangeFilePath.gameObject.SetActive(false);
            inputChangeFilePath.gameObject.SetActive(false);
        }
    }

    public void btn_ChangeAISettings()
    {
        btnResume.gameObject.SetActive(false);
        btnChangeAI.gameObject.SetActive(false);
        btnChangeCar.gameObject.SetActive(false);
        btnQuit.gameObject.SetActive(false);
        pauseMenuAISettings.SetActive(true);
        togglePenalizeDeath.isOn = level.penalizeDeath;
        toggleTimeMatters.isOn = level.timeMattersAtTheBeginning;
        toggleFileIO.isOn = speciesController.fileIO_toggled;
        txtTresholdGen.text = string.Format("Max Threshold Gen: {0}", level.thresholdGeneration);
        txtMaxTime.text = string.Format("Max Time: {0}", level.MaxTime);
        txtMutationRate.text = string.Format("Mutation Rate: {0:0.00}", speciesController.mutationRate);
        txtLearningRate.text = string.Format("Learning Rate: {0}", level.learningRate);
        txtPercentToKeep.text = string.Format("Precent to Keep: {0}", level.percentToKeep);
        txtPenalizeDeath.text = string.Format("Penalize Death: {0}", level.penalizeDeath);
        txtTimeMatters.text = string.Format("Time Matters @ Beginning: {0}", level.timeMattersAtTheBeginning);
        txtFileIO.text = string.Format("Enable File Saving: {0}", speciesController.fileIO_toggled);
    }

    public void btn_ChangeCarSettings()
    {
        btnResume.gameObject.SetActive(false);
        btnChangeAI.gameObject.SetActive(false);
        btnChangeCar.gameObject.SetActive(false);
        btnQuit.gameObject.SetActive(false);
        pauseMenuCarSettings.SetActive(true);
        txtSlipperyForce.text = string.Format("Max Slippery Drifting Force: {0}", carSpecies[0].GetComponent<Car2DController>().driftingForceSlippery);
        txtStickyForce.text = string.Format("Max Sticky Drifting Force: {0}", carSpecies[0].GetComponent<Car2DController>().driftingForceSticky);
        txtStickyVelocity.text = string.Format("Max Sticky Velocity: {0}", carSpecies[0].GetComponent<Car2DController>().maxStickyVelocity);
        txtSpeedForce.text = string.Format("Speed Force: {0}", carSpecies[0].GetComponent<Car2DController>().speedForce);
        txtTorqueForce.text = string.Format("Torque Force: {0}", carSpecies[0].GetComponent<Car2DController>().torqueForce);
        txtSensorLength.text = string.Format("Sensor Length: {0}", carSpecies[0].GetComponent<Car2DController>().sensorLength);
    }

    public void btn_PauseGame()
    {
        Time.timeScale = 0.0f;
        pauseMenu.SetActive(true);
        isGamePaused = true;
        carSpecies = speciesController.GetCarSpecies();
    }

    public void btn_QuitGame()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("MainMenu");
    }

    public void btn_AIBack()
    {
        btnResume.gameObject.SetActive(true);
        btnChangeAI.gameObject.SetActive(true);
        btnChangeCar.gameObject.SetActive(true);
        btnQuit.gameObject.SetActive(true);
        pauseMenuAISettings.SetActive(false);
    }

    public void btn_CarBack()
    {
        btnResume.gameObject.SetActive(true);
        btnChangeAI.gameObject.SetActive(true);
        btnChangeCar.gameObject.SetActive(true);
        btnQuit.gameObject.SetActive(true);
        pauseMenuCarSettings.SetActive(false);
    }

    public void btn_ApplyAIChanges()
    {
        try
        {
            if (!string.IsNullOrEmpty(inputChangeThresholdGen.text))
                level.thresholdGeneration = int.Parse(inputChangeThresholdGen.text);
            if (!string.IsNullOrEmpty(inputChangeMaxTime.text))
                level.MaxTime = float.Parse(inputChangeMaxTime.text);
            if (!string.IsNullOrEmpty(inputChangeMutationRate.text))
                speciesController.mutationRate = float.Parse(inputChangeMutationRate.text);
            if (!string.IsNullOrEmpty(inputChangeLearningRate.text))
                level.learningRate = double.Parse(inputChangeLearningRate.text);
            if (!string.IsNullOrEmpty(inputChangePercentToKeep.text))
                level.percentToKeep = double.Parse(inputChangePercentToKeep.text);
            if (!string.IsNullOrEmpty(inputChangeFilePath.text))
            {
                level.FilePath = inputChangeFilePath.text;
                if (!Directory.Exists(level.FilePath))
                {
                    throw new Exception(string.Format("Directory '{0}' does not exist.", level.FilePath));
                }
            }
            level.penalizeDeath = togglePenalizeDeath.isOn;
            level.timeMattersAtTheBeginning = toggleTimeMatters.isOn;
            level.ToggleFileIO = speciesController.fileIO_toggled = toggleFileIO.isOn;
            txtTresholdGen.text = string.Format("Max Threshold Gen: {0}", level.thresholdGeneration);
            txtMaxTime.text = string.Format("Max Time: {0}", level.MaxTime);
            txtMutationRate.text = string.Format("Mutation Rate: {0:0.00}", speciesController.mutationRate);
            txtLearningRate.text = string.Format("Learning Rate: {0}", level.learningRate);
            txtPercentToKeep.text = string.Format("Precent to Keep: {0}", level.percentToKeep);
            txtPenalizeDeath.text = string.Format("Penalize Death: {0}", level.penalizeDeath);
            txtTimeMatters.text = string.Format("Time Matters @ Beginning: {0}", level.timeMattersAtTheBeginning);
            txtFileIO.text = string.Format("Enable File Saving: {0}", speciesController.fileIO_toggled);
            if (toggleFileIO.isOn)
            {
                txtChangeFilePath.text = string.Format("File Path: {0}", level.FilePath);
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    public void btn_ApplyCarChanges()
    {
        try
        {
            for (int i = 0; i < speciesController.popSize; i++)
            {
                if (!string.IsNullOrEmpty(inputChangeMaxSlipperyForce.text))
                    carSpecies[i].GetComponent<Car2DController>().driftingForceSlippery = float.Parse(inputChangeMaxSlipperyForce.text);
                if (!string.IsNullOrEmpty(inputChangeMaxStickyForce.text))
                    carSpecies[i].GetComponent<Car2DController>().driftingForceSticky = float.Parse(inputChangeMaxStickyForce.text);
                if (!string.IsNullOrEmpty(inputChangeMaxStickyVelocity.text))
                    carSpecies[i].GetComponent<Car2DController>().maxStickyVelocity = float.Parse(inputChangeMaxStickyVelocity.text);
                if (!string.IsNullOrEmpty(inputChangeSpeedForce.text))
                    carSpecies[i].GetComponent<Car2DController>().speedForce = float.Parse(inputChangeSpeedForce.text);
                if (!string.IsNullOrEmpty(inputChangeTorqueForce.text))
                    carSpecies[i].GetComponent<Car2DController>().torqueForce = float.Parse(inputChangeTorqueForce.text);
                if (!string.IsNullOrEmpty(inputChangeSensorLength.text))
                    carSpecies[i].GetComponent<Car2DController>().sensorLength = float.Parse(inputChangeSensorLength.text);
            }
            txtSlipperyForce.text = string.Format("Max Slippery Drifting Force: {0}", carSpecies[0].GetComponent<Car2DController>().driftingForceSlippery);
            txtStickyForce.text = string.Format("Max Sticky Drifting Force: {0}", carSpecies[0].GetComponent<Car2DController>().driftingForceSticky);
            txtStickyVelocity.text = string.Format("Max Sticky Velocity: {0}", carSpecies[0].GetComponent<Car2DController>().maxStickyVelocity);
            txtSpeedForce.text = string.Format("Speed Force: {0}", carSpecies[0].GetComponent<Car2DController>().speedForce);
            txtTorqueForce.text = string.Format("Torque Force: {0}", carSpecies[0].GetComponent<Car2DController>().torqueForce);
            txtSensorLength.text = string.Format("Sensor Length: {0}", carSpecies[0].GetComponent<Car2DController>().sensorLength);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }
}
