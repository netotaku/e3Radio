
var track = function(){}
track.prototype.html = function(data){
  return _.template($('#track-detail').html(), data);
}
track.prototype.draw = function(data){
  $('#content').empty().html(this.html(data[0]));
  channel.publish({
    event: 'after-draw'
  });
  this.unbind();
}
track.prototype.bind = function(){
  channel.subscribe(this, {
    event: 'init-track',
    cb: function(data){
      this.draw(data);
    }
  });
}
track.prototype.unbind = function(){
  channel.unsubscribe(this);
}