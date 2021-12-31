[![Open in Visual Studio Code](https://classroom.github.com/assets/open-in-vscode-f059dc9a6f8d3a56e377f745f24479a46679e63a5d9fe6f495e02850cd0d8118.svg)](https://classroom.github.com/online_ide?assignment_repo_id=459630&assignment_repo_type=GroupAssignmentRepo)

**The University of Melbourne**
# COMP30019 – Graphics and Interaction

Final Electronic Submission (project): **4pm, November 1**

Do not forget **One member** of your group must submit a text file to the LMS (Canvas) by the due date which includes the commit ID of your final submission.

You can add a link to your Gameplay Video here but you must have already submit it by **4pm, October 17**

# Project-2 README

You must modify this `README.md` that describes your application, specifically what it does, how to use it, and how you evaluated and improved it.

Remember that _"this document"_ should be `well written` and formatted **appropriately**. This is just an example of different formating tools available for you. For help with the format you can find a guide [here](https://docs.github.com/en/github/writing-on-github).

**Get ready to complete all the tasks:**

- [x] Read the handout for Project-2 carefully.

- [x] Brief explanation of the game.

- [x] How to use it (especially the user interface aspects).

- [x] How you designed objects and entities.

- [x] How you handled the graphics pipeline and camera motion.

- [x] The procedural generation technique and/or algorithm used, including a high level description of the implementation details.

- [x] Descriptions of how the custom shaders work (and which two should be marked).

- [x] A description of the particle system you wish to be marked and how to locate it in your Unity project.

- [x] Description of the querying and observational methods used, including a description of the participants (how many, demographics), description of the methodology (which techniques did you use, what did you have participants do, how did you record the data), and feedback gathered.

- [x] Document the changes made to your game based on the information collected during the evaluation.

- [x] References and external resources that you used.

- [x] A description of the contributions made by each member of the group.

# Table of contents
* [Explanation of the game](#explanation-of-the-game)
* [Technologies](#technologies)
* [How to interact with the user interface](#how-to-interact-with-the-user-interface)
* [Description of how objects and entities are designed](#description-of-how-objects-and-entities-are-designed)
* [Procedural Generation Overview](#procedural-generation-overview)
* [Description of how the non-trivial shader works](#description-of-how-the-non-trivial-shader-works)
* [Description of the particle system](#description-of-the-particle-system)
* [Description of the querying and observational methods used](#description-of-the-querying-and-observational-methods-used)
* [References and external resources](#references-and-external-resources)
* [Description of the contributions made by each member](#description-of-the-contributions-made-by-each-member)

# Explanation of the game
The genre of this game is a top-down 2.5D shooter,inspired by the game _Hotline
Miami_. As a lone lich stuck in a space dungeon, the objective of this game is to shoot
your way through and try to survive as long as you can in the dungeon. There will be
health pickups randomly appearing in different rooms of the dungeon, and there is also
a scoring system to encourage players to replay the game for a higher score. Scores
are obtained by slaying enemies, and added at the end of each stage according to the
remaining player's health.
	
# Technologies
Project is created with:
* Unity 2021.1.13f1

# How to interact with the user interface

The game begins in the main menu, with a “play” button that starts the game, an
“instruction” button that brings the player to a written tutorial page, and a “quit” button
that quits the game. Game related controls are all documented in the instruction page.

When playing the game, the player can press “Esc” to enter the pause menu,
which has an icon indicating the game is paused, and a “Return to main menu” button
that navigates back to the main menu.

When the player character dies after losing all the HP, a “game over” menu will
show up which contains a “replay” button that restarts the game, and a “Main menu”
that navigates to the main menu.

When the player manages to collect the trophy at the end room a menu will
appear. The menu will contain the current score that the player has, a “Next level”
button that loads the new level (with the current score saved), and a “Return to main
menu” button that navigates back to the main menu.

_Note: If you play the game within Unity, you may have to increase the intensity of the directional light within Test Scene._

# Description of how objects and entities are designed

Our game contains one playable character and two types of enemies. The
structure of the player prefab is:

1. Player object. It contains scripts that control the player to move as well as the
    health management. It also has a rigidbody and a box collider.
2. Body object. It contains a script that reads the position of the mouse and allows
    the player to rotate their character by rotating their mouse.
3. An external asset that provides the model and animations of the character.
4. Wand object. It contains a script that allows the player to shoot projectiles and
    deal damage to enemies.
5. FirePoint object. It is an empty game object used as a reference position for
    generating projectiles.
6. Point light. It is placed inside of the character model to make it more
    distinguishable in a dark environment.

The structure of the ranged enemy(Beholder):

1. RangedEnemy object. It contains scripts that control Enemy AI, health, and shoot
    projectiles. It also has a rigidbody, a box collider, and a nav mesh agent. The
    basic AI for enemies is to chase the player in the shortest path on a baked nav
    mesh. When an enemy is spawned, it will be assigned to a random speed,
    angular speed, acceleration, aware range and fire rate within a predefined range.
    Therefore, every enemy would act slightly differently. A ray is casted between 
    each enemy and their target to check if there is any obstacle in between to avoid
    situations when enemies attack even if they cannot see the player.
2. An external asset that provides the model and animations.
3. Point light. It is placed inside of the character model to make it more
    distinguishable in a dark environment.

The structure of the melee enemy(Mimic):

1. Enemy object. It contains scripts that control Enemy AI, health. It also has a
    rigidbody, a box collider, and a nav mesh agent. Same basic AI logic as the
    ranged enemy and same randomly generated attributes with higher predefined
    range(except for the aware range, which is much lower than the ranged enemy).
    It is designed to be a fast enemy that attacks players when they are off guard.
    When idle, it appears as a normal treasure chest.
2. An external asset that provides the model and animations.

Our game has only one orthographic camera that is used to show players their
character and surroundings. It also keeps following the player. To achieve natural
camera movement, we used SmoothDamp(). In addition, players can also hold shift to
let the camera move a certain distance towards the mouse smoothly.

# Procedural Generation Overview

## Dungeon Levels Rooms

Our dungeons used as levels for our game are procedurally generated. This is
done by having sets of premade floor layouts which we apply a placement algorithm to
in order to select, rotate and transform each room accordingly. Rooms have associated
weights with them which are dynamically adjusted depending on ‘memories’ of
previously placed layouts. The procedural generation algorithm can be broken down
into the following high level steps:

1. Start room placement
2. Generic room placement
3. End room placement
4. ‘Secret’ room placement
5. Level Rounding
6. Updating generation memory
7. Procedural fog decoration

All rooms used in generating the dungeon have an associated ‘ratio’ which is
basically a variable used to determine how frequently a room is placed relative to other
rooms of that same type. A history of room placements is stored in our ‘memoryMatrix’
which is used as a reference when updating the ratios of rooms. The reason for this is
that even if all placement ratios are equal, there are rooms which are placed less
frequently than would be expected due to their awkward mesh shape. In order to deal
with this we call the method updateRatioByMemories() in order to compare expected
room counts with those we actually get in previous level generation. The strength of
each memory (array representing previously generated level) decreases linearly with
time based on a customisable multiplier. This is because older levels won't be as fresh
in the mind of players when compared to recently played ones.

For the actual placement of rooms, first the basic layout of the room is generated
by placing the start room, followed by the generic rooms, finished by the ending room.
All these placements are applied using the previously mentioned dynamic ratios. Rooms
are then placed by comparing the angle between room entrances, rotating the meshes
so that there is a 180 degree rotation between them, and transforming meshes to have
the same entrance location. A layer mask is used to detect and handle mesh collisions.
After this basic layout has finished placement, ‘secret’ rooms are placed which have
different ratio and memory tables from the generic rooms. The number of these rooms
which can appear in a level is determined by the ceiling of ((generic rooms placed) /
(adjustable divisor) + 1). We do this so there are a consistent number of ‘secret’ rooms
as the size of the levels increase. The memory tables for these rooms are more simple
than the generic rooms, we just check if the player got really lucky (all secrets were
treasures) or really unlucky (no secrets were treasures) and increase or decrease the
ratio of secret rooms for the next level appropriately to compensate.

Last, we perform level rounding, since level generation can leave awkward
hallways or entrances which lead to nowhere, we handle this by ‘rounding’ out the level
by placing dead end rooms where possible in order to make the final generated level
look prettier. Then we record all the placement decisions in memory to be used by the
algorithm the next time a level is generated.

## Dungeon Level Fog

The fog effect we use in the dungeon rooms is also procedurally generated with
the use of perlin noise. The code is based on the tutorials given by Sebastian League in
the Procedural Landmass Generation playlist
(https://youtube.com/playlist?list=PLFt_AvWsXl0eBW2EiBtl_sxmDtSgZBxB3)
,with some changes to create a fog effect instead of a mesh. The procedure starts within Map
Generator, where a noise map is generated, and then a texture is created based on this
noise map. It is then passed to the plane.

First a noise map is generated using the Noise Script. The mathematics for the
noise map is based on the Sebastian League method for manipulating perlin noise, via
the use of octaves, lacunarity, persistence, and a noise scale. Octaves are the levels of
noise, lacunarity controls the frequency in the octaves, persistence controls the
amplitude of the octaves. The noise scale controls how far into the noise map it zooms
into. There is also scaling of the values within the noise map, so that it remains between
0 and 1, for easy usage when manipulating the opacity of the fog. Randomness is
achieved by using an offset to pick a certain position of the perlin noise to display. A
seed is picked within _MapGenerator.cs_ using a randomnumber generator between 0
and 1000, which is passed into _Noise.cs_ which is thenused in System.Random object
to pick an offset between -100,000 and 100,000. It allows us to find and manipulate a
specific map if necessary.

The noise map is then sent to _TextureGenerator.cs_ that creates a texture to be
displayed. This texture generator creates a fog texture by assigning the noise map
values to the alpha of a color. Then there is post processing done on the texture. The
opacity of the fog at a certain alpha is controlled via a curve that takes noise map values
and outputs an alpha value. For example, an alpha value of 1 makes an object
completely opaque, but if you don't want it to be completely opaque, you can manipulate
the curve so that noise map values of 1 are then turned into your desired output.

Something similar had to be done near the edges of the fog as well. The plane is
divided into 4 quadrants and the distance is calculated by using the distance between
the point and the horizontal edge and then it is divided by the midpoint. The same is
done for the vertical edge. Both are multiplied together and then doubled. The resulting
distance is then multiplied to the alpha for that point in the fog texture as well. The
distance where the fog starts to fade and the rate at which it fades is determined via a
curve.

The texture is then returned to _MapGenerator.cs_ and is then applied to the plane.
There is also functionality to have the fog move, by making and storing multiple textures
within an array. Each subsequent texture has a slightly higher offset and switching
between textures provides the illusion of movement. However, the initial load time to
create the textures takes a while, although the texture switching is near instant, and it
was deemed unnecessary by the team. This functionality can still be seen within the Unity editor by opening up _Fog_ scene.

# Description of how the non-trivial shader works

```
Path to the source file: Assets/Shaders/Starry night
```
The starry night non-trivial shader is based on techniques we learnt through the
research of water effect shader: perlin noise and normal map distortion, which is also
where we retrieve the noise and distortion texture from.

The effect is achieved by first applying perlin noise on a plane and combine it
with the predefined color, then add scrolling animation to mimic the “star flashing effect”,
apply distortion to make the noise looks more natural (noises or “the stars” spread to
every direction instead of going only horizontally), and lastly include a cutoff parameter
to limit how many noise (stars) are shown. TRANSFORM_TEX() is used in the vertex
shader to allow modification of the UV input with provided texture's tiling and offset
settings, while tex2D() is used in the fragment shader to sample the noise and distortion
to feed into the animation and cutoff calculations. The distortSample is multiplied by 2
and subtracted 1 by adding to the noiseUV, since textures are originally in a range of 0
to 1, but what we want in noiseUV is something ranged between -1 to 1. Most code is
commented to explain what each line performs.

Here is the explanation of how the tweakable parameters work: tiling for noise
controls the number of tiles for noise (white tiles); tiling for distortion controls the area of
effect and strength of the distortion; noise cutoff controls the amount of white noises
shown; noise scroll amount controls the direction and speed of the scrolling animation;
distortion amount controls the strength of the distortion.

To achieve the starry night effect, every single parameter must be the same as
the ones listed in the first segment of the code, while also using the same perlin noise
and normal map texture we used. The code itself is pretty simple without any
complicated maths, but the most tricky part was to modify the values (e.g. cutoff and
distortion amount) to achieve the star effect, which to be honest is still not perfect and
has a lot of room for improvement. For example, occasionally some “stars” still
resemble the “water wave” movement and appear as a horizontal line.

# Description of how the trivial shader works

```
Path to the source file: Assets/Shaders/Wave
```
The wave trivial shader is based mostly on the solution code from workshop 5
question 5, which applies a basic sine wave effect to a textured object. We changed the
code by adding new variables to the sine equation, which allows us to modify the speed
and amplitude of the wave in the scene settings.

After tweaking the speed and amplitude parameter, we applied this shader on our
player and ranged enemy prefabs to create a “floating” effect as an idle animation on
the character models, so that they will look a lot more natural when not moving (instead
of standing still it will float around).

# Description of the particle system

The particle systems that we created from scratch are Beholder Death and the
Mimic Death. The particles play when an enemy is destroyed. These particle systems
have two parts, the explosion particles and the physical particles. The physical portion of the particles are meant to represent the actual body parts that scatter outwards. The explosion particles are meant to highlight the physical particles. The pink eyeball enemies have light pink explosion particles that complement the physical particles and the chest enemies' explosion particles have yellow lights attached to them to contrast against the dark brown physical particles. Both particles are destroyed when they are finished to avoid unecessary memory usage.
# Description of the querying and observational methods used

**Participants demographic**

- Total number of 5 participants
- All males, aged between 20 to 24, students
- 2 Malaysians, 1 Australian, 2 Hongkongers
- For privacy reasons, names are not recorded


## 1. Observational method

**Methodology**

- The games are built and sent to each participant
- They stream their gameplay via discord.
- A developer observes them as they play and takes notes.
- Participants are also encouraged to speak while playing, so we could take note
    of what they are thinking during the game.
    
**Results**

Observer’s Notes Summary
1. Players often go through rooms without care initially. This behaviour lead to all
participants getting overwhelmed as more enemies start following them. They become more cautious afterwards.
2. They struggled with killing enemies. They had too much health, and players did not know how much health the enemies had left.
3. They wander aimlessly throughout the majority of gameplay.
4. Most participants got bored within a few minutes of gameplay.
5. One participant glitched through the wall and ended up in space around the
dungeon room.
6. None used the SHIFT to lean.

Players’ Thoughts During Gameplay Summary
1. Most commented on the style of the game, positive feedback.
2. All were a bit confused on what to do.
3. All hated the bugs where enemy bullets could go through walls
4. Some stated that the rooms weren’t different enough
5. They thought that the enemies took too long to kill and they mobbed you too
quickly.
6. They got bored fighting the same enemy.
7. All players were frustrated at not being able to perceive their remaining health.

**Discussion**

There are multiple advantages and disadvantages to using the pen and paper
method. It is cheap and relatively easy to accomplish. We also had our participants
speak during gameplay, so we can record their thoughts. We had done this because we
were unable to watch them in person when they played, so we needed a way to gauge
their thoughts and reactions. Although this method is easy to accomplish, its
effectiveness is determined by the developer’s ability to take notes quickly. If a specific
event is forgotten and not noted, then that event does not become relevant when
changing and improving the game. The alternative would be to record while streaming,
but we were not able to record on discord. The short duration of gameplay we wanted
our participants to test was fairly short, and in general, we were able to keep up with
players when writing down their thoughts.

## 2. Querying method

- https://forms.gle/kXq94yvExXtMWPa
- USE (Usefulness, Satisfaction, and Ease of use) questionnaire
- 9 questions in total
- Handed out at the end of the development cycle

**Methodology**

- Google Forms
- Questionnaire, with 2 ease of use, 4 satisfaction, 1 usefulness and 2 extra
    questions
- 7 linear scale, 2 short answer questions
- Data analysis is done through Google Forms summary, as shown in the
    screenshot below

**Feedback gathered**

- 100% of participants think the menu is _extremelyeasy_ to comprehend
- 80% of participants think the controls are _extremelyeasy_ to learn, 20% thinks it’s
    _easy_ to learn
- 60% of participants think the game plays _like_ howthey imagined, 40% thinks the
    game plays _exactlylike_ how they imagined
- 20% of participants think the gameplay is _not satisfying_ ,40% has a _neutral_
    opinion, 40% thinks it’s _satisfying_
- 80% of participants think the visual effect is _satisfying_ , 20% thinks it’s _neutral_
- 40% of participants feel _perfectly normal_ after playingthe game, 40% feels
    _normal_ , 20% feels _neutral_
- 20% of participants think the game is _not worth_ replaying,40% has a _neutral_
    opinion, 40% thinks the game is _worth_ replaying

## 3. Changes made

- Ranged enemies only have 2 HP.
    - This also addresses the enemy health indicator issue indirectly because it
       is less necessary when enemies only have to be hit twice.
    - Should make gameplay more satisfying since enemies can see that they
       are making progress by being able to kill enemies more often.
- We provided pickups within the game for health, score, and victory.
    - It encourages exploration and gives a reason for the players to explore
       each dungeon level, while making it less tedious than having to go to
       every room to find every enemy.
    - The amount they decide to explore is indirectly tied to the score because
       exploration allows them to find score pickups and more enemies to kill.
    - Players now have an objective to go through the dungeon.
- Added fake chest (mimic) enemy type
    - An extra comment in the questionnaire suggests that “It’d be great if there
       are more enemy types”
    - Also addressed the boredom players have when playing the game.
- Added scoring system
    - An extra comment in the questionnaire suggests that “game is fun in the
       beginning but gets bored after a few rounds”
    - Gives the player a reason to play and replay the game.
    - Allows players to measure their skill against others if they want.
- Added Healthbar
    - Addresses player’s issue with knowing how much is left
- Bug fixes for wall collison for the bullets
    - It was an issue consistently found by every participant.
- Added objectives to game
    - Each time the traverse a dungeon
       - To increase score
       - To find the victory pickup
    - Game
       - To keep increasing your high score
    - Allows players to feel as if there is an actual point to the game.
- Added pause screen
    - Addresses player’s issue with no way to navigate back to the main menu
       in game or to take a rest
- Increases difficulty every time the dungeon loads
    - Addresses boredom issues, since the game should become more
       engaging as players continue to progress within the dungeon.
- Added Instructions page
    - Addressed the issue where players did not know they can lean forward
    - Addressed issue where players felt as if they did not have a clear
       objective.
    - (Not from feedback) Game mechanics have gotten slightly more complex,
       so a clear description of how to play the game is likely beneficial.

# References and external resources

- https://assetstore.unity.com/packages/2d/textures-materials/stone/stone-floor-texture-
- https://assetstore.unity.com/packages/3d/props/weapons/low-poly-weapons-0
- https://assetstore.unity.com/packages/3d/environments/fantasy/dinner-table-0
- https://assetstore.unity.com/packages/3d/environments/fantasy/stones-and-buried-treasure-
- https://assetstore.unity.com/packages/3d/environments/stylized-hand-painted-dungeon-free-
- https://assetstore.unity.com/packages/3d/props/trophy-cups-chalices-free-
- https://assetstore.unity.com/packages/3d/props/potions-coin-and-box-of-pandora-pack-
- https://assetstore.unity.com/packages/vfx/particles/fire-explosions/procedural-fire-141496
- https://assetstore.unity.com/packages/3d/characters/humanoids/fantasy/mini-legion-lich-pbr-hp-polyart-
- https://assetstore.unity.com/packages/3d/characters/creatures/rpg-monster-partners-pbr-polyart-
- https://assetstore.unity.com/packages/3d/environments/dungeons/low-poly-dungeons-lite-
- https://catlikecoding.com/unity/tutorials/flow/texture-distortion/
- https://roystan.net/articles/toon-water.html
- https://github.com/DMeville/Unity3d-Dungeon-Generator
- https://www.fontspace.com/vecna-font-f
- https://www.youtube.com/watch?v=v8_TCobmj-M&ab_channel=lesnik
- https://youtube.com/playlist?list=PLFt_AvWsXl0eBW2EiBtl_sxmDtSgZBxB

# Description of the contributions made by each member

There are four members in our team: Mattias Andersen,Kritnand Suwanna-Arj,
Pak Ho Wong and Yue Teng.

Mattias was responsible for working on the procedural level generation of the
entire dungeon, including constructing blender models for rooms and placement of the
decorations in each room.

Kritnand was responsible for various particle effects in the game. He was also the
person who recorded and edited the video, while Mattias was the one who did the
narratives. He also added the procedural fog to the dungeon generator. He also added
a few UI elements, like healthbar and some menu screens.

Pak Ho was responsible for making the custom shaders in the game, both the
wave and starry night shaders. He was also responsible for some parts of the UI and
their functionality (e.g. pause, scoring and next level transition).

Yue Teng was responsible for the gameplay-related elements of the game,
including but not limited to the shooting mechanics, player and enemy movements,
enemy AI and spawns.

Multiple team meetings were held to discuss the other parts of the project in
which everyone has equal contributions. For example the overall style and direction of
the game, searching for prefabs in the asset store for player models and the dungeon,
choreography of the video clip, and report drafting.
