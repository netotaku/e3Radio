          
  var debug = (function(){

    $('<div class="settings"><div class="tab"></div><div class="inner"><ul><li>Messages | </li><li><a data-role="ini" href="#">init-playQueue</a></li></ul></div></div>').appendTo('body');

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
            $('<li><a data-role="add" href="#">add-request</a></li><li> | <a data-role="pop" href="#">move-playhead</a></li>').appendTo('.settings ul');
            $this.parent().remove();
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