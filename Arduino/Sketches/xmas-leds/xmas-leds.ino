#include <Console.h>
#include <Mailbox.h>

const int RED = 10;
const int BLUE = 11;
const int GREEN = 9;

int prevR = 0;
float currentR = 0;
int prevG = 0;
float currentG = 0;
int prevB = 0;
float currentB = 0;

int increments = 1000;
int wait = 3;
int DEBUG = 1;
int loopCount = 50;

void setup() {
  Bridge.begin();
  Mailbox.begin();
  Console.begin();
  pinMode(RED, OUTPUT);
  pinMode(GREEN, OUTPUT);
  pinMode(BLUE, OUTPUT);
}

void loop() {
  String message;
  
  if (Mailbox.messageAvailable()) {
    Mailbox.readMessage(message);
    Console.print("Received message: ");
    Console.println(message);
    
    int commaIndex1 = message.indexOf(',');
    int commaIndex2 = message.indexOf(',', commaIndex1 + 1);
      
    int r = message.substring(0, commaIndex1).toInt();
    int g = message.substring(commaIndex1 + 1, commaIndex2).toInt();
    int b = message.substring(commaIndex2 + 1).toInt();
       
    fade(r, g, b);
  }
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

void fade(int r, int g, int b) {
  Console.print("Fading to (");
  Console.print(r);
  Console.print(", ");
  Console.print(g);
  Console.print(", ");
  Console.print(b);
  Console.println(")");
  
  float stepR = calculateFadeSteps(prevR, r);
  float stepG = calculateFadeSteps(prevG, g); 
  float stepB = calculateFadeSteps(prevB, b);
  
  if (DEBUG) {
    Console.print("Steps: ");
    Console.print(stepR);
    Console.print(" / ");
    Console.print(stepG);
    Console.print(" / ");
    Console.println(stepB);
  }
    
  for (int i = 0; i <= increments; i++) {
    currentR = calculateVal(stepR, currentR);
    currentG = calculateVal(stepG, currentG);
    currentB = calculateVal(stepB, currentB);

    analogWrite(RED, currentR); 
    analogWrite(GREEN, currentG);      
    analogWrite(BLUE, currentB); 

    //delay(wait);

    if (DEBUG) {
      if (i == 0 or i % loopCount == 0) { // b
        Console.print("Iteration #");
        Console.print(i);
        Console.print(": (");
        Console.print((int)currentR);
        Console.print(", ");
        Console.print((int)currentG);
        Console.print(", ");  
        Console.print((int)currentB);
        Console.println(")"); 
      } 
    }
  }
  
  // Update current values for next loop
  prevR = (int)currentR; 
  prevG = (int)currentG; 
  prevB = (int)currentB;
}
