# Z80TMS9900Transpiler
Transpile Assembly code from Z80 assembly to TMS9900 assembly

If this project were ever to be finished, the main goal would be to port freeware MSX software to the TI-99/4a with minimal effort.
Part of the reason that this is thought to be feasable is that both machines use the same VDP chip. 
Granted even if this were to work, the resulting TMS9900 code would still need to be manually inspected for possible optimizations.
The idea is to:
* Parse the Z80 assembly code
* Create TMS9900 assembly code with the same functionality
* Transpose Z80 program's memory addresses to a set of memory addresses that make sense on the TMS9900 system
* When there is no TMS9900 equivalent to a Z80 operation, output a non-assemblable comment that must be manually replaced.
* Optimize the code to run faster on the TMS9900 than it would with a strict translation
* Recognize MSX assembly commands that access the VDP and replace them with TI-99/4a commands that access the VDP (and likewise for sound, keyboard, and joystick)
