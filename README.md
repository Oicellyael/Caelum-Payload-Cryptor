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

<img width="674" height="666" alt="image" src="https://github.com/user-attachments/assets/25c18498-7eec-4c38-82ac-e4d83fa0e872" />


<img width="1114" height="771" alt="image" src="https://github.com/user-attachments/assets/e3d7fe9d-4a75-4a31-9138-e72fcdad4425" />


### ⚠️ Disclaimer
This project was created solely for educational purposes to study the Windows API and security methods. The author assumes no liability for any illegal use of this code.


