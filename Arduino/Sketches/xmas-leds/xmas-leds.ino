#include <Console.h>
#include <Mailbox.h>
#include "xmas-leds.h"

float currentR = 0;
float currentG = 0;
float currentB = 0;

int increments = 1000;
bool DEBUG = true;
int loopCount = 50;
RGB colours[10];
RGB prevColour = {0, 0, 0};
int numColours;

void setup() {
  Bridge.begin();
  Mailbox.begin();
  Console.begin();
  pinMode(RED_PIN, OUTPUT);
  pinMode(GREEN_PIN, OUTPUT);
  pinMode(BLUE_PIN, OUTPUT);
}

int freeRam () {
  extern int __heap_start, *__brkval; 
  int v; 
  return (int) &v - (__brkval == 0 ? (int) &__heap_start : (int) __brkval); 
}

void loop() {
  Console.println("\n[memCheck]");
  Console.println(freeRam());  
  
  uint8_t message[128];
  int i;

  if (Mailbox.messageAvailable()) {
    parseMessage();
    turnOffLeds();
    delay(500);
  }

  for (int j = 0; j < numColours; j++) {
    fade(&colours[j]);
    delay(3000);

    if (Mailbox.messageAvailable()) {
      break;
    }
  }
}

void setColour(struct RGB *colour, long value) {
  colour->r = (value >> 16) & 0xFF;
  colour->g = (value >> 8) & 0xFF;
  colour->b = value & 0xFF;
}

void turnOffLeds() {
  analogWrite(RED_PIN, 0);
  analogWrite(GREEN_PIN, 0);
  analogWrite(BLUE_PIN, 0);

  currentR = 0;
  currentG = 0;
  currentB = 0;

  prevColour.r = 0;
  prevColour.g = 0;
  prevColour.b = 0;
}

void parseMessage() {
  uint8_t message[128];
  int i;

  Mailbox.readMessage(message, 128);
  Console.print("Received message: ");
  Console.println((char *)message);

  char *token = strtok((char *)message, ",");

  while (token != NULL && i < 10) {
    Console.println(token);

    long num = strtol(token, NULL, 16);
    setColour(&colours[i++], num);

    token = strtok(NULL, ",");
  }

  numColours = i;
  Console.print("Number of items: ");
  Console.println(numColours);
}

float calculateFadeSteps(int prevValue, int endValue) {
  float diff = endValue - prevValue;
  return diff / (float)increments;
}

float calculateVal(float step, float val) {
  val += step;

  if (val > 255) {
    val = 255;
  }
  else if (val < 0) {
    val = 0;
  }

  return val;
}

void printLedStatus() {
  Console.print("LED Status: (");
  Console.print((int)currentR);
  Console.print(", ");
  Console.print((int)currentG);
  Console.print(", ");
  Console.print((int)currentB);
  Console.println(")");
}

void printFadeMessage(struct RGB *colour) {
  Console.print("Fading to (");
  Console.print(colour->r);
  Console.print(", ");
  Console.print(colour->g);
  Console.print(", ");
  Console.print(colour->b);
  Console.println(")");
}

bool shouldPrintFadeStatus(int iteration) {
  return (DEBUG && (iteration == 0 || iteration % loopCount == 0));
}

void fade(struct RGB *colour) {
  printFadeMessage(colour);

  float stepR = calculateFadeSteps((int)prevColour.r, (int)colour->r);
  float stepG = calculateFadeSteps((int)prevColour.g, (int)colour->g);
  float stepB = calculateFadeSteps((int)prevColour.b, (int)colour->b);

  for (int i = 0; i <= increments; i++) {
    currentR = calculateVal(stepR, currentR);
    currentG = calculateVal(stepG, currentG);
    currentB = calculateVal(stepB, currentB);

    analogWrite(RED_PIN, currentR);
    analogWrite(GREEN_PIN, currentG);
    analogWrite(BLUE_PIN, currentB);

    if (shouldPrintFadeStatus(i)) {
      printLedStatus();
    }
  }

  // Update current values for next loop
  prevColour.r = currentR;
  prevColour.g = currentG;
  prevColour.b = currentB;
}
