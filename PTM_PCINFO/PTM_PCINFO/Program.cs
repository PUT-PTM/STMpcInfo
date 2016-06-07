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
            info.timer = new System.Timers.Timer();
            info.timer.Interval = 5000;
            info.openPort();
            info.getInfo();
            Console.WriteLine("Rozpoczęto wysyłanie");
            info.timer.Elapsed += new ElapsedEventHandler(info.sendInfo);
            info.timer.Start();

        }
    }
   
    public class SystemInfo
    {
        static bool _continue;
        bool _cpu = true;
        bool _gpu = true;
        bool _hdd = true;
        bool _ram = true;
        public System.Timers.Timer timer;
        static SerialPort _serialPort = new SerialPort("COM3",
            9600, Parity.None, 8, StopBits.One);
        static int p = 1;
        static int k = 0;
        static int x = 0;
        static int l = 0;
        static int number = 0;
        static int numberOfCores = 0;
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
                    foreach (var sensor in hardware.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Temperature)
                        {
                            numberOfCores++;
                            cpuTemp = true;
                        }
                        if (sensor.SensorType == SensorType.Load)
                        {
                            cpuLoad = true;

                        }
                        if (sensor.SensorType == SensorType.Clock && sensor.Name != "Bus Speed")
                        {
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
            number = (numberOfCores * 3);
            Console.WriteLine("Zakończono wykrywanie sprzętów");
        }

        public void sendInfo(object sender, EventArgs e)
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
                    foreach (var sensor in hardware.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Temperature && cpuTemp && p <= numberOfCores && (((p == 1) && k == 0) || ((p == 2) && k == 1) || ((p == 3) && k == 2) || ((p == 4) && k == 3) || ((p == 5) && k == 4) || ((p == 6) && k == 5) || ((p == 7) && k == 6) || ((p == 8) && k == 7)))
                        {
                            k++;
                            _serialPort.WriteLine("C" + k + "C" + Convert.ToString((int)(float)sensor.Value.GetValueOrDefault()));
                        }

                        if (sensor.SensorType == SensorType.Load && cpuLoad && p <= (numberOfCores * 2) && (((p == numberOfCores + 1) && x == 0) || ((p == numberOfCores + 2) && x == 1) || ((p == numberOfCores + 3) && x == 2) || ((p == numberOfCores + 4) && x == 3) || ((p == numberOfCores + 5) && x == 4) || ((p == numberOfCores + 6) && x == 5) || ((p == numberOfCores + 7) && x == 6) || ((p == numberOfCores + 8) && x == 7)))
                        {
                            x++;
                            _serialPort.WriteLine("C" + x + "%" + Convert.ToString((int)(float)sensor.Value.GetValueOrDefault()));
                        }

                        if (sensor.SensorType == SensorType.Clock && cpuClock && p <= (numberOfCores * 3) && sensor.Name != "Bus Speed" && (((p == (number / 3 * 2) + 1) && l == 0) || ((p == (number / 3 * 2) + 2) && l == 1) || ((p == (number / 3 * 2) + 3) && l == 2) || ((p == (number / 3 * 2) + 4) && l == 3) || ((p == (number / 3 * 2) + 5) && l == 4) || ((p == (number / 3 * 2) + 6) && l == 5) || ((p == (number / 3 * 2) + 7) && l == 6) || ((p == (number / 3 * 2) + 8) && l == 7)))
                        {
                            l++;
                            _serialPort.WriteLine("C" + l + "M" + Convert.ToString((int)(float)sensor.Value.GetValueOrDefault()));
                        }

                        if (sensor.SensorType == SensorType.Clock && busLock && sensor.Name.Contains("Bus Speed") && p == (1 + number))
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
                            if (sensor.SensorType == SensorType.Fan && fan && sensor.Name.Equals("Fan #1", StringComparison.OrdinalIgnoreCase) && _cpu == true && p == (2 + number))
                            {
                                _serialPort.WriteLine("CF" + "R" + Convert.ToString((int)(float)sensor.Value));

                            }
                        }
                    }
                }
                if (fan != true && p == (2 + number)) p++;

                if ((hardware.HardwareType == HardwareType.GpuNvidia || hardware.HardwareType == HardwareType.GpuAti) && _gpu == true)
                {
                    hardware.Update();
                    foreach (var sensor in hardware.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Fan && gpuFan && p == (3 + number))
                        {
                            _serialPort.WriteLine("GF" + "R" + Convert.ToString((int)(float)sensor.Value));

                        }
                        if (gpuFan != true && p == (3 + number)) p++;

                        if (sensor.SensorType == SensorType.Clock && gpuMemory && sensor.Name == "GPU Memory" && p == (4 + number))
                        {
                            _serialPort.WriteLine("GM" + "M" + Convert.ToString((int)(float)sensor.Value));

                        }
                        if (gpuMemory != true && p == (4 + number)) p++;

                        if (sensor.SensorType == SensorType.Clock && gpuShader && sensor.Name == "GPU Shader" && p == (5 + number))
                        {
                            _serialPort.WriteLine("GS" + "M" + Convert.ToString((int)(float)sensor.Value));
                        }
                        if (gpuShader != true && p == (5 + number)) p++;

                        if (sensor.SensorType == SensorType.Clock && gpuCore && sensor.Name == "GPU Core" && p == (6 + number))
                        {
                            _serialPort.WriteLine("GC" + "M" + Convert.ToString((int)(float)sensor.Value));
                        }
                        if (gpuCore != true && p == (6 + number)) p++;

                        if (sensor.SensorType == SensorType.Temperature && gpuTemp && p == (7 + number))
                        {
                            _serialPort.WriteLine("GC" + "C" + Convert.ToString((int)(float)sensor.Value));
                        }
                        if (gpuTemp != true && p == (7 + number)) p++;

                        if (sensor.SensorType == SensorType.Load && sensor.Name == "GPU Core" && gcuCore2 && p == (8 + number))
                        {
                            _serialPort.WriteLine("GC" + "%" + Convert.ToString((int)(float)sensor.Value));
                        }
                        if (gcuCore2 != true && p == (8 + number)) p++;

                        if (sensor.SensorType == SensorType.Load && sensor.Name == "GPU Memory Controller" && gpuContr && p == (9 + number))
                        {
                            _serialPort.WriteLine("GR" + "%" + Convert.ToString((int)(float)sensor.Value));
                        }
                        if (gpuContr != true && p == (9 + number)) p++;

                        if (sensor.SensorType == SensorType.Load && sensor.Name == "GPU Video Engine" && gpuVEn && p == (10 + number))
                        {
                            _serialPort.WriteLine("GV" + "%" + Convert.ToString((int)(float)sensor.Value));
                        }
                        if (gpuVEn != true && p == (10 + number)) p++;

                        if (sensor.SensorType == SensorType.Load && sensor.Name == "GPU Memory" && gpuMem && p == (11 + number))
                        {
                            _serialPort.WriteLine("GM" + "%" + Convert.ToString((int)(float)sensor.Value));
                        }
                        if (gpuMem != true && p == (11 + number)) p++;


                    }
                }


                if (hardware.HardwareType == HardwareType.RAM && _ram == true)
                {
                    hardware.Update();
                    foreach (var sensor in hardware.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Load && memLoad && p == (12 + number))
                        {
                            _serialPort.WriteLine("RL" + "%" + Convert.ToString((int)(float)sensor.Value));
                        }
                        if (memLoad != true && p == (12 + number)) p++;

                        if (sensor.SensorType == SensorType.Data && sensor.Name == "Used Meomory" && memUsed && p == (13 + number))
                        {
                            _serialPort.WriteLine("RU" + "G" + Convert.ToString((int)(float)sensor.Value));
                        }
                        if (memUsed != true && p == (13 + number)) p++;

                        if (sensor.SensorType == SensorType.Data && sensor.Name == "Available Memory" && memAv && p == (14 + number))
                        {
                            _serialPort.WriteLine("RA" + "G" + Convert.ToString((int)(float)sensor.Value));
                        }
                        if (memAv != true && p == (14 + number)) p++;


                    }
                }

                if (hardware.HardwareType == HardwareType.HDD && _hdd == true)
                {
                    hardware.Update();
                    foreach (var sensor in hardware.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Load && hddLoad && p == (15 + number))
                        {
                            _serialPort.WriteLine("HL" + "%" + Convert.ToString((int)(float)sensor.Value));
                        }
                        if (hddLoad != true && p == (15 + number)) p++;

                        if (sensor.SensorType == SensorType.Temperature && hddTemp && p == (16 + number))
                        {
                            _serialPort.WriteLine("HT" + "C" + Convert.ToString((int)(float)sensor.Value));
                        }
                        if (hddTemp != true && p == (16 + number)) p++;

                    }
                }
            }

            if (p == (17 + number))
            {
                k = x = l = 0;
                p = 1;

            }
            else { p++; }
        }

        public static void Read()
        {
            while (_continue)
            {
                   Thread.Sleep(1);
            }
        }
    }
}

