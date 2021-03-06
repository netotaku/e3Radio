
var track = function(){
  // this.id;
}
track.prototype.html = function(data){
  return _.template($('#track-detail').html(), data);
}
track.prototype.render = function(data){
  l('rendering track');
  page.chrome();
  $('#content').empty().html(this.html(data[0]));
  modal.hide();
  // this.unbind();
}
track.prototype.bind = function(id){
  // this.id = id;
  channel.subscribe(this, {
    event: 'RX-track',
    cb: function(data){
      l('track recieved');
      this.render(data);
    }
  });
  channel.subscribe(this, {
    event: 'ready',
    cb: function(){
      l('requesting track');
      socket.send('TX-track');  
    }
  });
}
track.prototype.unbind = function(){
  channel.unsubscribe(this);
}