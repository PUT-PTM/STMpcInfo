using System;
using System.IO.Ports;
using OpenHardwareMonitor.Hardware;
using System.Threading;



namespace PTM_PCINFO
{
    class Program
    {

        static void Main(string[] args)
        {
            SystemInfo info = new SystemInfo();
            info.getInfo();
            Console.ReadLine();
        }

    }


    public class SystemInfo
    {
        static bool _continue;
        static SerialPort _serialPort = new SerialPort("COM6",
            9600, Parity.None, 8, StopBits.One);

        public void getInfo()
        {
            Console.WriteLine("Available Ports:");
            foreach (string s in SerialPort.GetPortNames())
            {
                Console.WriteLine("   {0}", s);
            }

            Thread readThread = new Thread(Read);
            _serialPort.ReadTimeout = 500;
            _serialPort.WriteTimeout = 500;
            _serialPort.Open();
            _continue = true;
            readThread.Start();

            
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

            int k = 0;
            while (_continue)
            {
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
                        k++;
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
                readThread.Join();
            }

            _serialPort.Close();
        }
        public static void Read()
        {
            while (_continue)
            {
                try
                {
                    string message = _serialPort.ReadLine();
                    Console.WriteLine(message);
                }
                catch (TimeoutException) { }
            }
        }

    }
}


