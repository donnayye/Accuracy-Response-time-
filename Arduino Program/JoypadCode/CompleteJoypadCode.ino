#include <SoftwareSerial.h>

// Define pins for buttons and LED
const int buttonPin1 = 2;
const int buttonPin2 = 3;
const int ledPin1 = 4;
const int ledPin2 = 5;

// Define variables for storing button states
int buttonState1 = 0;
int buttonState2 = 0;

// Define variables for storing force sensor values
int forceSensor1Value = 0;
int forceSensor2Value = 0;

// Create a SoftwareSerial object for communicating with Unity
SoftwareSerial unitySerial(10, 11);

void setup() {
  // Set button and LED pins as inputs and outputs
  pinMode(buttonPin1, INPUT);
  pinMode(buttonPin2, INPUT);
  pinMode(ledPin1, OUTPUT);
  pinMode(ledPin2, OUTPUT);

  // Set baud rate for communication with Unity
  unitySerial.begin(9600);
}

void loop() {
  // Read button states
  buttonState1 = digitalRead(buttonPin1);
  buttonState2 = digitalRead(buttonPin2);

  // Turn on/off LEDs based on button states
  digitalWrite(ledPin1, !buttonState1);
  digitalWrite(ledPin2, !buttonState2);

  // Read force sensor values
  forceSensor1Value = analogRead(A0);
  forceSensor2Value = analogRead(A1);

  // Send button and force sensor data to Unity
  unitySerial.print(buttonState1);
  unitySerial.print(",");
  unitySerial.print(buttonState2);
  unitySerial.print(",");
  unitySerial.print(forceSensor1Value);
  unitySerial.print(",");
  unitySerial.println(forceSensor2Value);

  // Delay to prevent overwhelming Unity with data
  delay(100);
}
