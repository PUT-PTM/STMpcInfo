#define HSE_VALUE ((uint32_t)8000000) /* STM32 discovery uses a 8Mhz external crystal */

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

int main(void) {
	SystemInit();
	uint8_t theByte = 0;
	int i = 0;
	int j = 0;
	int x = 0;
	int count = 16;
	char values[10] = { '\0' };
	char name[3] = { '\0' };
	char letter[2] = { '\0' };
	char clear[17] = { ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ',
			' ', ' ', ' ', ' ', ' ', '\0' };
	init();
	LCD_ConfigurePort(GPIOE, GPIO_Pin_5, GPIO_Pin_12, GPIO_Pin_6, NULL, NULL,
			NULL, NULL, GPIO_Pin_7, GPIO_Pin_8, GPIO_Pin_9, GPIO_Pin_10);
	LCD_Initalize(BUS_WIDTH_4, DISPLAY_LINES_2, FONT_5x10);
	LCD_Home();
	LCD_CursorOn(0);
	LCD_CursorBlink(0);
	while (1) {
		if (VCP_get_char(&theByte)) {
			if (theByte != '\n' && theByte!='x') {
				if (j < 2) {
					name[j] = theByte;
					j++;
				} else if (j == 2) {
					letter[0] = theByte;
					j++;
				} else {
					values[x] = theByte;
					x++;
				}
			} else if (theByte == '\n') {
				LCD_MoveToPosition(0x00);
				LCD_Print(clear);
				LCD_MoveToPosition(0x40);
				LCD_Print(clear);
				char string[26] = { '\0' };
				char unit[4] = { '\0' };
				char top[16] = { '\0' };
				char bottom[16] = { '\0' };

				if (strcmp(letter, "G") == 0) {
					sprintf(unit, "GB");
				} else if (strcmp(letter, "%") == 0) {
					sprintf(unit, "%c", 37);
				} else if (strcmp(letter, "C") == 0) {
					sprintf(unit, "C");
				} else if (strcmp(letter, "R") == 0) {
					sprintf(unit, "RPM");
				} else if (strcmp(letter, "M") == 0) {
					sprintf(unit, "MHz");
				}

				if (strcmp(name, "C1") == 0) {
					sprintf(string, "CPU Core #1: %s%s", values, unit);
				} else if (strcmp(name, "C2") == 0) {
					sprintf(string, "CPU Core #2: %s%s", values, unit);
				} else if (strcmp(name, "C3") == 0) {
					sprintf(string, "CPU Core #3: %s%s", values, unit);
				} else if (strcmp(name, "C4") == 0) {
					sprintf(string, "CPU Core #4: %s%s", values, unit);
				} else if (strcmp(name, "C5") == 0) {
					sprintf(string, "CPU Core #5: %s%s", values, unit);
				} else if (strcmp(name, "C6") == 0) {
					sprintf(string, "CPU Core #6: %s%s", values, unit);
				} else if (strcmp(name, "C7") == 0) {
					sprintf(string, "CPU Core #7: %s%s", values, unit);
				} else if (strcmp(name, "C8") == 0) {
					sprintf(string, "CPU Core #8: %s%s", values, unit);
				} else if (strcmp(name, "C9") == 0) {
					sprintf(string, "CPU Core #9: %s%s", values, unit);
				} else if (strcmp(name, "BS") == 0) {
					sprintf(string, "Bus Speed: %s%s", values, unit);
				} else if (strcmp(name, "CF") == 0) {
					sprintf(string, "CPU Cooler Speed: %s%s", values, unit);
				} else if (strcmp(name, "GF") == 0) {
					sprintf(string, "GPU Fan: %s%s", values, unit);
				} else if (strcmp(name, "GM") == 0) {
					sprintf(string, "GPU Memory: %s%s", values, unit);
				} else if (strcmp(name, "GS") == 0) {
					sprintf(string, "GPU Shader: %s%s", values, unit);
				} else if (strcmp(name, "GC") == 0) {
					sprintf(string, "GPU Core: %s%s", values, unit);
				} else if (strcmp(name, "GR") == 0) {
					sprintf(string, "GPU Memory Controller: %s%s", values,
							unit);
				} else if (strcmp(name, "GV") == 0) {
					sprintf(string, "GPU Video Engine: %s%s", values, unit);
				} else if (strcmp(name, "RL") == 0) {
					sprintf(string, "Memory Load: %s%s", values, unit);
				} else if (strcmp(name, "RU") == 0) {
					sprintf(string, "Used Memory: %s%s", values, unit);
				} else if (strcmp(name, "RA") == 0) {
					sprintf(string, "Available Memory: %s%s", values, unit);
				} else if (strcmp(name, "HL") == 0) {
					sprintf(string, "Used HDD Space: %s%s", values, unit);
				} else if (strcmp(name, "HT") == 0) {
					sprintf(string, "HDD Temperature: %s%s", values, unit);
				}

				for (i = 0; i < 16; i++) {
					if (i == 15) {
						top[i] = '\0';
					}

					top[i] = string[i];
					bottom[i] = string[count];
					count++;
				}

				LCD_MoveToPosition(0x00);
				LCD_Print(top);
				LCD_MoveToPosition(0x40);
				LCD_Print(bottom);
				count = 16;
				x = 0;
				j = 0;

				for (i = 0; i < 10; i++) {
					if (i < 2) {
						letter[i] = '\0';
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

void init() {
	/* STM32F4 discovery LEDs */
	GPIO_InitTypeDef LED_Config;

	/* Always remember to turn on the peripheral clock...  If not, you may be up till 3am debugging... */
	RCC_AHB1PeriphClockCmd(RCC_AHB1Periph_GPIOD, ENABLE);
	LED_Config.GPIO_Pin = GPIO_Pin_12 | GPIO_Pin_13 | GPIO_Pin_14 | GPIO_Pin_15;
	LED_Config.GPIO_Mode = GPIO_Mode_OUT;
	LED_Config.GPIO_OType = GPIO_OType_PP;
	LED_Config.GPIO_Speed = GPIO_Speed_25MHz;
	LED_Config.GPIO_PuPd = GPIO_PuPd_NOPULL;
	GPIO_Init(GPIOD, &LED_Config);

	/* Setup SysTick or CROD! */
	if (SysTick_Config(SystemCoreClock / 1000)) {
		ColorfulRingOfDeath();
	}

	/* Setup USB */
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
