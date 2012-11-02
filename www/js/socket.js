
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

    },
    track: function(id){

    },
    like: function(){

    }
  }

})();