
var socket = (function(){

  return {
    fetch: function(){
      $.getJSON('data/playQueue.json', function(data){
        channel.publish({
          event: 'init-playQueue',
          data: data
        });
      });
    }, 
    playQueue: function(){
      $.getJSON('data/playQueue.json', function(data){
        channel.publish({
          event: 'init-playQueue',
          data: data
        });
      });
    },
    track: function(id){
      $.getJSON('data/track.json', function(data){
        channel.publish({
          event: 'init-track',
          data: data
        });
      });
    },
    like: function(){

    }
  }

})();