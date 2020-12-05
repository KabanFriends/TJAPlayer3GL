using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using OpenTK;
using OpenTK.Graphics;
using FFmpeg.AutoGen; 

namespace FDK
{
	/// <summary>
	/// Presents an easy to use wrapper for making games and samples.
	/// </summary>
	public abstract class Game : GameWindow
	{
		public Game()
			: base(GameWindowSize.Width, GameWindowSize.Height, GraphicsMode.Default, "TJAPlayer3GL")
		{
            VSync = VSyncMode.On;
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
		}

		protected override void OnUnload(EventArgs e)
		{
			base.OnUnload(e);
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
		}
		protected override void OnUpdateFrame(FrameEventArgs e)
		{
			base.OnUpdateFrame(e);
		}
		protected override void OnRenderFrame(FrameEventArgs e)
		{
			base.OnRenderFrame(e);
		}
		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);
		}
	}
}
