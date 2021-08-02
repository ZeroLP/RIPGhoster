# RIPGhoster
 Ghosts(spoofs) a x86 return address. 
 
This demonstrates how a return address can be ghosted/spoofed in x86 when a function checks whether the return address resides with-in the function's module address space. It firstly allocates bytes at the near-end of the module address space(0x1000 bytes away from the end), NOPs out the instructions for our caller stub function instructions to be written over, and then it writes the instructions of the caller stub function. It then allocates a JMPTable somewhere in the memory allocated by the CLR/CoreCLR that is GC fixed and jmp/calls into our caller stub function allocated earlier to call the original function.

For a simpler execution tree view:
Call CallFoo(int a1) -> JMPTable(push eax, ret) -> CallFoo(int a1) -> Foo(a1)

# Execution in scene
![](https://i.imgur.com/0YnYMER.png)

![](https://i.imgur.com/cdWZVQe.png)