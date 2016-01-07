struct RGB {
  byte r;
  byte g;
  byte b;
};

const int RED_PIN = 10;
const int BLUE_PIN = 11;
const int GREEN_PIN = 9;
const int NUM_COLOURS = 20;
const int MESSAGE_SIZE = 170;

const char FADEINOUT = 'f';
const char FLASH = 'F';
const char STROBE = 's';
const char CYCLE = 'c';
const char DEFAULTFADE = 'D';
const char FADEOUT = 'o';

struct RGB BLACK = {0, 0, 0};
struct RGB currentColour = BLACK; 
  
void initColour(struct RGB *colour, long value);
void turnOffLeds();
void parseMessage();
float calculateFadeSteps(int prevValue, int endValue);
float calculateVal(float step, float val);
void printLedStatus();
void printFadeMessage(struct RGB *colour);
bool shouldPrintFadeStatus(int iteration);
void fade(struct RGB *colour);
void fadeInOut(int increments, int incrementDuration);
void fadeOut(int colourDuration, int offDuration, int increments, int incrementDuration);
void flashAlternate(int durationOn, int durationOff);
void cycle(int cycleColourDuration);
void fadeAlternate(int colourDuration, int increments, int incrementDuration);  
void strobe(int interval, int colourDuration);
void setLedColour(struct RGB *colour);
