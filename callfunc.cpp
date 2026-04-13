#include <windows.h>
#include <stdio.h>
#include <string>

void main() {
    AllocConsole();
    FILE* f;
    freopen_s(&f, "CONOUT$", "w", stdout);

    HWND hwnd = GetForegroundWindow();
    char originalTitle[256];
    GetWindowTextA(hwnd, originalTitle, 256);

    printf("Sagittarius Loader Active...\n");
    printf("Executing payload for 10 seconds.\n\n");

    const int duration = 10;
    const int barWidth = 20;

    for (int i = 0; i <= 100; i += 2) {
        printf("\rProgress: [");
        int pos = (i * barWidth) / 100;
        for (int j = 0; j < barWidth; ++j) {
            if (j < pos) printf("=");
            else if (j == pos) printf(">");
            else printf(" ");
        }
        printf("] %d%%", i);
        fflush(stdout);

        std::string newTitle = "Injection Status: " + std::to_string(i) + "%";
        SetWindowTextA(hwnd, newTitle.c_str());

        Sleep(200);
    }

    SetWindowTextA(hwnd, originalTitle);
    printf("\n\n[+] Task complete. Notepad title restored.\n");
    MessageBoxA(NULL, "Payload finished! Now go for a workout.", "Success", MB_OK | MB_ICONEXCLAMATION);
    Sleep(1500);
}
