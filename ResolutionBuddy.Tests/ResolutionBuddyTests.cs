using Microsoft.Xna.Framework;
using NUnit.Framework;
using ResolutionBuddy;
using Shouldly;

namespace ResolutionBuddyTests
{
	[TestFixture]
	public class ResolutionBuddyTests
	{
		class TestResolutionAdapter : ResolutionAdapter
		{
			public new void RecreateScaleMatrix(Point vp)
			{
				base.RecreateScaleMatrix(vp);
			}
		}

		[Test]
		public void MatrixSetX()
		{
			var resolution = new TestResolutionAdapter();
			resolution.SetVirtualResolution(100, 50);
			resolution.RecreateScaleMatrix(new Point(200, 100));

			//check scale matrix
			resolution.ScreenMatrix.M11.ShouldBe(0.5f);
		}

		[Test]
		public void MatrixSetY()
		{
			var resolution = new TestResolutionAdapter();
			resolution.SetVirtualResolution(100, 50);
			resolution.RecreateScaleMatrix(new Point(200, 25));

			//check scale matrix
			resolution.ScreenMatrix.M22.ShouldBe(2.0f);
		}

		[Test]
		public void TransformX()
		{
			var resolution = new TestResolutionAdapter();
			resolution.SetVirtualResolution(100, 50);
			resolution.RecreateScaleMatrix(new Point(200, 25));

			Vector2 trans = resolution.ScreenToGameCoord(new Vector2(1000.0f, 1000.0f));

			//check scale matrix
			trans.X.ShouldBe(500.0f);
		}

		[Test]
		public void TransformY()
		{
			var resolution = new TestResolutionAdapter();
			resolution.SetVirtualResolution(100, 50);
			resolution.RecreateScaleMatrix(new Point(200, 25));

			Vector2 trans = resolution.ScreenToGameCoord(new Vector2(1000.0f, 1000.0f));

			//check scale matrix
			trans.Y.ShouldBe(2000.0f);
		}
	}
}
