﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

using SkiaSharp;

namespace Skia.WindowsDesktop.Demo
{
	public partial class SkiaView : Control
	{
		private Action<SKCanvas, int, int> onDrawCallback;

		public SkiaView()
		{
			DoubleBuffered = true;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			if (onDrawCallback == null)
				return;

			var width = Width;
			var height = Height;

			using (var bitmap = new Bitmap(width, height, PixelFormat.Format32bppPArgb))
			{
				var data = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, bitmap.PixelFormat);
				using (var surface = SKSurface.Create(width, height, SKColorType.N_32, SKAlphaType.Premul, data.Scan0, width * 4))
				{
					var skcanvas = surface.Canvas;
					onDrawCallback(skcanvas, width, height);
				}
				bitmap.UnlockBits(data);

				e.Graphics.DrawImage(bitmap, new Rectangle(0, 0, Width, Height));
			}
		}

		protected override void OnClientSizeChanged(EventArgs e)
		{
			base.OnClientSizeChanged(e);
			Invalidate();
		}

		public Action<SKCanvas, int, int> OnDrawCallback
		{
			get { return onDrawCallback; }
			set
			{
				onDrawCallback = value;
				Invalidate();
			}
		}
	}
}
