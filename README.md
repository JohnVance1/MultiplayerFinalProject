# MultiplayerFinalProject
My semester long project for IGME 690 in Fall 2021
John Vance

### Controls:
 - W - To jump
 - A - To move left
 - D - To move right
 - Left Mouse Button - Fire weapon

### How to Run
I have included a package called ParelSync in the Unity Project which allows for clones of the project to exist. You'll want to open up ParelSync on the top bar with File and Edit and create at least one new clone in the Clones Manager tab. This way you can have both a host and another client open at the same time. You can then select host on one of the Unity Projects and client on the other in order to play around with both players and interact with each other. Also you can create a third instance of the project and have a deticated Server and two seperate Clients which is how I primarily test the game. 


## Milestone 3

For this milestone I was mostly focused on cleaning up a lot of the code that currently existed in the game rather than adding in new gameplay features. I am actually pretty proud of myself as I actually figured out what gets called where. 

The main change that I made was getting the correct instance of the other Player. I changed it from running on the NetworkManager690 script to being detected through the collisions. Whenever a weapon collides with another Player, they each get a reference to each other. This makes the code a lot simpler and more efficient.

I also figured out a lot of the lagginess was becausing I was sending packages really slow. I edited so that the packages were sent faster and that fixed the lagging issue. 

Besides the two points above I also simplified all of my other Command and RPC calls across the entire Player script. The other thing I changed was moving the collisions from the Server to the Client. This allowed for me to make Command calls based off of the collisions which helped a ton. I was running into a lot of issues with handling my collisions on the Server and switching it to the Client allowed me to call things easier. 

To run this milestone you should open up the Unity Editor and create clones of it similar to the method outlined in Mileston 2 below. 

Controls are also the same as the previous Milestone.


## Milestone 2

This milestone was definitly a lot more difficult than the last milestone however I did manage to complete all of my goals for it. 

In terms of what I struggled with a lot of it was learning Mirror's Documentation (https://mirror-networking.gitbook.io/docs/) and how to use it. I eventually figured out how the basics of it worked, but I am more than sure that some of my code/transfers to and from the server could be more efficient. 

The main issue that I had with the code was figuring out how to get a reference to the other player. I probably looked at 10 different tutorials on how to do multiplayer in Unity with and without Mirror to try and get some idea on how it would work. I eventually got each player to hold a reference to the other by calling a method on the clients when the server detects the number of players is equal to two.  

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

