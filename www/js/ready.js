
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
  CHT.unbind();
  TRK.bind(id);
  SCH.unbind();
});

RTR.on('route:search', function(query) {
  CHT.unbind();
  console.log(' > unbind track');
  TRK.unbind();
  SCH.bind(query);
});

RTR.on('route:defaultRoute', function(actions) {
  console.log(' > unbind track');
  TRK.unbind(); 
  CHT.bind();
  SCH.unbind(); 
});

RTR.bind("all", function(route) {
  l(route);
  modal.show();
  socket.connect();
});
