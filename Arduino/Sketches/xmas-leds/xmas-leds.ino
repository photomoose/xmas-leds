#include <Console.h>
#include <Mailbox.h>

const int RED = 10;
const int BLUE = 11;
const int GREEN = 9;

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
    
    Console.print("Red: ");
    Console.println(r);
    Console.print("Green: ");
    Console.println(g);
    Console.print("Blue: ");
    Console.println(b);
    
    analogWrite(RED, r);
    analogWrite(GREEN, g);
    analogWrite(BLUE, b);
  }
}
