#include <windows.h>

void StartPayload() {

    if (!AllocConsole()) return;
    HANDLE hOut = GetStdHandle(STD_OUTPUT_HANDLE);

    HWND hwnd = FindWindowA("Notepad", NULL);
    char originalTitle[256];
    GetWindowTextA(hwnd, originalTitle, 256);

    const char* msgStart = "Sagittarius Loader Active...\nExecuting payload for 10 seconds.\n\n";
    WriteConsoleA(hOut, msgStart, lstrlenA(msgStart), NULL, NULL);

    char buffer[128];
    const int barWidth = 20;

    for (int i = 0; i <= 100; i += 5) {
        int pos = (i * barWidth) / 100;

        int len = wsprintfA(buffer, "\rProgress: [");
        for (int j = 0; j < barWidth; ++j) {
            buffer[len++] = (j < pos) ? '=' : (j == pos ? '>' : ' ');
        }
        wsprintfA(buffer + len, "] %d%%", i);

        WriteConsoleA(hOut, buffer, lstrlenA(buffer), NULL, NULL);

        char titleBuf[64];
        wsprintfA(titleBuf, "Injection Status: %d%%", i);
        SetWindowTextA(hwnd, titleBuf);

        Sleep(200);
    }

    SetWindowTextA(hwnd, originalTitle);
    const char* msgEnd = "\n\n[+] Task complete. Notepad title restored.\n";
    WriteConsoleA(hOut, msgEnd, lstrlenA(msgEnd), NULL, NULL);

    MessageBoxA(NULL, "Payload finished! Now go for a workout.", "Success", MB_OK | MB_ICONEXCLAMATION);

    Sleep(2000);
    FreeConsole();
}
