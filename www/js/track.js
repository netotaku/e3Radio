
var track = function(){}
track.prototype.html = function(data){
  return _.template($('#track-detail').html(), data);
}
track.prototype.render = function(data){
  page.chrome();
  $('#content').empty().html(this.html(data[0]));
  modal.hide();
  this.unbind();
}
track.prototype.bind = function(id){
  channel.subscribe(this, {
    event: 'RX-track',
    cb: function(data){
      this.render(data);
    }
  });
  socket.connect(function(){
    socket.send('TX-track', { TrackID: id });  
  });
}
track.prototype.unbind = function(){
  channel.unsubscribe(this);
}