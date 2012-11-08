
// var socket = (function(){

//   var fetch = function(file, e){
//     modal.show();
//     $.getJSON(file, function(data){
//       modal.hide();  
//       channel.publish({
//         event: e,
//         data: data
//       });
//     });
//   }

//   return {
//     playQueue: function(){
//       fetch('data/playQueue.json', 'init-playQueue');
//     },
//     track: function(id){
//       fetch('data/track.json?id='+id, 'init-track');
//     }
//   }

// })();

var socket = (function(){

  var ws;  
  var $ci = $('#connection-indicator');
  
  return {
    connect: function(cb){
      ws = new WebSocket("ws://echo.websocket.org/");
      ws.onopen = function(evt){
        $ci.addClass('connected').live('click', function(){});
        cb();
      };
      ws.onclose = function(evt){  };
      ws.onmessage = function(evt){ 
        channel.publish(JSON.parse(evt.data));
      };
      ws.onerror = function(evt){ };
    },
    disconnect: function(){
      $ci.removeClass('connected').unbind();
      ws.close();
    },
    send: function(m){
      ws.send(JSON.stringify(message.get(m)));
    }
  }

})();