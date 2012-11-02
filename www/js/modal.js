var modal = function(){

	this.$inst = $('#modal');

	channel.subscribe(this, {
	  event: 'after-draw',
	  cb: function(){
	    this.$inst.hide();
	  }
	});

};