<?php
/**
 * @package Quick Contact
 * @version 0.1
 */
/*
Plugin Name: Quick Contact
Plugin URI: http://techslides.com/
Description: Quick Contact WordPress Plugin to make an Ajax form submission, store it in the database, and show it in a Admin backend page.
Version: 0.1
Author URI: http://techslides.com/
*/

register_activation_hook(__FILE__,'qc_install');
register_deactivation_hook(__FILE__, 'qc_uninstall' );

global $jal_db_version;
$jal_db_version = "1.0";

function qc_install() {
  global $wpdb;
  global $jal_db_version;

  $table_name = $wpdb->prefix . "quickcontact";

  //http://codex.wordpress.org/Creating_Tables_with_Plugins
  $sql = "CREATE TABLE $table_name (
    id mediumint(9) NOT NULL AUTO_INCREMENT,
    time datetime DEFAULT '0000-00-00 00:00:00' NOT NULL,
    name VARCHAR(120) NOT NULL,
    email VARCHAR(120) DEFAULT '' NOT NULL,
    message text NOT NULL,
    UNIQUE KEY id (id)
  );";

  require_once(ABSPATH . 'wp-admin/includes/upgrade.php');
  dbDelta($sql);

  add_option("jal_db_version", $jal_db_version);

}

function qc_uninstall() {
  global $wpdb;
  global $jal_db_version;

  $table_name = $wpdb->prefix . "quickcontact";

  $wpdb->query("DROP TABLE IF EXISTS $table_name");

  require_once(ABSPATH . 'wp-admin/includes/upgrade.php');
  dbDelta($sql);
}

//http://codex.wordpress.org/Shortcode_API
function function_qc_form_ex(){
	
  ?>
    <h2> Registration Form Here</h2>
	<form method="POST" action="" onsubmit="qc_process(this); return false;">
	
	
	
	<div class="form-group">
	<label for="firstname" > First Name</label>
	<input type="text" name="firstname" id="firstname" class="form-control" />
	</div><br/>
	<div class="form-group">
	<label for="lastname" > Last Name</label>
	<input type="text" name="lastname" id="lastname" class="form-control" />
	</div><br/>
	<div class="form-group">
	<label for="email" > Email</label>
	<input type="text" name="email" id="email" class="form-control" />
	</div><br/>
	<div class="form-group">
	<label for="phone" > Phone Number</label>
	<input type="text" name="phone" id="phone" class="form-control" />
	</div>
	<input type="submit" name="submit" value = "Submit" class="button button-primary" />
	
	
	</form>
  <?php
 
  ?>
   <script>
    function qc_process(e){

      var data = {
          action: "my_qc_form",
          name: e["name"].value,
          email:e["email"].value,
          message:e["message"].value
      };

      jQuery.post("'.admin_url("admin-ajax.php").'", data, function(response) {
          jQuery("#qc_form").html(response);
      });

    }
  </script>
  <?php 
}
add_shortcode( 'qc_form', 'function_qc_form_ex' );


//http://codex.wordpress.org/AJAX_in_Plugins
add_action('wp_ajax_nopriv_my_qc_form', 'my_qc_form_callback');
add_action('wp_ajax_my_qc_form','my_qc_form_callback');

function my_qc_form_callback() {
  global $wpdb; // this is how you get access to the database

   $table_name = $wpdb->prefix . "quickcontact";

  $name = $_POST['name'];
  $email = $_POST['email'];
  $message = $_POST['message'];


  $rows_affected = $wpdb->insert( $table_name, array( 
    'id' => null, 
    'time' => current_time('mysql'),
    'name' => $name,
    'email' => $email,
    'message' => $message
  ));


  if($rows_affected==1){
    echo "Your message was sent.";
  } else {
    echo "Error, try again later.";
  }

  die(); // this is required to return a proper result
}




add_action('admin_menu', 'qc_admin_add_page');

function qc_admin_add_page() {
  //http://codex.wordpress.org/Function_Reference/add_menu_page
  add_menu_page( 'Quick Contact Page', 'Quick Contact', 'manage_options', 'form-menu-options-demo' , 'function_qc_form_ex', '', 200);
  
  add_submenu_page(
    'form-menu-options-demo',       // parent slug
    'Settings ',    // page title
    'Settings',             // menu title
    'manage_options',           // capability
    'form-sub-menu-options', // slug
    'scipts_sub_page' // callback
); 

}

function qc_enqueue($hook) {
  //only for our special plugin admin page
  if( 'quickcontact/adminpage.php' != $hook )
    return;
 
  wp_register_style('quickcontact', plugins_url('quickcontact/pluginpage.css'));
  wp_enqueue_style('quickcontact');
 
  wp_enqueue_script('pluginscript', plugins_url('pluginpage.js', __FILE__ ), array('jquery'));
}
 
add_action( 'admin_enqueue_scripts', 'qc_enqueue' );
?>
