using Microsoft.Xna.Framework;

namespace ResolutionBuddy
{
	/// <summary>
	/// Manages resolution-independent rendering for a MonoGame project. Defines a virtual
	/// coordinate space that is scaled to match the actual screen resolution at runtime.
	/// </summary>
	public interface IResolution
	{
		/// <summary>
		/// The title-safe rectangle in virtual (game) coordinates. Inset by 5% on each side.
		/// </summary>
		Rectangle TitleSafeArea { get; }

		/// <summary>
		/// The full-screen rectangle in virtual (game) coordinates.
		/// </summary>
		Rectangle ScreenArea { get; }

		/// <summary>
		/// Matrix to convert screen coordinates to game coordinates. Use for mapping mouse clicks and touch events.
		/// </summary>
		Matrix ScreenMatrix { get; }

		/// <summary>
		/// The virtual resolution that defines the game's coordinate space.
		/// Must be set before the component is initialized.
		/// </summary>
		Point VirtualResolution { get; set; }

		/// <summary>
		/// The desired physical screen or window resolution.
		/// Must be set before the component is initialized.
		/// </summary>
		Point ScreenResolution { get; set; }

		/// <summary>
		/// Returns the matrix that maps game (virtual) coordinates to screen coordinates.
		/// Pass this into <see cref="Microsoft.Xna.Framework.Graphics.SpriteBatch.Begin"/> to render in game space.
		/// To combine with a camera matrix, use: <c>cameraMatrix * TransformationMatrix()</c>.
		/// </summary>
		/// <returns>The scale/translation matrix from virtual to screen space.</returns>
		Matrix TransformationMatrix();

		/// <summary>
		/// Converts a screen-space coordinate (e.g. from mouse or touch input) to the
		/// corresponding virtual (game) coordinate.
		/// </summary>
		/// <param name="screenCoord">The position in screen pixels.</param>
		/// <returns>The equivalent position in game/virtual coordinates.</returns>
		Vector2 ScreenToGameCoord(Vector2 screenCoord);

		/// <summary>
		/// Recalculates and applies the viewport to maintain the correct aspect ratio,
		/// adding letterbox or pillarbox bars as needed. Call this each frame before drawing.
		/// </summary>
		void ResetViewport();
	}
}
