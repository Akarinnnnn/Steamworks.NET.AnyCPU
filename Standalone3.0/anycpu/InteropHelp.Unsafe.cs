using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Steamworks
{
	public static partial class InteropHelp
	{
		public static unsafe string PtrToStringUTF8(IntPtr nativeUtf8) {
			byte* cStr = (byte*)nativeUtf8;
			ReadOnlySpan<byte> spanUnchecked = new(cStr, int.MaxValue);
			ReadOnlySpan<byte> stringSpan = spanUnchecked.Slice(0, spanUnchecked.IndexOf((byte)0));

			return Encoding.UTF8.GetString(stringSpan);
		}
	}
}
