
# Soloist :guitar:  
**Application Home Page :** __*http://soloist.gear.host*__  

## Description  :musical_score:  
Soloist is a web application which generates solo-melodies improvisations over a given playback.  

![DesignDiagramSnippet](Design/ScreenShots/designMix.png)
<hr/>

## Further Details :musical_keyboard: 
Given a midi-file & a chord-progression as input,   
along with other user preferences & constraints,  
Soloist analyzes the chord-progression,    
generates a new melody over it using a [genetic algorithm](https://en.wikipedia.org/wiki/Genetic_algorithm),  
and finally replaces the original melody track in the MIDI file  
with the new generated melody.

## Try It Yourself! :musical_note:
[_**Click Here**_](http://soloist.gear.host/Composition/Compose) to try it out your self: 
Just select a song, mark your preferences,  
hit the submit button, and VWallaaaaa -  
your new generated melody would be automatically downloaded as MIDI file. 
![DesignDiagramSnippet](Design/ScreenShots/compose.png)





This appliation implements a genetic algorithm to carry out the composition process. 

## Initial Prototype Sample for Desktop Application :notes: 
![Initial Prototype Sample](Design/prototype-screenshot.png)

