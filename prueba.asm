;Archivo: prueba.cpp
;Fecha: 26/10/2022 09:59:43 a. m.
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
	k DW ?
	c DW ?
inicioFor0:
MOV AX, 0
PUSH AX
POP AX
MOV i, AX
MOV AX, 1
PUSH AX
POP BX
POP AX
CMP AX, BX
JGE 
inicioFor1:
MOV AX, 0
PUSH AX
POP AX
MOV j, AX
MOV AX, 1
PUSH AX
POP BX
POP AX
CMP AX, BX
JGE 
MOV AX, 1
PUSH AX
POP BX
POP AX
ADD AX,BX
PUSH AX
POP AX
MOV x, AX
inicioFor2:
MOV AX, 0
PUSH AX
POP AX
MOV k, AX
MOV AX, 1
PUSH AX
POP BX
POP AX
CMP AX, BX
JGE 
inicioFor3:
MOV AX, 0
PUSH AX
POP AX
MOV c, AX
MOV AX, 1
PUSH AX
POP BX
POP AX
CMP AX, BX
JGE 
MOV AX, 2
PUSH AX
POP BX
POP AX
ADD AX,BX
PUSH AX
POP AX
MOV y, AX
MOV AX, 1
PUSH AX
POP BX
POP AX
CMP AX, BX
JGE 
MOV AX, 2
PUSH AX
POP BX
POP AX
ADD AX,BX
PUSH AX
POP AX
MOV y, AX
finFor3:
MOV AX, 1
PUSH AX
POP BX
POP AX
CMP AX, BX
JGE 
inicioFor4:
MOV AX, 0
PUSH AX
POP AX
MOV c, AX
MOV AX, 1
PUSH AX
POP BX
POP AX
CMP AX, BX
JGE 
MOV AX, 2
PUSH AX
POP BX
POP AX
ADD AX,BX
PUSH AX
POP AX
MOV y, AX
finFor4:
finFor2:
MOV AX, 1
PUSH AX
POP BX
POP AX
CMP AX, BX
JGE 
MOV AX, 1
PUSH AX
POP BX
POP AX
ADD AX,BX
PUSH AX
POP AX
MOV x, AX
inicioFor5:
MOV AX, 0
PUSH AX
POP AX
MOV k, AX
MOV AX, 1
PUSH AX
POP BX
POP AX
CMP AX, BX
JGE 
inicioFor6:
MOV AX, 0
PUSH AX
POP AX
MOV c, AX
MOV AX, 1
PUSH AX
POP BX
POP AX
CMP AX, BX
JGE 
MOV AX, 2
PUSH AX
POP BX
POP AX
ADD AX,BX
PUSH AX
POP AX
MOV y, AX
finFor6:
finFor5:
finFor1:
MOV AX, 1
PUSH AX
POP BX
POP AX
CMP AX, BX
JGE 
inicioFor7:
MOV AX, 0
PUSH AX
POP AX
MOV j, AX
MOV AX, 1
PUSH AX
POP BX
POP AX
CMP AX, BX
JGE 
MOV AX, 1
PUSH AX
POP BX
POP AX
ADD AX,BX
PUSH AX
POP AX
MOV x, AX
inicioFor8:
MOV AX, 0
PUSH AX
POP AX
MOV k, AX
MOV AX, 1
PUSH AX
POP BX
POP AX
CMP AX, BX
JGE 
inicioFor9:
MOV AX, 0
PUSH AX
POP AX
MOV c, AX
MOV AX, 1
PUSH AX
POP BX
POP AX
CMP AX, BX
JGE 
MOV AX, 2
PUSH AX
POP BX
POP AX
ADD AX,BX
PUSH AX
POP AX
MOV y, AX
finFor9:
finFor8:
finFor7:
finFor0:
RET
END