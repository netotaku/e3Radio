var search = function(){

	this.query;

	var spotifyURI;

	$('#search form').live('submit', function(e){
		e.preventDefault();
		var $form = $('#search form');
		var $input = $form.find('input[type=text]'); 
		var query = $input.val();

		if(query){
			Backbone.history.navigate('search/' + encodeURI(query), true); 			
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
	$('#content').empty().html($('#result').html());
	$list = $('#list tbody');

	var url = "http://ws.spotify.com/search/1/track.json?q=" + this.query;

	$.ajax({
	  url: url,
	  context: document.body,
	  crossDomain: true
	}).done(function(data) {
		modal.hide();
	  	console.log(data);
	  	$.each(data.tracks, function(){
	  		$list.append(_.template($('#search-result-row').html(), this));
	  	});

	});

}