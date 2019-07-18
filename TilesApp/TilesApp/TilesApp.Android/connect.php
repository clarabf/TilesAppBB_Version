<?php

$host_db = "db4free.net";
$user_db = "usertiles";
$pass_db = "11111111";
$db_name = "tilesdb";

$conexion = new mysqli($host_db, $user_db, $pass_db, $db_name);

if ($conexion->connect_error) {
 die("La conexion falló: " . $conexion->connect_error);
}

$type = $_POST['type'];
$id = $_POST['id'];

switch ($type) {
	case "tile-info":
		$sql = "SELECT * FROM tiles WHERE id = '$id'";
		break;
	case "update-user":
		$user = $_POST['user'];
		$sql = "UPDATE tiles SET lastuser = '$user' WHERE id = '$id'";
		break;
	case "pdf-info":
	    $category = (int)$id;
		$sql = "SELECT url FROM pdfs WHERE tile = $category";
		break;
	case "update-url":
		$pdf = $_POST['pdf'];
		$sql = "UPDATE tiles SET pdf = '$pdf' WHERE id = '$id'";
		break;
}

$result = $conexion->query($sql);

if ($type == "tile-info") {
	if ($result->num_rows > 0) {
		$row = $result->fetch_array(MYSQLI_ASSOC);
		$myObj = new \stdClass();
		$myObj->category = $row["category"];
		$myObj->laststep = $row["laststep"];
		$myObj->maxsteps = $row["maxsteps"];
		$myObj->pdf = $row["pdf"];
		$myJSON = json_encode($myObj);
		echo $myJSON;
	}
}
else if ($type == "update-user"){
	echo "User updated.";
}
else if ($type == "pdf-info") {
	if ($result->num_rows > 0) {
		$row = $result->fetch_array(MYSQLI_ASSOC);
		echo $row["url"];
	}
}
else {
	echo "Url updated.";
}

mysqli_close($conexion);
 ?>