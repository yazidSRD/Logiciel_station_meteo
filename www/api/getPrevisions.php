<?php
include "../php/config.php";

//Connection a la bdd
$db = new mysqli($serverIp, $admin["id"], $admin["passworld"], $dbName);

//Créer qui récupère la date la plus ancienne de api_prevision
$lastdate = $db->prepare("SELECT date FROM api_prevision ORDER BY date ASC LIMIT 1"); //Permet de récupérer la dernière date

//Exécuter la reqûete
$lastdate->execute(); 

//Permet de transformer ces valeurs en variable
$lastdate->bind_result($DateComp);

$products1 = array();   //Affiche les date si l'api a déja été chargé aujourd'hui
$products2 = array();   //Affiche les date si l'api n'a pas déjà été chargé aujourd'hui
$etat = 0;              //si 1 alors pareil sinon si 2 différent

//on stock la date dans une variable
while ($lastdate->fetch()) {
    $prev = array();
    $prev['date'] = $DateComp;
}

// Format de l'heure : 2023-03-10
$heure = date("Y-m-d");

if ($heure == $DateComp){

    $AncienVal = $db->prepare("SELECT * FROM api_prevision ORDER BY date ASC"); //Permet de récupérer la dernière date

    //Exécuter la reqûete
    $AncienVal->execute(); 

    $AncienVal->bind_result($date, $tempmax, $tempmin, $icon);  //Permet de transformer ces valeurs en variable

    //Permet de traverser tous les résultats
    while ($AncienVal->fetch()) {
        $same = array();
        $same['date'] = $date;
        $same['tempmax'] = $tempmax; 
        $same['tempmin'] = $tempmin; 
        $same['icon'] = $icon; 
        array_push($products1, $same);
    }

    $etat = 1;

}else{

    $drop = "TRUNCATE TABLE `api_prevision`";
    $db->query($drop);

    //Récuperation des données de l'API
    
    $dataAPI = file_get_contents('https://api.weatherbit.io/v2.0/forecast/daily?lat=48.70&lon=6.20&days=7&lang=fr&key=9f72f73e1c0c4723aac94014a1c09225');
    $dataAPI_array = json_decode($dataAPI, true);

    $liste_data = $dataAPI_array['data'];

    $listeDate = array();
    $listeTempMax = array();
    $listeTempMin = array();
    $listeIcon = array();
    $index = 0;

    foreach($liste_data as $element){
        //récup date
        $donneesDate = $element['datetime'];
        $listeDate[$index] = $donneesDate;

        //récup tempmax
        $donneesTempMax = $element['max_temp'];
        $listeTempMax[$index] = $donneesTempMax;

        //recup tempmin
        $donneesTempMin = $element['min_temp'];
        $listeTempMin[$index] = $donneesTempMin;

        //recup icon
        $donneesIcon = $element['weather']['icon'];
        $listeIcon[$index] = $donneesIcon;

        $index++;
    }

   // Parcours des données et insertion dans la base de données
    for($i = 0; $i < 7; $i++){
        $dateIns = $listeDate[$i];
        $tempmaxIns = $listeTempMax[$i];
        $tempminIns = $listeTempMin[$i];
        $iconIns = $listeIcon[$i];

        // Préparation de la requête SQL
        $insert = "INSERT INTO api_prevision (date, tempmax, tempmin, icon) 
			       VALUES ('$dateIns', '$tempmaxIns', '$tempminIns', '$iconIns')";


        $db->query($insert);

    }

    //Permet de récupérer les prevision
    $NewVal = $db->prepare("SELECT * FROM api_prevision ORDER BY date ASC"); 

    //Exécuter la reqûete
    $NewVal->execute();
  
    //Permet de transformer ces valeurs en variable
    $NewVal->bind_result($date2, $tempmax2, $tempmin2, $icon2);

    //Permet de traverser tous les résultats
    while ($NewVal->fetch()) {
        $dif = array();
        $dif['date'] = $date2;
        $dif['tempmax'] = $tempmax2; 
        $dif['tempmin'] = $tempmin2; 
        $dif['icon'] = $icon2; 
        array_push($products2, $dif);
    }

    $etat = 2;
}

if ($etat == 1){
    echo json_encode($products1); //Permet d'afficher les données en format json
}else if ($etat == 2){
    echo json_encode($products2); //Permet d'afficher les données en format json
}

?>