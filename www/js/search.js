var search = function(){

	this.query;

	var spotifyURI;

	$('#search form').live('submit', function(e){
		e.preventDefault();
		var $form = $('#search form');
		var $input = $form.find('input[type=text]'); 
		var query = $input.val();

		if(query){
			Backbone.history.navigate('search/' + query, true); 			
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
search.prototype.bind = function(query){
	this.query = query;
	channel.subscribe(this, {
		event: 'ready',
		cb: function(){
			this.render();
		}
	});
}
search.prototype.unbind = function(){
	channel.unsubscribe(this);
}
search.prototype.render = function(){
	page.chrome();
	// console.log($('#content'));
	//console.log($('#result').html());
	$('#content').empty().html($('#result').html());
	l('rendering search results');
}