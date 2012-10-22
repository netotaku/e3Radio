
// Initialise Spotify objects
// https://github.com/ptrwtts/kitchensink/blob/master/js/kitchensink.js
var sp = getSpotifyApi(1);
var models = sp.require('sp://import/scripts/api/models');

// My objects
var radioQueue;
var nowPlayingUri;
var radioEnabled;

// Init the app by loading the play queue and reload it every X seconds
loadQueueFromWebservice();
window.setInterval(loadQueueFromWebservice, 10000);

// Callback when a song finishes playing to play the next song
models.player.observe(models.EVENT.CHANGE, function (event) {
	console.log("Something changed!", event);

	if (radioEnabled) {
		// Check if current track is different to the one we expect and if so play the next one
		if (!models.player.track || models.player.track.data.uri != nowPlayingUri) {
			playNextTrack();
		}
	}

});

function enableDisable() {
	var btnEnable = $('#btnEnable');
	if (btnEnable.val() == 'Disable e3Radio') {
		radioEnabled = false;
		btnEnable.val('Enable e3Radio');
	} else {
		radioEnabled = true;
		btnEnable.val('Disable e3Radio');
	}
}

// Handle click on play
function playNextTrack(){
	if (radioQueue && radioQueue.length > 0) {
		// Play track 
		nowPlayingUri = radioQueue[0].SpotifyUri;
		models.player.play(radioQueue[0].SpotifyUri);
		$('#nowPlaying').html(radioQueue[0].Artist + ' - ' + radioQueue[0].TrackName);
	}
	else {
		$('#nowPlaying').html("No tracks in queue!");
	}
}

// Load queue, shift determines whether to remove one from the queue by spotify uri
function loadQueueFromWebservice() {
	$.getJSON('http://localhost:50711/services/Queue.ashx?nowPlaying=' + nowPlayingUri, function (data) {
		console.log(data);
		radioQueue = data;

		displayQueue();
		ensureTracksAreQueued();
	});
}

function displayQueue() {
	// Display the queue
	var queueHtml = '<ul>';
	for (var i = 0; i < radioQueue.length; i++) {
		queueHtml += '<li>' + radioQueue[i].Artist + ' - ' + radioQueue[i].TrackName + '</li>';
	}
	queueHtml += '</ul>';
	$('#upcoming').html(queueHtml);
}

function ensureTracksAreQueued() {
	if (radioQueue.length < 5) {
		// get e3 radio playlist
		var pl = models.Playlist.fromURI("spotify:user:1110472675:playlist:18814Z7fbecsjVm1iPBIAs", function (playlist) {
			console.log("Playlist loaded", playlist.name);
			//console.log(playlist);

			// Add tracks to make up to 5
			var queueHtml = '<ul>';
			var i = 0;
			while (i < (5 - radioQueue.length)) {
				// get track at random, make sure its playable
				var tk = playlist.get(Math.floor(Math.random() * playlist.length));
				console.log(tk);
				if (tk.playable) {
					var displayName = tk.name;
					if (tk.artists && tk.artists[0]) {
						displayName += ' - ' + tk.artists[0].name;
					}
					if (tk.album) {
						displayName += ' (' + tk.album.name + ')';
					}
					queueHtml += '<li>' + displayName + '</li>';

					// add track via webservice
					$.getJSON('http://localhost:50711/services/Enqueue.ashx?uri=' + escape(tk.uri));
					i++;
				}
			}
			queueHtml += '</ul>';
			$('#recentlyAdded').html(queueHtml);
		});
	}
}