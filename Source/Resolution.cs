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

		static private int _Width = 800;
		static private int _Height = 600;
		static private int _VWidth = 1280;
		static private int _VHeight = 720;
		static private Matrix _ScaleMatrix;
		static private bool _FullScreen = false;
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

		static public void SetResolution(int Width, int Height, bool FullScreen)
		{
			_Width = Width;
			_Height = Height;

			_FullScreen = FullScreen;

			ApplyResolutionSettings();
		}

		static public void SetVirtualResolution(int Width, int Height)
		{
			_VWidth = Width;
			_VHeight = Height;

			//set up the title safe area
			_titleSafeArea.X = _VWidth / 20;
			_titleSafeArea.Y = _VHeight / 20;
			_titleSafeArea.Width = _VWidth - (2 * TitleSafeArea.X);
			_titleSafeArea.Height = _VHeight - (2 * TitleSafeArea.Y);

			_dirtyMatrix = true;
		}

		static private void ApplyResolutionSettings()
		{
			// If we aren't using a full screen mode, the height and width of the window can
			// be set to anything equal to or smaller than the actual screen size.
			if (_FullScreen == false)
			{
				if (_Width > GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width)
				{
					_Width = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
				}

				if (_Height > GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height)
				{
					_Height = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
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
					if ((dm.Width == _Width) && (dm.Height == _Height))
					{
						// The mode is supported, so set the buffer formats, apply changes and return
						bFound = true;
					}
				}

				if (!bFound)
				{
					_Width = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
					_Height = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
				}
			}

			Device.PreferredBackBufferWidth = _Width;
			Device.PreferredBackBufferHeight = _Height;
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
				(float)Device.GraphicsDevice.Viewport.Width / _VWidth,
				(float)Device.GraphicsDevice.Viewport.Width / _VWidth,
				1f);
		}

		/// <summary>
		/// Get virtual Mode Aspect Ratio
		/// </summary>
		/// <returns>aspect ratio</returns>
		static private float getVirtualAspectRatio()
		{
			return (float)_VWidth / (float)_VHeight;
		}

		static public void ResetViewport()
		{
			float targetAspectRatio = getVirtualAspectRatio();

			// figure out the largest area that fits in this resolution at the desired aspect ratio
			int width = Device.PreferredBackBufferWidth;
			int height = (int)(width / targetAspectRatio + .5f);
			bool changed = false;

			if (height > Device.PreferredBackBufferHeight)
			{
				height = Device.PreferredBackBufferHeight;
				// PillarBox
				width = (int)(height * targetAspectRatio + .5f);
				changed = true;
			}

			// set up the new viewport centered in the backbuffer
			Viewport viewport = new Viewport();

			viewport.X = (Device.PreferredBackBufferWidth / 2) - (width / 2);
			viewport.Y = (Device.PreferredBackBufferHeight / 2) - (height / 2);
			viewport.Width = width;
			viewport.Height = height;
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
