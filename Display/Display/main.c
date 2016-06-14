#include "stm32f4xx_conf.h"
#include "stm32f4xx.h"
#include "stm32f4xx_gpio.h"
#include "stm32f4xx_rcc.h"
#include "stm32f4xx_exti.h"
#include "stm32f4xx_syscfg.h"
#include "usbd_cdc_core.h"
#include "usbd_usr.h"
#include "usbd_desc.h"
#include "usbd_cdc_vcp.h"
#include "usb_dcd_int.h"
#include "stdio.h"
#include "hd44780.h"
#include "misc.h"
#include "stm32f4xx_tim.h"
#include "string.h"

// deklaracje zwi¹zane z portem szeregowym
volatile uint32_t ticker, downTicker;

__ALIGN_BEGIN USB_OTG_CORE_HANDLE USB_OTG_dev __ALIGN_END;

void init();
void ColorfulRingOfDeath(void);

#ifdef __cplusplus
extern "C" {
#endif

void SysTick_Handler(void);
void NMI_Handler(void);
void HardFault_Handler(void);
void MemManage_Handler(void);
void BusFault_Handler(void);
void UsageFault_Handler(void);
void SVC_Handler(void);
void DebugMon_Handler(void);
void PendSV_Handler(void);
void OTG_FS_IRQHandler(void);
void OTG_FS_WKUP_IRQHandler(void);

#ifdef __cplusplus
}
#endif

// main
int main(void) {
	SystemInit();
	//deklaracja potrzebnych zmiennych
	uint8_t theByte = 0;
	int i = 0;
	int j = 0;
	int x = 0;
	char values[10] = { '\0' };
	char name[3] = { '\0' };
	char letter[2] = { '\0' };
	char number[2] = { '\0' };
	char clear[17] = { ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ',
			' ', ' ', ' ', ' ', ' ', '\0' };
	init();
	// przypisanie odpowiednich PIN'ów, które bêd¹ wykorzystywane do obs³ugi ekranu
	LCD_ConfigurePort(GPIOE, GPIO_Pin_5, GPIO_Pin_12, GPIO_Pin_6, NULL, NULL,
			NULL, NULL, GPIO_Pin_7, GPIO_Pin_8, GPIO_Pin_9, GPIO_Pin_10);
	// inicjalizacja ekranu
	LCD_Initalize(BUS_WIDTH_4, DISPLAY_LINES_2, FONT_5x10);
	// ustawienie kursora do pozycji wyjsciowej
	LCD_Home();
	// wy³¹czenie kursora
    LCD_CursorOn(0);
    // wy³¹czenie mrugania kursora
    LCD_CursorBlink(0);
    // pêtla g³ówna programu
	while (1) {
		// sprawdzenie czy jest dostêpny jakis znak na wejsciu
		if (VCP_get_char(&theByte)) {
			if (theByte != '\n' && theByte!='x') {
				// zapisywanie informacji przes³anych przez program do odpowiednich tabeli
				if (j < 2) {
					name[j] = theByte;
					j++;
				} else if (j == 2) {
					letter[0] = theByte;
					j++;
				}else if(j==3 && strcmp(name, "CC")==0)
				{
					number[0]=theByte;
					j++;
				}
				else
				{
					values[x] = theByte;
					x++;
				}
			} else if (theByte == '\n') {
				// czyszczenie ekranu
				LCD_MoveToPosition(0x00);
				LCD_Print(clear);
				LCD_MoveToPosition(0x40);
				LCD_Print(clear);
				char unit[9] = { '\0' };
				char top[17] = { '\0' };
				char bottom[17] = { '\0' };
				// przypisanie odpowiednich jednostek ze wzglêdu na przes³ane przez program informacje
				if (strcmp(letter, "G") == 0) {
					sprintf(unit, "GB");
				} else if (strcmp(letter, "%") == 0) {
					sprintf(unit, "%%");
				} else if (strcmp(letter, "C") == 0) {
					sprintf(unit, "%cC", 0xDF);
				} else if (strcmp(letter, "R") == 0) {
					sprintf(unit, "RPM");
				} else if (strcmp(letter, "M") == 0) {
					sprintf(unit, "MHz");
				}
                // przypisanie odpowiednich nazw urz¹dzeñ ze wzglêdu na przes³ane przez program informacje
				if (strcmp(name, "CC") == 0) {
					sprintf(top, "CPU Core #%s",number);
					sprintf(bottom, "%s%s", values, unit);
				} else if (strcmp(name, "BS") == 0) {
					sprintf(top, "Bus Speed");
					sprintf(bottom, "%s%s", values, unit);
				} else if (strcmp(name, "CF") == 0) {
					sprintf(top, "CPU Fan Speed");
					sprintf(bottom, "%s%s",values, unit);
				} else if (strcmp(name, "GF") == 0) {
					sprintf(top, "GPU Fan");
					sprintf(bottom, "%s%s", values, unit);
				} else if (strcmp(name, "GM") == 0) {
					sprintf(top, "GPU Memory");
					sprintf(bottom,"%s%s", values, unit);
				} else if (strcmp(name, "GS") == 0) {
					sprintf(top, "GPU Shader");
					sprintf(bottom, "%s%s" , values, unit);
				} else if (strcmp(name, "GC") == 0) {
					sprintf(top, "GPU Core");
					sprintf(bottom, "%s%s", values, unit);
				} else if (strcmp(name, "GR") == 0) {
					sprintf(top, "GPU Memory Cont.");
					sprintf(bottom,"%s%s", values, unit);
				} else if (strcmp(name, "GV") == 0) {
					sprintf(top, "GPU Video Engine");
					sprintf(bottom,"%s%s", values, unit);
				} else if (strcmp(name, "RL") == 0) {
					sprintf(top, "Memory Load");
					sprintf(bottom, "%s%s", values, unit);
				} else if (strcmp(name, "RU") == 0) {
					sprintf(top, "Used Memory");
					sprintf(bottom, "%s%s", values, unit);
				} else if (strcmp(name, "RA") == 0) {
					sprintf(top, "Available Memory");
					sprintf(bottom,"%s%s", values, unit);
				} else if (strcmp(name, "HL") == 0) {
					sprintf(top, "Used HDD Space");
					sprintf(bottom, "%s%s", values, unit);
				} else if (strcmp(name, "HT") == 0) {
					sprintf(top, "HDD Temperature");
					sprintf(bottom,"%s%s", values, unit);
				}
				// wyswietlanie informacji w dwóch wierszach na ekranie
				LCD_MoveToPosition(0x00);
				LCD_Print(top);
				LCD_MoveToPosition(0x40);
				LCD_Print(bottom);
				// przywracanie zmiennym ich pocz¹tkowych wartosci
				x = 0;
				j = 0;
				for (i = 0; i < 10; i++) {
					if (i < 2) {
						letter[i] = '\0';
						number[i]='\0';
					}
					if (i < 3) {
						name[i] = '\0';
					}
					values[i] = '\0';
				}
			}
		}
	}
	return 0;
}

