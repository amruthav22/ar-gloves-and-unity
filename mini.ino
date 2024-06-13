const int flexPin = A0;
const int flexPin1 = A1;
const int flexPin2 = A2;
const float VCC = 5.0;            // voltage at Arduino 5V line
const float R_DIV = 47000.0;      // resistor used to create a voltage divider
const float flatResistance = 25000.0;    // resistance when flat
const float bendResistance = 100000.0;   // resistance at 90 deg

void setup() {
    Serial.begin(9600);
}

void loop() {
    // Read values from flex sensors
    int flexValue1 = analogRead(flexPin); // Read value from flex sensor 1
    int flexValue2 = analogRead(flexPin1); // Read value from flex sensor 2
   // int flexValue3 = analogRead(flexPin2); // Read value from flex sensor 3

    // Process flex sensor 1
    float angle1 = calculateAngle(flexValue1);
    if(angle1>=10){
    Serial.print("Flex Sensor 1: ");
    Serial.print(angle1);
    Serial.println(" degrees");
    delay(1000);
    }
    // Process flex sensor 2
    float angle2 = calculateAngle(flexValue2);
    //Serial.print("Flex Sensor 2: ");
    //Serial.println(angle2);
    if (angle2 >= 27.0 && angle2<=57.0) {
        Serial.println("move 10 cm left");
    }
    
    
    delay(1000);

    // Process flex sensor 3
    

    // Add a small delay to avoid overwhelming the serial monitor
    delay(2000);
}

float calculateAngle(int flexValue) {
    float Vflex = flexValue * VCC / 1023.0;
    float Rflex = R_DIV * (VCC / Vflex - 1.0);
    // Map resistance to an angle between 0 and 90 degrees
    float angle = map(Rflex, flatResistance, bendResistance, 0, 90); // Adjust the range as needed
    return angle;
}

// Custom map function for floating-point numbers
float map(float x, float in_min, float in_max, float out_min, float out_max) {
    return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
}