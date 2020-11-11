﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace FDK
{
	// 構造体

	[StructLayout( LayoutKind.Sequential )]
	public struct STInputEvent
	{
		public int nKey { get; set; }
		public bool b押された { get; set; }
		public long nTimeStamp { get; set; }
	}
}
