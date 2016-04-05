using System;
using OpenHardwareMonitor.Hardware;


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
            int k = 0;
            foreach (var hardware in computer.Hardware)
            {
                if (hardware.HardwareType == HardwareType.CPU)
                {
                    hardware.Update();
                    Console.WriteLine("Procesor:");
                    foreach (var sensor in hardware.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Temperature)
                            Console.WriteLine(sensor.Name + ": " + sensor.Value.GetValueOrDefault() + "°C");
                        if (sensor.SensorType == SensorType.Load)
                            Console.WriteLine(sensor.Name + ": " + sensor.Value.GetValueOrDefault() + "%");
                        if (sensor.SensorType == SensorType.Clock)
                            Console.WriteLine(sensor.Name + ": " + sensor.Value.GetValueOrDefault() + "MHz");
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
                                Console.WriteLine("Prędkość coolera: " + Convert.ToString((int)(float)sensor.Value) + "RPM\n");
                            }
                        }
                    }
                }

                if (hardware.HardwareType == HardwareType.GpuNvidia)
                {
                    hardware.Update();
                    Console.WriteLine("\nKarta graficzna:");
                    foreach (var sensor in hardware.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Fan)
                            Console.WriteLine(sensor.Name + ": " + Convert.ToString(sensor.Value) + "RPM");
                        if (sensor.SensorType == SensorType.Clock)
                            Console.WriteLine(sensor.Name + ": " + Convert.ToString(sensor.Value) + "MHz");
                        if (sensor.SensorType == SensorType.Temperature)
                            Console.WriteLine(sensor.Name + ": " + Convert.ToString(sensor.Value) + "°C");
                        if (sensor.SensorType == SensorType.Load)
                            Console.WriteLine(sensor.Name + ": " + Convert.ToString(sensor.Value) + "%");
                    }
                }

                if (hardware.HardwareType == HardwareType.RAM)
                {
                    hardware.Update();
                    Console.WriteLine("\nPamięć RAM:");
                    foreach (var sensor in hardware.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Load)
                            Console.WriteLine(sensor.Name + ": " + Convert.ToString(sensor.Value) + "%");
                        if (sensor.SensorType == SensorType.Data)
                            Console.WriteLine(sensor.Name + ": " + Convert.ToString(sensor.Value) + "GB");
                    }
                }

                if (hardware.HardwareType == HardwareType.HDD)
                {
                    k++;
                    hardware.Update();
                    Console.WriteLine("\nDysk twardy " + k + ":");
                    foreach (var sensor in hardware.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Load)
                            Console.WriteLine(sensor.Name + ": " + Convert.ToString(sensor.Value) + "%");
                        if (sensor.SensorType == SensorType.Temperature)
                            Console.WriteLine(sensor.Name + ": " + Convert.ToString(sensor.Value) + "°C");
                    }
                }

            }
        }

    }
}

