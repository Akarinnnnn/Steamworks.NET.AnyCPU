using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Steamworks.AnyCPU
{
	/// <summary>
	/// Helper class to resolve the correct platform-specific binary name for Steamworks native libraries.
	/// </summary>
	public static class SteamNativeLibraryNameHelper
	{
		/// <summary>
		/// Resolves the platform-specific binary name for the requested Steamworks native library.
		/// </summary>
		/// <param name="requestedSteamBinaryPInvokeName">The name of the Steamworks native library to resolve from this library.</param>
		/// <returns>Simple module name that P/Invoke can use for first-chance search.</returns>
		/// <exception cref="ArgumentException">Attempt to request arbitrary native library.</exception>
		public static string ResolvePlatformBinarySimpleFileName(string requestedSteamBinaryPInvokeName) {
			if (!requestedSteamBinaryPInvokeName.AsSpan(0, NativeMethods.AnyCPUPrefix.Length).SequenceEqual(NativeMethods.AnyCPUPrefix)) {
				Debug.Assert(false, "requestedSteamBinaryName does not start with AnyCPU prefix, this is unexpected.");
				throw new ArgumentException("Attempt to request arbitrary native library.", nameof(requestedSteamBinaryPInvokeName));
			}

			if (requestedSteamBinaryPInvokeName == NativeMethods.NativeLibraryName || requestedSteamBinaryPInvokeName == NativeMethods.NativeLibrary_SDKEncryptedAppTicket) {
				// check are we on win64, the special case we are going to handle
				if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && Environment.Is64BitProcess) {
					// modify library name to x64 version
					requestedSteamBinaryPInvokeName = $"{requestedSteamBinaryPInvokeName.AsSpan(NativeMethods.AnyCPUPrefix.Length)}64";
				}

			} else {
				throw new ArgumentException("Attempt to request arbitrary native library.", nameof(requestedSteamBinaryPInvokeName));
			}

			return requestedSteamBinaryPInvokeName;
		}

		/// <summary>
		/// Resolves the platform-specific binary file name for the requested Steamworks native library, including the appropriate file extension and prefix based on the operating system.
		/// </summary>
		/// <remarks>
		/// </remarks>
		/// <param name="requestedSteamBinaryPInvokeName">The name of the Steamworks native library to resolve from this library.</param>
		/// <returns>The platform-specific binary file name.</returns>
		/// <exception cref="ArgumentException">Attempt to request arbitrary native library.</exception>
		public static string ResolvePlatformBinaryFileName(string requestedSteamBinaryPInvokeName) {
			string simpleName = ResolvePlatformBinarySimpleFileName(requestedSteamBinaryPInvokeName);
			string extension;
			string libFilenamePrefix = "lib";
			if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
				extension = ".dylib";
			else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
				extension = ".dll";
				libFilenamePrefix = ""; // no prefix is applied on Windows
			} else
				extension = ".so"; // I can't imagine what else platforms other than linux that
								   // Steamworks.NET.AnyCPU will run on, but let's be future proof
			return Path.ChangeExtension(libFilenamePrefix + simpleName, extension);
		}
	}
}
