﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace FDK
{
	public class CAction
	{
		public static Matrix4 ModelView
		{
			get
			{
				return Matrix4.LookAt(new Vector3(0f, 0f, (float)(-GameWindowSize.Height / 2 * Math.Sqrt(3.0))), new Vector3(0f, 0f, 0f), new Vector3(0f, 1f, 0f));
			}
		}


		public static Matrix4 Projection
		{
			get
			{
				return Matrix4.CreatePerspectiveFieldOfView(C変換.DegreeToRadian((float)60f), ((float)GameWindowSize.Width) / ((float)GameWindowSize.Height), 0.0000001f, 100f);
			}
		}

		public static void LoadContentAction()
		{
			Matrix4 tmpmat = ModelView;
			GL.MatrixMode(MatrixMode.Modelview);
			GL.LoadMatrix(ref tmpmat);

			tmpmat = Projection;
			GL.MatrixMode(MatrixMode.Projection);
			GL.LoadMatrix(ref tmpmat);

			GL.ClearColor(Color4.Black);
			GL.Disable(EnableCap.DepthTest);
			GL.Enable(EnableCap.Blend);
			GL.Enable(EnableCap.Texture2D);
		}

		public static void BeginScene()
		{
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
		}

		public static void Flush()
		{
			GL.Flush();
		}
	}
}