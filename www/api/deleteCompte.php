<?php
    if (!array_key_exists("login",getallheaders())) die();
    if (!array_key_exists("password",getallheaders())) die();
    if (!array_key_exists("ID",getallheaders())) die();

    $login = getallheaders()["login"];
    $password = getallheaders()["password"];
    $id = getallheaders()["ID"];

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

    $sql = "DELETE FROM compte WHERE ID='".$id."'";
    
    $stmt = $db->prepare($sql);
    $result = $stmt->execute();

    if ($result !== false) {
        echo "true";
    } else {
        echo "false";
    }
    $db = null;
?>