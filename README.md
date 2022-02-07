# KSP Companion

This program is designed to be used alongside the game Kerbal Space Program (KSP). Personally, when I played the game and used trial-and-error to find successful designs, I felt a bit like I was cheating. I wanted to play the game with the same stakes as a real space agency. You have to know before launch that your rocket is big enough, but you can't just waste resources when it comes to expensive rocketry. The goal is that it should be possible for a player to launch a mission with full confidence that the spacecraft they've built will be able to complete all the mission objectives - without unnecessary waste. This first prototype allows for the planning of a basic mission around the game's home planet Kerbin, and has all the core mechanisms needed for a final mission design tool.

![](/Other/Images/ToolScreenshot.png)

## For Employers
This project has been significantly larger in scale and complexity compared to my previous coding experiences; I estimate I used 460 A5 notebook pages from the start to v1.0.0. An outline of some of the features of this tool is:
* Data serialisation for saving and loading
* 

This project involved many firsts for me, and has taught me several valuable lessons in software design and engineering. Before writing any code at all I had to spend time thinking about what the user experience would be, and so how the program would be structured. I looked into different software design tools such as UML to help me make sense of what the goals of the program were, and how they could be achieved. \
This process also inspired me to read "A Philosophy Of Software Design" by Ousterhout for some guidance and a framework to assess the choices I made in the design. Even though I knew I would be the sole developer, I felt that it would be best if I coded this project as though other developers could come along and have to build on top of my work without prior experience in orbital mechanics or my code. 

### The Highlights
As I worked on early designs of the tool it became clear that some classes would be more important to the program's operation than others, and so I paid special attention to ensure I built those classes with a deliberate mindset. \
For example, the Orbit class was built to represent an object in an orbit around a gravitational body. Knowing how much time would be spent working with this class, and that it represented something quite challenging and complex, I put a lot of effort into making this class well tested, well documented, and made with clear and descriptive identifiers throughout. The goal was to minimise the cognitive-load needed for a new developer to use the class, while still providing an easy to use interface for external modules to use. \
Ideally many more classes in the program would have received a similar level of attention, but I was cautious to apply such focus to aspects I knew were likely to evolve dramatically, and I thought it best for my learning to practise applying good design principles to a small and manageable area, with the intention of starting a good habit to take forward to later projects.

### Things I'd Do Differently
An aspect of the program that I think really needs a re-do is the MissionTimeline class and the related TimelineStep class hierarchy. 
The MissionTimeline class contains a list of TimelineStep objects that represent the steps needed to achieve the mission. This class is important, but it ended up with several issues. For brevity, the fundamental problem is that the MissionTimeline class represents several different abstractions - it's a mess of UI listening and various data management tasks all at once. It evolved this way due to needs of the program being met with quick ad-hoc solutions at times, and MissionTimeline's central role drew those solutions towards it. I was aware of the growing problem and decided it was tolerable for the prototype, but it is something I'd want to sort out properly in any future versions. \
Despite the issues, I'm glad for the experience of having to work with this class and its problems for the sake of understanding the motivation for often repeated programming principles such as SOLID.

### The Main Lessons Learned
The completion of this project demonstrated through experience some software engineering principles that I have read about in books and articles around the web. For example, I found that a small misalignment between the interface of a class and the abstraction the class is meant to represent could lead to unnecessarily complex problems arising in other modules when they try to use the class. \
I discovered the joy of proper testing during this project - this is something I found a bit of a chore when I first explored it but by the end I loved the confidence that my testing provided. \
I could make similar remarks on the writing of comments - by the end I enjoyed the experience of knowing my work in documenting effectively would pay off later when working with the code again later.

I'd be happy to discuss this project in more detail, and I'm excited to further develop my software development skills in a professional collaborative environment.

## Credits:
* Most of the orbital mechanics knowledge was learned from Prussing and Conway's Orbital Mechanics textbook, and from [this](http://braeunig.us/space/index_top.htm) website.
* I am deeply indebted to [this](https://www.esa.int/gsp/ACT/doc/MAD/pub/ACT-RPR-MAD-2014-RevisitingLambertProblem.pdf) paper by Izzo for an algorithm for solving [Lambert's problem](https://en.wikipedia.org/wiki/Lambert's_problem).
* Many of the nice UI icons are from RainbowArt's [Clean Flat Icons](https://assetstore.unity.com/packages/2d/gui/icons/clean-flat-icons-98117) asset pack.
* The nice 3D spheres and lines use Freya Holmér's [Shapes](https://assetstore.unity.com/packages/tools/particles-effects/shapes-173167) asset pack.
* Some of the double-precision versions of Unity's maths libraries came from [this](https://github.com/sldsmkd/vector3d) repo and [this](https://github.com/Darkziyu/Mathd) repo.
