
var search = function(){

	var spotifyURI;

	$('#search form').live('submit', function(e){
		e.preventDefault();
		var $form = $('#search form');
		var $input = $form.find('input[type=text]'); 
		var query = $input.val();

		if(query){
			Backbone.history.navigate('search/' + query, true); 
			page.chrome();			
		} else {
			$form.css({
				'border-color': 'magenta'
			});
			$input.on('focus', function(){
				$(this).unbind();
				$form.css({
					'border-color': '#ccc'
				});
			});
		}

	});
	
}
search.prototype.bind = function(){

}
search.prototype.unbind = function(){
	
}