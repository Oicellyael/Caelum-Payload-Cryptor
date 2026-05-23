# Advanced Process Instrumentation and Thread Context Control Framework

## Overview

This research project is a low-level engineering proof of concept (PoC) written in C# and MASM. It is designed to demonstrate advanced concepts in native Windows process manipulation, cross-process memory management, and thread context synchronization.

The framework showcases how a controller application can programmatically launch a target process, modify its initialization attributes, securely update specific memory sections, and redirect execution flow at the hardware register level.

---

##  Technical Concepts Demonstrated

* **Process Attribute Customization (Parent-Child Hierarchy Management)**
Leverages native Windows APIs (`InitializeProcThreadAttributeList` and `UpdateProcThreadAttribute`) to programmatically assign an alternative parent process (such as `explorer.exe`) during target creation. This explores how operating systems handle process tree inheritance and structural metadata.
* **Module Overlay and Section Replacement (Dynamic Code Hot-Patching)**
Demonstrates automated runtime memory analysis:
* Traverses the remote **Process Environment Block (PEB)** to locate loaded modules like `kernelbase.dll`.
* Parses the Portable Executable (PE) structure to find the executable `.text` section.
* Replaces specific code segments with custom logic, practicing dynamic code instrumentation within signed memory spaces.


* **Context-Driven Thread Hijacking & State Preservation**
Controls execution flow at the CPU level without breaking the host process:
* Initializes the target process in a suspended state.
* Intersects the main thread context using `GetThreadContext` and `SetThreadContext`.
* Manages architectural registers (`RIP`, `RSP`) to redirect execution while using a low-level **MASM stub** to preserve original CPU flags and registers for a stable state transition.


* **Cryptographic Data Protection**
Implements secure data handling by storing payloads in an encrypted format (AES-256 CBC), ensuring that raw data is decrypted strictly in volatile memory only at the exact moment of execution.

---

##  Project Architecture

* **/sharp/**: The core C# manager that handles process creation, memory operations, and low-level thread architecture.
* **/asm/**: A Microsoft Macro Assembler (MASM) bootstrap stub that manages the CPU register stack, ensuring system stability during execution transitions.
* **/payload/**: A sample C++ runtime library designed to demonstrate successful environment initialization and safe context handoff.

---
##  Dynamic Instrumentation Workflow

1. **Configuration Retrieval:** The application securely fetches configuration parameters and decrypts the targeted payload directly into runtime memory.
2. **Attribute Allocation:** The system creates a custom process attribute list to define the process tree hierarchy.
3. **Suspended State Initialization:** The target process is safely created in a suspended state to allow memory preparation.
4. **PEB Traversal:** The manager manually parses the target's internal PEB structures to resolve core module base addresses.
5. **Memory Section Update:** The manager reconfigures section memory protections and updates the code segment with the prepared payload.
6. **Register Synchronization:** The execution flow is synchronized by updating the thread's instruction pointer (`RIP`), followed by resuming the process thread execution.

---

<img width="674" height="666" alt="image" src="https://github.com/user-attachments/assets/25c18498-7eec-4c38-82ac-e4d83fa0e872" />

### **Section: Detection Insights**

> *"To demonstrate the effectiveness of the implemented techniques, the loader was tested against Windows Defender without any obfuscation or runtime encryption."*


<img width="1173" height="1008" alt="image" src="https://github.com/user-attachments/assets/4ce5255d-2585-462e-9532-31768ea9318a" />

## ⚠️ Academic Disclaimer

*This repository is intended solely for educational, academic, and defensive research into operating system mechanics, software debugging, and native process instrumentation*
