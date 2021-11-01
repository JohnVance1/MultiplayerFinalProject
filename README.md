# MultiplayerFinalProject
My semester long project for IGME 690 in Fall 2021


## Milestone 2

This milestone was definitly a lot more difficult than the last milestone however I managed to complete all of my goals for it. 

In terms of what I struggled with a lot of it was learning Mirror's Documentation (https://mirror-networking.gitbook.io/docs/) and how to use it. I eventually figured out how it worked but I am more than sure that some of my code/transfers to and from the server could be more efficient. 

Running this milestone should be similar to the previous way to run it except I have included a package called ParelSync which allows for clones of the Unity Project to exist. You'll want to open up ParelSync and create at least one new clone in the Clones Manager tab so you can have both a host and another client open at the same time. You can then select host on one of the Unity Projects and client on the other in order to play around with both players and interact with each other. 

In terms of controls they are the same as in the last milestone.
Controls:
 - W - To jump
 - A - To move left
 - D - To move right
 - Left Mouse Button - Fire weapon

There are some gameplay differences from the first milestone as well. The main one being that instead of an orange beam moving across the screen the weapon(yellow cube) moves accross the screen. Also there is a bit of a delay when the player moves and it is grabbing the other player. 


## Milestone 1

Currently I have finished up what I set out to do for this milestone and I have started working on tasks for the second milestone.

To run this milestone just load up Unity and open up the SinglePlayer scene to test out the first milestone's functionallity.

Controls:
 - W - To jump
 - A - To move left
 - D - To move right
 - Left Mouse Button - Fire weapon

The Player is the white rectangle and the weapon is the smaller yellow square on the side of them.
The single red square in the level is the target that the player will have to hit with their weapon to grab.

I have started working on the multiplayer aspects of the project as well however they are not fully implemented. I am planning on using Mirror within this project however I am still learning it's structure and may switch over to Unity's Multiplayer API if I run into too many issues. https://assetstore.unity.com/packages/tools/network/mirror-129321 

