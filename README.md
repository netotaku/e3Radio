e3Radio 2.0 1st Release
====================

The application consists of a continuos circular buffer of 'tracks' 

1. Play back commences
---------------------

* 'play head (PH)' is set to the track played longest ago
* the 'play queue (PQ)' is ordered by 'last played (LP)' ascending 

2. During play back
---------------------

* The user may vote up or down any track in the PQ up to the point the track has finished playing
* The user may request songs to be added to the PQ
* At the point a track finishes playing if it has attained neutral or positive karma it will remain in the PQ and played the next time it becomes the last played. Otherwise it will be discarded, Discarded tracks can be requested again. 

3. Play back is stopped
---------------------

* The remaining PQ will be added back to the UP 
* user can add requests directly to a colaborative playlist on spotify

User Stories
====================

The user must be able to:

* authenticate via their Facebook account
* be notified is their browser is inadiquate 
* vote tracks up
* vote tracks down
* request tracks
* determine which track is currently be played
* determine which tracks are about to be played
* determine which tracks have been played
* view information about the track. 
** Title
** Artist
** votes 
** voters

## Browsers
latest IE, Chrome, Safari, Safari mobile, chrome mobile, with experience optimised for desktop, tablet and mobile

## Iterations

# 1st
* Requests
* Sockets ( see messaging section )
* facebook
* basic experience for lesser browsers

# 2nd
* voting
* track detail
* graphs

# 3rd 
* charts
* notifications

Messaging
---------------------

# Socket

> { event: 'init-playQueue', data: [ ... tracks ... ] }
> { event: 'add-request', data: [ ... tracks ... ] }
> { event: 'move-playhead' }

# Events

> { event: 'draw-chart', data: [ ... tracks ... ] }

