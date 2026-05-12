using Microsoft.Xna.Framework;
using System;

namespace ResolutionBuddy
{
    /// <summary>
    /// A <see cref="DrawableGameComponent"/> that manages resolution-independent rendering.
    /// Add this to your game in the constructor and it will automatically scale all drawing
    /// from the virtual coordinate space to the physical screen resolution. Also registers
    /// itself as an <see cref="IResolution"/> service on <see cref="Game.Services"/>.
    /// </summary>
    public class ResolutionComponent : DrawableGameComponent, IResolution
    {
        #region Fields

        private Point _virtualResolution;

        private Point _screenResolution;

        private bool _fullscreen;

        private bool _letterbox;

        private readonly GraphicsDeviceManager _graphics;

        #endregion //Fields

        #region Properties

        private ResolutionAdapter ResolutionAdapter { get; set; }

        /// <summary>
        /// The title-safe rectangle in virtual (game) coordinates. Inset by 5% on each side.
        /// </summary>
        public Rectangle TitleSafeArea
        {
            get
            {
                return Resolution.TitleSafeArea;
            }
        }

        /// <summary>
        /// The full-screen rectangle in virtual (game) coordinates.
        /// </summary>
        public Rectangle ScreenArea
        {
            get
            {
                return Resolution.ScreenArea;
            }
        }

        /// <summary>
        /// Matrix to convert screen coordinates to game coordinates. Use for mapping mouse clicks and touch events.
        /// </summary>
        public Matrix ScreenMatrix
        {
            get
            {
                return Resolution.ScreenMatrix;
            }
        }

        /// <summary>
        /// The virtual resolution that defines the game's coordinate space.
        /// Throws <see cref="Exception"/> if set after <see cref="Initialize"/> has been called.
        /// </summary>
        public Point VirtualResolution
        {
            get
            {
                return _virtualResolution;
            }
            set
            {
                if (null != ResolutionAdapter)
                {
                    throw new Exception("Can't change VirtualResolution after the ResolutionComponent has been initialized");
                }
                _virtualResolution = value;
            }
        }

        /// <summary>
        /// The desired physical screen or window resolution.
        /// Throws <see cref="Exception"/> if set after <see cref="Initialize"/> has been called.
        /// </summary>
        public Point ScreenResolution
        {
            get
            {
                return _screenResolution;
            }
            set
            {
                if (null != ResolutionAdapter)
                {
                    throw new Exception("Can't change ScreenResolution after the ResolutionComponent has been initialized");
                }
                _screenResolution = value;
            }
        }

        /// <summary>
        /// Whether the game runs in fullscreen mode.
        /// Throws <see cref="Exception"/> if set after <see cref="Initialize"/> has been called.
        /// </summary>
        public bool FullScreen
        {
            get
            {
                return _fullscreen;
            }
            set
            {
                if (null != ResolutionAdapter)
                {
                    throw new Exception("Can't change FullScreen after the ResolutionComponent has been initialized");
                }
                _fullscreen = value;
            }
        }

        bool? _useDeviceResolution;

        /// <summary>
        /// When <c>true</c>, ignores <see cref="ScreenResolution"/> and uses the device's native resolution instead.
        /// When <c>null</c>, falls back to the value of <see cref="FullScreen"/>.
        /// Throws <see cref="Exception"/> if set after <see cref="Initialize"/> has been called.
        /// </summary>
        public bool? UseDeviceResolution
        {
            get
            {
                return _useDeviceResolution;
            }
            set
            {
                if (null != ResolutionAdapter)
                {
                    throw new Exception("Can't change UseDeviceResolution after the ResolutionComponent has been initialized");
                }
                _useDeviceResolution = value;
            }
        }

        /// <summary>
        /// When <c>true</c>, black bars are added to preserve the virtual resolution's exact aspect ratio.
        /// When <c>false</c>, the virtual resolution is adjusted to match the screen's aspect ratio.
        /// Throws <see cref="Exception"/> if set after <see cref="Initialize"/> has been called.
        /// </summary>
        public bool LetterBox
        {
            get
            {
                return _letterbox;
            }
            set
            {
                if (null != ResolutionAdapter)
                {
                    throw new Exception("Can't change LetterBox after the ResolutionComponent has been initialized");
                }
                _letterbox = value;
            }
        }

