
var chart = function(){
  $('#tracks li').live('click', function(){
    Backbone.history.navigate('track/' + $(this).data('track-id'), true); 
  });
}
chart.prototype.draw = function(data){
  var $ul = $($('#chart').html()).appendTo($('#content').empty());
  var i = 1;

  $.each(data, function(){
    var t = new track($.extend(this, { i: i++ }));
    $ul.append(t.html('list'));
  });

  channel.publish({
    event: 'after-draw'
  })
}
chart.prototype.bind = function(){
  channel.subscribe(this, {
    event: 'after-playQueueUpdate',
    cb: function(data){
      this.draw(data);
    }
  });
}
chart.prototype.unbind = function(){
  channel.unsubscribe(this);
}