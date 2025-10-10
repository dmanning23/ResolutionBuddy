using Microsoft.Xna.Framework;

namespace ResolutionBuddy
{
	/// <summary>
	/// Singleton for easy access to various resolution items
	/// </summary>
	public static class Resolution
	{
		#region Properties

		private static IResolution _resolution;

		public static Rectangle TitleSafeArea
		{
			get { return _resolution.TitleSafeArea; }
		}

		public static Rectangle ScreenArea
		{
			get { return _resolution.ScreenArea; }
		}

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

		public static void Init(IResolution resolution)
		{
			_resolution = resolution;
		}

		#endregion Initialization

		/// <summary>
		/// Get the transformation matrix for when you call SpriteBatch.Begin
		/// To add this to a camera matrix, do CameraMatrix * TransformationMatrix
		/// </summary>
		/// <returns>The matrix.</returns>
		public static Matrix TransformationMatrix()
		{
			return _resolution.TransformationMatrix();
		}

		/// <summary>
		/// Given a screen coord, convert to game coordinate system.
		/// </summary>
		/// <param name="screenCoord"></param>
		/// <returns></returns>
		public static Vector2 ScreenToGameCoord(Vector2 screenCoord)
		{
			return _resolution.ScreenToGameCoord(screenCoord);
		}

		public static void ResetViewport()
		{
			_resolution.ResetViewport();
		}

		#endregion //Methods
	}
}