        #endregion //Properties

        #region Methods

        /// <summary>
        /// Creates the resolution component and registers it as a game component and service.
        /// All properties must be configured before <see cref="Initialize"/> is called.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="graphics">The graphics device manager.</param>
        /// <param name="virtualResolution">The dimensions of the virtual (game) coordinate space.</param>
        /// <param name="screenResolution">The desired physical screen or window dimensions.</param>
        /// <param name="fullscreen">Whether to run the game in fullscreen mode.</param>
        /// <param name="letterbox">
        /// <c>true</c> to preserve the virtual aspect ratio with black bars;
        /// <c>false</c> to adjust the virtual resolution to match the screen aspect ratio.
        /// </param>
        /// <param name="useDeviceResolution">
        /// <c>true</c> to use the device's native resolution, ignoring <paramref name="screenResolution"/>;
        /// <c>null</c> to fall back to the value of <paramref name="fullscreen"/>.
        /// </param>
        public ResolutionComponent(Game game, GraphicsDeviceManager graphics, Point virtualResolution, Point screenResolution, bool fullscreen, bool letterbox, bool? useDeviceResolution) : base(game)
        {
            _graphics = graphics;
            VirtualResolution = virtualResolution;
            ScreenResolution = screenResolution;
            _fullscreen = fullscreen;
            _letterbox = letterbox;
            _useDeviceResolution = useDeviceResolution;

            //Add to the game components
            Game.Components.Add(this);

            //add to the game services
            Game.Services.AddService<IResolution>(this);
        }

        /// <summary>
        /// Initializes the resolution adapter and applies the screen resolution settings.
        /// Also initializes the <see cref="Resolution"/> singleton for global access.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            //initialize the ResolutionAdapter object
            ResolutionAdapter = new ResolutionAdapter(_graphics);
            ResolutionAdapter.SetVirtualResolution(VirtualResolution.X, VirtualResolution.Y);
            ResolutionAdapter.SetScreenResolution(ScreenResolution.X, ScreenResolution.Y, _fullscreen, _letterbox, _useDeviceResolution);
            ResolutionAdapter.ResetViewport();

            //initialize the Resolution singleton
            Resolution.Init(ResolutionAdapter);
        }

        /// <summary>
        /// Converts a screen-space coordinate (e.g. from mouse or touch input) to the
        /// corresponding virtual (game) coordinate.
        /// </summary>
        /// <param name="screenCoord">The position in screen pixels.</param>
        /// <returns>The equivalent position in game/virtual coordinates.</returns>
        public Vector2 ScreenToGameCoord(Vector2 screenCoord)
        {
            return ResolutionAdapter.ScreenToGameCoord(screenCoord);
        }

        /// <summary>
        /// Returns the matrix that maps game (virtual) coordinates to screen coordinates.
        /// Pass this into <see cref="Microsoft.Xna.Framework.Graphics.SpriteBatch.Begin"/> to render in game space.
        /// </summary>
        /// <returns>The scale/translation matrix from virtual to screen space.</returns>
        public Matrix TransformationMatrix()
        {
            return ResolutionAdapter.TransformationMatrix();
        }

        /// <summary>
        /// Resets the viewport each frame to maintain the correct aspect ratio.
        /// </summary>
        /// <param name="gameTime">Snapshot of game timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            //Calculate Proper Viewport according to Aspect Ratio
            ResolutionAdapter.ResetViewport();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Recalculates and applies the viewport to maintain the correct aspect ratio,
        /// adding letterbox or pillarbox bars as needed. Call this each frame before drawing.
        /// </summary>
        public void ResetViewport()
        {
            //Calculate Proper Viewport according to Aspect Ratio
            ResolutionAdapter.ResetViewport();
        }

        #endregion //Methods
    }
}
