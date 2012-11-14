
channel.subscribe(this, {
  event: 'connected',
  cb: function(){
     $('#connection').addClass('connected');
  }
});

channel.subscribe(this, {
  event: 'disconnected',
  cd: function(){
    $('#connection').removeClass('connected');
  }
});

///////////////////////

var socket = (function(){

  var uri = "ws://echo.websocket.org/";
  var ws;  
  
  var onOpen = function(evt){
    channel.publish({ event: 'connected' });
  }

  var onClose = function(){
    channel.publish({ event: 'disconnected' });
  }

  var onMessage = function(evt){
    channel.publish(JSON.parse(evt.data));
  }

  return {
    connect: function(){
      if(ws){
        onOpen();
      } else {
        ws = new WebSocket(uri);
        ws.onopen = onOpen;
        ws.onmessage = onMessage;
        ws.onclose = onClose;
      }
    },
    disconnect: function(){
      ws.close();
    },
    send: function(m){
      ws.send(JSON.stringify(message.get(m)));
    }
  }

})();