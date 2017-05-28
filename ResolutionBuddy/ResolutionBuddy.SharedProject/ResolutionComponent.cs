using Microsoft.Xna.Framework;

namespace ResolutionBuddy
{
	public class ResolutionComponent : DrawableGameComponent, IResolution
	{
		#region Properties

		private ResolutionAdapter ResolutionAdapter { get; set; }

		public Rectangle TitleSafeArea
		{
			get
			{
				return Resolution.TitleSafeArea;
			}
		}

		public Rectangle ScreenArea
		{
			get
			{
				return Resolution.ScreenArea;
			}
		}

		public Matrix ScreenMatrix
		{
			get
			{
				return Resolution.ScreenMatrix;
			}
		}

		private readonly Point _virtualResolution;

		private readonly Point _screenResolution;

		private readonly bool _fullscreen;

		private readonly bool _letterbox;

		private readonly GraphicsDeviceManager _graphics;

		#endregion //Properties

		#region Methods

		/// <summary>
		/// Create the resolution component!
		/// </summary>
		/// <param name="game"></param>
		/// <param name="graphics"></param>
		/// <param name="virtualResolution">The dimensions of the desired virtual resolution</param>
		/// <param name="screenResolution">The desired screen dimensions</param>
		/// <param name="fullscreen">Whether or not to fullscreen the game (Always true on android & ios)</param>
		/// <param name="letterbox">Whether to add letterboxing, or change the virtual resoltuion to match aspect ratio of screen resolution.</param>
		public ResolutionComponent(Game game, GraphicsDeviceManager graphics, Point virtualResolution, Point screenResolution, bool fullscreen, bool letterbox) : base(game)
		{
			_graphics = graphics;
			_virtualResolution = virtualResolution;
			_screenResolution = screenResolution;
			_fullscreen = fullscreen;
			_letterbox = letterbox;

			//Add to the game components
			Game.Components.Add(this);

			//add to the game services
			Game.Services.AddService<IResolution>(this);
		}

		public override void Initialize()
		{
			base.Initialize();

			//initialize the ResolutionAdapter object
			ResolutionAdapter = new ResolutionAdapter(_graphics);
			ResolutionAdapter.SetVirtualResolution(_virtualResolution.X, _virtualResolution.Y);
			ResolutionAdapter.SetScreenResolution(_screenResolution.X, _screenResolution.Y, _fullscreen, _letterbox);
			ResolutionAdapter.ResetViewport();

			//initialize the Resolution singleton
			Resolution.Init(ResolutionAdapter);
		}

		public Vector2 ScreenToGameCoord(Vector2 screenCoord)
		{
			return ResolutionAdapter.ScreenToGameCoord(screenCoord);
		}

		public Matrix TransformationMatrix()
		{
			return ResolutionAdapter.TransformationMatrix();
		}

		public override void Draw(GameTime gameTime)
		{
			//Calculate Proper Viewport according to Aspect Ratio
			ResolutionAdapter.ResetViewport();

			base.Draw(gameTime);
		}

		#endregion //Methods
	}
}
