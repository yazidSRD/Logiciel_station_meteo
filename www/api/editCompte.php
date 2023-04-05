<?php
    if (!array_key_exists("login",getallheaders())) die();
    if (!array_key_exists("password",getallheaders())) die();
    if (!array_key_exists("ID",getallheaders())) die();
    if (!array_key_exists("Nom",getallheaders())) die();
    if (!array_key_exists("Prenom",getallheaders())) die();
    if (!array_key_exists("Identifiant",getallheaders())) die();
    if (!array_key_exists("Tel",getallheaders())) die();
    if (!array_key_exists("Fonction",getallheaders())) die();
    if (!array_key_exists("Droit",getallheaders())) die();

    $login = getallheaders()["login"];
    $password = getallheaders()["password"];
    $password = hash("sha256", $password);
    $id = getallheaders()["ID"];
    $Nom = getallheaders()["Nom"];
    $Prenom = getallheaders()["Prenom"];
    $Identifiant = getallheaders()["Identifiant"];
    $Tel = getallheaders()["Tel"];
    $Fonction = getallheaders()["Fonction"];
    $Droit = getallheaders()["Droit"];

    include "../php/bddConnexion.php";

    $sql = "SELECT ID,Droit FROM compte WHERE Identifiant ='".$login."' AND Mdp='".$password."'";

    $perm = 0;

    foreach($db->query($sql) as $row) {
        if ($row["Droit"] == 1) {
            $perm = 2;
        } else {
            if ($row["ID"] == getallheaders()["ID"]) {
                $perm = 1;
            } else $perm = 0;
        }
        break;
    }
    if ($perm == 0) {
        echo "false";
        return;
    }

    $sql = "UPDATE compte SET";

    if ($perm >= 1) $sql .= " Nom = '".$Nom."',";
    if ($perm >= 1) $sql .= " Prenom = '".$Prenom."',";
    if ($perm >= 2) $sql .= " Identifiant = '".$Identifiant."',";
    if (array_key_exists("Mdp",getallheaders()) && $perm >= 1) {
		$Mdp = hash("sha256", getallheaders()["Mdp"]);
		$sql .= " Mdp = '".$Mdp."',";
	}
    if ($perm >= 1) $sql .= " Tel = ".$Tel.",";
    if ($perm >= 2) $sql .= " Fonction = '".$Fonction."',";
    if ($perm >= 2) $sql .= " Droit = '".$Droit."',";
    
    $sql = substr($sql, 0, strlen($sql)-1);

    $sql .= " WHERE id = ".$id;
    
    $stmt = $db->prepare($sql);
    $result = $stmt->execute();

    if ($result !== false) {
        echo "true";
    } else {
        echo "false";
    }
    $db = null;
?>