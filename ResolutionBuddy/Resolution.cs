using Microsoft.Xna.Framework;

namespace ResolutionBuddy
{
	/// <summary>
	/// Static singleton that provides global access to resolution data after a
	/// <see cref="ResolutionComponent"/> has been initialized. Delegates to the active
	/// <see cref="IResolution"/> instance.
	/// </summary>
	public static class Resolution
	{
		#region Properties

		private static IResolution _resolution;

		/// <summary>
		/// The title-safe rectangle in virtual (game) coordinates. Inset by 5% on each side.
		/// </summary>
		public static Rectangle TitleSafeArea
		{
			get { return _resolution.TitleSafeArea; }
		}

		/// <summary>
		/// The full-screen rectangle in virtual (game) coordinates.
		/// </summary>
		public static Rectangle ScreenArea
		{
			get { return _resolution.ScreenArea; }
		}

		/// <summary>
		/// Matrix to convert screen coordinates to game coordinates. Use for mapping mouse clicks and touch events.
		/// </summary>
		public static Matrix ScreenMatrix
		{
			get
			{
				return _resolution.ScreenMatrix;
			}
		}

		#endregion //Properties

		#region Methods

		#region Initialization

		/// <summary>
		/// Initializes the singleton with the active resolution implementation.
		/// Called automatically by <see cref="ResolutionComponent.Initialize"/>.
		/// </summary>
		/// <param name="resolution">The <see cref="IResolution"/> instance to delegate to.</param>
		public static void Init(IResolution resolution)
		{
			_resolution = resolution;
		}

		#endregion Initialization

		/// <summary>
		/// Returns the matrix that maps game (virtual) coordinates to screen coordinates.
		/// Pass this into <see cref="Microsoft.Xna.Framework.Graphics.SpriteBatch.Begin"/> to render in game space.
		/// To combine with a camera matrix, use: <c>cameraMatrix * TransformationMatrix()</c>.
		/// </summary>
		/// <returns>The scale/translation matrix from virtual to screen space.</returns>
		public static Matrix TransformationMatrix()
		{
			return _resolution.TransformationMatrix();
		}

		/// <summary>
		/// Converts a screen-space coordinate (e.g. from mouse or touch input) to the
		/// corresponding virtual (game) coordinate.
		/// </summary>
		/// <param name="screenCoord">The position in screen pixels.</param>
		/// <returns>The equivalent position in game/virtual coordinates.</returns>
		public static Vector2 ScreenToGameCoord(Vector2 screenCoord)
		{
			return _resolution.ScreenToGameCoord(screenCoord);
		}

		/// <summary>
		/// Recalculates and applies the viewport to maintain the correct aspect ratio,
		/// adding letterbox or pillarbox bars as needed. Call this each frame before drawing.
		/// </summary>
		public static void ResetViewport()
		{
			_resolution.ResetViewport();
		}

		#endregion //Methods
	}
}