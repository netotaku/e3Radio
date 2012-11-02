
channel.subscribe(this, {
  event: 'after-chrome',
  cb: function(){

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
}); 
