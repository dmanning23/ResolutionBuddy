using System;
using MatrixExtensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ResolutionBuddy
{
	/// <summary>
	/// Internal implementation of <see cref="IResolution"/> that handles the actual scaling
	/// math, viewport management, and aspect-ratio correction. Consumed by
	/// <see cref="ResolutionComponent"/> and exposed through the <see cref="Resolution"/> singleton.
	/// </summary>
	internal class ResolutionAdapter : IResolution
	{
		#region Fields

		/// <summary>
		/// The title safe area for our virtual resolution
		/// </summary>
		/// <value>The title safe area.</value>
		private Rectangle _titleSafeArea;

		/// <summary>
		/// This will be a rectangle of the whole screen in our "virtual resolution"
		/// </summary>
		private Rectangle _screenArea;

		/// <summary>
		/// The actual screen resolution
		/// </summary>
		private Point _screenResolution = new Point(1280, 720);

		/// <summary>
		/// The screen rect we want for our game, and are going to fake
		/// </summary>
		private Point _virtualResolution = new Point(1280, 720);

		/// <summary>
		/// The scale matrix from the desired rect to the screen rect
		/// </summary>
		private Matrix _scaleMatrix;

		/// <summary>
		/// Scale matrix used to convert screen coords (mouse click, touch events) to game coords.
		/// </summary>
		private Matrix _screenMatrix;

		/// <summary>
		/// whether or not we want full screen 
		/// </summary>
		private bool _fullScreen;

		/// <summary>
		/// When true, uses the device's native resolution instead of <see cref="_screenResolution"/>.
		/// </summary>
		private bool _useDeviceResolution;

		/// <summary>
		/// Whether the scale matrix needs to be recalculated before the next draw.
		/// </summary>
		private bool _dirtyMatrix = true;

		/// <summary>
		/// When true, black bars are added to preserve the virtual aspect ratio.
		/// When false, the virtual resolution is adjusted to match the screen aspect ratio.
		/// </summary>
		private bool _letterBox;

		/// <summary>
		/// Offset applied to the screen matrix to account for letterbox/pillarbox bars.
		/// </summary>
		private Vector2 _pillarBox;

		/// <summary>
		/// The graphics device
		/// </summary>
		/// <value>The device.</value>
		private GraphicsDeviceManager Device { get; set; }

		#endregion //Fields

		#region Properties

		/// <summary>
		/// The title-safe rectangle in virtual (game) coordinates. Inset by 5% on each side.
		/// </summary>
		public Rectangle TitleSafeArea
		{
			get { return _titleSafeArea; }
		}

		/// <summary>
		/// The full-screen rectangle in virtual (game) coordinates.
		/// </summary>
		public Rectangle ScreenArea
		{
			get { return _screenArea; }
		}

		/// <summary>
		/// Matrix to convert screen coordinates to game coordinates. Use for mapping mouse clicks and touch events.
		/// </summary>
		public Matrix ScreenMatrix
		{
			get
			{
				return _screenMatrix;
			}
		}

		/// <summary>
		/// The aspect ratio of the virtual (game) resolution (width / height).
		/// </summary>
		private float VirtualAspectRatio
		{
			get
			{
				return _virtualResolution.X / (float)_virtualResolution.Y;
			}
		}

		/// <summary>
		/// The aspect ratio of the physical screen resolution (width / height).
		/// </summary>
		private float ScreenAspectRatio
		{
			get
			{
				return _screenResolution.X / (float)_screenResolution.Y;
			}
		}

		/// <summary>
		/// The virtual resolution that defines the game's coordinate space.
		/// Setting this recalculates the title-safe area and marks the scale matrix dirty.
		/// </summary>
		public Point VirtualResolution
		{
			get
			{
				return _virtualResolution;
			}
			set
			{
				SetVirtualResolution(value.X, value.Y);
			}
		}

		/// <summary>
		/// The desired physical screen or window resolution.
		/// Setting this re-applies all resolution settings via <see cref="SetScreenResolution"/>.
		/// </summary>
		public Point ScreenResolution
		{
			get
			{
				return _screenResolution;
			}
			set
			{
				SetScreenResolution(value.X, value.Y, _fullScreen, _letterBox, _useDeviceResolution);
			}
		}

		#endregion //Properties

		#region Methods

		#region Initialization

		/// <summary>
		/// default constructor for testing
		/// </summary>
		public ResolutionAdapter()
		{
		}

		/// <summary>
		/// Init the specified device.
		/// </summary>
		/// <param name="deviceMananger">Device.</param>
		public ResolutionAdapter(GraphicsDeviceManager deviceMananger)
		{
			Device = deviceMananger;
			_screenResolution.X = Device.PreferredBackBufferWidth;
			_screenResolution.Y = Device.PreferredBackBufferHeight;
		}

		/// <summary>
		/// The the resolution our game is designed to run in.
		/// </summary>
		/// <param name="Width">Width.</param>
		/// <param name="Height">Height.</param>
		public void SetVirtualResolution(int Width, int Height)
		{
			_virtualResolution = new Point(Width, Height);

			_screenArea = new Rectangle(0, 0, _virtualResolution.X, _virtualResolution.Y);

			//set up the title safe area
			_titleSafeArea.X = (int)(_virtualResolution.X / 20.0f);
			_titleSafeArea.Y = (int)(_virtualResolution.Y / 20.0f);
			_titleSafeArea.Width = (int)(_virtualResolution.X - (2.0f * TitleSafeArea.X));
			_titleSafeArea.Height = (int)(_virtualResolution.Y - (2.0f * TitleSafeArea.Y));

			_dirtyMatrix = true;
		}

		/// <summary>
		/// Applies the screen resolution settings and configures the graphics device.
		/// </summary>
		/// <param name="width">The desired back buffer width in pixels.</param>
		/// <param name="height">The desired back buffer height in pixels.</param>
		/// <param name="fullScreen">Whether to run in fullscreen mode.</param>
		/// <param name="letterbox">
		/// <c>true</c> to preserve the virtual aspect ratio with black bars;
		/// <c>false</c> to adjust the virtual resolution to match the screen aspect ratio.
		/// </param>
		/// <param name="useDeviceResolution">
		/// <c>true</c> to use the device's native resolution, ignoring <paramref name="width"/> and <paramref name="height"/>;
		/// <c>null</c> to fall back to the value of <paramref name="fullScreen"/>.
		/// </param>
		public void SetScreenResolution(int width, int height, bool fullScreen, bool letterbox, bool? useDeviceResolution)
		{
			_screenResolution.X = width;
			_screenResolution.Y = height;
			_letterBox = letterbox;
			_useDeviceResolution = useDeviceResolution.HasValue ? useDeviceResolution.Value : fullScreen;

			_fullScreen = fullScreen;

			ApplyResolutionSettings();
		}

		/// <summary>
		/// Validates the screen resolution against available display modes, clamps it to the
		/// display bounds if needed, then applies the settings to the graphics device.
		/// If <see cref="_letterBox"/> is false, also adjusts the virtual resolution to match
		/// the screen aspect ratio.
		/// </summary>
		protected virtual void ApplyResolutionSettings()
		{
			// If we aren't using a full screen mode, the height and width of the window can
			// be set to anything equal to or smaller than the actual screen size.
			if (!_fullScreen && !_useDeviceResolution)
			{
				//Make sure the width isn't bigger than the screen
				if (_screenResolution.X > GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width)
				{
					_screenResolution.X = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
				}

				//Make sure the height isn't bigger than the screen
				if (_screenResolution.Y > GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height)
				{
					_screenResolution.Y = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
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
					if ((dm.Width == _screenResolution.X) && (dm.Height == _screenResolution.Y))
					{
						// The mode is supported, so set the buffer formats, apply changes and return
						bFound = true;
						break;
					}
				}

				if (!bFound)
				{
					_screenResolution.X = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
					_screenResolution.Y = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
				}
			}

			//ok, found a good set of stuff... set the graphics device
			Device.PreferredBackBufferWidth = _screenResolution.X;
			Device.PreferredBackBufferHeight = _screenResolution.Y;
			Device.IsFullScreen = _fullScreen;
			Device.ApplyChanges();

			//Update the virtual resolution to match the aspect ratio of the new actual resolution
			if (!_letterBox)
			{
				UpdateVirtualResolution();
			}

			//we are gonna have to redo that scale matrix
			_dirtyMatrix = true;
		}

		/// <summary>
		/// Adjusts the virtual resolution so its aspect ratio matches the current screen aspect ratio,
		/// keeping whichever axis fits and pulling in the other to match.
		/// Called when <see cref="_letterBox"/> is false.
		/// </summary>
		protected void UpdateVirtualResolution()
		{
			if (ScreenAspectRatio < VirtualAspectRatio)
			{
				//the width needs to be pulled in to match the screen aspect ratio
				var width = ((_screenResolution.X * _virtualResolution.Y) / _screenResolution.Y);
				SetVirtualResolution(width, _virtualResolution.Y);
			}
			else if (ScreenAspectRatio > VirtualAspectRatio)
			{
				//the height needs to be pulled in to match the screen aspect ratio
				var height = ((_virtualResolution.X * _screenResolution.Y) / _screenResolution.X);
				SetVirtualResolution(_virtualResolution.X, height);
			}
		}

		#endregion Initialization

		/// <summary>
		/// Get the transformation matrix for when you call SpriteBatch.Begin
		/// To add this to a camera matrix, do CameraMatrix * TransformationMatrix
		/// </summary>
		/// <returns>The matrix.</returns>
		public Matrix TransformationMatrix()
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
		public Vector2 ScreenToGameCoord(Vector2 screenCoord)
		{
			return MatrixExt.Multiply(_screenMatrix, screenCoord);
		}

		/// <summary>
		/// Rebuilds the scale and screen matrices based on the current viewport dimensions.
		/// </summary>
		/// <param name="vp">The current viewport width and height in pixels.</param>
		protected virtual void RecreateScaleMatrix(Point vp)
		{
			_dirtyMatrix = false;
			_scaleMatrix = Matrix.CreateScale(
				((float)vp.X / (float)_virtualResolution.X),
				((float)vp.Y / (float)_virtualResolution.Y),
				1.0f);

			//trasnlate by the pillar box to account for the border on top/bottom/left/right
			var translation = Matrix.CreateTranslation(_pillarBox.X, _pillarBox.Y, 0f);

			_screenMatrix = Matrix.Multiply(translation, Matrix.CreateScale(
				((float)_virtualResolution.X / (float)vp.X),
				((float)_virtualResolution.Y / (float)vp.Y),
				1.0f));
		}

		/// <summary>
		/// Recalculates and applies the viewport to maintain the virtual aspect ratio,
		/// centering it within the back buffer and adding letterbox or pillarbox bars as needed.
		/// Marks the scale matrix dirty when the viewport dimensions change.
		/// </summary>
		public void ResetViewport()
		{
			// figure out the largest area that fits in this resolution at the desired aspect ratio
			int width = Device.GraphicsDevice.Viewport.Width;
			var height = (int)(width / VirtualAspectRatio + .5f);
			bool changed = false;

			if (height != Device.GraphicsDevice.Viewport.Height || width != Device.GraphicsDevice.Viewport.Width)
			{
				if (height > Device.GraphicsDevice.Viewport.Height)
				{
					// PillarBox
					height = Device.PreferredBackBufferHeight;
					width = (int)(height * VirtualAspectRatio + .5f);
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

				_pillarBox = new Vector2(-viewport.X, -viewport.Y);

				if (changed)
				{
					_dirtyMatrix = true;
				}

				Device.GraphicsDevice.Viewport = viewport;
			}
		}

		#endregion //Methods
	}
}
