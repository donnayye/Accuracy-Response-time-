const int forceSensor1Pin = A0; // Analog input pin for force sensor 1
const int forceSensor2Pin = A1; // Analog input pin for force sensor 2
const int button1Pin = 2; // Digital input pin for button 1
const int button2Pin = 3; // Digital input pin for button 2
const int led1Pin = 4; // Digital output pin for LED 1
const int led2Pin = 5; // Digital output pin for LED 2

void setup() {
  Serial.begin(9600); // Start serial communication
  pinMode(button1Pin, INPUT_PULLUP); // Set button 1 pin to input with pull-up resistor
  pinMode(button2Pin, INPUT_PULLUP); // Set button 2 pin to input with pull-up resistor
  pinMode(led1Pin, OUTPUT); // Set LED 1 pin to output
  pinMode(led2Pin, OUTPUT); // Set LED 2 pin to output
}

void loop() {
  // Read force sensor 1 and print the value
  int forceSensor1Value = analogRead(forceSensor1Pin);
  Serial.print("Force sensor 1: ");
  Serial.println(forceSensor1Value);

  // Read force sensor 2 and print the value
  int forceSensor2Value = analogRead(forceSensor2Pin);
  Serial.print("Force sensor 2: ");
  Serial.println(forceSensor2Value);

  // Check button 1 state and control LED 1
  bool button1State = digitalRead(button1Pin);
  digitalWrite(led1Pin, button1State ? LOW : HIGH); // LED 1 is on if button 1 is pressed

  // Check button 2 state and control LED 2
  bool button2State = digitalRead(button2Pin);
  digitalWrite(led2Pin, button2State ? LOW : HIGH); // LED 2 is on if button 2 is pressed

  // Print button state changes with time stamp
  static bool button1PrevState = false;
  static bool button2PrevState = false;
  bool button1StateChanged = (button1State != button1PrevState);
  bool button2StateChanged = (button2State != button2PrevState);
  if (button1StateChanged || button2StateChanged) {
    Serial.print("Button state changed at ");
    Serial.print(millis());
    Serial.print(" ms: ");
    if (button1StateChanged) {
      Serial.print("Button 1: ");
      Serial.print(button1State ? "pressed" : "released");
    }
    if (button2StateChanged) {
      if (button1StateChanged) {
        Serial.print(", ");
      }
      Serial.print("Button 2: ");
      Serial.print(button2State ? "pressed" : "released");
    }
    Serial.println();
  }

  // Update previous button states
  button1PrevState = button1State;
  button2PrevState = button2State;

  delay(100); // Wait for 100ms before reading sensors and buttons again
}
