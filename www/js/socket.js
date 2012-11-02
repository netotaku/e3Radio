
var socket = (function(){

  var fetch = function(file, e){
    $.getJSON(file, function(data){
      channel.publish({
        event: e,
        data: data
      });
    });
  }

  return {
    playQueue: function(){
      fetch('data/playQueue.json', 'init-playQueue');
    },
    track: function(id){
      fetch('data/track.json?id='+id, 'init-track');
    }
  }

})();