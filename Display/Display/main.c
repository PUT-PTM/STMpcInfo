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

volatile uint32_t ticker, downTicker;

/*
 * The USB data must be 4 byte aligned if DMA is enabled. This macro handles
 * the alignment, if necessary (it's actually magic, but don't tell anyone).
 */
__ALIGN_BEGIN USB_OTG_CORE_HANDLE USB_OTG_dev __ALIGN_END;

void init();
void ColorfulRingOfDeath(void);

/*
 * Define prototypes for interrupt handlers here. The conditional "extern"
 * ensures the weak declarations from startup_stm32f4xx.c are overridden.
 */
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


void TimerInit() {
	RCC_APB1PeriphClockCmd(RCC_APB1Periph_TIM4, ENABLE);
	TIM_TimeBaseInitTypeDef TIM_TimeBaseStructure;
	TIM_TimeBaseStructure.TIM_Period = 9999;
	TIM_TimeBaseStructure.TIM_Prescaler = 41999;
	TIM_TimeBaseStructure.TIM_ClockDivision = 1;
	TIM_TimeBaseStructure.TIM_CounterMode = TIM_CounterMode_Up;
	TIM_TimeBaseInit(TIM4, &TIM_TimeBaseStructure);
}
void PrzerwanieInit() {
	SYSCFG_EXTILineConfig(GPIOA, EXTI_PinSource0);
	NVIC_InitTypeDef NVIC_InitStructure;
	NVIC_InitStructure.NVIC_IRQChannel = EXTI0_IRQn;
	NVIC_InitStructure.NVIC_IRQChannelPreemptionPriority = 0x00;
	NVIC_InitStructure.NVIC_IRQChannelSubPriority = 0x01;
	NVIC_InitStructure.NVIC_IRQChannelCmd = ENABLE;
	NVIC_Init(&NVIC_InitStructure);
	EXTI_InitTypeDef EXTI_InitStructure;
	EXTI_InitStructure.EXTI_Line = EXTI_Line0;
	EXTI_InitStructure.EXTI_Mode = EXTI_Mode_Interrupt;
	EXTI_InitStructure.EXTI_Trigger = EXTI_Trigger_Rising;
	EXTI_InitStructure.EXTI_LineCmd = ENABLE;
	EXTI_Init(&EXTI_InitStructure);
}
int main(void) {
	SystemInit();
	uint8_t theByte = 0;
	int i = 0;
	int j = 0;
	int y = 0;
	int z = 0;
	char tab[29][26] = { '\0' };
	char top[16] = { '\0' };
	char bottom[16] = { '\0' };
	int count = 0;
	char clear[17] = { ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ',
			' ', ' ', ' ', ' ', ' ', '\0' };
	PrzerwanieInit();
	TimerInit();
	TIM_Cmd(TIM4, ENABLE);
	TIM_Cmd(TIM2, ENABLE);
	/* Initialize USB, IO, SysTick, and all those other things you do in the morning */
	init();
	// Custom character definitions
	uint8_t CustChar1[8] = { b00011, b00100, b01010, b10000, b10100, b01011,
			b00100, b00011 };

	uint8_t CustChar2[8] = { b11000, b00100, b01010, b00001, b00101, b11010,
			b00100, b11000 };

	// Initialize the LCD
	LCD_ConfigurePort(GPIOE, GPIO_Pin_5, GPIO_Pin_12, GPIO_Pin_6, NULL, NULL,
			NULL, NULL, GPIO_Pin_7, GPIO_Pin_8, GPIO_Pin_9, GPIO_Pin_10);
	LCD_Initalize(BUS_WIDTH_4, DISPLAY_LINES_2, FONT_5x10);
	LCD_Home();
	while (1) {
		int counter = TIM4->CNT;
		if (counter == 0 && y == 29) {

			y = 0;
			LCD_MoveToPosition(0x00);
			LCD_Print(clear);
			LCD_MoveToPosition(0x40);
			LCD_Print(clear);
			for (i = 0; i < 16; i++) {
				top[i] = '\0';
				bottom[i] = '\0';
			}
			for (i = 0; i < 16; i++) {
				if (i == 15) {
					top[i] = '\0';
				}

				top[i] = tab[z][i];
			}
			for (i = 16; i < 26; i++) {
				bottom[count] = tab[z][i];
				count++;
			}
			count = 0;
			LCD_MoveToPosition(0x00);
			LCD_Print(top);
			LCD_MoveToPosition(0x40);
			LCD_Print(bottom);
			z++;
			if (z == 29) {
				z = 0;
			}

		} else if (VCP_get_char(&theByte) && y != 29) {
			if (theByte != '\n') {
				tab[y][j] = theByte;
				j++;
			} else {
				for (i = j; i < 26; i++) {
					tab[y][i] = '\0';
				}
				y++;
				j = 0;
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

/*
 * Call this to indicate a failure.  Blinks the STM32F4 discovery LEDs
 * in sequence.  At 168Mhz, the blinking will be very fast - about 5 Hz.
 * Keep that in mind when debugging, knowing the clock speed might help
 * with debugging.
 */
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

/*
 * Interrupt Handlers
 */

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
