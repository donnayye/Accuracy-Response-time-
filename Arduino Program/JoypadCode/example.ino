// Define pins for buttons and LED
const int buttonPin1 = 2;
const int buttonPin2 = 3;
const int ledPin1 = 4;
const int ledPin2 = 5;

// Define variables for storing force sensor values and button state
int forceSensor1Value = 0;
int forceSensor2Value = 0;
int buttonState1 = 0;
int buttonState2 = 0;
void setup() {
  // Set button and LED pins as inputs and outputs
  pinMode(buttonPin1, INPUT); 
  pinMode(buttonPin2, INPUT); 
  pinMode(ledPin1, OUTPUT);
  pinMode(ledPin2, OUTPUT);

  // Set baud rate for communication with Unity
  Serial.begin(9600);
}

void loop() {
 digitalWrite(ledPin1 , HIGH) ;
 digitalWrite(ledPin2, HIGH); 
  // Read button states
  // read the state of the pushbutton value:
  digitalWrite(buttonPin1, HIGH);
  buttonState1 = digitalRead(buttonPin1);

  // check if the pushbutton is pressed.
  // if it is, the buttonState is HIGH:
  if (buttonState1 == LOW) {     
    // turn LED on:    
    digitalWrite(buttonPin1, HIGH);  
  } 
  else {
    // turn LED off:
    digitalWrite(buttonPin1, LOW); 
  }

  // Read button states
  // read the state of the pushbutton value:
  digitalWrite(buttonPin2, HIGH);
  buttonState2 = digitalRead(buttonPin2);

  // check if the pushbutton is pressed.
  // if it is, the buttonState is HIGH:
  if (buttonState2 == LOW) {     
    // turn LED on:    
    digitalWrite(buttonPin2, HIGH);  
  } 
  else {
    // turn LED off:
    digitalWrite(buttonPin2, LOW); 
  }
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
