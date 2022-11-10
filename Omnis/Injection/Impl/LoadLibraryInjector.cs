using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;

namespace Omnis.Injection.Impl;

public class LoadLibraryInjector : IInject
{
    [DllImport("kernel32.dll", EntryPoint = "OpenProcess")]
    private static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

    [DllImport("kernel32.dll", EntryPoint = "GetModuleHandle", CharSet = CharSet.Auto)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

    [DllImport("kernel32", EntryPoint = "GetProcAddress", CharSet = CharSet.Ansi, ExactSpelling = true,
        SetLastError = true)]
    private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

    [DllImport("kernel32.dll", EntryPoint = "VirtualAllocEx", SetLastError = true, ExactSpelling = true)]
    private static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress,
        uint dwSize, uint flAllocationType, uint flProtect);

    [DllImport("kernel32.dll", EntryPoint = "WriteProcessMemory", SetLastError = true)]
    private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer,
        uint nSize, out UIntPtr lpNumberOfBytesWritten);

    [DllImport("kernel32.dll", EntryPoint = "CreateRemoteThread")]
    private static extern IntPtr CreateRemoteThread(IntPtr hProcess,
        IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter,
        uint dwCreationFlags, IntPtr lpThreadId);

    public void Inject(string filePath)
    {
        ApplyAppPackages(filePath);
        var target = Process.GetProcessesByName("Minecraft.Windows").FirstOrDefault();
        if (target == null)
            throw new Exception("Unable to find Minecraft.");
        var hProc = OpenProcess(0xFFFF, false, target.Id);
        var loadLibraryProc = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");
        var allocated = VirtualAllocEx(hProc, IntPtr.Zero, (uint)filePath.Length + 1, 0x00001000 | 0x00002000,
            0x40);
        WriteProcessMemory(hProc, allocated, Encoding.UTF8.GetBytes(filePath), (uint)filePath.Length + 1, out _);
        CreateRemoteThread(hProc, IntPtr.Zero, 0, loadLibraryProc, allocated, 0, IntPtr.Zero);
    }

    private static void ApplyAppPackages(string dllPath)
    {
        var infoFile = new FileInfo(dllPath);
        var fSecurity = infoFile.GetAccessControl();
        fSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier("S-1-15-2-1"),
            FileSystemRights.FullControl, InheritanceFlags.None, PropagationFlags.NoPropagateInherit,
            AccessControlType.Allow));
        infoFile.SetAccessControl(fSecurity);
    }
}