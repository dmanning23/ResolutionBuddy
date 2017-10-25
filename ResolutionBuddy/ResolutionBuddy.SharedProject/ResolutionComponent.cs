using Microsoft.Xna.Framework;
using System;

namespace ResolutionBuddy
{
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
			VirtualResolution = virtualResolution;
			ScreenResolution = screenResolution;
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
			ResolutionAdapter.SetVirtualResolution(VirtualResolution.X, VirtualResolution.Y);
			ResolutionAdapter.SetScreenResolution(ScreenResolution.X, ScreenResolution.Y, _fullscreen, _letterbox);
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

		public void ResetViewport()
		{
			//Calculate Proper Viewport according to Aspect Ratio
			ResolutionAdapter.ResetViewport();
		}

		#endregion //Methods
	}
}
