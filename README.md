# Sagittarius Loader (R&D)

A sophisticated proof-of-concept shellcode loader written in C#, designed to demonstrate advanced evasion techniques and process instrumentation. This project focuses on in-memory execution while maintaining a low forensic footprint.

## 🛠 Technical Features

This loader implements several "under-the-hood" techniques to bypass modern defensive solutions (EDR/AV):

* **PPID Spoofing:** Uses `InitializeProcThreadAttributeList` and `UpdateProcThreadAttribute` to spoof the parent process to `explorer.exe`, breaking process tree heuristics.
* **Module Stomping (Section Overwriting):** * Dynamically locates `kernelbase.dll` via **PEB (Process Environment Block)** traversal.
    * Identifies the `.text` section of the legitimate module.
    * Overwrites the legitimate code section with encrypted payload to reside in "signed" memory space.
* **Thread Hijacking:** * Launches a target process in a `SUSPENDED` state.
    * Uses `GetThreadContext` and `SetThreadContext` to redirect execution flow to the stomped section.
    * Preserves execution flow by pushing the original `RIP` onto the stack via an ASM stub.
* **AES-256 Decryption:** Payloads are stored in AES-CBC encrypted format and decrypted in-memory only at runtime.
* **Dynamic Configuration:** Fetches payload and target paths via a remote JSON config using `HttpClient`.

## 🏗 Project Structure

* **/sharp/**: The core C# loader responsible for process creation, injection, and thread manipulation.
* **/asm/**: Low-level MASM stub used to preserve CPU registers and flags, ensuring a clean transition between the loader and the payload.
* **/payload/**: A demo C++ payload that performs console allocation and UI manipulation (Notepad title bar progress) as a proof of execution.

## 🧪 Injection Workflow

1.  **Configuration:** Download JSON config -> Decrypt AES-256 payload.
2.  **Spoofing:** Locate `explorer.exe` -> Initialize Attribute List for Parent Process.
3.  **Creation:** Start `target.exe` in `SUSPENDED` mode with spoofed PPID.
4.  **Discovery:** Manually parse PEB to find `kernelbase.dll` base address in the remote process.
5.  **Stomping:** Locate `.text` section -> `VirtualProtectEx` -> `WriteProcessMemory`.
6.  **Hijacking:** `GetThreadContext` -> Update `Rsp`/`Rip` -> `SetThreadContext` -> `ResumeThread`.


<img width="674" height="666" alt="image" src="https://github.com/user-attachments/assets/25c18498-7eec-4c38-82ac-e4d83fa0e872" />


### **Section: Detection Insights**

> *"To demonstrate the effectiveness of the implemented techniques, the loader was tested against Windows Defender without any obfuscation or runtime encryption."*


<img width="1173" height="1008" alt="image" src="https://github.com/user-attachments/assets/4ce5255d-2585-462e-9532-31768ea9318a" />


**Why it is detected:**
The detection is primarily triggered by the **behavioral signatures** of the injection workflow:
1. **PPID Spoofing** via `UpdateProcThreadAttribute`.
2. **Thread Hijacking** using `SetThreadContext`.
3. **Module Stomping** in `kernelbase.dll`'s `.text` section.

> *"This proves that the core logic successfully interacts with the target process's memory and control flow. The next phase of this R&D project will focus on custom obfuscation and 'Indirect Syscalls' to minimize this signature."*


## ⚙️ Requirements

* .NET 6.0+
* Visual Studio 2022 (for C# and C++ parts)
* MASM (Microsoft Macro Assembler) for the stub

## ⚠️ Research Disclaimer

This repository is intended for **educational and research purposes only**. It explores the concepts of software instrumentation, hardware-software synchronization, and real-time data visualization.
