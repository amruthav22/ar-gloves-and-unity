using System;
using System.IO.Ports;
using System.Threading;
using UnityEngine;

public class SimpleSerialPortTest : MonoBehaviour
{
    private SerialPort serialPort;
    private Thread readThread;
    private bool keepReading;
    private string serialData;
    private object lockObject = new object();

    void Start()
    {
        try
        {
            serialPort = new SerialPort("COM5", 9600); 
            serialPort.ReadTimeout = 2000; 
            serialPort.Open();
            serialPort.DiscardInBuffer(); 
            Debug.Log("Serial port opened successfully.");
            keepReading = true;
            readThread = new Thread(ReadSerialPort);
            readThread.Start();
        }
        catch (Exception e)
        {
            Debug.LogError("Error opening serial port: " + e.Message);
        }
    }

    void ReadSerialPort()
    {
        while (keepReading)
        {
            try
            {
                while (serialPort.BytesToRead > 0)
                {
                    int byteReceived = serialPort.ReadByte();
                    char receivedChar = (char)byteReceived;
                    lock (lockObject)
                    {
                        serialData += receivedChar;
                    }
                }
            }
            catch (TimeoutException)
            {
                Debug.LogWarning("Serial read timeout.");
            }
            catch (Exception e)
            {
                Debug.LogError("Error reading serial port: " + e.Message);
            }
        }
    }

    void Update()
    {
        string data = null;
        lock (lockObject)
        {
            data = serialData;
        }

        if (!string.IsNullOrEmpty(data))
        {
            float rotationValue;
            if (float.TryParse(data, out rotationValue))
            {
                Vector3 currentRotation = transform.eulerAngles;
                rotationValue = rotationValue - currentRotation[1];
                transform.Rotate(Vector3.up, rotationValue);
                
                Debug.Log("Rotated by: " + rotationValue + " degrees on the X axis");
  
            }
            else
            {
                Debug.LogWarning("Received data is not a valid float: " + data);
            }

            lock (lockObject)
            {
                serialData = null; 
            }
        }
    }

    void OnApplicationQuit()
    {
        keepReading = false;
        if (readThread != null && readThread.IsAlive)
        {
            readThread.Join();
        }
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
            Debug.Log("Serial port closed.");
        }
    }
}
