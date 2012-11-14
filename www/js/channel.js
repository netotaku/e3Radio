
var channel = (function(){

  var stack = [];

  var tst = function(evs, ev){
    return $.inArray(ev.event, evs.split(' '));
  }

  return {
    publish: function(e){
      $.each(stack, function(){
        if(tst(this.event, e) > -1){
          this.cb.call(this.inst, e.data);
        }
      })
    },
    subscribe: function(i, data){
      stack.push(_.extend({
        inst: i
      }, data));
    },
    unsubscribe: function(obj, event){
      var i = 0;
      for(o in stack){
        if(stack[o].inst == obj){
          stack.splice(i, 1);
        }
        i++;
      }
    } 
  }

})();