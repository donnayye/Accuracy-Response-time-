#include <SoftwareSerial.h>


SoftwareSerial mySerial(10, 11); // RX, TX

const int sensor1_pin = A0;
const int sensor2_pin = A1;
const int button1_pin = 2;
const int button2_pin = 3;

int sensor1_value = 0;
int sensor2_value = 0;
int button1_state = 0;
int button2_state = 0;
unsigned long button1_press_time = 0;
unsigned long button2_press_time = 0;

unsigned long currentMillis = 0;
unsigned long previousMillis = 0;
unsigned long interval = 1000; // set the recording interval to 1 second

void setup() {
  Serial.begin(9600);
  mySerial.begin(9600);
  pinMode(sensor1_pin, INPUT);
  pinMode(sensor2_pin, INPUT);
  pinMode(button1_pin, INPUT_PULLUP);
  pinMode(button2_pin, INPUT_PULLUP);
}

void loop() {
  currentMillis = millis();

  if (currentMillis - previousMillis >= interval) {
    previousMillis = currentMillis;

    // read sensor values
    sensor1_value = analogRead(sensor1_pin);
    sensor2_value = analogRead(sensor2_pin);

    // read button states
    button1_state = digitalRead(button1_pin);
    button2_state = digitalRead(button2_pin);

    // check if button1 is pressed or held down
    if (button1_state == LOW && button1_press_time == 0) {
      button1_press_time = currentMillis;
      mySerial.print(currentMillis);
      mySerial.print(",button1_pressed\n");
    } else if (button1_state == LOW && button1_press_time > 0) {
      mySerial.print(currentMillis);
      mySerial.print(",button1_held,");
      mySerial.print(currentMillis - button1_press_time);
      mySerial.print("\n");
    } else if (button1_state == HIGH && button1_press_time > 0) {
      mySerial.print(currentMillis);
      mySerial.print(",button1_released\n");
      button1_press_time = 0;
    }

    // check if button2 is pressed or held down
    if (button2_state == LOW && button2_press_time == 0) {
      button2_press_time = currentMillis;
      mySerial.print(currentMillis);
      mySerial.print(",button2_pressed\n");
    } else if (button2_state == LOW && button2_press_time > 0) {
      mySerial.print(currentMillis);
      mySerial.print(",button2_held,");
      mySerial.print(currentMillis - button2_press_time);
      mySerial.print("\n");
    } else if (button2_state == HIGH && button2_press_time > 0) {
      mySerial.print(currentMillis);
      mySerial.print(",button2_released\n");
      button2_press_time = 0;
    }

    // send sensor data to serial port
    mySerial.print(currentMillis);
    mySerial.print(",sensor1_value,");
    mySerial.print(sensor1_value);
    mySerial.print(",sensor2_value,");
    mySerial.print(sensor2_value);
    mySerial.print("\n");
  }
}
