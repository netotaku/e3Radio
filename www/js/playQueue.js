
var playQueue = function(){

  this.tracks;
  var that = this;

  channel.subscribe(this, {
    event: 'connected',
    cb: function(){
      if(!this.tracks){
        console.log('requesting playQueue'); 
        socket.send('TX-playQueue');
      } else {
        this.render();
      }
    }
  });

  channel.subscribe(this, {
    event: 'RX-playQueue',
    cb: function(data){
      this.ini(data);
    }
  });

  channel.subscribe(this, {
    event: 'RX-request',
    cb: function(data){
      this.add(data);
    }
  });

  channel.subscribe(this, {
    event: 'RX-movePlayhead',
    cb: function(){
      this.pop();
    }
  });

}
playQueue.prototype.pop = function(){
  this.tracks.reverse();  
  this.tracks.pop();
  this.tracks.reverse();
  this.render();
} 
playQueue.prototype.ini = function(data){
  this.tracks = data;
  this.render();
}
playQueue.prototype.add = function(data){
  var that = this;
  $.each(data, function(){
    that.tracks.splice(1, 0, this);
  });
  this.render();
}
playQueue.prototype.render = function(){
  var that = this;
  channel.publish({ 
    event: 'ready', 
    data: that.tracks 
  });
}

