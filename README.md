
## messaging:

> {
>	event: 'init-playQueue',
>	data: [ ... tracks ... ]
> }

> {
> 	event: 'add-request',
> 	data: [ ... tracks ... ]
> }

> {
> 	event: 'move-playhead'
> }


e3Radio 2.0 1st Release
====================

The application consists of 2 sacks

The play queue, ( PQ ) 
* Reflects the users requests that are about to be played. 

The ultimate playlist, ( UP ) 
* Reflects the requests that have been played and have positive or neutral karma. 

1. Play back commences
---------------------

* UP is emptied into PQ

2. During play back
---------------------

* The user may vote up or down any track in the PQ up to the point the track has finished playing
* The user may request songs to be added to the PQ
* At the point a track finishes playing if it has attained neutral or positive karma it will be popped on to the UP stack otherwise it will be discarded, Discarded tracks can be requested again. 

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

We need to split this up into technical iterations.

## 2nd release
* desktop notifications
* charts
* click to play in spotify 