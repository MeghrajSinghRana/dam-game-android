# Game Vision: Source Brief

The following is the user's original brief, saved verbatim for future reference.

> with you doing almost all the work, I want to do rapid development of a mobile game. The game is supposed to be a 3d idle clicker game, the graphics of which should look like the game "HUman fall flat". Smooth 3d curvature of objects. I will not be modelling anything, neither will I buy any assets online. I expect you to create the 3d assets. No detailed textures are needed. Plain colors on 3d models. I can install blender on my system and connect it to you using Blender MCP if that works our favor. I am open to other ideas too.
>
> The main aim of this project is to deploy a rapidly developable game with minimum overhead of development (creating assets, etc.), include ads in it (for which player should be given in-game rewards), and earn money from ads. I dont want to spend too much develop complex systems in game. There should be legit reward loops in game and game should be playable infinitely in a single playthrough. Game's difficulty should scale automattically.
>
> FOr the premise of gameplay, I was thinking there should be a Dam on the left side of screen containing water, and that water should keep getting filled by occassional rains. Rain intensity can vary from drizzles to thunderstorms. player would need to click on the screen to earn coin. With coin they can install a water pump that takes water away from Dam, and puts it in ground. There would be a village in the center and right side of screen. If the dam overfills, water physically drops from the dam into the village and the village starts taking damage. If village's health reaches 0, game over. With every pump installed, a new pump's price should increase substantially. Once all pumps have been installed, they should be infinitely upgradable. How the game scales difficulty  infinitely? Answer = The stronger the rain, the exponentially faster the dam starts getting filled. Longer the duration of stronger rain means a lot more water gets accumulated in that dam. At maximum difficulty it should be impossible to hold off the village from flooding when the rain is at its highest intensity for a set amount of duration (lets say 1 minute). That duration increases everytime player survives it, for example of player survived 1 minute, that duration goes upto 1 minute 10 seconds next time. And the rains should not be constant, there should some Randomness, Procedural nature to it. Not every playthrough should have same pattern of rain events.
>
> I am thinking about many more ideas to make the game more engaging, but this is the barebones basic gameplay loop i am thinking.
>
> As for monetization, player can watch ads to gain health on village or to upgrade any of their water pumps by a level.
>
> Tell me what you think about this.  I am working on this alone, with your help. My goal is to start earning from this game as soon as possible. You will be sole driver of all the development of the game. I will just help you setup things as you need. I will rely on you to proactively suggest me to take certain actions wherever those actions would benefit us to reach my goal as I just stated.
>
> I request you to save this whole prompt (message) of mine somewhere, so you can reference it later.

## Project decisions

- Tapping earns coins.
- Pumps only remove water from the dam; they do not generate coins.
- The player increases coins per tap by buying tap-strength upgrades with coins earned by tapping. The upgrade is represented by a visible village worksite or tool improvement, so increasing tap output feels part of the world.
- No coins are earned without tapping.
- Target run length: a strong run may last roughly 50–100 hours; the player should generally feel capable, with occasional severe weather spikes that create tense recovery moments.
- Rain, flooding, pump operation, and village damage should use natural-feeling visual and sound feedback. Village damage should also be clear through a health bar and visual changes.
- Pump upgrades increase the pump's water-draining power.
- Initial ad concept: a periodic offer (about every 2–3 minutes) to watch a rewarded ad for extra coins. Keep the ad optional and give the reward only after the ad is completed.
- The original ad ideas—village-health recovery and a free pump level—remain possible future rewarded-ad options, but are not part of the first monetization implementation.
- Additional gameplay and monetization mechanics can be considered after the core loop is playable.
- The game uses landscape orientation.
- We will make the 3D assets ourselves, with smooth rounded shapes and simple coloured materials; no purchased art assets or detailed textures are required.
- The target is a studio-quality presentation, achieved through coherent art direction, responsive effects, clear UI, sound, and reliable mobile performance.
