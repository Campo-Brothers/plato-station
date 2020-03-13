// ESP8266 WiFi <-> UART Bridge

#include <ESP8266WiFi.h>

// config: ////////////////////////////////////////////////////////////

#define UART_BAUD 19200
#define packTimeout 5 // ms (if nothing more on UART, then send packet)
#define bufferSize 8192

// For STATION mode:
const char *ssid = "FASTWEB-1-D31843";  // Your ROUTER SSID
const char *pw = "25698D48B1"; // and WiFi PASSWORD
const int port = 502;

//////////////////////////////////////////////////////////////////////////

#include <WiFiClient.h>
WiFiServer server(port);
WiFiClient client;

uint8_t buf1[bufferSize];
uint8_t i1=0;

uint8_t buf2[bufferSize];
uint8_t i2=0;

void setup() 
{
  delay(500);
  pinMode(D0, OUTPUT);  
  Serial.begin(UART_BAUD);
  WiFi.mode(WIFI_STA);
  WiFi.begin(ssid, pw);
  while (WiFi.status() != WL_CONNECTED) 
  {
    delay(100);
  }
  server.begin(); // start TCP server 
}

void loop() 
{
  if(!client.connected()) 
  { 
    client = server.available(); 
    return;
  }

  if(client.available()) 
  {
    while(client.available()) 
    {
      buf1[i1] = (uint8_t)client.read(); 
      if(i1<bufferSize-1) i1++;
    }
    // now send to UART:
    digitalWrite(D0, HIGH);
    Serial.write(buf1, i1);
    delay(10);
    digitalWrite(D0, LOW);
    i1 = 0;
  }

  if(Serial.available()) 
  {
    while(1) {
      if(Serial.available()) 
      {
        buf2[i2] = (char)Serial.read(); 
        if(i2<bufferSize-1) i2++;
      } 
      else 
      {
        delay(packTimeout);
        if(!Serial.available()) {
          break;
        }
      }
    }
    client.write((char*)buf2, i2);
    i2 = 0;
  }
}
