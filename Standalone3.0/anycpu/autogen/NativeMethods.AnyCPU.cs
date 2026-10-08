using Steamworks.AnyCPU;
using System;
using System.IO;
using System.Runtime.InteropServices;

#if STEAMWORKS_ANYCPU

namespace Steamworks
{
	internal partial class NativeMethods
	{
		static NativeMethods() {
			NativeLibrary.SetDllImportResolver(typeof(NativeMethods).Assembly, DefaultDllImportResolver);
		}

		private static IntPtr DefaultDllImportResolver(string libraryName, System.Reflection.Assembly assembly, DllImportSearchPath? searchPath) {
			// check is requesting library name matches steam native
			// we don't check requester here because we want to ensure we are the first loader of steam native
			// otherwise other libraries may have already loaded steam native with wrong architecture
			if (libraryName == NativeLibraryName || libraryName == NativeLibrary_SDKEncryptedAppTicket) {
				// check are we on win64, the special case we are going to handle
				string librarySimpleName = SteamNativeLibraryNameHelper.ResolvePlatformBinarySimpleFileName(libraryName);

				if (!NativeLibrary.TryLoad(librarySimpleName, assembly, searchPath, out nint lib)) {
					// godot non-NAOT specific search
					// in case of first chance search failed, build the full path of steam native, include extension name,
					// and try load again, this is for the case when steam native is not in default `dlopen()` search path
					// but in the same directory as the assembly.
					string searchDirectory = Path.GetDirectoryName(assembly.Location);

					if (string.IsNullOrEmpty(searchDirectory)) {
						System.Diagnostics.Debug.WriteLine("It seems you are loading Steamworks.NET.AnyCPU from memory," +
							" auto-detect steam native location is not possible," +
							" now trying to load from AppDomain.BaseDirectory." +
							" If still fails, please edit your program to manually set your resolver by" +
							" `NativeLibrary.SetDllImporterResplver(typeof(Steamworks.SteamAPI).Assembly, YourResolver)`." +
							" Use `class Steamworks.AnyCPU.SteamNativeLibraryNameHelper` to retrive appropriate library name");

						searchDirectory = AppDomain.CurrentDomain.BaseDirectory;
					}

					libraryName = SteamNativeLibraryNameHelper.ResolvePlatformBinaryFileName(libraryName);
					string path = Path.Combine(searchDirectory, libraryName);

					if (!NativeLibrary.TryLoad(path, assembly, null, out lib)) {
						throw new DllNotFoundException($"Failed to load native library: {libraryName}. Refer to debug output or `NativeMethods.AnyCPU.cs` source code for instructions.");
					}
				}

				return lib;
			}

			return 0;
		}
	}
}
#else
#error This file is Steamworks.NET.AnyCPU specific, not applicable to other vairant
#endif
