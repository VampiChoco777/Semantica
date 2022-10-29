;Archivo: prueba.cpp
;Fecha: 29/10/2022 05:34:40 p. m.
make COM
include 'emu 8086.inc'
ORG 100h
;Variables: 
	area DW ?
	radio DW ?
	pi DW ?
	resultado DW ?
	a DW ?
	d DW ?
	altura DW ?
	x DW ?
	y DW ?
	i DW ?
	j DW ?
	l DW ?
	k DW ?
	p DW ?
MOV AX, 1
PUSH AX
MOV AX, 2
PUSH AX
POP BX
POP AX
CMP AX, BX
JNE if1
MOV AX, 0
PUSH AX
POP AX
MOV i, AX
POP AX
PRINTN ""
MOV AX, 1
PUSH AX
MOV AX, 2
PUSH AX
POP BX
POP AX
CMP AX, BX
JNE 
PRINTN "Holi crayoli"
if1:
RET
DEFINE_SCAN_NUM
