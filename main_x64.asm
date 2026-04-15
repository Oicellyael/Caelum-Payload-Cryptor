PUBLIC main

.code
extern StartPayload : proc

main PROC
    push rax
    push rcx
    push rdx
    push r8
    push r9
    push r10
    push r11
    pushfq

    sub rsp, 28h

    call StartPayload

    add rsp, 28h

    popfq
    pop r11
    pop r10
    pop r9
    pop r8
    pop rdx
    pop rcx
    pop rax

    ret
main ENDP

END