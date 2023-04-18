// Define pins for buttons and LED
const int buttonPin1 = 2;
const int buttonPin2 = 3;
const int ledPin1 = 4;
const int ledPin2 = 5;

// Define variables for storing force sensor values
int forceSensor1Value = 0;
int forceSensor2Value = 0;

void setup() {
  // Set button and LED pins as inputs and outputs
  pinMode(buttonPin1, INPUT_PULLUP); // Use internal pull-up resistors
  pinMode(buttonPin2, INPUT_PULLUP); // Use internal pull-up resistors
  pinMode(ledPin1, OUTPUT);
  pinMode(ledPin2, OUTPUT);

  // Set baud rate for communication with Unity
  Serial.begin(9600);
}

void loop() {
  // Read button states
  int buttonState1 = !digitalRead(buttonPin1); // Invert button state
  int buttonState2 = !digitalRead(buttonPin2); // Invert button state

  // Turn on/off LEDs based on button states
  digitalWrite(ledPin1, !buttonState1);
  digitalWrite(ledPin2, !buttonState2);

  // Read force sensor values
  forceSensor1Value = analogRead(A5);
  forceSensor2Value = analogRead(A4);

  // Send button and force sensor data to Unity
  Serial.print(buttonState1);
  Serial.print(",");
  Serial.print(buttonState2);
  Serial.print(",");
  Serial.print(forceSensor1Value);
  Serial.print(",");
  Serial.print(forceSensor2Value);
  Serial.println(); // Print a newline to separate lines

  // Delay to prevent overwhelming Unity with data
  delay(10);
}
