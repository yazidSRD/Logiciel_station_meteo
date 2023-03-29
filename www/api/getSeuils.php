<?php

    include "../php/bddConnexion.php";
    
    $sql = "SELECT * FROM seuil";
    
    $data = array();
    foreach($db->query($sql) as $row) {
        $data[] =  $row["SeuilVitVent"].";".$row["SeuilTemp"].";".$row["SeuilPluvio"].";".$row["SeuilRadSol"];
    }
    echo json_encode($data);
    $db = null;
?>