<?php
    include "config.php";

    $db = new PDO('mysql:host='.$serverIp.';dbname='.$dbName, $user["id"], $user["passworld"]);
?>