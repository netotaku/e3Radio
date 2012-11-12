


var socket = (function(){

  var ws;  
  var $ci = $('#connection');

  var debug = (function(){
    var $controls = $('.settings .debug');  
    return {
      connect: function(){
        $ci.addClass('connected').find('a').click(function(e){
          e.preventDefault();
          socket.send($(this).attr('href'));
        });
        $controls.show();
      },
      disconnect: function(){
        $controls.hide();
        $ci.removeClass('connected').unbind();
      }
    }
  })();
  
  return {
    connect: function(cb){
      if(ws){
        cb();
      } else {
        ws = new WebSocket("ws://echo.websocket.org/");
        ws.onopen = function(evt){
          debug.connect();        
          cb();
        };
        ws.onclose = function(evt){  };
        ws.onmessage = function(evt){ 
          channel.publish(JSON.parse(evt.data));
        };
        ws.onerror = function(evt){ };
      }
    },
    disconnect: function(){
      debug.disconnect();
      ws.close();
    },
    send: function(m){
      ws.send(JSON.stringify(message.get(m)));
    }
  }

})();