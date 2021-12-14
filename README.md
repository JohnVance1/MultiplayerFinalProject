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

## Final Milestone

For the final milestone I worked on actually picking up the weapons and connecting from two different machines. Out of the two of these I mostly focused on the weapon pickup as connecting to two different machines proved to be somewhat varried in difficulty. 

While working connecting to multiple machines I first tried inputting the IP address of two different machines on the same network at my apartment. This worked flawleslly and I have displayed the IP address of each local machine so that players can connect that way. The next thing I tried was connecting two lab machines in the lab however I wasn't able to connect them the same way I did at my apartment. I am assuming that this is because of RIT's firewall however I was able to connect a lab computer to my personal laptop but not the other way around. Again, I am assuming that this is because of RIT's firewall.

Because of the fact that I wasn't able to get port forwarding to work and communication between the lab computers I instead attempted to work on a Jackbox/Amoung Us style room code for the game. However this proved to be a large time sink and was ulimately scrapped. My initial idea for it was to take the IP address of the server/host and do a very simple number to letter swap for each number within the IP address. Even before working on this I know that this is how no game should ever design a room code like system as it is extremely easy to decode, however I really wanted to try and get something like this implemented. Creating the encoding and decoding proved to have too many variables and I am 100% certain that my implementation of it was incorrect. There proved to be too many factors and differences in IP addresses to properly read the letter code that was produced to decode it into the correct address which is the main reason for removing it from the project.  

When I started working on the weapon pick-up I was definitly more confident that it would be easier than it ended up being. I initially got the pick-up to work for someone who was the host, however when I tested the client connected to the host it was completely broken. A lot of the bug fixes that I had to do were related to how the weapon fired and how it was attached to the Player. 

After I fixed the bug on the client connected to the host I then checked two clients connected to a deticated server. While I assumed that both of the clients would work similarly to the one connected to the host, this was far from the case. Both of the clients wouldn't correctly place the weapons on the player and wouldn't allow for either Player to shoot/fire the weapon. Ultimatly I fixed these bugs by reorganizing how the Player's prefab worked and calling some methods on the Server and the Client to display the actions on both places. Currently the bug fixes are pretty messy as the collisions are still being detected on the Client and not the Server. I think that some of the bugs could be fixed if the collisions were moved to the Server however I feel like that would involve an even larger refactoring of the code that is out of scope. 

Overall I really enjoyed working on this project. I am really happy with the final product and I am thinking about continuing this project in the future as a personal project. Even though I wasn't able to connect across two seperate networks with port forwarding, I am excited to now be able to create games and projects that are no longer confined to being singleplayer!


## Milestone 3

For this milestone I was mostly focused on cleaning up a lot of the code that currently existed in the game rather than adding in new gameplay features. I am actually pretty proud of myself as I actually figured out what gets called where. 

The main change that I made was getting the correct instance of the other Player. I changed it from running on the NetworkManager690 script to being detected through the collisions. Whenever a weapon collides with another Player, they each get a reference to each other. This makes the code a lot simpler and more efficient.

I also figured out a lot of the lagginess was becausing I was sending packages really slow. I edited so that the packages were sent faster and that fixed the lagging issue. 

Besides the two points above I also simplified all of my other Command and RPC calls across the entire Player script. The other thing I changed was moving the collisions from the Server to the Client. This allowed for me to make Command calls based off of the collisions which helped a ton. I was running into a lot of issues with handling my collisions on the Server and switching it to the Client allowed me to call things easier. 

To run this milestone you should open up the Unity Editor and create clones of it similar to the method outlined in Milestone 2 below. 

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

