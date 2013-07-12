using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ResolutionBuddy
{
	public static class Resolution
	{
		#region Members

		/// <summary>
		/// The graphics device
		/// </summary>
		/// <value>The device.</value>
		static public GraphicsDeviceManager Device { get; private set; }

		/// <summary>
		/// The title safe area for our virtual resolution
		/// </summary>
		/// <value>The title safe area.</value>
		static public Rectangle _titleSafeArea = new Rectangle();

		static public Rectangle TitleSafeArea
		{
			get
			{
				return _titleSafeArea;
			}
		}

		/// <summary>
		/// The actual screen rectangle
		/// </summary>
		static private Vector2 _ScreenRect = new Vector2(800, 600);

		/// <summary>
		/// The screen rect we want for our game, and are going to fake
		/// </summary>
		static private Vector2 _VirtualRect = new Vector2(1280, 720);

		/// <summary>
		/// The scale matrix from the desired rect to the screen rect
		/// </summary>
		static private Matrix _ScaleMatrix;

		/// <summary>
		/// whether or not we want full screen 
		/// </summary>
		static private bool _FullScreen = false;

		/// <summary>
		/// whether or not the matrix needs to be recreated
		/// </summary>
		static private bool _dirtyMatrix = true;

		#endregion //Members

		#region Methods

		static public void Init(ref GraphicsDeviceManager device)
		{
			Device = device;
		}

		/// <summary>
		/// Get the transformation matrix for when you call SpriteBatch.Begin
		/// To add this to a camera matrix, do CameraMatrix * TransformationMatrix
		/// </summary>
		/// <returns>The matrix.</returns>
		static public Matrix TransformationMatrix()
		{
			if (_dirtyMatrix)
			{
				RecreateScaleMatrix();
			}

			return _ScaleMatrix;
		}

		static public void SetScreenResolution(int Width, int Height, bool FullScreen)
		{
			_ScreenRect.X = Width;
			_ScreenRect.Y = Height;

			_FullScreen = FullScreen;

			ApplyResolutionSettings();
		}

		static public void SetDesiredResolution(int Width, int Height)
		{
			_VirtualRect.X = Width;
			_VirtualRect.Y = Height;

			//set up the title safe area
			_titleSafeArea.X = (int)(_VirtualRect.X / 20.0f);
			_titleSafeArea.Y = (int)(_VirtualRect.Y / 20.0f);
			_titleSafeArea.Width = (int)(_VirtualRect.X - (2 * TitleSafeArea.X));
			_titleSafeArea.Height = (int)(_VirtualRect.Y - (2 * TitleSafeArea.Y));

			_dirtyMatrix = true;
		}

		static private void ApplyResolutionSettings()
		{
			// If we aren't using a full screen mode, the height and width of the window can
			// be set to anything equal to or smaller than the actual screen size.
			if (_FullScreen == false)
			{
				//Make sure the width isn't bigger than the screen
				if (_ScreenRect.X > GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width)
				{
					_ScreenRect.X = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
				}

				//Make sure the height isn't bigger than the screen
				if (_ScreenRect.Y > GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height)
				{
					_ScreenRect.Y = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
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
					if ((dm.Width == _ScreenRect.X) && (dm.Height == _ScreenRect.Y))
					{
						// The mode is supported, so set the buffer formats, apply changes and return
						bFound = true;
					}
				}

				if (!bFound)
				{
					_ScreenRect.X = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
					_ScreenRect.Y = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
				}
			}

			Device.PreferredBackBufferWidth = (int)_ScreenRect.X;
			Device.PreferredBackBufferHeight = (int)_ScreenRect.Y;
			Device.IsFullScreen = _FullScreen;
			Device.ApplyChanges();

			_dirtyMatrix = true;
		}

		/// <summary>
		/// Sets the device to use the draw pump
		/// Sets correct aspect ratio
		/// </summary>
		static public void BeginDraw()
		{
			// Clear to Black
			Device.GraphicsDevice.Clear(Color.Black);

			// Calculate Proper Viewport according to Aspect Ratio
			ResetViewport();
		}

		static private void RecreateScaleMatrix()
		{
			_dirtyMatrix = false;
			_ScaleMatrix = Matrix.CreateScale(
				_ScreenRect.X / _VirtualRect.X,
				_ScreenRect.X / _VirtualRect.X,
				//(float)Device.GraphicsDevice.Viewport.Width / _VWidth,
				//(float)Device.GraphicsDevice.Viewport.Height / _VHeight,
				1.0f);
		}

		/// <summary>
		/// Get virtual Mode Aspect Ratio
		/// </summary>
		/// <returns>aspect ratio</returns>
		static private float getVirtualAspectRatio()
		{
			return _VirtualRect.X / _VirtualRect.Y;
		}

		static public void ResetViewport()
		{
			float targetAspectRatio = getVirtualAspectRatio();

			// figure out the largest area that fits in this resolution at the desired aspect ratio
			float width = _ScreenRect.X;
			float height = (width / targetAspectRatio + .5f);
			bool changed = false;

			if (height > _ScreenRect.Y)
			{
				// PillarBox
				height = _ScreenRect.Y;
				width = (height * targetAspectRatio + .5f);
				changed = true;
			}

			// set up the new viewport centered in the backbuffer
			Viewport viewport = new Viewport();

			viewport.X = (int)((_ScreenRect.X / 2.0f) - (width / 2.0f));
			viewport.Y = (int)((_ScreenRect.Y / 2.0f) - (height / 2.0f));
			viewport.Width = (int)width;
			viewport.Height = (int)height;
			viewport.MinDepth = 0;
			viewport.MaxDepth = 1;

			if (changed)
			{
				_dirtyMatrix = true;
			}

			Device.GraphicsDevice.Viewport = viewport;
		}

		#endregion //Methods
	}
}
