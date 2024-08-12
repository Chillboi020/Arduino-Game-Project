// Define pin connections
const int joystickXPin = A0;    // Joystick X-axis connected to analog pin A0
const int joystickYPin = A1;    // Joystick Y-axis connected to analog pin A1
const int buttonPin = 12;        // Button connected to digital pin 2

// Define thresholds for joystick direction
const int thresholdLeft = 400;   // Threshold for left direction
const int thresholdRight = 900;  // Threshold for right direction
const int thresholdDown = 400;   // Threshold for down direction
const int thresholdUp = 600;     // Threshold for up direction

void setup() {
  // Initialize the serial communication at 9600 baud rate
  Serial.begin(9600);

  // Set button pin as input with internal pull-up resistor
  pinMode(buttonPin, INPUT_PULLUP);
}

void loop() {
  static bool connected = false;
  
  if (!connected) {
    // Send "OK?" and wait for "OK!" response
    Serial.println("OK?");
    delay(1000);  // Wait for response
    
    if (Serial.available() > 0) {
      String response = Serial.readStringUntil('\n');
      response.trim();
      if (response == "OK!") {
        connected = true;
      }
    }
  } else {
    // Read the joystick X and Y values (0 to 1023)
    int xValue = analogRead(joystickXPin);
    int yValue = analogRead(joystickYPin);

    // Convert analog readings to -1, 0, or 1 using direction-specific thresholds
    int xDirection = getXDirection(xValue);
    int yDirection = getYDirection(yValue);

    // Read the button status (LOW when pressed, HIGH when not pressed)
    int buttonStatus = digitalRead(buttonPin);

    // Convert button status to 1 (pressed) or 0 (not pressed)
    buttonStatus = (buttonStatus == LOW) ? 1 : 0;

    // Print the values in the format "X-Value:Y-Value:Button-Status"
    Serial.print(xDirection);
    Serial.print(":");
    Serial.print(yDirection);
    Serial.print(":");
    Serial.println(buttonStatus);

    // Small delay to avoid flooding the serial port
    delay(15);
  }
}

// Function to determine X direction based on analog value
int getXDirection(int value) {
  if (value < thresholdLeft) {
    return -1;  // Left
  } else if (value > thresholdRight) {
    return 1;   // Right
  } else {
    return 0;   // Center
  }
}

// Function to determine Y direction based on analog value
int getYDirection(int value) {
  if (value < thresholdDown) {
    return -1;  // Down
  } else if (value > thresholdUp) {
    return 1;   // Up
  } else {
    return 0;   // Center
  }
}
