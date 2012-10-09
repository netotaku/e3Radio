
			var data = (function(){

				var $data;

				return {
					get: function(options){

						var success = options.success;

						$.extend(options, {
							success: function(data){
								success($data = $(data));
							}
						});

						if(!$data){
							$.ajax(options);
						} else {
							success($data);
						}	
					},
					trackData: function($d){
						return {
							rank: $d.attr('rank'),
							artist: $d.find('artist name').text(),
							title: $d.find(' > name').text(),
							image: $d.find('image[size=extralarge]').text(),
							id: $d.find('mbid').text()
						}
					},
					track: function(id, options){
						data.get({
							url: 'toptracks.xml',
							success: function($data){
								var t;
								// var selector = "mbid:contains('" + id + "')";
								
								$data.find('track').each(function(){
									var $this = $(this);
									var mbid = $this.find('mbid').text();
									if(mbid == id) t = $this;
								});
								
								options.success(data.trackData(t));
							}
						})
					}				
				}
			})();
