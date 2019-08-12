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

## Z80 Bible
I'm using the following document as instructions for how Z80 operations should be interpretted. 
http://z80.info/zip/z80cpu_um.pdf 
This document doesn't tell me anything about comments, or data defintions. 
I'm relying on internet searches and on http://map.grauw.nl/ to tell me the normal format for that.

## TMS9900 Bible
I'm using the TI-99/4a's Editor/Assembler manual to tell me the standards that I should follow for TMS9900 output. 
I don't have a link to it right now.
I downloaded a PDF copy of that document a long time ago.

## Register Mapping
The code that has been written so far assumes that the Z80 registers can be mapped to TMS9900 workspace registers 1 through 10.
The remaining TMS workspace registers have special purposes.
Z80 registers A,B,C,D,E,H,L,IX,IY, and SP can all be mapped to a TMS register.
Z80 register F cannot currently be mapped.

TMS workspace registers are 16-bit, but most Z80 registers are 8-bit.
Z80 register A is always mapped to the high byte of a TMS register.
The other 8-bit registers are slightly more complex.
The user of the transpiler is supposed to have the option of mapping registers in a Z80 register pair to separate TMS registers or to different bytes in the same TMS register.
For instance register B and C could be mapped to the high bytes of TMS registers 2 and 3 respectively.
In this scenario, "ld B,C" could be translated as "MOVB R3,R2"
More often, a user would be wise to map both Z80 registers to the same TMS register.
For example, if the chosen register is R5, then register B would be mapped to the high byte in R5 and register C would be mapped to the low byte in R5.
In this scenario, "ld B,C" would be translated as "MOVB *R13,R5", because register 13 holds a memory address that corresponds to the low byte of register 5.

TMS workspace registers 12 through 15 have a special purpose.
Each of them holds a memory address that corresponds to the low byte of a TMS workspace register.
Register 12 hold an address corresponding the the low byte of whichever TMS register is mapped from Z80 register A.
Register 13 hold an address corresponding the the low byte of whichever TMS register is mapped from Z80 register B.
Register 14 hold an address corresponding the the low byte of whichever TMS register is mapped from Z80 register D.
Register 15 hold an address corresponding the the low byte of whichever TMS register is mapped from Z80 register H.

For example, assume that Z80 register B is mapped to TMS register 2.
Additionally, assume that the TMS workspace is set to address >8300.
In this situation, the high byte of workspace register 2 is located at address >8304 and the low byte is located at >8305.
Register 13 is always supposed to hold the address of the low byte of the TMS workspace register that is mapped from Z80 register B.
So, in this example register 13 holds the value >8305.
The value of register 13 is not affected by mapping of Z80 register C.
In this example, register B and C can both be mapped to TMS register 2, 
or register C can be mapped to a different TMS register,
but TMS register 13 will hold the value >8305 in either situation.
The value in register 13 only changes based on the mapping of register B.
If register B is mapped to TMS register 4 when the workspace is at address >8300,
then register 13 will hold the value >8309.

TMS Register 0 is reserved for different Z80 to TMS9900 translations to use.
See some of the "Load Immediate" unit tests in this file for exmaples: https://github.com/bkrug/Z80TMS9900Transpiler/blob/master/TMS9900TranslatingTests/LoadEightBitTests.cs

Like in most TMS9900 assembly language programs,
TMS Register 11 is reserved for storing the return address of a "Branch Link" operation.
