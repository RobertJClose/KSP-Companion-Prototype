# KSP Companion

This program is designed to be used alongside the game Kerbal Space Program (KSP). Personally, when I played the game and used trial-and-error to find successful designs, I felt a bit like I was cheating. I wanted to play the game with the same stakes as a real space agency. You have to know before launch that your rocket is big enough, but you can't just waste resources when it comes to expensive rocketry. The goal is that it should be possible for a player to launch a mission with full confidence that the spacecraft they've built will be able to complete all the mission objectives, without unnecessary waste. 

Similar tools to this one have been made before, so why did I make this as well?

* First of all, many of the tools were isolated from one another. There was a tool for one kind of maneuver, and another tool for another kind, and linking them all together into one mission still took a lot of note taking and manual work.

* Secondly, some maneuvers were too complex and rare to get a tool. Only the most optimal or simple maneuvers would get a helpful tool. Some of the crazier or suboptimal ideas would get left out, despite their use cases.

* Thirdly, as a maths and physics nerd I knew I should have the skills to work it out for myself whether my mission was feasible or not. I didn't want someone else to hold my hand - it was personal.

* Finally, making a complex piece of software like this would help sharpen my software design and programming skills, and might help me get a job.

So this tool should, one day, cover entire missions from start to finish no matter what sequence of complicated maneuvers is involved. 

Currently, the KSP Companion is just barely useful. All the core mechanisms are in place for coming up with a complicated mission around Kerbin (KSP's Earth equivalent), but the way the orbits are presented is still quite unfriendly and probably does require some manual calculation to actually be used with the game. Finding an optimal trajectory is basically impossible as well. There is also little explanation about what some of the technical terms and numbers mean, so some knowledge of orbital mechanics is required. A final limitation is that it is assumed that all maneuvers happen instantaneously, so results may be inaccurate for spacecraft with low thrust engines. Hopefully soon I'll have sorted these issues. 

The KSP Companion is made in Unity, the same game engine as KSP itself. This probably means that the KSP Companion could be easily incorporated into KSP as a mod, but I know very little about how that is done and so it'll remain separate for now.

## Credits:
* Many of the nice UI icons are from RainbowArt's [Clean Flat Icons](https://assetstore.unity.com/packages/2d/gui/icons/clean-flat-icons-98117) asset pack.
* The nice 3D spheres and lines use Freya Holmér's [Shapes](https://assetstore.unity.com/packages/tools/particles-effects/shapes-173167) asset pack.
