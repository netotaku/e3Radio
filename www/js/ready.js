
var router = Backbone.Router.extend({
  routes: {
    "track/:id": "track",
    "*actions": "defaultRoute"
  }
});

var PGE = new page;
var RTR = new router;
var PLQ = new playQueue;
var CHT = new chart;
var TRK = new track;

var e3R = (function(){

  return {
    begin: function(){
      Backbone.history.start();
    },
    end: function(){
      socket.disconnect();
      Backbone.history.stop();
    }
  }

})();

///////////////////////////

RTR.on('route:track', function(id) {
  modal.show();
  // PGE.chrome();
  CHT.unbind();
  TRK.bind();
  socket.connect(function(){
    socket.send('TX-track');  
  });
});

RTR.on('route:defaultRoute', function(actions) {
  modal.show();
  // PGE.chrome();
  CHT.bind();
  TRK.unbind();
  if(PLQ.tracks){
    PLQ.render();
  } else {
    socket.connect(function(){
      socket.send('TX-playQueue');  
    });
  }
});


