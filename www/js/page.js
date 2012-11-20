
var page = (function(){

  $('h1').live('click', function(){
    Backbone.history.navigate('/', true); 
  });

  var t = function(id, data){
    return _.template($('#'+id).html(), data || {});
  }

  return {
    chrome: function(){
      
      l('rendering chrome');

      var logo = { logo: t('logo') };
      var user  = { user: auth.user() };

      $('#e3r').empty().html(t('page', $.extend({}, {
        header: t('header', $.extend(logo, user)),
        footer: t('footer')
      })));
    }
  }

})();