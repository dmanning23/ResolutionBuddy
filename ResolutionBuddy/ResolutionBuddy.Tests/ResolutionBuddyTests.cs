using Microsoft.Xna.Framework;
using ResolutionBuddy;
using Xunit;

namespace ResolutionBuddyTests
{
	public class ResolutionBuddyTests
	{
		[Fact]
		public void MatrixSetX()
		{
			Resolution.VirtualRect = new Point(100, 50);
			Resolution.RecreateScaleMatrix(new Point(200, 100));

			//check scale matrix
			Assert.Equal(0.5f, Resolution.ScreenMatrix.M11);
		}

		[Fact]
		public void MatrixSetY()
		{
			Resolution.VirtualRect = new Point(100, 50);
			Resolution.RecreateScaleMatrix(new Point(200, 25));

			//check scale matrix
			Assert.Equal(2.0f, Resolution.ScreenMatrix.M22);
		}

		[Fact]
		public void TransformX()
		{
			Resolution.VirtualRect = new Point(100, 50);
			Resolution.RecreateScaleMatrix(new Point(200, 25));

			Vector2 trans = Resolution.ScreenToGameCoord(new Vector2(1000.0f, 1000.0f));

			//check scale matrix
			Assert.Equal(500.0f, trans.X);
		}

		[Fact]
		public void TransformY()
		{
			Resolution.VirtualRect = new Point(100, 50);
			Resolution.RecreateScaleMatrix(new Point(200, 25));

			Vector2 trans = Resolution.ScreenToGameCoord(new Vector2(1000.0f, 1000.0f));

			//check scale matrix
			Assert.Equal(2000.0f, trans.Y);
		}
	}
}
