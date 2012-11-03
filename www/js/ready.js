
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

///////////////////////////

RTR.on('route:track', function(id) {
  PGE.chrome();
  CHT.unbind();
  TRK.bind();
  socket.track(id);
});

RTR.on('route:defaultRoute', function(actions) {
  PGE.chrome();
  CHT.bind();
  TRK.unbind();
  if(PLQ.tracks){
    PLQ.render();
  } else {
    socket.playQueue();
  }
});