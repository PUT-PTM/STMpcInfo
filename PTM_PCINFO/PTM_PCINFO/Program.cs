using System;
using System.IO.Ports;
using OpenHardwareMonitor.Hardware;
using System.Threading;
using System.Timers;


namespace PTM_PCINFO
{
    class Program
    {

        static void Main(string[] args)
        {
            SystemInfo info = new SystemInfo();
            info.te = new System.Timers.Timer();
            info.te.Interval = 5000;
            info.openPort();
            info.te.Elapsed += new ElapsedEventHandler(info.getInfo);
            info.te.Start();
        }
    }


    public class SystemInfo
    {
        static bool _continue;
        public System.Timers.Timer te;
        static SerialPort _serialPort = new SerialPort("COM3",
            9600, Parity.None, 8, StopBits.One);

        public void openPort()
        {
            Console.WriteLine("Available Ports:");
            foreach (string s in SerialPort.GetPortNames())
            {
                Console.WriteLine("   {0}", s);
            }
            Thread readThread = new Thread(Read);
            _serialPort.ReadTimeout = 500;
            _serialPort.WriteTimeout = 500;           
            _continue = true;
            _serialPort.Open();
            readThread.Start();
        }

        public void getInfo(object sender, EventArgs e)
        {
            Computer computer = new Computer()
            {
                CPUEnabled = true,
                FanControllerEnabled = true,
                GPUEnabled = true,
                MainboardEnabled = true,
                RAMEnabled = true,
                HDDEnabled = true

            };
            computer.Open();
            //int k = 0;
            //while (_continue)
            //{
            foreach (var hardware in computer.Hardware)
            {
                if (hardware.HardwareType == HardwareType.CPU)
                {
                    hardware.Update();
                    //Console.WriteLine("Procesor:");
                    foreach (var sensor in hardware.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Temperature)
                        {
                            _serialPort.WriteLine(sensor.Name + ": " + sensor.Value.GetValueOrDefault() + "°C");
                        }
                        if (sensor.SensorType == SensorType.Load)
                        {
                            _serialPort.WriteLine(sensor.Name + ": " + sensor.Value.GetValueOrDefault() + "°%");
                        }
                        if (sensor.SensorType == SensorType.Clock)
                        {
                            _serialPort.WriteLine(sensor.Name + ": " + sensor.Value.GetValueOrDefault() + "Mhz");
                        }
                    }
                }

                foreach (var subhardware in hardware.SubHardware)
                {
                    subhardware.Update();
                    if (subhardware.Sensors.Length > 0)
                    {
                        foreach (var sensor in subhardware.Sensors)
                        {
                            if (sensor.SensorType == SensorType.Fan && sensor.Name.Equals("Fan #1", StringComparison.OrdinalIgnoreCase))
                            {
                                _serialPort.WriteLine("Prędkość coolera: " + Convert.ToString((int)(float)sensor.Value) + "RPM\n");
                            }
                        }
                    }
                }

                if (hardware.HardwareType == HardwareType.GpuNvidia)
                {
                    hardware.Update();
                    //Console.WriteLine("\nKarta graficzna:");
                    foreach (var sensor in hardware.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Fan)
                        {
                            _serialPort.WriteLine(sensor.Name + ": " + Convert.ToString(sensor.Value) + "RPM");
                        }
                        if (sensor.SensorType == SensorType.Clock)
                        {
                            _serialPort.WriteLine(sensor.Name + ": " + Convert.ToString(sensor.Value) + "MHz");
                        }
                        if (sensor.SensorType == SensorType.Temperature)
                        {
                            _serialPort.WriteLine(sensor.Name + ": " + Convert.ToString(sensor.Value) + "°C");
                        }
                        if (sensor.SensorType == SensorType.Load)
                        {
                            _serialPort.WriteLine(sensor.Name + ": " + Convert.ToString(sensor.Value) + "%");
                        }

                    }
                }

                if (hardware.HardwareType == HardwareType.RAM)
                {
                    hardware.Update();
                    //Console.WriteLine("\nPamięć RAM:");
                    foreach (var sensor in hardware.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Load)
                        {
                            _serialPort.WriteLine(sensor.Name + ": " + Convert.ToString(sensor.Value) + "%");
                        }
                        if (sensor.SensorType == SensorType.Data)
                        {
                            _serialPort.WriteLine(sensor.Name + ": " + Convert.ToString(sensor.Value) + "GB");
                        }
                    }
                }

                if (hardware.HardwareType == HardwareType.HDD)
                {
                    //k++;
                    hardware.Update();
                    //Console.WriteLine("\nDysk twardy " + k + ":");
                    foreach (var sensor in hardware.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Load)
                        {
                            _serialPort.WriteLine(sensor.Name + ": " + Convert.ToString(sensor.Value) + "%");
                        }
                        if (sensor.SensorType == SensorType.Temperature)
                        {
                            _serialPort.WriteLine(sensor.Name + ": " + Convert.ToString(sensor.Value) + "°C");
                        }
                    }
                }

            }
            //readThread.Join();
            //Console.ReadKey();
        }
        //}
        public static void Read()
        {
            while (_continue)
            {
                try
                {
                    if(_serialPort.IsOpen==false)
                    {
                        _serialPort.Open();
                    }
                    string message = _serialPort.ReadLine();
                    Console.WriteLine(message);
                }
                catch (TimeoutException) {
                    _serialPort.Close();
                }
            }
        }

    }
}


