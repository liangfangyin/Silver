using System;
using System.IO.Ports;

namespace Silver.Basic
{
    /// <summary>
    /// 串口帮助类
    /// </summary>
    public class SerialPortUtil
    {

        private SerialPort serialPort;

        public SerialPortUtil(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits)
        {
            serialPort = new SerialPort(portName, baudRate, parity, dataBits, stopBits);
        }

        public void Open()
        {
            if (!serialPort.IsOpen)
            {
                serialPort.Open();
            }
        }

        public void Close()
        {
            if (serialPort.IsOpen)
            {
                serialPort.Close();
            }
        }

        public void Write(string text)
        {
            serialPort.Write(text);
        }

        public void WriteLine(string text)
        {
            serialPort.WriteLine(text);
        }

        public string ReadLine()
        {
            return serialPort.ReadLine();
        }

        /// <summary>
        /// 清空写入缓冲区
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void DiscardInBuffer()
        {
            serialPort.DiscardInBuffer();
        }

        public event SerialDataReceivedEventHandler DataReceived
        {
            add { serialPort.DataReceived += value; }
            remove { serialPort.DataReceived -= value; }
        }

    }
}
