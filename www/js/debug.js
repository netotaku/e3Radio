          
  var debug = (function(){

    $('<div class="settings"><div class="tab"></div><div class="inner"><ul><li><a data-role="ini" href="#">Init play queue</a></li></ul></div></div>').appendTo('body');

    $('.settings a').live('click', function(e){
      e.preventDefault();

      var $this = $(this);

      switch($this.data('role')){
        case 'ini':
          $.getJSON('data/playQueue.json', function(data){
            channel.publish({
              event: 'init-playQueue',
              data: data
            });
            $('<li><a data-role="add" href="#">Add request</a></li><li> | <a data-role="pop" href="#">Move play head</a></li>').appendTo('.settings ul');
            $this.parent().remove();
          });
        break;
        case 'add':
          $.getJSON('data/request.json', function(data){
            channel.publish({
              event: 'add-request',
              data: data
            });
          });
        break;
        case 'pop':
          channel.publish({
            event: 'move-playhead'
          })
        break;
      }

    });
  })(); 