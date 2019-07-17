<?php

$host_db = "db4free.net";
$user_db = "usertiles";
$pass_db = "11111111";
$db_name = "tilesdb";
$tbl_name = "tiles";

$conexion = new mysqli($host_db, $user_db, $pass_db, $db_name);

if ($conexion->connect_error) {
 die("La conexion falló: " . $conexion->connect_error);
}

$type = $_POST['type'];
$id = $_POST['id'];

if ($type == "tile-info") {
	$sql = "SELECT * FROM $tbl_name WHERE id = '$id'";
}
else {
	$user = $_POST['user'];
	$sql = "UPDATE $tbl_name SET lastuser = '$user' WHERE id = '$id'";
}

$result = $conexion->query($sql);

if ($type == "tile-info") {
	if ($result->num_rows > 0) {
		$row = $result->fetch_array(MYSQLI_ASSOC);
		$myObj = new \stdClass();
		$myObj->laststep = $row["laststep"];
		$myObj->maxsteps = $row["maxsteps"];
		$myObj->category = $row["category"];
		$myJSON = json_encode($myObj);

		echo $myJSON;
		//echo $row["laststep"];
	}
}
else {
	echo "User updated.";
}

mysqli_close($conexion);
 ?>