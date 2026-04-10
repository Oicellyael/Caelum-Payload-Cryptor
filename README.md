# 🛠️ Sharp-PPID-Loader

**Proof of Concept (PoC)** A simple C# tool to learn about process injection and hiding tricks in Windows.

### 🚀 What it does:
* **Parent Spoofing:** Makes `explorer.exe` look like the real "father" of your process.
* **Folder Masking:** Changes the working folder to `System32` so it looks more official.
* **AES Encryption:** Keeps the shellcode encrypted so it's not just sitting there in plain text.
* **Remote Config:** Downloads the settings and the encrypted code from a GitHub Gist link.
* **Suspended Start:** Starts a process (like Notepad) in "frozen" mode, puts code inside, and then runs it.
  
### 🛡️ Injection techniques:
1. `VirtualAllocEx` + `WriteProcessMemory`
2. `CreateRemoteThread`
3. `ResumeThread`

### ⚠️ Disclaimer
This project was created solely for educational purposes to study the Windows API and security methods. The author assumes no liability for any illegal use of this code.


