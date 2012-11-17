
// Initialise Spotify objects
// https://github.com/ptrwtts/kitchensink/blob/master/js/kitchensink.js
var sp = getSpotifyApi(1);
var models = sp.require('sp://import/scripts/api/models');

// My objects
var radioQueue;
var nowPlayingUri;
var radioEnabled;

// Callback when a song finishes playing to play the next song
models.player.observe(models.EVENT.CHANGE, function (event) {
	console.log("Something changed!", event);

	if (radioEnabled) {

		// Check if current track is different to the one we expect and if so play the next one
		if (!models.player.track){// || models.player.track.data.uri != nowPlayingUri) {
			playNextTrack();
		}
	}

});

// handle enable/disable button. checkbox didn't work for some reason.
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

	    // Update e3radio db
		var val = {
		    event: 'TX-movePlayhead',
		    data: nowPlayingUri,
		};
		ws.send(JSON.stringify(val));

	    // then update the play queue
		radioQueue.shift();
		displayQueue();
		requestUpdatedPlayQueue();
	}
	else {
		$('#nowPlaying').html("No tracks in queue!");
	}
}

// Display the queue
function displayQueue() {
	var queueHtml = '<ul>';
	for (var i = 0; i < radioQueue.length; i++) {
		queueHtml += '<li>' + radioQueue[i].Artist + ' - ' + radioQueue[i].TrackName + '</li>';
	}
	queueHtml += '</ul>';
	$('#upcoming').html(queueHtml);
}

// web socket stuff
var start = function () {
    var inc = document.getElementById('messages');
    var wsImpl = window.WebSocket || window.MozWebSocket;

    inc.innerHTML += "connecting to server ..<br/>";

    // create a new websocket and connect
    window.ws = new wsImpl('ws://localhost:8181/consoleappsample', 'my-protocol');

    // when data is comming from the server, this metod is called
    ws.onmessage = function (evt) {

        // messages are sent in json - parse and handle this one
        var json = JSON.parse(evt.data);

        // handle play queue listing
        if (json.event == 'RX-playQueue') {
            radioQueue = json.data;
            displayQueue();
        }

        // handle if request then lets get another full pq listing to be safe
        if (json.event == 'RX-request') {
            requestUpdatedPlayQueue();
        }
    };

    // when the connection is established, this method is called
    ws.onopen = function () {
        inc.innerHTML += '.. connection open<br/>';

        // get play queue as soon as socket is ready
        requestUpdatedPlayQueue();
    };

    // when the connection is closed, this method is called
    ws.onclose = function () {
        inc.innerHTML += '.. connection closed<br/>';
    }
}
window.onload = start;

function requestUpdatedPlayQueue() {
    // request play queue
    var val = {
        event: 'TX-playQueue',
        //chart: document.getElementById('ddlChart').value,
        //page: document.getElementById('txtPage').value,
        //pageSize: document.getElementById('txtPageSize').value,
    };
    ws.send(JSON.stringify(val));
}