using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Steamworks
{
	public partial class InteropHelp
	{
		public unsafe static string PtrToStringUTF8(IntPtr nativeUtf8) {
			byte* ptr = (byte*)nativeUtf8;
			ArgumentNullException.ThrowIfNull(ptr, nameof(nativeUtf8));

			ReadOnlySpan<byte> nativeBufferUnchecked = new ReadOnlySpan<byte>(ptr, int.MaxValue);
			ReadOnlySpan<byte> nativeBuffer = nativeBufferUnchecked.Slice(0, nativeBufferUnchecked.IndexOf((byte)0));

			return Encoding.UTF8.GetString(nativeBuffer);
		}
	}
}
