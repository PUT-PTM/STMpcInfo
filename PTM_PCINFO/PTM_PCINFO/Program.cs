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
            info.getInfo();
            Console.WriteLine("Rozpoczeto wysylanie");
            info.te.Elapsed += new ElapsedEventHandler(info.getInfo);
            info.te.Start();
        }
    }


    public class SystemInfo
    {
        static int p = 0;
        static bool _continue;
        bool _cpu = true;
        bool _gpu = true;
        bool _hdd = true;
        bool _ram = true;
        public System.Timers.Timer te;
        static SerialPort _serialPort = new SerialPort("COM3",
            9600, Parity.None, 8, StopBits.One);

        static int k = 0;
        static int x = 0;
        static int l = 0;
        static int no_cores = 0;
        static bool cpuTemp, cpuLoad, cpuClock, busLock, fan, gpuFan, gpuMemory, gpuShader, gpuCore, gpuTemp, gcuCore2, gpuContr, gpuVEn, gpuMem, memLoad, memUsed, memAv, hddLoad, hddTemp = false;

        public void openPort()
        {
            Console.WriteLine("Dostępne porty:");
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

        public void getInfo()
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
                            k++;
                            cpuTemp = true;
                        }
                        if (sensor.SensorType == SensorType.Load)
                        {
                            x++;
                            cpuLoad = true;

                        }
                        if (sensor.SensorType == SensorType.Clock && sensor.Name != "Bus Speed")
                        {
                            l++;
                            cpuClock = true;
                        }
                        else if (sensor.SensorType == SensorType.Clock && sensor.Name.Contains("Bus Speed"))
                        {
                            busLock = true;
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
                                fan = true;
                            }
                        }
                    }
                }

                if ((hardware.HardwareType == HardwareType.GpuNvidia || hardware.HardwareType == HardwareType.GpuAti) && _gpu == true)
                {
                    hardware.Update();
                    //Console.WriteLine("\nKarta graficzna:");
                    foreach (var sensor in hardware.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Fan)
                        {
                            gpuFan = true;
                        }

                        if (sensor.SensorType == SensorType.Clock && sensor.Name == "GPU Memory")
                        {
                            gpuMemory = true;
                        }

                        if (sensor.SensorType == SensorType.Clock && sensor.Name == "GPU Shader")
                        {
                            gpuShader = true;
                        }

                        if (sensor.SensorType == SensorType.Clock && sensor.Name == "GPU Core")
                        {
                            gpuCore = true;
                        }

                        if (sensor.SensorType == SensorType.Temperature)
                        {
                            gpuTemp = true;
                        }

                        if (sensor.SensorType == SensorType.Load && sensor.Name == "GPU Core")
                        {
                            gcuCore2 = true;
                        }

                        if (sensor.SensorType == SensorType.Load && sensor.Name == "GPU Memory Controller")
                        {
                            gpuContr = true;
                        }

                        if (sensor.SensorType == SensorType.Load && sensor.Name == "GPU Video Engine")
                        {
                            gpuVEn = true;
                        }

                        if (sensor.SensorType == SensorType.Load && sensor.Name == "GPU Memory")
                        {
                            gpuMem = true;
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
                            memLoad = true;
                        }

                        if (sensor.SensorType == SensorType.Data && sensor.Name == "Used Meomory")
                        {
                            memUsed = true;
                        }

                        if (sensor.SensorType == SensorType.Data && sensor.Name == "Available Memory")
                        {
                            memAv = true;
                        }
                    }
                }

                if (hardware.HardwareType == HardwareType.HDD && _hdd == true)
                {
                    hardware.Update();
                    //Console.WriteLine("\nDysk twardy " + k + ":");
                    foreach (var sensor in hardware.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Load)
                        {
                            hddLoad = true;
                        }

                        if (sensor.SensorType == SensorType.Temperature)
                        {
                            hddTemp = true;
                        }

                    }
                }
            }
            no_cores = x;
            k = x = l = 0;
            Console.WriteLine("Zakonczono wykrywanie sprzetow");
        }  //wykrecie sprzętów

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

            Stack<string> stack = new Stack<string>();

            if (p == 29)
            {
                k = x = l = p = 0;
            }
            else p++;

            foreach (var hardware in computer.Hardware)
            {
                if (hardware.HardwareType == HardwareType.CPU && _cpu == true)
                {
                    hardware.Update();

                    //Console.WriteLine("Procesor:");
                    foreach (var sensor in hardware.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Temperature && cpuTemp && ((p == 1 && k == 0) || (p == 2 && k == 1) || (p == 3 && k == 2) || (p == 4 && k == 3)))
                        {
                            k++;
                            //stack.Push(sensor.Name + ": " + Convert.ToString((int)(float)sensor.Value.GetValueOrDefault()) + "C");
                            _serialPort.WriteLine("C" + k + "C" + Convert.ToString((int)(float)sensor.Value.GetValueOrDefault()));

                        }
                        //else { if (k == 3) p = 4; }
                        if (sensor.SensorType == SensorType.Load && cpuLoad && ((p == 5 && x == 0) || (p == 6 && x == 1) || (p == 7 && x == 2) || (p == 8 && x == 3)))
                        {
                            x++;
                            //stack.Push(sensor.Name + ": " + Convert.ToString((int)(float)sensor.Value.GetValueOrDefault()) + "%");
                            _serialPort.WriteLine("C" + x + "%" + Convert.ToString((int)(float)sensor.Value.GetValueOrDefault()));

                        }
                        //else if (x == 0) p = 8;
                        if (sensor.SensorType == SensorType.Clock && cpuClock && sensor.Name != "Bus Speed" && ((p == 9 && l == 0) || (p == 10 && l == 1) || (p == 11 && l == 2) || (p == 12 && l == 3)))
                        {
                            l++;
                            //stack.Push(sensor.Name + ": " + Convert.ToString((int)(float)sensor.Value.GetValueOrDefault()) + "Mhz");
                            _serialPort.WriteLine("C" + l + "M" + Convert.ToString((int)(float)sensor.Value.GetValueOrDefault()));
                        }
                        //else if (l == 0) p = 13;
                        else if (sensor.SensorType == SensorType.Clock && busLock && sensor.Name.Contains("Bus Speed") && p == 13)
                        {
                            _serialPort.WriteLine("BS" + "M" + Convert.ToString((int)(float)sensor.Value.GetValueOrDefault()));

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
                            if (sensor.SensorType == SensorType.Fan && fan && sensor.Name.Equals("Fan #1", StringComparison.OrdinalIgnoreCase) && _cpu == true && p == 14)
                            {
                                //stack.Push("Cooler speed: " + Convert.ToString((int)(float)sensor.Value) + "RPM");
                                _serialPort.WriteLine("CF" + "R" + Convert.ToString((int)(float)sensor.Value));

                            } if (fan != true && p == 14) p++;

                        }
                    }
                }

                if ((hardware.HardwareType == HardwareType.GpuNvidia || hardware.HardwareType == HardwareType.GpuAti) && _gpu == true)
                {
                    hardware.Update();
                    //Console.WriteLine("\nKarta graficzna:");
                    foreach (var sensor in hardware.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Fan && gpuFan && p == 15)
                        {
                            //stack.Push(sensor.Name + ": " + Convert.ToString((int)(float)sensor.Value) + "RPM");
                            _serialPort.WriteLine("GF" + "R" + Convert.ToString((int)(float)sensor.Value));

                        } if (gpuFan != true && p == 15) p++;

                        if (sensor.SensorType == SensorType.Clock && gpuMemory && sensor.Name == "GPU Memory" && p == 16)
                        {
                            //stack.Push(sensor.Name + ": " + Convert.ToString((int)(float)sensor.Value) + "MHz");
                            _serialPort.WriteLine("GM" + "M" + Convert.ToString((int)(float)sensor.Value));

                        } if (gpuMemory != true && p == 16) p++;

                        if (sensor.SensorType == SensorType.Clock && gpuShader && sensor.Name == "GPU Shader" && p == 17)
                        {
                            //stack.Push(sensor.Name + ": " + Convert.ToString((int)(float)sensor.Value) + "MHz");
                            _serialPort.WriteLine("GS" + "M" + Convert.ToString((int)(float)sensor.Value));
                        } if (gpuShader != true && p == 17) p++;

                        if (sensor.SensorType == SensorType.Clock && gpuCore && sensor.Name == "GPU Core" && p == 18)
                        {
                            //stack.Push(sensor.Name + ": " + Convert.ToString((int)(float)sensor.Value) + "MHz");
                            _serialPort.WriteLine("GC" + "M" + Convert.ToString((int)(float)sensor.Value));
                        } if (gpuCore != true && p == 18) p++;

                        if (sensor.SensorType == SensorType.Temperature && gpuTemp && p == 19)
                        {
                            //stack.Push(sensor.Name + ": " + Convert.ToString((int)(float)sensor.Value) + "C");
                            _serialPort.WriteLine("GC" + "C" + Convert.ToString((int)(float)sensor.Value));
                        } if (gpuTemp != true && p == 19) p++;

                        if (sensor.SensorType == SensorType.Load && sensor.Name == "GPU Core" && gcuCore2 && p == 20)
                        {
                            //stack.Push(sensor.Name + ": " + Convert.ToString((int)(float)sensor.Value) + "%");
                            _serialPort.WriteLine("GC" + "%" + Convert.ToString((int)(float)sensor.Value));
                        } if (gcuCore2 != true && p == 20) p++;

                        if (sensor.SensorType == SensorType.Load && sensor.Name == "GPU Memory Controller" && gpuContr && p == 21)
                        {
                            //stack.Push(sensor.Name + ": " + Convert.ToString((int)(float)sensor.Value) + "%");
                            _serialPort.WriteLine("GR" + "%" + Convert.ToString((int)(float)sensor.Value));
                        } if (gpuContr != true && p == 21) p++;

                        if (sensor.SensorType == SensorType.Load && sensor.Name == "GPU Video Engine" && gpuVEn && p == 22)
                        {
                            //stack.Push(sensor.Name + ": " + Convert.ToString((int)(float)sensor.Value) + "%");
                            _serialPort.WriteLine("GV" + "%" + Convert.ToString((int)(float)sensor.Value));
                        } if (gpuVEn != true && p == 22) p++;

                        if (sensor.SensorType == SensorType.Load && sensor.Name == "GPU Memory" && gpuMem && p == 23)
                        {
                            //stack.Push(sensor.Name + ": " + Convert.ToString((int)(float)sensor.Value) + "%");
                            _serialPort.WriteLine("GM" + "%" + Convert.ToString((int)(float)sensor.Value));
                        } if (gpuMem != true && p == 23) p++;


                    }
                }


                if (hardware.HardwareType == HardwareType.RAM && _ram == true)
                {
                    hardware.Update();
                    //Console.WriteLine("\nPamięć RAM:");
                    foreach (var sensor in hardware.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Load && memLoad && p == 24)
                        {
                            //stack.Push(sensor.Name + ": " + Convert.ToString((int)(float)sensor.Value) + "%");
                            _serialPort.WriteLine("RL" + "%" + Convert.ToString((int)(float)sensor.Value));
                        } if (memLoad != true && p == 24) p++;

                        if (sensor.SensorType == SensorType.Data && sensor.Name == "Used Meomory" && memUsed && p == 25)
                        {
                            //stack.Push(sensor.Name + ": " + Convert.ToString((int)(float)sensor.Value) + "GB");
                            _serialPort.WriteLine("RU" + "G" + Convert.ToString((int)(float)sensor.Value));
                        } if (memUsed != true && p == 25) p++;

                        if (sensor.SensorType == SensorType.Data && sensor.Name == "Available Memory" && memAv && p == 26)
                        {
                            //stack.Push(sensor.Name + ": " + Convert.ToString((int)(float)sensor.Value) + "GB");
                            _serialPort.WriteLine("RA" + "G" + Convert.ToString((int)(float)sensor.Value));
                        } if (memAv != true && p == 26) p++;


                    }
                }

                if (hardware.HardwareType == HardwareType.HDD && _hdd == true)
                {
                    //k++;
                    hardware.Update();
                    //Console.WriteLine("\nDysk twardy " + k + ":");
                    foreach (var sensor in hardware.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Load && hddLoad && p == 27)
                        {
                            //stack.Push(sensor.Name + ": " + Convert.ToString((int)(float)sensor.Value) + "%");
                            _serialPort.WriteLine("HL" + "%" + Convert.ToString((int)(float)sensor.Value));
                        } if (hddLoad != true && p == 27) p++;

                        if (sensor.SensorType == SensorType.Temperature && hddTemp && p == 28)
                        {
                            //stack.Push(sensor.Name + ": " + Convert.ToString((int)(float)sensor.Value) + "C");
                            _serialPort.WriteLine("HT" + "C" + Convert.ToString((int)(float)sensor.Value));
                        } if (hddTemp != true && p == 28) p++;

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
