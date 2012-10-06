	var util = (function(){

				var template = function(id, data){
					return _.template($('#'+id).html(), data || {});
				}

				return {
					page: function(id, data){

						var menu = { menu: template('menu') };
						var logo = { logo: template('logo') };

						$('#e3r').html(template('page', $.extend(menu, {
							header: template('header', $.extend(data, menu, logo)),
							article: template(id),
							footer: template('footer')
						})));

						util.placeholder();

					},
					placeholder: function(){
						$('[placeholder]').each(function(){
							var $ths = $(this),
								scrpt = $ths.attr('placeholder') + '        ',
								chr,
								str,
								reset = function(){
									str = ' > ';
									chr = 0;	
								},
								set = function(){
									if(scrpt[chr]){
										str += scrpt[chr++];
									} else {
										reset();							
									}
									$ths.attr('placeholder', str);
								},
								tck = function(){
									setTimeout(function(){
										set();
										tck();
									}, 160);	
								};

							reset();	
							set();
							tck();

						});	
					}
				}

			})();