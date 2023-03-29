<?php
    if (!array_key_exists("login",getallheaders())) die();
    if (!array_key_exists("password",getallheaders())) die();

    $login = getallheaders()["login"];
    $password = getallheaders()["password"];

    include "../php/bddConnexion.php";
    
    $sql = "SELECT ID,Nom,Prenom,Identifiant,Tel,Fonction,Droit FROM compte WHERE Identifiant ='".$login."' AND Mdp='".$password."'";

    $data = array();
    foreach($db->query($sql) as $row) {
        $data[] = $row;
    }
    echo json_encode($data);
    $db = null;
?>