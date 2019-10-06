// Written by Megan Alnico from original sources by Tony DiCola of Adafruit Industries
// Released under an MIT license.

// REQUIRES the following Arduino libraries:
// - DHT Sensor Library: https://github.com/adafruit/DHT-sensor-library
// - Adafruit Unified Sensor Lib: https://github.com/adafruit/Adafruit_Sensor

#include <Adafruit_Sensor.h>
#include <DHT.h>
#include <DHT_U.h>
#include <Servo.h>
#include <Math.h>

#define DHTPIN 2 
#define DHTTYPE    DHT22

DHT_Unified dht(DHTPIN, DHTTYPE);

uint32_t delayMS;

Servo windowServo;
int pos = 0;    // variable to store the servo position

void setup() {
  Serial.begin(115200);  
  pinMode(7, INPUT_PULLUP);
  pinMode(6, INPUT_PULLUP);
  // Initialize device.
  dht.begin();
  sensor_t sensor;
  dht.temperature().getSensor(&sensor);
  dht.humidity().getSensor(&sensor);
  delayMS = 500;

  windowServo.attach(9);
}

void loop() {
  
  int incomingByte = 0;
  
  String output = "";
  
  // Delay between measurements.
  delay(delayMS);
  

  int windowOpen = digitalRead(7);
  int toggleWindow;

  if (Serial.available() > 0) {
    // read the incoming byte:
    incomingByte = Serial.read();
    if(incomingByte == 'O'){
      windowServo.write(180);
    } else if(incomingByte == 'C'){
      windowServo.write(0);
    }
  }

  output = output + windowOpen + ",";

  // Get temperature event and print its value.
  sensors_event_t event;
  dht.temperature().getEvent(&event);
  if (isnan(event.temperature)) {
    Serial.println(F("Error reading temperature!"));
  }
  else {
    output = output + round(event.temperature) + ",";
  }
  // Get humidity event and print its value.
  dht.humidity().getEvent(&event);
  if (isnan(event.relative_humidity)) {
    Serial.println(F("Error reading humidity!"));
  }
  else {
    output = output + round(event.relative_humidity) + ";";
  }
  Serial.println(output);
}
