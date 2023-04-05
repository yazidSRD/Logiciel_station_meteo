<?php
    if (!array_key_exists("login",getallheaders())) die();
    if (!array_key_exists("password",getallheaders())) die();
    if (!array_key_exists("niv",getallheaders())) die();
    if (!array_key_exists("target",getallheaders())) die();
    if (!array_key_exists("var",getallheaders())) die();

    $login = getallheaders()["login"];
    $password = getallheaders()["password"];
    $password = hash("sha256", $password);
    $niv = getallheaders()["niv"];
    $target = getallheaders()["target"];
    $var = getallheaders()["var"];

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

    $sql = "UPDATE seuil SET ". $target."=".$var." WHERE NivID=".$niv;

    $stmt = $db->prepare($sql);
    $result = $stmt->execute();

    if ($result !== false) {
        echo "true";
    } else {
        echo "false";
    }
    $db = null;
?>