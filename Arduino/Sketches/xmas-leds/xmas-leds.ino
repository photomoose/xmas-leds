#include <Console.h>
#include <Mailbox.h>
#include "xmas-leds.h"

int fadeIncrements;
int fadeIncrementDuration;
int fadeColourDuration;
int fadeOffDuration;
int strobeInterval;
int strobeColourDuration;
int cycleColourDuration;
int flashDurationOn;
int flashDurationOff;

bool DEBUG = true;
int loopCount = 50;
RGB colours[NUM_COLOURS];
int numColours;
char programme;


const int messagePollFrequency = 5000;
unsigned long lastMessagePoll;

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

  if (Mailbox.messageAvailable()) {
    parseMessage();
    turnOffLeds();
    delay(1500);
  }
  
  switch (programme) {
    case FADEINOUT:
      fadeInOut(fadeIncrements, fadeIncrementDuration);
      break;
      
    case FADEOUT:
      fadeOut(fadeColourDuration, fadeOffDuration, fadeIncrements, fadeIncrementDuration);
      break;
      
    case STROBE:
      strobe(strobeInterval, strobeColourDuration);
      break;
    
    case CYCLE:
      cycle(cycleColourDuration);
      break;
      
    case FLASH:
      flashAlternate(flashDurationOn, flashDurationOff);
      break;
    
    case DEFAULTFADE:
      fadeAlternate(fadeColourDuration, fadeIncrements, fadeIncrementDuration);
      break;
      
    default:  
      fadeAlternate(3000, 1000, 0);  
  }
}

void initColour(struct RGB *colour, long value) {
  colour->r = (value >> 16) & 0xFF;
  colour->g = (value >> 8) & 0xFF;
  colour->b = value & 0xFF;
}

void turnOffLeds() {
  setLedColour(BLACK);
}

void parseMessage() {
  uint8_t message[MESSAGE_SIZE];
  int i;

  Mailbox.readMessage(message, MESSAGE_SIZE);
  Console.print("Received message: ");
  Console.println((char *)message);

  char *token = strtok((char *)message, ",");

  if (strlen(token) == 1) {
    programme = token[0];
    Console.print("Programme: ");
    Console.println(programme);
    
    if (programme == DEFAULTFADE) {
      token = strtok(NULL, ",");
      fadeIncrements = atoi(token);
      token = strtok(NULL, ",");
      fadeIncrementDuration = atoi(token);
      token = strtok(NULL, ",");
      fadeColourDuration = atoi(token);
    } else if (programme == FADEINOUT) {
      token = strtok(NULL, ",");
      fadeIncrements = atoi(token);
      token = strtok(NULL, ",");
      fadeIncrementDuration = atoi(token);
    } else if (programme == FADEOUT) {
      token = strtok(NULL, ",");
      fadeIncrements = atoi(token);
      token = strtok(NULL, ",");
      fadeIncrementDuration = atoi(token);   
      token = strtok(NULL, ",");
      fadeColourDuration = atoi(token);
      token = strtok(NULL, ",");
      fadeOffDuration = atoi(token);      
    } else if (programme == STROBE) {
      token = strtok(NULL, ",");
      strobeInterval = atoi(token);
      token = strtok(NULL, ",");
      strobeColourDuration = atoi(token);
    } else if (programme == CYCLE) {
      token = strtok(NULL, ",");
      cycleColourDuration = atoi(token);
    } else if (programme == FLASH) {
      token = strtok(NULL, ",");
      flashDurationOn = atoi(token);
      token = strtok(NULL, ",");
      flashDurationOff = atoi(token);   
    }   
        
    token = strtok(NULL, ",");
  } else {
    programme = NULL;
  }
  
  while (token != NULL && i < NUM_COLOURS) {
    Console.println(token);

    long num = strtol(token, NULL, 16);
    initColour(&colours[i++], num);

    token = strtok(NULL, ",");
  }

  numColours = i;
  Console.print("Number of colours: ");
  Console.println(numColours);
}

