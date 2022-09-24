# [KSP Companion](https://play.unity.com/mg/other/webgl-builds-148518)

This program is designed to be used alongside the game [Kerbal Space Program](https://en.wikipedia.org/wiki/Kerbal_Space_Program) (KSP) - a space flight simulation game with realistic orbital physics. Personally, when I played the game and used trial-and-error to find successful designs, I felt a bit like I was cheating. I wanted to play the game with the same stakes as a real space agency. You have to know before launch that your rocket is big enough, but you can't just waste resources when it comes to expensive rocketry. My dream was that I should be able to start a mission with full confidence that the spacecraft I'd built will be able to complete all my objectives - without unnecessary expenditure. Some extant tools allowed partial planning of simple cases, or did everything for you in game. However, with my maths/physics background, I knew I should be able to do it all by myself - and so this tool was born. This prototype allows for the planning of a basic mission around the game's home planet Kerbin, and has all the core mechanisms needed for a full mission design tool.

Try out the tool [here](https://play.unity.com/mg/other/webgl-builds-148518).

![](/Other/Images/ToolScreenshot.png)

## For Potential Employers
This project has been significantly larger in scale and complexity compared to my previous coding experiences, and gave me my first tastes of Git source control, data serialisation, and unit testing. From the start of the project to version 1.0.0, I estimate I used 460 A5 notebook pages of dated notes that document my progess and the problems I had to solve along the way. \
Before writing any code at all, I spent time thinking about what the user experience would be, and how the program might be structured. I looked into different software design tools such as UML to help me make sense of what the goals of the program were, and how they could be achieved. This process also inspired me to read "A Philosophy Of Software Design" by Ousterhout for some guidance and a framework to assess the choices I made in the design. Even though I knew I would be the sole developer, I felt that it would be best if I coded this project as though other developers could come along and have to build on top of my work without prior experience in orbital mechanics or my code. 

![](/Other/Images/NotebookUML.jpg)

### The Standouts
I paid special care in the creation of some classes ([_Orbit_](https://github.com/RobertJClose/KSP-Companion-Prototype/blob/921940345136123d4c88bd0b5804d151334a8ea3/Assets/KSP%20Companion/Scripts/Orbital%20Stuff/Orbit.cs), 
[_Angled_](https://github.com/RobertJClose/KSP-Companion-Prototype/blob/921940345136123d4c88bd0b5804d151334a8ea3/Assets/KSP%20Companion/Scripts/Utilities/Angled.cs)) 
that I knew would have massive importance to the program's operation and could find reuse after the project was complete. I did my best to build these classes with a deliberate mindset that put to use good software design principles. I ensured that these classes were [well tested](https://github.com/RobertJClose/KSP-Companion-Prototype/blob/921940345136123d4c88bd0b5804d151334a8ea3/Assets/Tests/Editor%20Tests/Orbit%20Tests/OrbitTests.cs), 
well documented, made with clear and descriptive identifiers, and built according to the [StyleCop](https://en.wikipedia.org/wiki/StyleCop) guidance on C# code files. \
I would still make a few minor changes to the design of the [_Orbit_](https://github.com/RobertJClose/KSP-Companion-Prototype/blob/921940345136123d4c88bd0b5804d151334a8ea3/Assets/KSP%20Companion/Scripts/Orbital%20Stuff/Orbit.cs)
class before reusing it or committing it into a library, but overall I was happy with the design and found the classes very pleasant to use.

![](/Other/Images/NotebookHandwrittenTestIterations.png)
![](/Other/Images/TestsScreenshot.png)

### Things I'd Do Differently
Ideally more classes in the program would have received a similar level of attention as those above, but I was cautious to apply such focus to parts of the program whose design I was less confident in and knew could evolve dramatically over the lifetime of the project. Furthermore, I thought it best for my learning to practise applying good design principles to a small and manageable area, to start a good habit to take forward to later projects. \
With that said, one key area that needs a re-do is the important [_MissionTimeline_](https://github.com/RobertJClose/KSP-Companion-Prototype/blob/751a3b4429945ba52f43e0c434f600244a9f105e/Assets/KSP%20Companion/Scripts/UI%20Scripts/Timeline%20Scripts/MissionTimeline.cs) class and the related [_TimelineStep_](https://github.com/RobertJClose/KSP-Companion-Prototype/blob/751a3b4429945ba52f43e0c434f600244a9f105e/Assets/KSP%20Companion/Scripts/UI%20Scripts/Timeline%20Scripts/TimelineStep.cs) class hierarchy. There are several issues, but the crux is that _MissionTimeline_ is an uncomfortable mix of many different abstractions and is not very SOLID. This happened as the needs of the evolving project were sometimes met with ad-hoc solutions. As _MissionTimeline_ contains the all important list of _TimelineSteps_ that **_are_** the mission plan, that central role drew a lot of those ad-hoc solutions into it. I was aware of the growing issues but decided to tolerate them for this prototype. 

### Conclusion
The completion of this project demonstrated through experience some software engineering principles that I have read about in books and articles around the web. 
I faced quite a few tricky grinds througout this project, but I also discovered the joy of proper testing and good commenting - these are things I found to be a bit of a chore at first, but by the end I loved the confidence that they provided. \
Finally, this project was also an opportunity to practise skills beyond just coding. I know how important things like high-level software design and source control are to modern game studios, and I actually really enjoyed the time spent learning about things like software design or how to create good Git commits, branches, and issues - even if I was the only person working on this project. 

I'd be happy to discuss this project in more detail, and I'm excited to further develop my software development skills in a professional collaborative environment.

The tool can be found [here](https://play.unity.com/mg/other/webgl-builds-148518).

## Credits:
* Most of the orbital mechanics knowledge was learned from Prussing and Conway's Orbital Mechanics textbook, and from [this](http://braeunig.us/space/index_top.htm) website.
* I am deeply indebted to [this](https://www.esa.int/gsp/ACT/doc/MAD/pub/ACT-RPR-MAD-2014-RevisitingLambertProblem.pdf) paper by Izzo for an algorithm for solving [Lambert's problem](https://en.wikipedia.org/wiki/Lambert's_problem).
* Many of the nice UI icons are from RainbowArt's [Clean Flat Icons](https://assetstore.unity.com/packages/2d/gui/icons/clean-flat-icons-98117) asset pack.
* The nice 3D spheres and lines use Freya Holmér's [Shapes](https://assetstore.unity.com/packages/tools/particles-effects/shapes-173167) asset pack.
* Some of the double-precision versions of Unity's maths libraries came from [this](https://github.com/sldsmkd/vector3d) repo and [this](https://github.com/Darkziyu/Mathd) repo.
