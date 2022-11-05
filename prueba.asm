;Archivo: prueba.cpp
;Fecha: 04/11/2022 09:18:05 a. m.
#make_COM#
include 'emu8086.inc'
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
MOV AX, 256
PUSH AX
POP AX
MOV AX, 0
PUSH AX
POP AX
MOV i, AX
MOV AX, i
PUSH AX
POP AX
END
RET
DEFINE_SCAN_NUM
