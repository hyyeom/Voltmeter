#include <MsTimer2.h>

int voltage = 0;
int channel = 0;
String inString = ""; 
int cmd = 1;
int pOut = 8;

void setup() {
 MsTimer2::set(10, readVoltage); // 10ms period
  MsTimer2::start();
  Serial.begin(57600);
}

void loop() {

}

ISR(TIMER1_COMPA_vect){
  
  readVoltage();
}

void readVoltage()
{
   digitalWrite(pOut, HIGH);
    voltage=analogRead(channel);
    Serial.print(voltage>>8);
    Serial.print("\r\n");
    Serial.print(voltage%256);
    Serial.print("\r\n");
    digitalWrite(pOut, LOW);  
}

int ReadSerial()
{
    while (Serial.available() > 0) {
    int inChar = Serial.read();
    if (isDigit(inChar)) {
      inString += (char)inChar;
    }
    if (inChar == '\n') {
      cmd = inString.toInt();
      inString = "";
      return cmd;
    }
  }
  return 0;
}
