
var playQueue = function(){

  this.tracks;
  var that = this;

  channel.subscribe(this, {
    event: 'RX-playQueue',
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
    event: 'render-chart', 
    data: that.tracks 
  });
}

