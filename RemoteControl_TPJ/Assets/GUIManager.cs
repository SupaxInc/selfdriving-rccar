using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour {

    public BTLibraryController btController;

    public Text startScanningTxt;
    public Text statusTxt;
    public Text connDeviceName;
    public Text scrollText;
    public Dropdown dd;
    public Button discoveryBtn;
    public Button stopBtn;
    public Button pairBtn;
    public Toggle autopilotToggle;
    public Scrollbar scrollTime;
    
    public List<string> devices;
    public bool discoveryFinished;

	// Use this for initialization
	void Start () {
        scrollTime.value = 0.0f;
        statusTxt.text = "Status: No device has been connected, please scan for devices in range.";
        stopBtn.gameObject.SetActive(false);
        autopilotToggle.gameObject.SetActive(false);
        discoveryFinished = false;
        devices = new List<string>();
        btController = FindObjectOfType<BTLibraryController>();
        scrollTime.onValueChanged.AddListener(UpdateTimeScale);
        UpdateTimeScale(scrollTime.value);
    }

	// Update is called once per frame
	void Update ()
    {
        if (discoveryFinished)
        {
            startScanningTxt.text = "";
            dd.AddOptions(devices);
            discoveryFinished = false;
        }

    }

    public void getPairedDevices()
    {
        btController.getPairedDevices();
    }

    public void sendAutopilotMode()
    {
        if (btController.device.IsConnected)
        {
            if (autopilotToggle.isOn)
            {
                btController.autoPilotMode = true;
                statusTxt.text = string.Format("Status: Connected to the device [{0}] AUTOPILOT MODE ON", btController.connectedMacAddress);
                btController.device.send(System.Text.Encoding.ASCII.GetBytes(" AUTOMATIC "));
            }
            else if (!autopilotToggle.isOn)
            {
                btController.autoPilotMode = false;
                statusTxt.text = string.Format("Status: Connected to the device [{0}] AUTOPILOT MODE OFF", btController.connectedMacAddress);
                btController.device.send(System.Text.Encoding.ASCII.GetBytes(" MANUAL "));
            }
        }
    }

    public void StartSearchingForDevices()
    {
        btController.startDiscovery();
    }

    public void stopDeviceConnection()
    {
        btController.device.close();
        stopBtn.gameObject.SetActive(false);
        autopilotToggle.gameObject.SetActive(false);
        dd.value = 0;
    }

    public void dd_IndexChanged(int index)
    {
        btController.ConnectToDevice(index);
    }

    public void UpdateTimeScale(float value)
    {
        btController.changedTime = value;
        scrollText.text = string.Format("Delay: {0} second(s)", value);
    }
}
