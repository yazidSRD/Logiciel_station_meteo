const mqtt = require('mqtt');
const mysql = require('mysql2');

const mqttHost = '';
const mqttPort = 0;
const mqttTopic = '';
const mqttClientId = '';
const mqttUsername = '';
const mqttPassword = '';

const mqttOptions = {
    host: mqttHost,
    port: mqttPort,
    clientId: mqttClientId,
    username: mqttUsername,
    password: mqttPassword
};

//const mysqlHost = '192.168.1.223';
//const mysqlUser = 'Application';
//const mysqlPassword = 'stationmeteo';
//const mysqlDatabase = 'station_meteo';

const mysqlHost = 'localhost';
const mysqlUser = 'root';
const mysqlPassword = '';
const mysqlDatabase = 'station_meteo';

const mysqlOptions = {
    host: mysqlHost,
    user: mysqlUser,
    password: mysqlPassword,
    database: mysqlDatabase
};

const mqttClient = mqtt.connect(mqttOptions);

mqttClient.on('connect', () => {
    console.log(`Connected to MQTT broker at ${mqttHost}:${mqttPort}`);
    mqttClient.subscribe(mqttTopic);
});

mqttClient.on('message', (topic, message) => {
    console.log(`----------------------------------------------------------------------`);
    console.log(`Received message on topic ${topic}`);

    var data = JSON.parse(message);
    console.log(`-------`);
    console.log(data);

    const date = new Date(data.received_at);
    const formattedDate = `${date.getUTCFullYear()}-${(date.getUTCMonth() + 1).toString().padStart(2, '0')}-${date.getUTCDate().toString().padStart(2, '0')} ${date.getUTCHours().toString().padStart(2, '0')}:${date.getUTCMinutes().toString().padStart(2, '0')}:${date.getUTCSeconds().toString().padStart(2, '0')}.${date.getUTCMilliseconds().toString().padStart(6, '0')}`;

    data = data.uplink_message.decoded_payload
    
    console.log(`-------`);
    console.log(`DateHeureReleve = auto`);
    //console.log(`DateHeureReleve = ${formattedDate}`);
    console.log(`Temperature = ${data.outsideTemperature}`);
    console.log(`Hygrometrie = ${data.outsideHumidity}`);
    console.log(`VitesseVent = ${data.windSpeed}`);
    console.log(`DirectionVent = ${data.windDirection}`);
    console.log(`PressionAtmospherique = ${data.pressure}`);
    console.log(`Pluviometre = ${data.rainRate}`);
    console.log(`RayonnementSolaire = ${data.solarRadiation}`);
    console.log(`-------`);

    const connection = mysql.createConnection(mysqlOptions);
    connection.connect((err) => {
        if (err) {
            console.error('Erreur de connexion à la base de données: ' + err.stack);
			console.log(`----------------------------------------------------------------------`);
            return;
        }
        console.log('Connecté à la base de données MySQL');
		console.log(`-------`);

        connection.query(
            'INSERT INTO relevemeteo (Temperature, Hygrometrie, VitesseVent, DirectionVent, PressionAtmospherique, Pluviometre, RayonnementSolaire) VALUES ( ?, ?, ?, ?, ?, ?, ?)',
            [data.outsideTemperature, data.outsideHumidity, data.windSpeed, data.windDirection, data.pressure, data.rainRate, data.solarRadiation],
            (error, results, fields) => {
                if (error) {
                    console.error(`Error inserting data into MySQL: ${error.message}`);
                } else {
                    console.log(`Inserted data into MySQL with ID ${results.insertId}`);
                }
                connection.end();
				console.log(`----------------------------------------------------------------------`);
            }
        );
    });
});