;Archivo: prueba.cpp
;Fecha: 31/10/2022 07:13:14 p. m.
#make_COM
include 'emu8086.inc'
ORG 1000h
inicioWhile 0:
MOV AX, 10
PUSH AX
POP BX
POP AX
CMP AX, BX
JGE finWhile 0
MOV AX, 0
PUSH AX
POP AX
MOV x, AX
MOV AX, 0
PUSH AX
POP AX
MOV j, AX
finWhile 0:
MOV AX, 20
PUSH AX
MOV AX, 8
PUSH AX
POP BX
POP AX
SUB AX, BX
PUSH AX
MOV AX, 2
PUSH AX
POP BX
POP AX
MUL BX
PUSH AX
MOV AX, 20
PUSH AX
MOV AX, 3
PUSH AX
POP BX
POP AX
SUB AX, BX
PUSH AX
POP BX
POP AX
DIV BX
PUSH AX
POP AX
MOV x, AX
RET
;Variables: 
	area DD ?
	radio DD ?
	pi DD ?
	resultado DD ?
	a DW ?
	d DW ?
	altura DW ?
	x DD ?
	y DW ?
	i DB ?
	j DW ?
	l DW ?
	k DW ?
	p DW ?
DEFINE_SCAN_NUM
