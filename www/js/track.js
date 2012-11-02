
var track = function(data){
  this.id;
  this.data = data;
}
track.prototype.html = function(view){
  return _.template($('#track-'+view).html(), this.data);
}
track.prototype.draw = function(data, id){
  this.data = this.byID(data, id);
  $('#content').empty().html(this.html('detail'));
  channel.publish({
    event: 'after-draw'
  });
  this.unbind();
}
track.prototype.bind = function(id){
  channel.subscribe(this, {
    event: 'after-playQueueUpdate',
    cb: function(data){
      this.draw(data, id);
    }
  });
}
track.prototype.unbind = function(){
  channel.unsubscribe(this);
}
track.prototype.byID = function(data, id){
  for(var i = 0; i < data.length; i++){
    if(data[i].TrackID == id) return data[i];
  }
}
