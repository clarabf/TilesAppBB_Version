<?php
include 'kint.php';

$url = "https://sacodoo-13-0-parametric-626490.dev.odoo.com";
$db = "sacodoo-13-0-parametric-626490";
$username = "miguelfontgivell@saco.com";
$password = "Saco-2019";

require_once('ripcord.php');
$common = ripcord::client("$url/xmlrpc/2/common");

// Login and show the uid from the user authenticated
$uid = $common->authenticate($db, $username, $password, array());

// CHeck if user has access rights
$models = ripcord::client("$url/xmlrpc/2/object");

echo '/////////////////// PRUEBA CON TILES ///////////////////';

//Get the Tiles
$tile100= $models->execute_kw($db, $uid, $password, 'product.template', 'search_read', [[['name', 'ilike', 'SPA_Tile100']]], array('fields'=>array('name','product_variant_count','type','version','categ_id','route_ids','responsible_id','attribute_line_ids')));
d($tile100);
$tile_id = $tile100[0]['id'];
echo 'Tile id: ', $tile_id;

//Consult BOM associated with the id
$bom = $models->execute_kw($db, $uid, $password, 'mrp.bom', 'search_read', [[['product_tmpl_id', '=', $tile_id]]], array('fields'=>array('type','product_qty','ready_to_produce','consumption','bom_line_ids')));
d($bom);

$bom_ids = array();
foreach ($bom as $b) {
    array_push($bom_ids, $b['id']);
}
d($bom_ids);

//Extract products associated with the BOMs
$bom_line = $models->execute_kw($db, $uid, $password, 'mrp.bom.line', 'search_read', [[['id', 'in', $bom_ids]]], array('fields'=>array('product_id','product_qty','sequence')));
+d($bom_line);

$products_ids = array();
foreach ($bom_line as $bl) {
    array_push($products_ids, $bl['product_id'][0]);
}

//Info of variants
$variants = $models->execute_kw($db, $uid, $password, 'product.product', 'search_read', [[['id', 'in', $products_ids]]], array('fields'=>array('name','sale_ok','purchase_ok','can_be_expensed','type')));
+d($variants);

echo '/////////////////// PRUEBA CON EMPLOYEEES ///////////////////';
$employees= $models->execute_kw($db, $uid, $password, 'hr.employee', 'search_read', [[['id', '>', '0']]], array('fields'=>array('name','department_id','job_id','address_id','category_ids','gender','barcode','pin')));
+d($employees);

$category_ids = array();
foreach ($employees as $e) {
    if ($e['category_ids']!=null) {
		foreach ($e['category_ids'] as $cid ) 
			if (!in_array($cid, $category_ids)) array_push($category_ids, $cid);
	}
}
+d($category_ids);

$category_names= $models->execute_kw($db, $uid, $password, 'hr.employee.category', 'search_read', [[['id', 'in', $category_ids]]], array('fields'=>array('name')));
+d($category_names);

exit;
?>