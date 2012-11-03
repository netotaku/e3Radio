
var login = (function(){	
	return {
		show: function(cb){
			modal.hide();
			$('#e3r').empty().html($('#login').html());
			$('.login a').live('click', function(e){
				e.preventDefault();
				cb();	
			})
		}
	}
})();