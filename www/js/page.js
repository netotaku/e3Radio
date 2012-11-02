
var page = function(){
  $('h1').live('click', function(){
    Backbone.history.navigate('', true); 
  });
  channel.subscribe(this, {
    event: 'playQueue-initialised',
    cb: function(){
      this.chrome();
    }
  })
}
page.prototype.chrome = function(){
  var t = this;
  var logo = { logo: t.t('logo') };
  $('#e3r').empty().html(t.t('page', $.extend({}, {
    header: t.t('header', logo),
    footer: t.t('footer')
  })));
  channel.publish({
    event: 'after-chrome'
  })
}
page.prototype.t = function(id, data){
  return _.template($('#'+id).html(), data || {});
}
