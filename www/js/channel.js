
var channel = (function(){

  var stack = [];

  var tst = function(evs, ev){
    return $.inArray(ev.event, evs.split(' '));
  }

  return {
    publish: function(e){

      var i = 0;

      var each = function(){

        if(stack[i]){
          try{
            if(tst(stack[i].event, e) > -1){
              stack[i].cb.call(stack[i].inst, e.data);
            }
          } catch(err){
            console.log(this); // catching unsubscribe / subscribe clash?
            console.log(err);
          }

          i++;
          each();

        } else {
          i = 0;
        }

      }

      each();      

    },
    subscribe: function(i, data){
      stack.push(_.extend({
        inst: i
      }, data));
    },
    unsubscribe: function(obj, event){
      l('unsubscribing');
      var i = 0;
      for(o in stack){
        console.log(obj);
        console.log(stack[o].inst);
        console.log('-------');        
        if(stack[o].inst == obj){
          console.log(stack.splice(i, 1));
          console.log('----------------------------------'); 
        }
        i++;
      }
      //console.log(stack);
    } 
  }

})();