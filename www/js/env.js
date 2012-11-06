var env = (function(){
	
    var config = {
        'localhost': {
            fbid: '147350892005614'
        },
		'e3radio.local': {
			fbid: '437450372968927'
		},
		'netotaku.github.com': {
			fbid: '301033066676474'
		}
	}

	return{
		get: function(what){
			return config[location.hostname][what];
		}
	}
	
})();