#include <ESP8266WebServer.h>
#include <ArduinoJson.h>

//connection parameters
#define HTTP_REST_PORT 80
#define WIFI_RETRY_DELAY 500
#define MAX_WIFI_INIT_RETRY 50
#define SUCCESS 0
#define ERROR -1

//wireless connection data
const char* wifi_ssid = "TP-LINK_3225";
const char* wifi_passwd = "44794378";

ESP8266WebServer http_rest_server(HTTP_REST_PORT);

//struttura di dati che rappresenta il termostato
struct Termostato {
    int id;
    char* type;
    int temperature;
    int humidity;
    int treshold;
} termo_data;


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

int update_termo_data()
{
  //------------------------------------------------------------
  // Qui va aggiunto il codice che legge il termostato da MODBUS
  // e aggiorna TUTTA la struttura termo_data (temperatura, 
  // umidità e soglia)
  //------------------------------------------------------------
  return SUCCESS;
}

int update_termo_treshold(int new_treshold)
{
  //------------------------------------------------------------------
  // Qui va aggiunto il codice che modifica la soglia sul termostato.
  // Se va tutto bene (aggiorna correttamente tramite MODBUS) ritorna
  // SUCCESS, se qualcosa va male ritorna ERROR
  //------------------------------------------------------------------
  return SUCCESS;
}

int init_wifi() 
{
    int retries = 0;

    Serial.print("Connecting to WiFi AP..........");

    WiFi.mode(WIFI_STA);
    WiFi.begin(wifi_ssid, wifi_passwd);
    // check the status of WiFi connection to be WL_CONNECTED
    while ((WiFi.status() != WL_CONNECTED) && (retries < MAX_WIFI_INIT_RETRY)) {
        retries++;
        delay(WIFI_RETRY_DELAY);
        Serial.print(".");
    }
    Serial.println("");
    return WiFi.status();
}

// Quando entro qui, è stato chiesto l'intero stato del termostato
void get_termo_data() {
    StaticJsonDocument<200> jsonDoc;
    JsonObject jsonObj = jsonDoc.to<JsonObject>();
    char JSONmessageBuffer[200];

    //aggiorno TUTTO il termo_data coi dati letti da MODBUS
    if(update_termo_data() == SUCCESS)
    {
      //leggo i dati dal termo_data e li riporto nel jsonObj
      jsonObj["id"] = termo_data.id;
      jsonObj["type"] = termo_data.type;
      jsonObj["temperature"] = termo_data.temperature;
      jsonObj["humidity"] = termo_data.humidity;
      jsonObj["treshold"] = termo_data.treshold;
  
      //converto il json e invio la risposta
      serializeJson(jsonDoc, JSONmessageBuffer, sizeof(JSONmessageBuffer));
      http_rest_server.send(200, "application/json", JSONmessageBuffer);
    }
    else 
    {
      http_rest_server.send(500);
    }
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
    Serial.println(post_body);

    //carica jsonBody con il dato deserializzato letto dal server args
    deserializeJson(jsonBody, post_body);

    Serial.print("HTTP Method: ");
    Serial.println(http_rest_server.method());
    
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
    Serial.begin(115200);

    init_termo_data();

    if (init_wifi() == WL_CONNECTED) {
        Serial.print("Connetted to ");
        Serial.print(wifi_ssid);
        Serial.print("--- IP: ");
        Serial.println(WiFi.localIP());
    }
    else {
        Serial.print("Error connecting to: ");
        Serial.println(wifi_ssid);
    }

    config_rest_server_routing();

    http_rest_server.begin();
    Serial.println("HTTP REST Server Started");
}

void loop(void) {
    http_rest_server.handleClient();
}
