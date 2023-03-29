<?php
    if (!array_key_exists("sql",getallheaders())) die();

    $sql = getallheaders()["sql"];

    include "../php/bddConnexion.php";

    $sql = "SELECT * FROM relevemeteo ".$sql;

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
?>