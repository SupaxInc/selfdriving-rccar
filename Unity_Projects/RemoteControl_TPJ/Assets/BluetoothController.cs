using System;
using System.Collections;
using System.Collections.Generic;
//using System.Threading;
//using InTheHand;
//using InTheHand.Net;
//using InTheHand.Net.Bluetooth;
//using InTheHand.Net.Sockets;
//using InTheHand.Net.Ports;
using UnityEngine;

public class BluetoothController : MonoBehaviour {
 //   BluetoothEndPoint localEndpoint;
 //   BluetoothClient localClient;
 //   BluetoothComponent localComponent;
 //   BluetoothDeviceInfo[] devices;

 //   private GUIManager guiManager;

 //   public List<string> deviceList;

 //   // Use this for initialization
 //   void Start () {
 //       deviceList = new List<string>();
 //       guiManager = FindObjectOfType<GUIManager>();
 //       FindBluetoothDevices();
 //   }
	
	//// Update is called once per frame
	//void Update () {
		
	//}

    //public static BluetoothAddress GetBTLocalMacAddress()
    //{
    //    BluetoothRadio myRadio = BluetoothRadio.PrimaryRadio;
    //    if(myRadio == null)
    //    {
    //        Debug.Log("No hardware supported!");
    //        return null;
    //    }
    //    return myRadio.LocalAddress;
    //}

    //private void FindBluetoothDevices()
    //{
    //    try
    //    {
    //        localEndpoint = new BluetoothEndPoint(BluetoothAddress.Parse("9C:65:B0:5E:EC:C4"), BluetoothService.SerialPort);
    //        localClient = new BluetoothClient(localEndpoint);
    //        devices = localClient.DiscoverDevices();
    //        foreach (BluetoothDeviceInfo dev in devices)
    //        {
    //            deviceList.Add(dev.DeviceName + " (" + dev.DeviceAddress);
    //        }
    //        guiManager.devices = deviceList;
    //        guiManager.devicesHaveBeenPaired = true;
    //    }
    //    catch(PlatformNotSupportedException e)
    //    {
    //        Debug.Log(e);
    //        guiManager.error = true;
    //    }
        
        //try
        //{
        //    localClient = new BluetoothClient();   // Represents current device code is running on
        //    localComponent = new BluetoothComponent(localClient);   // Manages device
        //    // Event listeners when devices are discovered
        //    localComponent.DiscoverDevicesProgress += new System.EventHandler<DiscoverDevicesEventArgs>(component_showDiscoverDevicesProgress);
        //    localComponent.DiscoverDevicesComplete += new System.EventHandler<DiscoverDevicesEventArgs>(component_showDeviceDiscoveryHasCompleted);
        //    localComponent.DiscoverDevicesAsync(8, true, true, true, false, localClient);       // Starts finding devices
        //}
        //catch (PlatformNotSupportedException e)
        //{
        //    throw;
        //}
   // }

    //private void component_showDiscoverDevicesProgress(object sender, DiscoverDevicesEventArgs e)
    //{
    //    devices = e.Devices;      // Gets the devices
    //    for (int i = 0; i < e.Devices.Length; i++)
    //    {
    //        if (e.Devices[i].Remembered)            // Checks if devices are remembered
    //        {
    //            deviceList.Add(e.Devices[i].DeviceName + " [" + e.Devices[i].DeviceAddress + "] (Device previously scanned)");      // Adds device in list for dd menu
    //        }
    //        else
    //        {
    //            deviceList.Add(e.Devices[i].DeviceName + " [" + e.Devices[i].DeviceAddress + "] (Device unknown)");         // Adds device in list for dd menu
    //        }
    //    }
    //}

    //private void component_showDeviceDiscoveryHasCompleted(object sender, DiscoverDevicesEventArgs e)
    //{
    //    guiManager.devices = deviceList;
    //    guiManager.devicesHaveBeenPaired = true;     
    //    Debug.Log("Device has connected");
    //}
}
