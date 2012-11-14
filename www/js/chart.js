
var chart = function(){
  $('#tracks li').live('click', function(){
    Backbone.history.navigate('track/' + $(this).data('track-id'), true); 
  });
}
chart.prototype.render = function(data){
  l('rendering chart');
  page.chrome();
  var $ul = $($('#chart').html()).appendTo($('#content').empty());
  var i = 1;

  $.each(data, function(){
    $ul.append(_.template($('#track-list').html(), $.extend(this, { i: i++ })));
  });

  modal.hide();
}
chart.prototype.bind = function(){
  channel.subscribe(this, {
    event: 'ready',
    cb: function(data){
      this.render(data);
    }
  });
}
chart.prototype.unbind = function(){
  channel.unsubscribe(this);
}