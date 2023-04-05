<?php
    if (!array_key_exists("login",getallheaders())) die();
    if (!array_key_exists("password",getallheaders())) die();
    if (!array_key_exists("Nom",getallheaders())) die();
    if (!array_key_exists("Prenom",getallheaders())) die();
    if (!array_key_exists("Identifiant",getallheaders())) die();
    if (!array_key_exists("Mdp",getallheaders())) die();
    if (!array_key_exists("Tel",getallheaders())) die();
    if (!array_key_exists("Fonction",getallheaders())) die();
    if (!array_key_exists("Droit",getallheaders())) die();

    $login = getallheaders()["login"];
    $password = getallheaders()["password"];
    $Nom = getallheaders()["Nom"];
    $Prenom = getallheaders()["Prenom"];
    $Identifiant = getallheaders()["Identifiant"];
    $Mdp = getallheaders()["Mdp"];
    $Mdp = hash("sha256", $Mdp);
    $Tel = getallheaders()["Tel"];
    $Fonction = getallheaders()["Fonction"];
    $Droit = getallheaders()["Droit"];

    include "../php/bddConnexion.php";

    $sql = "SELECT Droit FROM compte WHERE Identifiant ='".$login."' AND Mdp='".$password."'";

    $perm = 0;

    foreach($db->query($sql) as $row) {
        $perm = $row["Droit"];
        break;
    }
    if ($perm == 0) {
        echo "false";
        return;
    }


    $sql = "SELECT ID FROM compte WHERE Identifiant ='".$Identifiant."'";
    foreach($db->query($sql) as $row) {
        echo "false";
        return;
        break;
    }


    $sql = "INSERT INTO compte (Nom, Prenom, Identifiant, Mdp, Tel, Fonction, Droit) VALUES ";
    $sql .= "('".$Nom."','".$Prenom."','".$Identifiant."','".$Mdp."','".$Tel."','".$Fonction."','".$Droit."');";
    
    $stmt = $db->prepare($sql);
    $result = $stmt->execute();

    if ($result !== false) {
        echo "true";
    } else {
        echo "false";
    }
    $db = null;
?>