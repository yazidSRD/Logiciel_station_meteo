<?php
    include "php/config.php";

    $db = new PDO('mysql:host='.$serverIp.';dbname='.$dbName, $user["id"], $user["passworld"]);

    switch($_SERVER['REQUEST_METHOD']){
        case 'GET':
            if (array_key_exists("sql",getallheaders())) get(getallheaders()["sql"]);
            else if (array_key_exists("getSeuils",getallheaders())) getSeuils();
            else if (array_key_exists("login",getallheaders()) && array_key_exists("password",getallheaders()) && array_key_exists("editSeuil",getallheaders())) editSeuil(getallheaders()["login"], getallheaders()["password"], getallheaders()["niv"], getallheaders()["target"], getallheaders()["var"]);
            else if (array_key_exists("login",getallheaders()) && array_key_exists("password",getallheaders()) && array_key_exists("deleteCompte",getallheaders())) deleteCompte(getallheaders()["login"], getallheaders()["password"], getallheaders()["ID"]);
            else if (array_key_exists("login",getallheaders()) && array_key_exists("password",getallheaders()) && array_key_exists("newCompte",getallheaders())) newCompte(getallheaders()["login"], getallheaders()["password"]);
            else if (array_key_exists("login",getallheaders()) && array_key_exists("password",getallheaders()) && array_key_exists("editCompte",getallheaders())) EditCompte(getallheaders()["login"], getallheaders()["password"]);
            else if (array_key_exists("login",getallheaders()) && array_key_exists("password",getallheaders()) && array_key_exists("allComptes",getallheaders())) getAllCompte(getallheaders()["login"], getallheaders()["password"]);
            else if (array_key_exists("login",getallheaders()) && array_key_exists("password",getallheaders())) getCompte(getallheaders()["login"], getallheaders()["password"]);
            break;
        //case 'POST':
        //    post();
    }

    function get($sql) {
        global $db;
        $sql = "SELECT ".$sql;

        $data = array();

        $result = $db->query($sql);
        
        if ($result === false) {
            echo '[]';
            $db = null;
            return;
        }

        foreach($result as $row) {
            $data[] = $row;
        }

        echo json_encode($data);
        $db = null;
    }

    function getCompte($login, $password) {
        global $db;
        $sql = "SELECT ID,Nom,Prenom,Identifiant,Tel,Fonction,Droit FROM compte WHERE Identifiant ='".$login."' AND Mdp='".$password."'";

        $data = array();
        foreach($db->query($sql) as $row) {
            $data[] = $row;
        }
        echo json_encode($data);
        $db = null;
    }
	
	function getAllCompte($login, $password) {
        global $db;
        $sql = "SELECT Droit FROM compte WHERE Identifiant ='".$login."' AND Mdp='".$password."'";

        $data = array();
		
		$flag = true;
        foreach($db->query($sql) as $row) {
            $flag = false;
			break;
        }
		if ($flag) return;
		
		$sql = "SELECT ID,Nom,Prenom,Identifiant,Tel,Fonction,Droit FROM compte WHERE Identifiant!='".$login."'";

        $data = array();
        foreach($db->query($sql) as $row) {
            $data[] = $row;
        }
        echo json_encode($data);
        $db = null;
    }

    function EditCompte($login, $password) {
        global $db;
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

        if (array_key_exists("Nom",getallheaders()) && $perm >= 1) $sql .= " Nom = '".getallheaders()["Nom"]."',";
        if (array_key_exists("Prenom",getallheaders()) && $perm >= 1) $sql .= " Prenom = '".getallheaders()["Prenom"]."',";
        if (array_key_exists("Identifiant",getallheaders()) && $perm >= 2) $sql .= " Identifiant = '".getallheaders()["Identifiant"]."',";
        if (array_key_exists("Mdp",getallheaders()) && $perm >= 1) $sql .= " Mdp = '".getallheaders()["Mdp"]."',";
        if (array_key_exists("Tel",getallheaders()) && $perm >= 1) $sql .= " Tel = ".getallheaders()["Tel"].",";
        if (array_key_exists("Fonction",getallheaders()) && $perm >= 2) $sql .= " Fonction = '".getallheaders()["Fonction"]."',";
        if (array_key_exists("Droit",getallheaders()) && $perm >= 2) $sql .= " Droit = '".getallheaders()["Droit"]."',";
        
        $sql = substr($sql, 0, strlen($sql)-1);

        $sql .= " WHERE id = ".getallheaders()["ID"];
        
        $stmt = $db->prepare($sql);
        $result = $stmt->execute();

       if ($result !== false) {
            echo "true";
        } else {
            echo "false";
        }
        $db = null;
    }

    function newCompte($login, $password) {
        global $db;
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


        $sql = "SELECT ID FROM compte WHERE Identifiant ='".getallheaders()["Identifiant"]."'";
        foreach($db->query($sql) as $row) {
            echo "false";
            return;
			break;
        }


        $sql = "INSERT INTO compte (Nom, Prenom, Identifiant, Mdp, Tel, Fonction, Droit) VALUES";
        $sql .= " ('".getallheaders()["Nom"]."','".getallheaders()["Prenom"]."','".getallheaders()["Identifiant"]."','".getallheaders()["Mdp"]."','".getallheaders()["Tel"]."','".getallheaders()["Fonction"]."','".getallheaders()["Droit"]."');";
        
        $stmt = $db->prepare($sql);
        $result = $stmt->execute();

       if ($result !== false) {
            echo "true";
        } else {
            echo "false";
        }
        $db = null;
    }

    function deleteCompte($login, $password, $id) {
        global $db;
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
    }

    function editSeuil($login, $password, $niv, $target, $var) {
        global $db;
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
    }

    function getSeuils() {
        global $db;
        $sql = "SELECT * FROM seuil";
        
        $data = array();
        foreach($db->query($sql) as $row) {
            $data[] =  $row["SeuilVitVent"].";".$row["SeuilTemp"].";".$row["SeuilPluvio"].";".$row["SeuilRadSol"];
        }
        echo json_encode($data);
        $db = null;
    }
?>