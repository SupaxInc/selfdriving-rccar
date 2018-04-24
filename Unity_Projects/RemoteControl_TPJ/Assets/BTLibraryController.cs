using System.Collections;
using System.Collections.Generic;
using TechTweaking;
using TechTweaking.Bluetooth;
using TechTweaking.BtCore;
using UnityEngine;

public class BTLibraryController : MonoBehaviour {

    public GUIManager guiManager;
    public Controller joystickController;

    public BluetoothDevice device;
    public BluetoothDevice[] pairedDevices;

    public string connectedMacAddress;
    public List<string> deviceList;
    public List<string> btDeviceMACAddress;
    public bool autoPilotMode;
    public float elapsedTime;
    public float changedTime;

	// Use this for initialization
	void Start () {
        autoPilotMode = false;
        device = new BluetoothDevice();
        joystickController = FindObjectOfType<Controller>();
        guiManager = FindObjectOfType<GUIManager>();
        btDeviceMACAddress = new List<string>();
        deviceList = new List<string>();
        btDeviceMACAddress.Add("PlaceholderIndex0");
        BluetoothAdapter.askEnableBluetooth();
        BluetoothAdapter.OnDeviceDiscovered += HandleOnDeviceDiscovered;
        BluetoothAdapter.OnDiscoveryFinished += OnDiscoveryFinished;
        BluetoothAdapter.OnConnected += HandleOnConnected;
        BluetoothAdapter.OnDisconnected += HandleOnDisconnected;
        elapsedTime = 0.0f;
    }


    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime > changedTime)
        {
            elapsedTime = 0.0f;
            if (device.IsConnected)
            {
                if (!autoPilotMode)
                {
                    string data = " Vertical:" + joystickController.vertical.Vertical().ToString() + " Horizontal:" + joystickController.horizontal.Horizontal().ToString() + " ";
                    device.send(System.Text.Encoding.ASCII.GetBytes(data));

                    //device.send(System.Text.Encoding.ASCII.GetBytes(" Vertical:" + joystickController.vertical.Vertical().ToString() + " "));
                    //device.send(System.Text.Encoding.ASCII.GetBytes(" Horizontal:" + joystickController.horizontal.Horizontal().ToString() + " "));
                }
                else
                {
                    device.send(System.Text.Encoding.ASCII.GetBytes(" AUTOMATIC "));
                }
            }
        }
    }

    public void startDiscovery()
    {
        BluetoothAdapter.refreshDiscovery();        // cancels ongoing discovery and starts a new one
        guiManager.startScanningTxt.text = "Scanning... Please wait...";
    }
    

    public void ConnectToDevice(int index)
    {
        device.MacAddress = btDeviceMACAddress[index];
        //device.setEndByte(255);     // packetizes data 
        //device.ReadingCoroutine = ManageConnection;     // IEnumerator starts when device is ready for reading
        device.connect();       // Connects the device
        connectedMacAddress = btDeviceMACAddress[index];

    }

    public void getPairedDevices()
    {
        pairedDevices = BluetoothAdapter.getPairedDevices();
        foreach (BluetoothDevice dev in pairedDevices)
        {
            btDeviceMACAddress.Add(dev.MacAddress);
            deviceList.Add((dev.Name + " [" + dev.MacAddress + "] "));
        }
        guiManager.devices = deviceList;
        guiManager.discoveryFinished = true;
    }

    private void HandleOnConnected(BluetoothDevice dev)
    {
        if (dev.IsConnected)
        {
            guiManager.statusTxt.text = string.Format("Status: Connected to the device [{0}] AUTOPILOT MODE OFF", connectedMacAddress);
            device.send(System.Text.Encoding.ASCII.GetBytes(" MANUAL "));
            guiManager.stopBtn.gameObject.SetActive(true);
            guiManager.autopilotToggle.gameObject.SetActive(true);
        }
    }

    private void HandleOnDisconnected(BluetoothDevice dev)
    {
        guiManager.statusTxt.text = "Status: Stopped connection with device, please scan again.";
        deviceList.Clear();
        btDeviceMACAddress.Clear();
        guiManager.devices.Clear();
        btDeviceMACAddress.Add("PlaceholderIndex0");
        guiManager.discoveryFinished = false;
    }

    private void HandleOnDeviceDiscovered(BluetoothDevice dev, short rssi)
    {
        btDeviceMACAddress.Add(dev.MacAddress);
        deviceList.Add(dev.Name + " [" + dev.MacAddress + "] " + rssi.ToString());
    }

    private void OnDiscoveryFinished()
    {
        guiManager.discoveryFinished = true;
        guiManager.devices = deviceList;
    }

    //IEnumerator ManageConnection (BluetoothDevice device)
    //{
    //    while(device.IsConnected && device.IsReading)
    //    {
    //        BtPackets packets = device.readAllPackets();        // starts reading next available packets

    //        if(packets != null)
    //        {
    //            /* Packets are ordered using index (0, 1, 2, 3, 4, N etc..)
    //             * The Nth packet is the latest packet and 0 is the packet that arrived first
    //             * While loop runs one timer per frame, so we need the Nth packet for latest joy stick position
    //             */
    //            int N = packets.Count - 1;
    //            // To receive a packet we need index and size of it
    //            int index = packets.get_packet_offset_index(N);
    //            int size = packets.get_packet_size(N);
                
    //        }

    //    }
    //    yield return null;
    //}

}
