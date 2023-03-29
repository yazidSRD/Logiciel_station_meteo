<?php
    if (!array_key_exists("minOrMax",getallheaders())) die();

    $minOrMax = getallheaders()["minOrMax"];

    include "../php/bddConnexion.php";
    
    $sql = "SELECT ".$minOrMax."(DateHeureReleve) FROM relevemeteo";
    
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