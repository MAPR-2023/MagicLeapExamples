﻿using UnityEngine;
using System;
using System.IO.Ports;
using System.Collections.Concurrent;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class BraccioController : MonoBehaviour
{

    //private SerialPort serialPort;
    //private string portName = "/dev/cu.usbmodem142401";
    //private int baudRate = 115200;
    private ConcurrentQueue<string> messageQueue;  // Thread-safe queue for messages
    //private Thread serialThread;
    private SerialController sc;
    private float progressedTime = 0f;

    public bool wireless = true;

    private Thread braccioWiFiThread;
    private Socket braccioSocket;
    private string braccioIP = "10.0.0.1";
    private int port = 80;

    private void SendThread()
    {
        while (true)
        {
            if (messageQueue.Count != 0)
            {
                string messageToBeSent = "";
                while (messageQueue.TryDequeue(out messageToBeSent))
                {
                    Debug.Log(messageToBeSent);
                    byte[] dataBytes = System.Text.Encoding.ASCII.GetBytes(messageToBeSent + "\n");
                    braccioSocket.Send(dataBytes);
                }
            }
        }
        ;
    }

    void Start()
    {
        messageQueue = new ConcurrentQueue<string>();

        if (wireless)
        {
            // Create a TCP/IP socket
            braccioSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Connect to the server
            braccioSocket.Connect(IPAddress.Parse(braccioIP), port);

            

            // Create braccio WiFi thread
            braccioWiFiThread = new Thread(SendThread);

            // start said thread
            braccioWiFiThread.Start();
        } else
        { 
            sc = GetComponent<SerialController>();
        }

        //serialPort = new SerialPort(portName, baudRate);
        //messageQueue = new Queue<string>(); // Initialize message queue
        //serialThread = new Thread(WriteToSerial); // Create thread for writing
        //serialThread.Start();
        //serialPort.WriteTimeout = 100;

        //try
        //{
        //    serialPort.Open();
        //    Debug.Log("Serial port opened successfully on: " + portName);
        //}
        //catch (System.Exception e)
        //{
        //    Debug.LogError("Error opening serial port: " + e.Message);
        //}
    }

    public void SendSerialMessage(string message)  // Public function to add messages
    {
        if (wireless)
        {
            messageQueue.Enqueue(message);   
        } else
        {
            sc.SendSerialMessage(message);
        }
        //messageQueue.Enqueue(message);
        //messageAvailableEvent.Set();  // Signal the event when a message is added
    }

    //private void WriteToSerial()
    //{
    //    while (true)
    //    {
    //        //messageAvailableEvent.WaitOne(1000);  // Wait on the event

    //        lock (messageQueue)  // Lock queue for thread safety
    //        {
    //            if (messageQueue.Count > 0)
    //            {

    //                string message = messageQueue.Dequeue();
    //                Debug.Log(message);
    //                serialPort.WriteLine(message);

    //            }
    //        }

    //        Thread.Sleep(1000);
    //    }
    //}

    private void Update()
    {
        progressedTime += Time.deltaTime;
        if (progressedTime > 3f) // seconds
        { 
            progressedTime = progressedTime - 3f; //seconds
            string[] portNames = SerialPort.GetPortNames();
            GameObject overviewDescription = GameObject.Find("Text - OverviewDescription");
            Text odText = overviewDescription.GetComponent<Text>();
            string portNameText = "\n portNames: \n" + String.Join("\n\t- ", portNames);
            odText.text = portNameText;
        }
    }

    void OnDisable()
    {
        if (wireless)
        {
            braccioSocket.Close();
            braccioWiFiThread.Abort();
        }
    }

}