float calculateFadeSteps(int prevValue, int endValue, int increments) {
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
  Console.print(currentColour.r);
  Console.print(", ");
  Console.print(currentColour.g);
  Console.print(", ");
  Console.print(currentColour.b);
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

void fadeInOut(int increments, int incrementDuration) {
  Console.println("Programme: Fade In/Out");
      
  for (int j = 0; j < numColours; j++) {
    fade(&colours[j], increments, incrementDuration);
   
    if (isMessageAvailable()) {
      break;
    }
    
    fade(&BLACK, increments, incrementDuration);

    if (isMessageAvailable()) {
      break;
    }
  }  
}

void fadeOut(int colourDuration, int offDuration, int increments, int incrementDuration) {
  Console.println("Programme: Fade Out");
  
  for (int j = 0; j < numColours; j++) {
    setLedColour(colours[j]);
    printLedStatus();
    delay(colourDuration);
    
    fade(&BLACK, increments, incrementDuration);
    delay(offDuration);

    if (isMessageAvailable()) {
      break;
    }
  }  
}

void fadeAlternate(int colourDuration, int increments, int incrementDuration) {
  for (int j = 0; j < numColours; j++) {
    fade(&colours[j], increments, incrementDuration);
    delay(colourDuration);

    if (isMessageAvailable()) {
      break;
    }
  }
}

void flashAlternate(int durationOn, int durationOff) {
  Console.println("Programme: Flash Alternate");
  
  for (int j = 0; j < numColours; j++) {
    setLedColour(colours[j]);
  
    delay(durationOn);
    
    analogWrite(RED_PIN, 0);
    analogWrite(GREEN_PIN, 0);
    analogWrite(BLUE_PIN, 0);
    
    delay(durationOff);
  } 
}

void cycle(int cycleColourDuration) {
  Console.println("Programme: Cycle");
  
  for (int j = 0; j < numColours; j++) {
    setLedColour(colours[j]);
    
    delay(cycleColourDuration);
    
    if (isMessageAvailable()) {
      break;
    }    
  }
}

void strobe(int interval, int colourDuration) {
  Console.println("Programme: Strobe");
  
  unsigned long until;
    
  for (int j = 0; j < numColours; j++) { 
   until = millis() + colourDuration;
   
    while (millis() < until) {

      setLedColour(colours[j]);
    
      delay(interval);
      
      analogWrite(RED_PIN, 0);
      analogWrite(GREEN_PIN, 0);
      analogWrite(BLUE_PIN, 0);
      
      delay(interval);      
    }
    
    if (isMessageAvailable()) {
      break;
    } 
  }  
}

void fade(struct RGB *colour, int increments, int incrementDuration) {
  printFadeMessage(colour);

  float stepR = calculateFadeSteps(currentColour.r, (int)colour->r, increments);
  float stepG = calculateFadeSteps(currentColour.g, (int)colour->g, increments);
  float stepB = calculateFadeSteps(currentColour.b, (int)colour->b, increments);

  float r = currentColour.r;
  float g = currentColour.g;
  float b = currentColour.b;

  for (int i = 0; i <= increments; i++) {
    r = calculateVal(stepR, r);
    g = calculateVal(stepG, g);
    b = calculateVal(stepB, b);

    setLedColour({r, g, b});

    if (shouldPrintFadeStatus(i)) {
      printLedStatus();
    }
    
    if (isMessageAvailable()) {
      break;
    }    
    
    delay(incrementDuration);
  }
}

bool isMessageAvailable() {
  bool result = false;
  
  if (millis() > lastMessagePoll + messagePollFrequency) {
    result = Mailbox.messageAvailable();
    lastMessagePoll = millis();
  }

  return result;
}  

void setLedColour(struct RGB colour) {
  analogWrite(RED_PIN, colour.r);
  analogWrite(GREEN_PIN, colour.g);
  analogWrite(BLUE_PIN, colour.b);
  
  currentColour = colour;
}
