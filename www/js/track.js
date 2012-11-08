
var track = function(){}
track.prototype.html = function(data){
  return _.template($('#track-detail').html(), data);
}
track.prototype.draw = function(data){
  PGE.chrome();
  $('#content').empty().html(this.html(data[0]));
  modal.hide();
  this.unbind();
}
track.prototype.bind = function(){
  channel.subscribe(this, {
    event: 'RX-track',
    cb: function(data){
      this.draw(data);
    }
  });
}
track.prototype.unbind = function(){
  channel.unsubscribe(this);
}