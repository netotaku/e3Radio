
var playQueue = function(){

  this.tracks;
  var that = this;

  channel.subscribe(this, {
    event: 'init-playQueue',
    cb: function(data){
      this.ini(data);
    }
  });

  channel.subscribe(this, {
    event: 'add-request',
    cb: function(data){
      this.add(data);
    }
  });

  channel.subscribe(this, {
    event: 'move-playhead',
    cb: function(){
      this.pop();
    }
  });

}
playQueue.prototype.pop = function(){
  this.tracks.reverse();  
  this.tracks.pop();
  this.tracks.reverse();
  this.finished();
} 
playQueue.prototype.ini = function(data){
  this.tracks = data;
  this.initialised();
  this.finished();
}
playQueue.prototype.add = function(data){
  var that = this;
  $.each(data, function(){
    that.tracks.splice(1, 0, this);
  });
  this.finished();
}
playQueue.prototype.finished = function(){
  var that = this;
  channel.publish({ 
    event: 'after-playQueueUpdate', 
    data: that.tracks 
  });
}
playQueue.prototype.initialised = function(){
  channel.publish({ 
    event: 'playQueue-initialised'
  });
}
playQueue.prototype.fetch = function(cb){
  if(this.tracks){
    this.finished();
  } else {
    socket.fetch();
  }
}