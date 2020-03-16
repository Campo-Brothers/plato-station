//ESP -> MODBUS bridge demo


#include <SoftwareSerial.h>
#include <ESP8266WiFi.h>
#include <WiFiClient.h>

const char* ssid = "FASTWEB-1-D31843";
const char* password = "25698D48B1";

// Start a TCP Server on port 502
WiFiServer server(502);
WiFiClient client ;

const long interval = 50 ;           // interval (milliseconds)
size_t len = 255;
uint8_t sbuf[255];
uint8_t rtu_buf[255];
uint8_t rtu_len;
uint16_t calcCRC(uint8_t u8_buff_size)
{
  uint32_t tmp, tmp2, flag;
  tmp = 0xFFFF;
  for (uint8_t i = 0; i < u8_buff_size; i++)
  {
    tmp = tmp ^ rtu_buf[i];
      for (uint8_t j = 1; j <= 8; j++)
    {
      flag = tmp & 0x0001;
      tmp >>= 1;
      if (flag)
        tmp ^= 0xA001;
    }
  }
  tmp2 = tmp >> 8;
  tmp = (tmp << 8) | tmp2;
  tmp &= 0xFFFF;

  return tmp;
}


void setup() {
  pinMode(D0,OUTPUT); //re rs485
 
 
  Serial.begin(19200);
  WiFi.begin(ssid,password);
 
  //Wait for connection
  while(WiFi.status() != WL_CONNECTED) {
    delay(100);
    //swSer.print(".");
  }
 
  server.begin();
 
}

void loop() {
      bailout:
      Serial.flush();
      yield();
      if (!client.connected()) {
      client = server.available();
      }
      else{     
   
    if (client.available() > 0) {
     
      int readnum = client.read(sbuf,client.available());
      rtu_len = sbuf[5];
      for (int i=0 ;i<readnum-6;i++){
        rtu_buf[i] = sbuf[i+6];
      }
      int value = calcCRC(rtu_len);
      rtu_buf[rtu_len]  = (value & 0xFF00) >> 8;
      rtu_buf[rtu_len+1] = value & 0x00FF;
      Serial.flush();
      digitalWrite(D0,HIGH);
      delay(1);
      Serial.write(rtu_buf,rtu_len + 2);
      Serial.flush();
      digitalWrite(D0,LOW);
      memset(rtu_buf, 0, 50);
      unsigned long currentMillis = millis();
      while(!Serial.available()){
        yield();
         if( millis() - currentMillis  > interval){
          goto bailout;
         }
        }
        int i = 6;
        currentMillis = millis();
       
        while (Serial.available() > 0)  {
          rtu_buf[i] = Serial.read();
          i ++;
          yield();
          if( millis() - currentMillis  > interval){
           
          goto bailout;
         }
           }
           if ( i > 10){
           rtu_len = i- 2;
           rtu_buf[5] = rtu_len -6 ;
           rtu_buf[0] = sbuf[0];
           rtu_buf[1] = sbuf[1];
           client.write((const uint8_t*)rtu_buf,rtu_len);
           Serial.flush();
           }
  }
 }
}
