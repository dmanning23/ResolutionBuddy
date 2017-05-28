ResolutionBuddy
===============

This is a library for managing the resolution of a MonoGame project.

You give it the virtual resolution, which is the target of the game. Then it tries to set up the screen to match your target. It has the option to either letter box and keep the exact desired virtual resolution, otherwise it can change the virtual resolution to match the screen aspect ratio.
Using this library, you don't have to change any code or assets to support multiple resolutions.

Base on code from <a href="http://www.david-amador.com/2010/03/xna-2d-independent-resolution-rendering/">david-amador.com</a>

To use this library, install the Nuget pacakge: <a href="https://www.nuget.org/packages/ResolutionBuddy/">https://www.nuget.org/packages/ResolutionBuddy/</a>

```
public Game1()
{
	graphics = new GraphicsDeviceManager(this);

	//initialize the ResolutionBuddy library. 
	//This will have game space of 720p but on a 3/4 aspect ratio window with letterbox
	IResolution resolution = new ResolutionComponent(this, graphics, new Point(1280, 720), new Point(1024, 768), false, true);

	...
}

protected override void Draw(GameTime gameTime)
{
	graphics.GraphicsDevice.Clear(Color.Black);

	//Pass the TransformationMatrix to the Begin method to draw from game space -> screen space
	spriteBatch.Begin(SpriteSortMode.Immediate, 
					  BlendState.AlphaBlend, 
					  null, null, null, null,
					  Resolution.TransformationMatrix());

	...

	spriteBatch.End();

	base.Draw(gameTime);
}
```

To see an example project, check out <a href="https://github.com/dmanning23/ResolutionBuddyExample">https://github.com/dmanning23/ResolutionBuddyExampleS</a>
