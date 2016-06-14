using System;
using System.Timers;
using System.Windows.Forms;
using System.IO.Ports;
using OpenHardwareMonitor.Hardware;

namespace PTM_PCINFO
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            //wyświetlenie dostępnych portów i inicjalizacja programu
            InitializeComponent();
            comboBox1.Items.Add("Available ports:");
            foreach (string s in SerialPort.GetPortNames())
            {
                comboBox1.Items.Add(s);
            }
        }

        public class SystemInfo
        {
            public static System.Timers.Timer timer;
            public static SerialPort _serialPort = new SerialPort(textBox1.Text,
                9600, Parity.None, 8, StopBits.One);
            static bool cpuTemp, cpuLoad, cpuClock, busLock, fan, gpuFan, gpuMemory, gpuShader, gpuCore, gpuTemp, gcuCore2, gpuContr, gpuVEn, gpuMem, memLoad, memUsed, memAv, hddLoad, hddTemp = false;
            public static int p = 1;
            public static int k = 0;
            public static int x = 0;
            public static int l = 0;
            public static int numberOfCores = 0;
            public static int cpuCounter = 0;
            public static int gpuCounter = 0;
            public static int ramCounter = 0;
            public static int hddCounter = 0;
            public static int gpuPeripheralsCounter = 0;
            public static int ramPeripheralsCounter = 0;
            public static int hddPeripheralsCounter = 0;
            public static bool _cpu = false;
            public static bool _gpu = false;
            public static bool _hdd = false;
            public static bool _ram = false;
            public void openPort()          // otworzenie portu
            {
                _serialPort.ReadTimeout = 500;
                _serialPort.WriteTimeout = 500;
                _serialPort.Open();
            }

            public void getInfo()           // pobranie informacji o sprzętach
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
                    if (hardware.HardwareType == HardwareType.CPU) //zebranie informacji o dostępnych miernikach związanych z CPU
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
                                if (sensor.SensorType == SensorType.Fan && sensor.Name.Equals("Fan #1", StringComparison.OrdinalIgnoreCase))
                                {
                                    fan = true;
                                }
                            }
                        }
                    }

                    if ((hardware.HardwareType == HardwareType.GpuNvidia || hardware.HardwareType == HardwareType.GpuAti)) //zebranie informacji o dostępnych miernikach związanych z GPU
                    {
                        hardware.Update();
                        foreach (var sensor in hardware.Sensors)
                        {
                            if (sensor.SensorType == SensorType.Fan)
                            {
                                gpuFan = true;
                                gpuPeripheralsCounter++;
                            }

                            if (sensor.SensorType == SensorType.Clock && sensor.Name == "GPU Memory")
                            {
                                gpuMemory = true;
                                gpuPeripheralsCounter++;
                            }

                            if (sensor.SensorType == SensorType.Clock && sensor.Name == "GPU Shader")
                            {
                                gpuShader = true;
                                gpuPeripheralsCounter++;
                            }

                            if (sensor.SensorType == SensorType.Clock && sensor.Name == "GPU Core")
                            {
                                gpuCore = true;
                                gpuPeripheralsCounter++;
                            }

                            if (sensor.SensorType == SensorType.Temperature)
                            {
                                gpuTemp = true;
                                gpuPeripheralsCounter++;
                            }

                            if (sensor.SensorType == SensorType.Load && sensor.Name == "GPU Core")
                            {
                                gcuCore2 = true;
                                gpuPeripheralsCounter++;
                            }

                            if (sensor.SensorType == SensorType.Load && sensor.Name == "GPU Memory Controller")
                            {
                                gpuContr = true;
                                gpuPeripheralsCounter++;
                            }

                            if (sensor.SensorType == SensorType.Load && sensor.Name == "GPU Video Engine")
                            {
                                gpuVEn = true;
                                gpuPeripheralsCounter++;
                            }

                            if (sensor.SensorType == SensorType.Load && sensor.Name == "GPU Memory")
                            {
                                gpuMem = true;
                                gpuPeripheralsCounter++;
                            }
                        }
                    }


                    if (hardware.HardwareType == HardwareType.RAM) //zebranie informacji o dostępnych miernikach związanych z RAM
                    {
                        hardware.Update();
                        foreach (var sensor in hardware.Sensors)
                        {
                            if (sensor.SensorType == SensorType.Load)
                            {
                                memLoad = true;
                                ramPeripheralsCounter++;
                            }

                            if (sensor.SensorType == SensorType.Data && sensor.Name == "Used Memory")
                            {
                                memUsed = true;
                                ramPeripheralsCounter++;
                            }

                            if (sensor.SensorType == SensorType.Data && sensor.Name == "Available Memory")
                            {
                                memAv = true;
                                ramPeripheralsCounter++;
                            }
                        }
                    }

                    if (hardware.HardwareType == HardwareType.HDD) //zebranie informacji o dostępnych miernikach związanych z HDD
                    {
                        hardware.Update();
                        foreach (var sensor in hardware.Sensors)
                        {
                            if (sensor.SensorType == SensorType.Load)
                            {
                                hddLoad = true;
                                hddPeripheralsCounter++;
                            }

                            if (sensor.SensorType == SensorType.Temperature)
                            {
                                hddTemp = true;
                                hddPeripheralsCounter++;
                            }

                        }
                    }
                }
                comboBox1.Items.Add("Peripherals Detected");
            }

            public void sendInfo(object sender, EventArgs e) // wysyłanie informacji o wybranych sprzętach przez port szeregowy
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
                    if (hardware.HardwareType == HardwareType.CPU && _cpu == true) //wysyłanie informacji o CPU
                    {
                        hardware.Update();
                        foreach (var sensor in hardware.Sensors)
                        {
                            if (sensor.SensorType == SensorType.Temperature && cpuTemp && p <= numberOfCores && (((p == 1) && k == 0) || ((p == 2) && k == 1) || ((p == 3) && k == 2) || ((p == 4) && k == 3) || ((p == 5) && k == 4) || ((p == 6) && k == 5) || ((p == 7) && k == 6) || ((p == 8) && k == 7)))
                            {
                                k++;
                                _serialPort.WriteLine("CC" + "C" + k + Convert.ToString((int)(float)sensor.Value.GetValueOrDefault()));
                            }

                            if (sensor.SensorType == SensorType.Load && cpuLoad && p <= (numberOfCores * 2) && (((p == numberOfCores + 1) && x == 0) || ((p == numberOfCores + 2) && x == 1) || ((p == numberOfCores + 3) && x == 2) || ((p == numberOfCores + 4) && x == 3) || ((p == numberOfCores + 5) && x == 4) || ((p == numberOfCores + 6) && x == 5) || ((p == numberOfCores + 7) && x == 6) || ((p == numberOfCores + 8) && x == 7)))
                            {
                                x++;
                                _serialPort.WriteLine("CC" + "%" + x + Convert.ToString((int)(float)sensor.Value.GetValueOrDefault()));
                            }

                            if (sensor.SensorType == SensorType.Clock && cpuClock && p <= (numberOfCores * 3) && sensor.Name != "Bus Speed" && (((p == ((cpuCounter - 2) / 3 * 2) + 1) && l == 0) || ((p == ((cpuCounter - 2) / 3 * 2) + 2) && l == 1) || ((p == ((cpuCounter - 2) / 3 * 2) + 3) && l == 2) || ((p == ((cpuCounter - 2) / 3 * 2) + 4) && l == 3) || ((p == ((cpuCounter - 2) / 3 * 2) + 5) && l == 4) || ((p == ((cpuCounter - 2) / 3 * 2) + 6) && l == 5) || ((p == ((cpuCounter - 2) / 3 * 2) + 7) && l == 6) || ((p == ((cpuCounter - 2) / 3 * 2) + 8) && l == 7)))
                            {
                                l++;
                                _serialPort.WriteLine("CC" + "M" + l + Convert.ToString((int)(float)sensor.Value.GetValueOrDefault()));
                            }

                            if (sensor.SensorType == SensorType.Clock && busLock && sensor.Name.Contains("Bus Speed") && p == (cpuCounter - 1))
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
                                if (sensor.SensorType == SensorType.Fan && fan && sensor.Name.Equals("Fan #1", StringComparison.OrdinalIgnoreCase) && _cpu == true && p == (cpuCounter))
                                {
                                    _serialPort.WriteLine("CF" + "R" + Convert.ToString((int)(float)sensor.Value));

                                }
                            }
                        }
                    }
                    if (fan != true && p == (cpuCounter)) p++;

                    if ((hardware.HardwareType == HardwareType.GpuNvidia || hardware.HardwareType == HardwareType.GpuAti) && _gpu == true) //wysyłanie informacji o GPU
                    {
                        hardware.Update();
                        foreach (var sensor in hardware.Sensors)
                        {
                            if (sensor.SensorType == SensorType.Fan && gpuFan && p == (1 + cpuCounter))
                            {
                                _serialPort.WriteLine("GF" + "R" + Convert.ToString((int)(float)sensor.Value));
                            }

                            if (gpuFan != true && p == (1 + cpuCounter))
                            {
                                p++;
                            }


                            if (sensor.SensorType == SensorType.Clock && gpuMemory && sensor.Name == "GPU Memory" && p == (2 + cpuCounter))
                            {
                                _serialPort.WriteLine("GM" + "M" + Convert.ToString((int)(float)sensor.Value));

                            }

                            if (gpuMemory != true && p == (2 + cpuCounter))
                            {
                                p++;
                            }


                            if (sensor.SensorType == SensorType.Clock && gpuShader && sensor.Name == "GPU Shader" && p == (3 + cpuCounter))
                            {
                                _serialPort.WriteLine("GS" + "M" + Convert.ToString((int)(float)sensor.Value));
                            }

                            if (gpuShader != true && p == (3 + cpuCounter))
                            {
                                p++;
                            }


                            if (sensor.SensorType == SensorType.Clock && gpuCore && sensor.Name == "GPU Core" && p == (4 + cpuCounter))
                            {
                                _serialPort.WriteLine("GC" + "M" + Convert.ToString((int)(float)sensor.Value));
                            }

                            if (gpuCore != true && p == (4 + cpuCounter))
                            {
                                p++;
                            }


                            if (sensor.SensorType == SensorType.Temperature && gpuTemp && p == (5 + cpuCounter))
                            {
                                _serialPort.WriteLine("GC" + "C" + Convert.ToString((int)(float)sensor.Value));
                            }

                            if (gpuTemp != true && p == (5 + cpuCounter))
                            {
                                p++;
                            }


                            if (sensor.SensorType == SensorType.Load && sensor.Name == "GPU Core" && gcuCore2 && p == (6 + cpuCounter))
                            {
                                _serialPort.WriteLine("GC" + "%" + Convert.ToString((int)(float)sensor.Value));
                            }

                            if (gcuCore2 != true && p == (6 + cpuCounter))
                            {
                                p++;
                            }


                            if (sensor.SensorType == SensorType.Load && sensor.Name == "GPU Memory Controller" && gpuContr && p == (7 + cpuCounter))
                            {
                                _serialPort.WriteLine("GR" + "%" + Convert.ToString((int)(float)sensor.Value));
                            }

                            if (gpuContr != true && p == (7 + cpuCounter))
                            {
                                p++;
                            }


                            if (sensor.SensorType == SensorType.Load && sensor.Name == "GPU Video Engine" && gpuVEn && p == (8 + cpuCounter))
                            {
                                _serialPort.WriteLine("GV" + "%" + Convert.ToString((int)(float)sensor.Value));
                            }

                            if (gpuVEn != true && p == (8 + cpuCounter))
                            {
                                p++;
                            }


                            if (sensor.SensorType == SensorType.Load && sensor.Name == "GPU Memory" && gpuMem && p == (9 + cpuCounter))
                            {
                                _serialPort.WriteLine("GM" + "%" + Convert.ToString((int)(float)sensor.Value));
                            }

                            if (gpuMem != true && p == (9 + cpuCounter))
                            {
                                p++;
                            }
                        }
                    }


                    if (hardware.HardwareType == HardwareType.RAM && _ram == true) //wysyłanie informacji o RAM
                    {
                        hardware.Update();
                        foreach (var sensor in hardware.Sensors)
                        {
                            if (sensor.SensorType == SensorType.Load && memLoad && p == (1 + cpuCounter + gpuCounter))
                            {
                                _serialPort.WriteLine("RL" + "%" + Convert.ToString((int)(float)sensor.Value));
                            }

                            if (memLoad != true && p == (1 + cpuCounter + gpuCounter))
                            {
                                p++;
                            }


                            if (sensor.SensorType == SensorType.Data && sensor.Name == "Used Memory" && memUsed && p == (2 + cpuCounter + gpuCounter))
                            {
                                _serialPort.WriteLine("RU" + "G" + Convert.ToString((int)(float)sensor.Value));
                            }

                            if (memUsed != true && p == (2 + cpuCounter + gpuCounter))
                            {
                                p++;
                            }

                            if (sensor.SensorType == SensorType.Data && sensor.Name == "Available Memory" && memAv && p == (3 + cpuCounter + gpuCounter))
                            {
                                _serialPort.WriteLine("RA" + "G" + Convert.ToString((int)(float)sensor.Value));
                            }

                            if (memAv != true && p == (3 + cpuCounter + gpuCounter))
                            {
                                p++;
                            }
                        }
                    }

                    if (hardware.HardwareType == HardwareType.HDD && _hdd == true) //wysyłanie informacji o HDD
                    {
                        hardware.Update();
                        foreach (var sensor in hardware.Sensors)
                        {
                            if (sensor.SensorType == SensorType.Load && hddLoad && p == (1 + cpuCounter + gpuCounter + ramCounter))
                            {
                                _serialPort.WriteLine("HL" + "%" + Convert.ToString((int)(float)sensor.Value));
                            }

                            if (hddLoad != true && p == (1 + cpuCounter + gpuCounter + ramCounter))
                            {
                                p++;
                            }


                            if (sensor.SensorType == SensorType.Temperature && hddTemp && p == (2 + cpuCounter + gpuCounter + ramCounter))
                            {
                                _serialPort.WriteLine("HT" + "C" + Convert.ToString((int)(float)sensor.Value));
                            }

                            if (hddTemp != true && p == (2 + cpuCounter + gpuCounter + ramCounter))
                            {
                                p++;
                            }
                        }
                    }
                }

                if (p == (1 + gpuCounter + cpuCounter + ramCounter + hddCounter)) // resetowanie liczników do początkowych wartości lub zwiększenie wartości licznika p
                {
                    k = x = l = 0;
                    p = 1;

                }
                else { p++; }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }


        private void checkBox4_CheckedChanged_1(object sender, EventArgs e) // funkcja dotycząca pola checkBox odpowiadającego za wyświetlanie informacji o HDD
        {
            if (checkBox4.Checked)
            {
                SystemInfo._hdd = true;
                SystemInfo.hddCounter = SystemInfo.hddPeripheralsCounter;
            }
            else
            {
                SystemInfo._hdd = false;
                SystemInfo.hddCounter = 0;
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e) // funkcja dotycząca pola checkBox odpowiadającego za wyświetlanie informacji o RAM
        {
            if (checkBox3.Checked)
            {
                SystemInfo._ram = true;
                SystemInfo.ramCounter = SystemInfo.ramPeripheralsCounter;
            }
            else
            {
                SystemInfo._ram = false;
                SystemInfo.ramCounter = 0;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e) // funkcja dotycząca pola checkBox odpowiadającego za wyświetlanie informacji o GPU
        {
            if (checkBox2.Checked)
            {
                SystemInfo._gpu = true;
                SystemInfo.gpuCounter = SystemInfo.gpuPeripheralsCounter;
            }
            else
            {
                SystemInfo._gpu = false;
                SystemInfo.gpuCounter = 0;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e) // funkcja dotycząca pola checkBox odpowiadającego za wyświetlanie informacji o CPU
        {
            if (checkBox1.Checked)
            {
                SystemInfo.cpuCounter = (SystemInfo.numberOfCores * 3 + 2);
                SystemInfo._cpu = true;
            }
            else
            {
                SystemInfo._cpu = false;
                SystemInfo.cpuCounter = 0;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e) // funkcja dotycząca zachowania aplikacji w razie jej zamknięcia
        {
            Application.Exit();
        }

        private void checkBox1_CheckStateChanged(object sender, EventArgs e) // funkcja dotycząca zachowania programu w razie zmiany stanu jednego z pól checkBox
        {
            SystemInfo.p = 1;
            SystemInfo.x = SystemInfo.k = SystemInfo.l = 0;
            if (!checkBox1.Checked && !checkBox2.Checked && !checkBox3.Checked && !checkBox4.Checked)
            {
                SystemInfo._serialPort.WriteLine("x");
            }
        }

        private void button1_Click(object sender, EventArgs e) // funkcja związana z otwieraniem portu szeregowego za pomocą przycisku button1
        {
            if (textBox1.Text != "")
            {
                SystemInfo info = new SystemInfo();
                SystemInfo.timer = new System.Timers.Timer();
                SystemInfo.timer.Interval = 5000;
                info.openPort();
                info.getInfo();
                comboBox1.Items.Add("Sending");
                button2.Enabled = true;
                button1.Enabled = false;
                checkBox1.Enabled = true;
                checkBox2.Enabled = true;
                checkBox3.Enabled = true;
                checkBox4.Enabled = true;
                SystemInfo.timer.Elapsed += new ElapsedEventHandler(info.sendInfo);
                SystemInfo.timer.Start();
            }
        }
        private void button2_Click(object sender, EventArgs e) // funkcja związana z zamykaniem portu szeregowego za pomocą przycisku button2
        {
            button2.Enabled = false;
            button1.Enabled = true;
            checkBox1.Enabled = false;
            checkBox2.Enabled = false;
            checkBox3.Enabled = false;
            checkBox4.Enabled = false;
            SystemInfo.timer.Stop();
            SystemInfo._serialPort.Close();
            comboBox1.Items.Add("Available Ports:");
            foreach (string s in SerialPort.GetPortNames())
            {
                comboBox1.Items.Add(s);
            }
        }
    }
}