// funkcje zwi¹zane z obs³ug¹ portu szeregowego i informowaniem o problemach z nim zwi¹zanych
void init() {
	GPIO_InitTypeDef LED_Config;

	RCC_AHB1PeriphClockCmd(RCC_AHB1Periph_GPIOD, ENABLE);
	LED_Config.GPIO_Pin = GPIO_Pin_12 | GPIO_Pin_13 | GPIO_Pin_14 | GPIO_Pin_15;
	LED_Config.GPIO_Mode = GPIO_Mode_OUT;
	LED_Config.GPIO_OType = GPIO_OType_PP;
	LED_Config.GPIO_Speed = GPIO_Speed_25MHz;
	LED_Config.GPIO_PuPd = GPIO_PuPd_NOPULL;
	GPIO_Init(GPIOD, &LED_Config);

	if (SysTick_Config(SystemCoreClock / 1000)) {
		ColorfulRingOfDeath();
	}

	USBD_Init(&USB_OTG_dev, USB_OTG_FS_CORE_ID, &USR_desc, &USBD_CDC_cb,
			&USR_cb);

	return;
}

void ColorfulRingOfDeath(void) {
	uint16_t ring = 1;
	while (1) {
		uint32_t count = 0;
		while (count++ < 500000)
			;

		GPIOD->BSRRH = (ring << 12);
		ring = ring << 1;
		if (ring >= 1 << 4) {
			ring = 1;
		}
		GPIOD->BSRRL = (ring << 12);
	}
}

void OTG_FS_IRQHandler(void) {
	USBD_OTG_ISR_Handler(&USB_OTG_dev);
}

void OTG_FS_WKUP_IRQHandler(void) {
	if (USB_OTG_dev.cfg.low_power) {
		*(uint32_t *) (0xE000ED10) &= 0xFFFFFFF9;
		SystemInit();
		USB_OTG_UngateClock(&USB_OTG_dev);
	}
	EXTI_ClearITPendingBit(EXTI_Line18);
}
