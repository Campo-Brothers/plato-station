#include <ESP8266WebServer.h>
#include <ArduinoJson.h>

//connection parameters
#define HTTP_REST_PORT 80
#define WIFI_RETRY_DELAY 500
#define MAX_WIFI_INIT_RETRY 50
#define SUCCESS 0
#define ERROR -1
#define ASK_TEMP 0
#define ASK_HUM 1
#define ASK_TRESHOLD 2

//wireless connection data

const char* wifi_ssid = "FRITZ!Powerline 540E";
const char* wifi_passwd = "13179208264364704063";

ESP8266WebServer http_rest_server(HTTP_REST_PORT);

//struttura di dati che rappresenta il termostato
struct Termostato {
    int id;
    char* type;
    float temperature;
    float humidity;
    float treshold;
} termo_data;

//Variabili
uint8_t treshold_array_CRC[8];
uint8_t rtu_buf[255];
uint8_t rtu_len;
uint8_t temperature_array[] = {0x02, 0x03, 0x00, 0x00, 0x00, 0x01, 0x84, 0x39};
uint8_t humidity_array[] = {0x02, 0x03, 0x00, 0x01, 0x00, 0x01, 0xD5, 0xF9};
uint8_t treshold_array_WR[] = {0x02, 0x06, 0x00, 0x02, 0x00, 0x00};
uint8_t treshold_array[] = {0x02, 0x03, 0x00, 0x02, 0x00, 0x01, 0x25, 0xF9};

//Calcolo CRC16 per Modbus
unsigned int ModRTU_CRC(uint8_t *buf, int len)
{
    unsigned int crc = 0xFFFF;
    int pos;
    int i;

    for(pos = 0; pos < len; pos++)
    {
        crc^=(unsigned int)buf[pos];

        for(i =8; i !=0; i--)
        {
           if((crc & 0x0001)!=0)
           {
               crc>>=1;
               crc^=0xA001;  //0xA001
           }
           else
               crc>>=1;
        }
    }
    return crc;
}


// Dati per l'inizializzazione, prima di leggere il termostato via MODBUS.
// Temperatura, umidità e soglia sono da considerarsi moltiplicati per 10
// così da poter essere interi
void init_termo_data()
{
    termo_data.id = 1;
    termo_data.type = "Termostato";
    termo_data.temperature = 0;
    termo_data.humidity = 0;
    termo_data.treshold = 0;
}

void ask_termo_data(int requestType)
{
  //Request
     
      Serial.flush();
      digitalWrite(D0,HIGH);
      delay(1);
      if(requestType == ASK_TEMP)
        Serial.write(temperature_array,8);
      if(requestType == ASK_HUM)
        Serial.write(humidity_array,8);
      if(requestType == ASK_TRESHOLD)
        Serial.write(treshold_array,8);
      Serial.flush();
      digitalWrite(D0,LOW);
}

void read_termo_data(int requestType)
{
 //Response
      int i = 6;
      while (Serial.available() > 0)  
        {
          rtu_buf[i] = Serial.read();
          i ++;
          yield();
          if(requestType == ASK_TEMP)
          {
            float temp = (rtu_buf[10] | (rtu_buf[9]<<8));
            termo_data.temperature = temp;
          }
            
          if(requestType == ASK_HUM)
          {
             float hum = (rtu_buf[10] | (rtu_buf[9]<<8));
             termo_data.humidity = hum; 
          }

          if(requestType == ASK_TRESHOLD)
          {
             float SP = (rtu_buf[10] | (rtu_buf[9]<<8));
             if(SP == 32768)  
              SP = 0;
             termo_data.treshold = SP; 
          }
        }
}

int update_termo_treshold(float new_treshold)
{
  uint8_t new_tresholdBH = (uint8_t)new_treshold>>8;
  uint8_t new_tresholdBL = (uint8_t)new_treshold & 0xFF; 
  treshold_array_WR[4] = new_tresholdBH;
  treshold_array_WR[5] = new_tresholdBL;

  unsigned int CRC = ModRTU_CRC(treshold_array_WR, 6);
  uint8_t CRCH = CRC>>8;
  uint8_t CRCL = CRC & 0xFF;

  for(int a=0; a<9; a++)
    treshold_array_CRC[a] = treshold_array_WR[a];
  treshold_array_CRC[6] = CRCL;
  treshold_array_CRC[7] = CRCH;
  
  Serial.flush();
  digitalWrite(D0,HIGH);
  delay(1);
  Serial.write(treshold_array_CRC,8);
  Serial.flush();
  digitalWrite(D0,LOW);
  
  return SUCCESS;
}

