#PcInfo
##Overview
Our project displays information about computer’s peripherals on HD47780 display.
##Description
Our project consists of two applications. C# application written in Visual Studio reads the state of peripherals and sends them to Virtual COM Port, each value is sent in 5 seconds intervals. The other one written in CooCox CoIDE displays name of peripheral, current value and unit of value.
##Tools 
- Visual Studio 2015
- CooCox CoIDE 1.7.7
- Virtual COM Port driver - http://www.st.com/content/st_com/en/products/development-tools/software-development-tools/stm32-software-development-tools/stm32-utilities/stsw-stm32102.html
- OpenHardwareMonitorLib.dll - http://openhardwaremonitor.org/downloads/

##Pins scheme
- HD44780PIN1 -> GND
- HD44780PIN2 -> 5V
- HD44780PIN3 -> GND
- HD44780PIN4 -> PE5
- HD44780PIN5 -> PE12
- HD44780PIN6 -> PE6
- HD44780PIN7 -> NOT PLUGGED
- HD44780PIN8 -> NOT PLUGGED
- HD44780PIN9 -> NOT PLUGGED
- HD44780PIN10 -> NOT PLUGGED
- HD44780PIN11 -> PE7
- HD44780PIN12 -> PE8
- HD44780PIN13 -> PE9
- HD44780PIN14 -> PE10
- HD44780PIN15 -> 5V
- HD44780PIN16 -> GND


##How to compile

1. Go to STMpcInfo/Display. 
2. Open CooCox project:  "STM32DiscoveryVCP.coproj".
3. Click "Build" to build program.
4. Download program to your STM.

##How to run
1. Connect STM with HD47780 display just like in Pins Scheme.
2. Download project from release section.
3. Run CooCox project located in Display folder and download program to your STM.
4. Run PTM_PCINFO.exe located in PTM_PCINFO folder.
5. Check available COM Ports at the bottom of the graphical interface and provide name of COM Port standing for Virtual COM Port in following format: “COMX”, when X stands for port’s number.
6. Click "OPEN PORT".
7. Check  which  peripherals you would like to display.
8. You can change them while application is running.

##Attributions 

We used following code to set up Virtual COM Port: 
- https://github.com/xenovacivus/STM32DiscoveryVCP

We used following libraries for our display: 
- https://github.com/ivannikov/HD44780/blob/master/HD44780.c
- https://github.com/ivannikov/HD44780/blob/master/HD44780.h

##License

##Credits 
Authors: Łukasz Knop, Maciej Zwiewka

The project was conducted during the Microprocessor Lab course held by the Institute of Control and Information Engineering, Poznan University of Technology.
Supervisor: Tomasz Mańkowski

