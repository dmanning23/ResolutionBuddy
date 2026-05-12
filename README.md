ResolutionBuddy
===============

A MonoGame library for managing resolution-independent rendering. Define a virtual resolution as your game's target coordinate space, and ResolutionBuddy handles scaling to any screen size — with optional letterboxing or aspect-ratio-matching.

Install via NuGet: [ResolutionBuddy](https://www.nuget.org/packages/ResolutionBuddy/)

## Setup

Create a `ResolutionComponent` in your `Game` constructor:

```csharp
public Game1()
{
    graphics = new GraphicsDeviceManager(this);

    // Virtual resolution: 720x1280 (your game's coordinate space)
    // Screen resolution: 720x1280 (desired window size)
    // fullscreen: false
    // letterbox: false (stretch virtual resolution to match screen aspect ratio)
    // useDeviceResolution: false (use the specified screen resolution, not the device's)
    _resolution = new ResolutionComponent(this, graphics,
        new Point(720, 1280),
        new Point(720, 1280),
        fullscreen: false,
        letterbox: false,
        useDeviceResolution: false);

    Content.RootDirectory = "Content";
}
```

## Drawing

Pass `Resolution.TransformationMatrix()` to `SpriteBatch.Begin` to map game coordinates to screen coordinates:

```csharp
protected override void Draw(GameTime gameTime)
{
    graphics.GraphicsDevice.Clear(Color.Black);

    spriteBatch.Begin(SpriteSortMode.Immediate,
                      BlendState.AlphaBlend,
                      null, null, null, null,
                      Resolution.TransformationMatrix());

    // Draw using game-space coordinates
    spriteBatch.Draw(_texture, Vector2.Zero, Color.White);

    spriteBatch.End();

    base.Draw(gameTime);
}
```

## Input Handling

To convert mouse/touch screen coordinates into game coordinates:

```csharp
Vector2 gameCoord = Resolution.ScreenToGameCoord(new Vector2(mouseX, mouseY));
```

Or use `_resolution.ScreenToGameCoord()` via the `IResolution` interface.

## API

### `ResolutionComponent` constructor

| Parameter | Type | Description |
|---|---|---|
| `game` | `Game` | The game instance |
| `graphics` | `GraphicsDeviceManager` | The graphics device manager |
| `virtualResolution` | `Point` | Your game's coordinate space dimensions |
| `screenResolution` | `Point` | The desired window/screen dimensions |
| `fullscreen` | `bool` | Whether to run fullscreen |
| `letterbox` | `bool` | `true` = add letterbox bars to preserve virtual resolution; `false` = stretch virtual resolution to match screen aspect ratio |
| `useDeviceResolution` | `bool?` | `true` = ignore `screenResolution` and use the device's native resolution |

### `Resolution` static singleton

| Member | Description |
|---|---|
| `TransformationMatrix()` | Matrix for `SpriteBatch.Begin` — maps game coords to screen coords |
| `ScreenToGameCoord(Vector2)` | Converts screen coordinates to game coordinates |
| `TitleSafeArea` | Title-safe rectangle in game coordinates |
| `ScreenArea` | Full screen rectangle in game coordinates |
| `ScreenMatrix` | Matrix to convert screen coords to game coords |

## Example Project

See [ResolutionBuddyExample](ResolutionBuddy/ResolutionBuddyExample) in this repository for a working demo.