int init_wifi() 
{
    int retries = 0;

    //Serial.print("Connecting to WiFi AP..........");

    WiFi.mode(WIFI_STA);
    WiFi.begin(wifi_ssid, wifi_passwd);
    // check the status of WiFi connection to be WL_CONNECTED
    while ((WiFi.status() != WL_CONNECTED) && (retries < MAX_WIFI_INIT_RETRY)) {
        retries++;
        delay(WIFI_RETRY_DELAY);
        //Serial.print(".");
    }
    //Serial.println("");
    return WiFi.status();
}

// Quando entro qui, è stato chiesto l'intero stato del termostato
void get_termo_data() {
    StaticJsonDocument<200> jsonDoc;
    JsonObject jsonObj = jsonDoc.to<JsonObject>();
    char JSONmessageBuffer[200];

    ask_termo_data(ASK_TEMP);
    delay(20);
    read_termo_data(ASK_TEMP);
    ask_termo_data(ASK_HUM);
    delay(20);
    read_termo_data(ASK_HUM);
    ask_termo_data(ASK_TRESHOLD);
    delay(20);
    read_termo_data(ASK_TRESHOLD);
    
     //leggo i dati dal termo_data e li riporto nel jsonObj dove aver aggiornato il modbus
    jsonObj["id"] = termo_data.id;
    jsonObj["type"] = termo_data.type;    ;
    jsonObj["temperature"] = termo_data.temperature;
    jsonObj["humidity"] = termo_data.humidity;
    jsonObj["treshold"] = termo_data.treshold;

    //converto il json e invio la risposta
    serializeJson(jsonDoc, JSONmessageBuffer, sizeof(JSONmessageBuffer));
    http_rest_server.send(200, "application/json", JSONmessageBuffer);

    //aggiorno TUTTO il termo_data coi dati letti da MODBUS
//    if(update_termo_data() == SUCCESS)
//    {
      //leggo i dati dal termo_data e li riporto nel jsonObj
      jsonObj["id"] = termo_data.id;
      jsonObj["type"] = termo_data.type;
      jsonObj["temperature"] = termo_data.temperature;
      jsonObj["humidity"] = termo_data.humidity;
      jsonObj["treshold"] = termo_data.treshold;
  
      //converto il json e invio la risposta
      serializeJson(jsonDoc, JSONmessageBuffer, sizeof(JSONmessageBuffer));
      http_rest_server.send(200, "application/json", JSONmessageBuffer);
//    }
//    else 
//    {
//      http_rest_server.send(500);
//    }
}

//Quando entro qui, è stato chiesto di aggiornare la soglia con un nuovo valore
int json_to_termo_data(StaticJsonDocument<500> doc) 
{
  //recupero il valore dal server
    int treshold = doc["treshold"];

    //qui va aggiornata la soglia sul termostato via MODBUS,
    //e deve tornare 0
    if( update_termo_treshold(treshold) == SUCCESS)
    {
      //aggiorno la struttura
      termo_data.treshold = treshold;
      return SUCCESS;
    }
    // se arrivo qui è andato male
    return ERROR;
}

void post_put_termo_data() {
    StaticJsonDocument<500> jsonBody;
    String post_body = http_rest_server.arg("plain");
    //Serial.println(post_body);

    //carica jsonBody con il dato deserializzato letto dal server args
    deserializeJson(jsonBody, post_body);

    //Serial.print("HTTP Method: ");
    //Serial.println(http_rest_server.method());
    
    if (http_rest_server.method() == HTTP_POST) {
          http_rest_server.send(405); //metodo non disponibile, non ci serve il POST
    }
    else if (http_rest_server.method() == HTTP_PUT) {
        if (jsonBody["id"] == termo_data.id) 
        {
          //la richiesta ha l'ID corretto
            if(json_to_termo_data(jsonBody) == SUCCESS)
            {
              //tutto bene, HTTP 200
              http_rest_server.sendHeader("Location", "/");
              http_rest_server.send(200);
            }
            else
            {
              //errore, ritorniamo HTTP 500 (Internal Server Error)
              http_rest_server.send(500);
            }
        }
        else
          http_rest_server.send(404); //risorsa non trovata
    }
}

void config_rest_server_routing() {
    http_rest_server.on("/", HTTP_GET, get_termo_data);
    http_rest_server.on("/", HTTP_POST, post_put_termo_data);
    http_rest_server.on("/", HTTP_PUT, post_put_termo_data);
}

void setup(void) {
    Serial.begin(19200);
    pinMode(D0,OUTPUT); //re rs485
    init_termo_data();

    if (init_wifi() == WL_CONNECTED) {
        //Serial.print("Connetted to ");
        //Serial.print(wifi_ssid);
        //Serial.print("--- IP: ");
        //Serial.println(WiFi.localIP());
    }
    else {
        //Serial.print("Error connecting to: ");
        //Serial.println(wifi_ssid);
    }

    config_rest_server_routing();

    http_rest_server.begin();

}

void loop(void) {
    http_rest_server.handleClient();
}
