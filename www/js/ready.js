
var router = Backbone.Router.extend({
  routes: {
    "track/:id": "track",
    "search/:query": "search",
    "*actions": "defaultRoute"
  }
});

var RTR = new router;
var PLQ = new playQueue;
var CHT = new chart;
var TRK = new track;
var SCH = new search;

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
  // CHT.unbind();
  // SCH.unbind();
  // TRK.bind(id);
});

RTR.on('route:search', function(query) {
  // CHT.unbind();
  // TRK.unbind();
  // SCH.bind(query);
});

RTR.on('route:defaultRoute', function(actions) {
  // CHT.bind();
  // TRK.unbind();
  // SCH.unbind();
});

RTR.bind("all", function(route, router) {
  modal.show();
  socket.connect();
  // if(PLQ.tracks){
  //   PLQ.render();
  // } else {
  //   socket.connect(function(){
  //     socket.send('TX-playQueue');  
  //   });
  // }
});
