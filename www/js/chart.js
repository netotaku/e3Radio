
var chart = function(){
  $('#tracks li').live('click', function(){
    Backbone.history.navigate('track/' + $(this).data('track-id'), true); 
  });
}
chart.prototype.draw = function(data){
  var $ul = $($('#chart').html()).appendTo($('#content').empty());
  var i = 1;

  $.each(data, function(){
    $ul.append(_.template($('#track-list').html(), $.extend(this, { i: i++ })));
  });

  channel.publish({
    event: 'after-draw'
  })
}
chart.prototype.bind = function(){
  channel.subscribe(this, {
    event: 'render-chart',
    cb: function(data){
      this.draw(data);
    }
  });
}
chart.prototype.unbind = function(){
  channel.unsubscribe(this);
}