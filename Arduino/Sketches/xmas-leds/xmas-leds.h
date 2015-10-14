struct RGB {
  byte r;
  byte g;
  byte b;
};

const int RED_PIN = 10;
const int BLUE_PIN = 11;
const int GREEN_PIN = 9;

void setColour(struct RGB *colour, long value);
void turnOffLeds();
void parseMessage();
float calculateFadeSteps(int prevValue, int endValue);
float calculateVal(float step, float val);
void printLedStatus();
void printFadeMessage(struct RGB *colour);
bool shouldPrintFadeStatus(int iteration);
void fade(struct RGB *colour);

