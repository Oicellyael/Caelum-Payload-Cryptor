#include <windows.h>


int main() {
    int hp = 100;
    while (hp > 0) {
        hp -= 10;
        Sleep(100); 
    }

    MessageBoxA(NULL, "Payload Executed! HP is 0.", "C++ Agent", MB_OK | MB_ICONEXCLAMATION);
    return 0;
}