<?php
    if (!array_key_exists("login",getallheaders())) die();
    if (!array_key_exists("password",getallheaders())) die();

    $login = getallheaders()["login"];
    $password = getallheaders()["password"];

    $password = hash("sha256", $password);

    include "../php/bddConnexion.php";
    
    $sql = "SELECT * FROM compte WHERE Identifiant ='".$login."' AND Mdp='".$password."'";

    $data = array();
    foreach($db->query($sql) as $row) {
        echo json_encode($row);
        $db = null;
        die();
    }
    $db = null;
?>