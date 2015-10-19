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
char programme;
const char FADE = 'f';
const char FLASH = 'F';
const char STROBE = 's';
const char CYCLE = 'c';

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
  
  switch (programme) {
    case FADE:
      fadeInOut();
      break;
      
    case STROBE:
      strobe();
      break;
    
    case CYCLE:
      cycle();
      break;
      
    case FLASH:
      flashAlternate();
      break;
      
    default:  
      fadeAlternate();  
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

  if (strlen(token) == 1) {
    programme = token[0];
    Console.print("Programme: ");
    Console.println(programme);
    token = strtok(NULL, ",");
  } else {
    programme = NULL;
  }
  
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

void flash() {
  for (int i = 0; i < numColours; i++) {
    for (int j = 0; j < 4; j++) {
      analogWrite(RED_PIN, colours[i].r);
      analogWrite(GREEN_PIN, colours[i].g);
      analogWrite(BLUE_PIN, colours[i].b);
    
      delay(1000);
      
      analogWrite(RED_PIN, 0);
      analogWrite(GREEN_PIN, 0);
      analogWrite(BLUE_PIN, 0);
      
      delay(1000);
    }
    
    if (Mailbox.messageAvailable()) {
      break;
    }    
  }  
}

void fadeInOut() {
  struct RGB black = {0, 0, 0};
  
  analogWrite(RED_PIN, 0);
  analogWrite(GREEN_PIN, 0);
  analogWrite(BLUE_PIN, 0);
      
  for (int j = 0; j < numColours; j++) {
    fade(&colours[j]);
    fade(&black);

    if (Mailbox.messageAvailable()) {
      break;
    }
  }  
}
void fadeAlternate() {
  for (int j = 0; j < numColours; j++) {
    fade(&colours[j]);
    delay(3000);

    if (Mailbox.messageAvailable()) {
      break;
    }
  }
}

void flashAlternate() {
  for (int i = 0; i < 4; i++) {
    for (int j = 0; j < numColours; j++) {
      analogWrite(RED_PIN, colours[j].r);
      analogWrite(GREEN_PIN, colours[j].g);
      analogWrite(BLUE_PIN, colours[j].b);
    
      delay(1000);
      
      analogWrite(RED_PIN, 0);
      analogWrite(GREEN_PIN, 0);
      analogWrite(BLUE_PIN, 0);
      
      delay(1000);
    }
    
    if (Mailbox.messageAvailable()) {
      break;
    }    
  }  
}

void cycle() {
  unsigned long until;
  until = millis() + 10000;
  
  while (millis() < until) {
    for (int j = 0; j < numColours; j++) {
      analogWrite(RED_PIN, colours[j].r);
      analogWrite(GREEN_PIN, colours[j].g);
      analogWrite(BLUE_PIN, colours[j].b);
    
      delay(1000);
    }
    
    if (Mailbox.messageAvailable()) {
      break;
    }    
  }  
}

void strobe() {
  unsigned long until;
    
  for (int j = 0; j < numColours; j++) { 
   until = millis() + 4000;
   
    while (millis() < until) {

      analogWrite(RED_PIN, colours[j].r);
      analogWrite(GREEN_PIN, colours[j].g);
      analogWrite(BLUE_PIN, colours[j].b);
    
      delay(30);
      
      analogWrite(RED_PIN, 0);
      analogWrite(GREEN_PIN, 0);
      analogWrite(BLUE_PIN, 0);
      
      delay(30);      
    }
    
    if (Mailbox.messageAvailable()) {
      break;
    }    
  }  
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
