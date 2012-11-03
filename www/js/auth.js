
var auth = (function(){

	var user;

	var show = function(){
		modal.hide();
		$('#e3r').empty().html($('#login').html());	
	}
	
	var loginSuccess = function(r){
	  if(r.status == 'connected'){
	    FB.api('/me', function(r) {
	      user = r;
	      Backbone.history.start();
	    });  
	  } else {
	    show();  
	  }
	}

	$('.login a').live('click', function(e){
		e.preventDefault();
		FB.login(loginSuccess);	
	});	

	$('#logout').live('click', function(e){
	  e.preventDefault();
	  FB.logout();
	  show();
	  Backbone.history.stop();
	})

	FB.init({
	  appId: env.get('fbid'), // App ID
	  status: true, // check login status
	  cookie: true, // enable cookies to allow the server to access the session
	  oauth: true, // enable OAuth 2.0
	  xfbml: true  // parse XFBML
	});

	FB.getLoginStatus(loginSuccess);

	return{
		user: function(){
			return user;
		}
	}

})();
