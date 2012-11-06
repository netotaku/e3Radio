  
  var debug = (function(){


    $('.settings a').live('click', function(e){
      e.preventDefault();

      var $this = $(this);

      var request = function(){
        $.getJSON('data/request.json', function(data){
          channel.publish({
            event: 'add-request',
            data: data
          });
        }); 
      }

      $('#request form').live('submit', function(e){
       e.preventDefault();
       request(); 
      })

      switch($this.data('role')){
        case 'ini':
          $.getJSON('data/playQueue.json', function(data){
            channel.publish({
              event: 'init-playQueue',
              data: data
            });
          });
        break;
        case 'add':
          request();
        break;
        case 'pop':
          channel.publish({
            event: 'move-playhead'
          })
        break;
      }

    });
  })();

