using System;
using System.IO.Ports;
using OpenHardwareMonitor.Hardware;
using System.Threading;
using System.Timers;
using System.Collections.Generic;

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
        bool _cpu = true;
        bool _gpu = true;
        bool _hdd = true;
        bool _ram = true;
        public System.Timers.Timer te;
        static SerialPort _serialPort = new SerialPort("COM3",
            9600, Parity.None, 8, StopBits.One);

        public void openPort()
        {
            Console.WriteLine("Dostępne porty:");
            foreach (string s in SerialPort.GetPortNames())
            {
                Console.WriteLine("   {0}", s);
            }
            /*Console.WriteLine("Co chciałbyś wyświetlić?");
            Console.WriteLine("CPU - wpisz 1");
            Console.WriteLine("GPU - wpisz 2");
            Console.WriteLine("RAM - wpisz 3");
            Console.WriteLine("HDD - wpisz 4");
            Console.WriteLine("Wszystkie informacje - wpisz dowolny ciąg znaków");
            string x = Console.ReadLine();
            switch (x)
            {
                case "1":
                    //Console.WriteLine("CPU");
                    _cpu = true;
                    break;
                case "2":
                    //Console.WriteLine("GPU");
                    _gpu = true;
                    break;
                case "3":
                    //Console.WriteLine("RAM");
                    _ram = true;
                    break;
                case "4":
                    //Console.WriteLine("HDD");
                    _hdd = true;
                    break;
                default:
                    //Console.WriteLine("Wszystkie informacje");
                    _gpu = true;
                    _cpu = true;
                    _hdd = true;
                    _ram = true;
                    break;
            }*/
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
            int k = 0;
            Stack<string> stack = new Stack <string>();
            foreach (var hardware in computer.Hardware)
            {
                if (hardware.HardwareType == HardwareType.CPU && _cpu == true)
                {
                    hardware.Update();
                    //Console.WriteLine("Procesor:");
                    foreach (var sensor in hardware.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Temperature)
                        {
                            //stack.Push(sensor.Name + ": " + Convert.ToString((int)(float)sensor.Value.GetValueOrDefault()) + "C");
                            _serialPort.WriteLine(sensor.Name + ": " + Convert.ToString((int)(float)sensor.Value.GetValueOrDefault()) + "C");
                            //k++;
                        }
                        if (sensor.SensorType == SensorType.Load)
                        {
                            //stack.Push(sensor.Name + ": " + Convert.ToString((int)(float)sensor.Value.GetValueOrDefault()) + "%");
                            _serialPort.WriteLine(sensor.Name + ": " + Convert.ToString((int)(float)sensor.Value.GetValueOrDefault()) + "%");
                            //k++;
                        }
                        if (sensor.SensorType == SensorType.Clock)
                        {
                            //stack.Push(sensor.Name + ": " + Convert.ToString((int)(float)sensor.Value.GetValueOrDefault()) + "Mhz");
                            _serialPort.WriteLine(sensor.Name + ": " + Convert.ToString((int)(float)sensor.Value.GetValueOrDefault()) + "Mhz");
                            //k++;
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
                            if (sensor.SensorType == SensorType.Fan && sensor.Name.Equals("Fan #1", StringComparison.OrdinalIgnoreCase) && _cpu == true)
                            {
                                //stack.Push("Cooler speed: " + Convert.ToString((int)(float)sensor.Value) + "RPM");
                                _serialPort.WriteLine("Cooler speed: " + Convert.ToString((int)(float)sensor.Value) + "RPM");
                                //k++;
                            }
                        }
                    }
                }

                if (hardware.HardwareType == HardwareType.GpuNvidia && _gpu == true)
                {
                    hardware.Update();
                    //Console.WriteLine("\nKarta graficzna:");
                    foreach (var sensor in hardware.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Fan)
                        {
                            //stack.Push(sensor.Name + ": " + Convert.ToString((int)(float)sensor.Value) + "RPM");
                            _serialPort.WriteLine(sensor.Name + ": " + Convert.ToString((int)(float)sensor.Value) + "RPM");
                            //k++;
                        }
                        if (sensor.SensorType == SensorType.Clock)
                        {
                            //stack.Push(sensor.Name + ": " + Convert.ToString((int)(float)sensor.Value) + "MHz");
                            _serialPort.WriteLine(sensor.Name + ": " + Convert.ToString((int)(float)sensor.Value) + "MHz");
                            //k++;
                        }
                        if (sensor.SensorType == SensorType.Temperature)
                        {
                            //stack.Push(sensor.Name + ": " + Convert.ToString((int)(float)sensor.Value) + "C");
                            _serialPort.WriteLine(sensor.Name + ": " + Convert.ToString((int)(float)sensor.Value) + "C");
                            //k++;
                        }
                        if (sensor.SensorType == SensorType.Load)
                        {
                            //stack.Push(sensor.Name + ": " + Convert.ToString((int)(float)sensor.Value) + "%");
                            _serialPort.WriteLine(sensor.Name + ": " + Convert.ToString((int)(float)sensor.Value) + "%");
                            //k++;
                        }

                    }
                }

                if (hardware.HardwareType == HardwareType.GpuAti && _gpu == true)
                {
                    hardware.Update();
                    //Console.WriteLine("\nKarta graficzna:");
                    foreach (var sensor in hardware.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Fan)
                        {
                            //stack.Push(sensor.Name + ": " + Convert.ToString((int)(float)sensor.Value) + "RPM");
                            _serialPort.WriteLine(sensor.Name + ": " + Convert.ToString((int)(float)sensor.Value) + "RPM");
                            //k++;
                        }
                        if (sensor.SensorType == SensorType.Clock)
                        {
                            //stack.Push(sensor.Name + ": " + Convert.ToString((int)(float)sensor.Value) + "MHz");
                            _serialPort.WriteLine(sensor.Name + ": " + Convert.ToString((int)(float)sensor.Value) + "MHz");
                            //k++;
                        }
                        if (sensor.SensorType == SensorType.Temperature)
                        {
                            //stack.Push(sensor.Name + ": " + Convert.ToString((int)(float)sensor.Value) + "C");
                            _serialPort.WriteLine(sensor.Name + ": " + Convert.ToString((int)(float)sensor.Value) + "C");
                            //k++;
                        }
                        if (sensor.SensorType == SensorType.Load)
                        {
                            //stack.Push(sensor.Name + ": " + Convert.ToString((int)(float)sensor.Value) + "%");
                            _serialPort.WriteLine(sensor.Name + ": " + Convert.ToString((int)(float)sensor.Value) + "%");
                            //k++;
                        }

                    }
                }

                if (hardware.HardwareType == HardwareType.RAM && _ram == true)
                {
                    hardware.Update();
                    //Console.WriteLine("\nPamięć RAM:");
                    foreach (var sensor in hardware.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Load)
                        {
                            //stack.Push(sensor.Name + ": " + Convert.ToString((int)(float)sensor.Value) + "%");
                            _serialPort.WriteLine(sensor.Name + ": " + Convert.ToString((int)(float)sensor.Value) + "%");
                            //k++;
                        }
                        if (sensor.SensorType == SensorType.Data)
                        {
                            //stack.Push(sensor.Name + ": " + Convert.ToString((int)(float)sensor.Value) + "GB");
                            _serialPort.WriteLine(sensor.Name + ": " + Convert.ToString((int)(float)sensor.Value) + "GB");
                            //k++;
                        }
                    }
                }

                if (hardware.HardwareType == HardwareType.HDD && _hdd == true)
                {
                    //k++;
                    hardware.Update();
                    //Console.WriteLine("\nDysk twardy " + k + ":");
                    foreach (var sensor in hardware.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Load)
                        {
                            //stack.Push(sensor.Name + ": " + Convert.ToString((int)(float)sensor.Value) + "%");
                            _serialPort.WriteLine(sensor.Name + ": " + Convert.ToString((int)(float)sensor.Value) + "%");
                            //k++;
                        }
                        if (sensor.SensorType == SensorType.Temperature)
                        {
                            //stack.Push(sensor.Name + ": " + Convert.ToString((int)(float)sensor.Value) + "C");
                            _serialPort.WriteLine(sensor.Name + ": " + Convert.ToString((int)(float)sensor.Value) + "C");
                            //k++;
                        }
                    }
                }
            }
            //_serialPort.WriteLine(Convert.ToString(k));
            //k = 0;
            /*while(stack.Count!=0)
            {
                _serialPort.WriteLine(stack.Pop());
            }*/
        }

        public static void Read()
        {
            while (_continue)
            {
                try
                {
                    if (_serialPort.IsOpen == false)
                    {
                        _serialPort.Open();
                    }
                    //string message = _serialPort.ReadLine();
                    //Console.WriteLine(message);
                }
                catch (TimeoutException)
                {
                    _serialPort.Close();
                }
            }
        }

    }
}


