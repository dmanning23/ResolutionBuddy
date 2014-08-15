using MatrixExtensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ResolutionBuddy
{
	/// <summary>
	/// This is a class for managing the resolution of a game.
	/// You give it the virtual resolution, which is the target of the game
	/// Then it tries to set up the screen to match your target, it will letterbox or whatever if it has to
	/// This way you don't have to change any code or assets to support multiple resolutions.
	/// </summary>
	public static class Resolution
	{
		#region Members

		/// <summary>
		/// The title safe area for our virtual resolution
		/// </summary>
		/// <value>The title safe area.</value>
		private static Rectangle _titleSafeArea;

		/// <summary>
		/// This will be a rectangle of the whole screen in our "virtual resolution"
		/// </summary>
		private static Rectangle _screenArea;

		/// <summary>
		/// The actual screen resolution
		/// </summary>
		private static Point _screenRect = new Point(1280, 720);

		/// <summary>
		/// The screen rect we want for our game, and are going to fake
		/// </summary>
		public static Point VirtualRect { get; set; }

		/// <summary>
		/// The scale matrix from the desired rect to the screen rect
		/// </summary>
		private static Matrix _scaleMatrix;

		/// <summary>
		/// Scale matrix used to convert screen coords (mouse click, touch events) to game coords.
		/// </summary>
		private static Matrix _screenMatrix;

		/// <summary>
		/// whether or not we want full screen 
		/// </summary>
		private static bool _fullScreen;

		/// <summary>
		/// whether or not the matrix needs to be recreated
		/// </summary>
		private static bool _dirtyMatrix = true;

		/// <summary>
		/// The graphics device
		/// </summary>
		/// <value>The device.</value>
		public static GraphicsDeviceManager Device { get; private set; }

		#endregion //Members

		#region Properties

		public static Rectangle TitleSafeArea
		{
			get { return _titleSafeArea; }
		}

		public static Rectangle ScreenArea
		{
			get { return _screenArea; }
		}

		public static Matrix ScreenMatrix
		{
			get
			{
				return _screenMatrix;
			}
		}

		#endregion //Properties

		#region Methods

		static Resolution()
		{
			VirtualRect = new Point(1280, 720);
		}

		/// <summary>
		/// Init the specified device.
		/// </summary>
		/// <param name="deviceMananger">Device.</param>
		public static void Init(ref GraphicsDeviceManager deviceMananger)
		{
			Device = deviceMananger;
			_screenRect.X = Device.PreferredBackBufferWidth;
			_screenRect.Y = Device.PreferredBackBufferHeight;
		}

		/// <summary>
		/// Get the transformation matrix for when you call SpriteBatch.Begin
		/// To add this to a camera matrix, do CameraMatrix * TransformationMatrix
		/// </summary>
		/// <returns>The matrix.</returns>
		public static Matrix TransformationMatrix()
		{
			if (_dirtyMatrix)
			{
				RecreateScaleMatrix(new Point(
					Device.GraphicsDevice.Viewport.Width,
					Device.GraphicsDevice.Viewport.Height));
			}

			return _scaleMatrix;
		}

		/// <summary>
		/// Given a screen coord, convert to game coordinate system.
		/// </summary>
		/// <param name="screenCoord"></param>
		/// <returns></returns>
		public static Vector2 ScreenToGameCoord(Vector2 screenCoord)
		{
			return MatrixExt.Multiply(_screenMatrix, screenCoord);
		}

		/// <summary>
		/// Sets the screen we are going to use for the screen
		/// </summary>
		/// <param name="Width">Width.</param>
		/// <param name="Height">Height.</param>
		/// <param name="FullScreen">If set to <c>true</c> full screen.</param>
		public static void SetScreenResolution(int Width, int Height, bool FullScreen)
		{
			_screenRect.X = Width;
			_screenRect.Y = Height;

#if ANDROID || OUYA
	//Android is always fullscreen
			_fullScreen = true;
#else
			_fullScreen = FullScreen;
#endif

			ApplyResolutionSettings();
		}

		/// <summary>
		/// The the resolution our game is designed to run in.
		/// </summary>
		/// <param name="Width">Width.</param>
		/// <param name="Height">Height.</param>
		public static void SetDesiredResolution(int Width, int Height)
		{
			VirtualRect = new Point(Width, Height);

			_screenArea = new Rectangle(0, 0, VirtualRect.X, VirtualRect.Y);

			//set up the title safe area
			_titleSafeArea.X = (int)(VirtualRect.X / 20.0f);
			_titleSafeArea.Y = (int)(VirtualRect.Y / 20.0f);
			_titleSafeArea.Width = (int)(VirtualRect.X - (2.0f * TitleSafeArea.X));
			_titleSafeArea.Height = (int)(VirtualRect.Y - (2.0f * TitleSafeArea.Y));

			_dirtyMatrix = true;
		}

		private static void ApplyResolutionSettings()
		{
			// If we aren't using a full screen mode, the height and width of the window can
			// be set to anything equal to or smaller than the actual screen size.
			if (!_fullScreen)
			{
				//Make sure the width isn't bigger than the screen
				if (_screenRect.X > GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width)
				{
					_screenRect.X = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
				}

				//Make sure the height isn't bigger than the screen
				if (_screenRect.Y > GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height)
				{
					_screenRect.Y = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
				}
			}
			else
			{
				// If we are using full screen mode, we should check to make sure that the display
				// adapter can handle the video mode we are trying to set.  To do this, we will
				// iterate through the display modes supported by the adapter and check them against
				// the mode we want to set.
				bool bFound = false;
				foreach (DisplayMode dm in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
				{
					// Check the width and height of each mode against the passed values
					if ((dm.Width == _screenRect.X) && (dm.Height == _screenRect.Y))
					{
						// The mode is supported, so set the buffer formats, apply changes and return
						bFound = true;
					}
				}

				if (!bFound)
				{
					_screenRect.X = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
					_screenRect.Y = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
				}
			}

			//don't screw around with portrait mode
			if (_screenRect.Y > _screenRect.X)
			{
				int height = _screenRect.Y;
				_screenRect.Y = _screenRect.X;
				_screenRect.X = height;
			}

			//ok, found a good set of stuff... set the graphics device
			Device.PreferredBackBufferWidth = _screenRect.X;
			Device.PreferredBackBufferHeight = _screenRect.Y;
			Device.IsFullScreen = _fullScreen;
			Device.ApplyChanges();

			//we are gonna have to redo that scale matrix
			_dirtyMatrix = true;
		}

		public static void RecreateScaleMatrix(Point vp)
		{
			_dirtyMatrix = false;
			_scaleMatrix = Matrix.CreateScale(
				((float)vp.X / (float)VirtualRect.X),
				((float)vp.Y / (float)VirtualRect.Y),
				1.0f);

			_screenMatrix = Matrix.CreateScale(
				((float)VirtualRect.X / (float)vp.X),
				((float)VirtualRect.Y / (float)vp.Y),
				1.0f);
		}

		/// <summary>
		/// Get virtual Mode Aspect Ratio
		/// </summary>
		/// <returns>aspect ratio</returns>
		private static float getVirtualAspectRatio()
		{
			return VirtualRect.X / (float)VirtualRect.Y;
		}

		public static void ResetViewport()
		{
			float targetAspectRatio = getVirtualAspectRatio();

			// figure out the largest area that fits in this resolution at the desired aspect ratio
			int width = Device.PreferredBackBufferWidth;
			var height = (int)(width / targetAspectRatio + .5f);
			bool changed = false;

			if (height > Device.PreferredBackBufferHeight)
			{
				// PillarBox
				height = Device.PreferredBackBufferHeight;
				width = (int)(height * targetAspectRatio + .5f);
				changed = true;
			}

			// set up the new viewport centered in the backbuffer
			var viewport = new Viewport()
			{
				X = (Device.PreferredBackBufferWidth / 2) - (width / 2),
				Y = (Device.PreferredBackBufferHeight / 2) - (height / 2),
				Width = width,
				Height = height,
				MinDepth = 0,
				MaxDepth = 1
			};

			if (changed)
			{
				_dirtyMatrix = true;
			}

			Device.GraphicsDevice.Viewport = viewport;
		}

		#endregion //Methods
	}
}