
var socket = (function(){

  var ws;  
  var $ci = $('#connection-indicator');
  
  return {
    connect: function(cb){
      ws = new WebSocket("ws://echo.websocket.org/");
      ws.onopen = function(evt){
        $ci.addClass('connected').find('a').click(function(e){
          e.preventDefault();
          socket.send($(this).attr('href'));
        });
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