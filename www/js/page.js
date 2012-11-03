
var page = function(){
  $('h1').live('click', function(){
    Backbone.history.navigate('/', true); 
  });
  // channel.subscribe(this, {
  //   event: 'init-playQueue',
  //   cb: function(){
  //     this.chrome();
  //     channel.unsubscribe(this);
  //   }
  // })
}
page.prototype.chrome = function(){
  var t = this;
  var logo = { logo: t.t('logo') };
  var user  = { user: auth.user() };
  console.log(user);
  $('#e3r').empty().html(t.t('page', $.extend({}, {
    header: t.t('header', $.extend(logo, user)),
    footer: t.t('footer')
  })));
  this.placeholder();
}
page.prototype.t = function(id, data){
  return _.template($('#'+id).html(), data || {});
}
page.prototype.placeholder = function(){
  
  $('[placeholder]').each(function(){
    var $ths = $(this),
        scrpt = $ths.attr('placeholder') + '        ',
        chr, str, reset = function() {
          str = ' > ';
          chr = 0;
        },
        set = function() {
          if (scrpt[chr]) {
            str += scrpt[chr++];
          } else {
            reset();
          }
          $ths.attr('placeholder', str);
        },
        tck = function() {
          setTimeout(function() {
            set();
            tck();
          }, 200);
        };

    reset();
    set();
    tck();

  });
} 
