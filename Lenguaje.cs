//Marco Adrián Domínguez Jiménez
using System;
using System.Collections.Generic;
//Requerimiento 1: Actualizacion:
//                 a)agregar el residuo de la division en porfactor - HECHO
//                 b)agregar en instruccion los incrementos de termino y los incrementos de factor
//                   a++, a--, a+=1, a-=1, a*=1, a/=1, a%=1
//                   en donde el uno puede ser una expresion 
//                 c)programar el destructor para ejecutar el metodo cerrarArchivo()
//                  #libreria especial
//Requerimiento 2:    
//                 a)Marcar errores semanticos cuando los incrementos de termino o incrementos de factor
//                   superen el rango de la variable
//                 b)Considerar el inciso b y c del requerimiento 1 para el for
//                 c)Que funcione el do y el while
//Requerimiento 3:
//                 a)Considerar las variables y los casteos de las expresiones matematicas en ensamblador 
//                 b)Considerar el residuo de la division en ensamblador   
//                 c)Programar el printf y el scanf (procedimientos) en ensamblador
//Requerimiento 4: 
//                 a)Programar el else en ensamblador
//                 b)Programar el for en ensamblador
//Requerimiento 5:
//                 a)Programar el while en ensamblador
//                 b)Programar el do while en ensamblador
namespace Semantica 
{
    public class Lenguaje : Sintaxis
    {
        List<Variable> variables = new List<Variable>();
        Stack<float> stack = new Stack<float>();
        Variable.TipoDato dominante;
        int cIf;
        int cFOR;

        int cWHILE;

        int cDoWhile;

        public Lenguaje()
        {
            cIf = cFOR = cWHILE = 0;
        }
        public Lenguaje(string nombre) : base(nombre)
        {
            cIf = cFOR = cWHILE = 0;
        }
        ~Lenguaje()
        {
            Console.WriteLine("\n<<Destructor>>");
            //cerrar();

        }
        private void addVariable(String nombre, Variable.TipoDato tipo)
        {
            variables.Add(new Variable(nombre, tipo));
        }
        private void displayVariables()
        {
            log.WriteLine(" ");
            log.WriteLine("Variables: ");
            foreach (Variable v in variables)
            {
                log.WriteLine(v.getNombre() + " " + v.getTipo() + " " + v.getValor());
            }
        }
        private void VariableAsm()
        {
            asm.WriteLine(";Variables: ");
            foreach (Variable v in variables)
            {
                    asm.WriteLine("\t" + v.getNombre() + " DW ?");
            }
        }
        private bool existeVariable(string nombre)
        {
            foreach (Variable v in variables)
            {
                if (v.getNombre().Equals(nombre))
                {
                    return true;
                }
            }
            return false;
        }
        private void ModificaValor(string nombre, float nuevoValor)
        {
            foreach (Variable v in variables)
            {
                if (v.getNombre().Equals(nombre))
                {
                    v.setValor(nuevoValor);
                }
            }

        }
        private float Convertir(float valor, Variable.TipoDato dominante)
        {
            if (dominante == Variable.TipoDato.Char && valor > 255)
            {
                valor = valor % 256;
                return valor;
            }
            else if (dominante == Variable.TipoDato.Int && valor > 65535)
            {
                valor = valor % 65536;
                return valor;
            }
            else
            {
                return valor;
            }

        }
        private float getValor(string nombreVariable)
        {
            foreach (Variable v in variables)
            {
                if (v.getNombre().Equals(nombreVariable))
                {
                    return v.getValor();
                }
            }
            return 0;
        }
        private Variable.TipoDato getTipo(string nombreVariable)
        {
            foreach (Variable v in variables)
            {
                if (v.getNombre().Equals(nombreVariable))
                {
                    return v.getTipo();
                }
            }
            return Variable.TipoDato.Char;
        }
        //Programa  -> Librerias? Variables? Main
        public void Programa()
        {
            asm.WriteLine("#make_COM#");
            asm.WriteLine("include 'emu8086.inc'");
            asm.WriteLine("ORG 100h");
            Libreria();
            Variables();
            VariableAsm();
            Main();
            displayVariables();
            asm.WriteLine("END");
            asm.WriteLine("RET");
            asm.WriteLine("DEFINE_SCAN_NUM");

        }
        //Librerias -> #include<identificador(.h)?> Librerias?
        private void Libreria()
        {
            if (getContenido() == "#")
            {
                match("#");
                match("include");
                match("<");
                match(Tipos.Identificador);
                if (getContenido() == ".")
                {
                    match(".");
                    match("h");
                }
                match(">");
                Libreria();
            }
        }
        //Variables -> tipo_dato Lista_identificadores; Variables?
        private void Variables()
        {
            if (getClasificacion() == Tipos.TipoDato)
            {
                Variable.TipoDato tipo = Variable.TipoDato.Char;
                switch (getContenido())
                {
                    case "int": tipo = Variable.TipoDato.Int; break;
                    case "float": tipo = Variable.TipoDato.Float; break;
                }
                match(Tipos.TipoDato);
                Lista_identificadores(tipo);
                match(Tipos.FinSentencia);
                Variables();
            }
        }
        //Lista_identificadores -> identificador (,Lista_identificadores)?
        private void Lista_identificadores(Variable.TipoDato tipo)
        {
            if (getClasificacion() == Tipos.Identificador)
            {
                if (!existeVariable(getContenido()))
                {
                    addVariable(getContenido(), tipo);
                }
                else
                {
                    throw new Error("Error de sintaxis, variable duplicada <" + getContenido() + "> en linea: " + linea, log);
                }
            }
            match(Tipos.Identificador);
            if (getContenido() == ",")
            {
                match(",");
                Lista_identificadores(tipo);
            }
        }
        private void Main()
        {
            match("void");
            match("main");
            match("(");
            match(")");
            BloqueInstrucciones(true,false);
        }
        //Bloque de instrucciones -> {listaIntrucciones?}
        private void BloqueInstrucciones(bool evaluacion, bool ejecutado)
        {
            match("{");
            if (getContenido() != "}")
            {
                ListaInstrucciones(evaluacion,ejecutado);
            }
            match("}");
        }
        //ListaInstrucciones -> Instruccion ListaInstrucciones?
        private void ListaInstrucciones(bool evaluacion, bool ejecutado)
        {
            Instruccion(evaluacion,ejecutado);
            if (getContenido() != "}")
            {
                ListaInstrucciones(evaluacion,ejecutado);
            }
        }
        //ListaInstruccionesCase -> Instruccion ListaInstruccionesCase?
        private void ListaInstruccionesCase(bool evaluacion, bool ejecutado)
        {
            Instruccion(evaluacion,ejecutado);
            if (getContenido() != "case" && getContenido() != "break" && getContenido() != "default" && getContenido() != "}")
            {
                ListaInstruccionesCase(evaluacion,ejecutado);
            }
        }
        //Instruccion -> Printf | Scanf | If | While | do while | For | Switch | Asignacion
        private void Instruccion(bool evaluacion, bool ejecutado)
        {
            if (getContenido() == "printf")
            {
                Printf(evaluacion,ejecutado);
            }
            else if (getContenido() == "scanf")
            {
                Scanf(evaluacion,ejecutado);
            }
            else if (getContenido() == "if")
            {
                If(evaluacion, ejecutado);
            }
            else if (getContenido() == "while")
            {
                While(evaluacion, ejecutado);
            }
            else if (getContenido() == "do")
            {
                Do(evaluacion, ejecutado);
            }
            else if (getContenido() == "for")
            {
                For(evaluacion, ejecutado);
            }
            else if (getContenido() == "switch")
            {
                Switch(evaluacion, ejecutado);
            }
            else
            {
                Asignacion(evaluacion,ejecutado);
            }
        }
        private Variable.TipoDato EvaluaNumero(float resultado)
        {
            if (resultado % 1 != 0)
            {
                return Variable.TipoDato.Float;
            }
            if (resultado <= 255)
            {
                return Variable.TipoDato.Char;
            }
            else if (resultado <= 65535)
            {
                return Variable.TipoDato.Int;
            }
            return Variable.TipoDato.Float;
        }
        private bool EvaluaSemantica(string variable, float resultado)
        {
            Variable.TipoDato tipoDato = getTipo(variable);

            return false;
        }

        //Asignacion -> identificador = cadena | Expresion;
        private void Asignacion(bool evaluacion, bool ejecutado)
        {
            if (existeVariable(getContenido()))
            {
                log.WriteLine();
                log.Write(getContenido() + " = ");
                string nombre = getContenido();
                match(Tipos.Identificador);
                if (getClasificacion() == Tipos.IncrementoTermino || getClasificacion() == Tipos.IncrementoFactor)
                {
                    //Requerimiento 1.b
                    Incremento(nombre, evaluacion, ejecutado);
                    match(";");
                    //Requetimiento 1.c
                }
                else
                {
                    match(Tipos.Asignacion);
                    Expresion(ejecutado);
                    match(";");
                    if(!ejecutado)
                    {
                        asm.WriteLine("POP AX");
                    }
                    float resultado = stack.Pop();
                    log.Write("= " + resultado);
                    log.WriteLine();
                    //dominante = Variable.TipoDato.Char;
                    //Console.Write(dominante);
                    //Console.WriteLine(EvaluaNumero(resultado));
                    if (dominante < EvaluaNumero(resultado))
                    {
                        dominante = EvaluaNumero(resultado);
                    }
                    if (dominante <= getTipo(nombre))
                    {
                        if (evaluacion)
                        {
                            ModificaValor(nombre, resultado);
                            //asm.WriteLine("MOV "+ nombre + ", AX");
                        }
                    }
                    else
                    {
                        throw new Error("Error de sintaxis, no podemos asignar un: < " + dominante + " > a un: " + getTipo(nombre) + " > en linea " + linea, log);
                    }
                    if(!ejecutado)
                    {
                        asm.WriteLine("MOV " + nombre + ", AX");
                    }
                }

            }
            else
            {
                throw new Error("Error de sintaxis, no existe la variable < " + getContenido() + " > en linea: " + linea, log);
            }
        }

        //While -> while(Condicion) bloque de instrucciones | instruccion
        private void While(bool evaluacion, bool ejecutado)
        {
            string inicioWhile = "inicioWhile" + cWHILE;
            string finWhile = "finWhile" + cWHILE++;
    
            match("while");
            match("(");
            bool validarWhile = false;
            int linea_1 = getLinea();
            int cont_1 = getCont();
            if(!ejecutado)
            {
                asm.WriteLine(inicioWhile + ":");
            }
            do{
                validarWhile = Condicion(finWhile,ejecutado);

                if (!evaluacion)
                {
                validarWhile = false;
                }   
                match(")");
                if (getContenido() == "{")
                {
                BloqueInstrucciones(validarWhile, ejecutado);
                }
                else
                {
                Instruccion(validarWhile, ejecutado);
                }
                if (validarWhile)
                {
                    archivo.DiscardBufferedData();
                    archivo.BaseStream.Seek(cont_1+2, SeekOrigin.Begin);
                    NextToken();
                    //Console.WriteLine(getContenido());
                    setLinea(linea_1);
                    setCont(cont_1);
                }
                if(!ejecutado)
                {
                    asm.WriteLine("JMP " + inicioWhile);
                    asm.WriteLine(finWhile + ":");
                    ejecutado = true;
                }
            }while(validarWhile);
        }

        //Do -> do bloque de instrucciones | intruccion while(Condicion)
        private void Do(bool evaluacion, bool ejecutado)
        {
            string iniciarDoWhile = "iniciarDoWhile" + cDoWhile;
            string finDoWhile = "finDoWhile" + cDoWhile++;
            bool validarDo = true;
            match("do");
            int linea_2 = getLinea();
            int cont_2 = getCont();
            if(!ejecutado)
            {
                asm.WriteLine(iniciarDoWhile + ":");
            }
            do
            {
                if (getContenido() == "{")
                {
                    BloqueInstrucciones(validarDo && evaluacion,ejecutado);
                }
                else
                {
                    Instruccion(validarDo && evaluacion,ejecutado);
                }

                match("while");
                match("(");
 
                validarDo = Condicion(finDoWhile,ejecutado) ;

                match(")");
                if(evaluacion && validarDo)
                {
                    archivo.DiscardBufferedData();
                    archivo.BaseStream.Seek(cont_2+2, SeekOrigin.Begin);
                    NextToken();
                    //Console.WriteLine(getContenido());
                    setLinea(linea_2);
                    setCont(cont_2);
                }
                if(!ejecutado)
                {
                    asm.WriteLine("JMP " + iniciarDoWhile);
                    asm.WriteLine(finDoWhile + ":");
                    ejecutado = true;
                }
            }while(validarDo && evaluacion);   
            match(";");
        }
        //For -> for(Asignacion Condicion; Incremento) BloqueInstruccones | Intruccion 
        private void For(bool evaluacion, bool ejecutado)
        {
            string nombre = "", operador = "";
            float guardar = 0;
            string etiquetaInicioFor = "inicioFor" + cFOR;
            string etiquetaFinFor = "finFor" + cFOR++;
            
            bool validarFor;
            match("for");
            match("(");
            Asignacion(evaluacion, ejecutado);

            int cont_1 = getCont();
            int linea_1 = getLinea();

            //a) Necesito guardar la posicion del archivo de texto 
            //Console.WriteLine("antes condicion "+getContenido());
            if(!ejecutado)
            {
                asm.WriteLine(etiquetaInicioFor + ":");
            }
            do
            {
                
                //Console.WriteLine("Evaluar condi  "+getContenido());
                validarFor = Condicion(etiquetaFinFor,ejecutado);
                if (!evaluacion)
                {
                    validarFor = false;
                }

                //b) Agregar un ciclo while depues de validar el For 
                //While()
                // {
                match(";");
                nombre = getContenido();
                match(Tipos.Identificador);
                //Console.WriteLine("despues del ; "+getContenido());
                operador = getContenido();
                guardar = Incremento(ejecutado);
                match(")");

                if (getContenido() == "{")
                {

                    BloqueInstrucciones(validarFor, ejecutado);

                }
                else
                {
                    Instruccion(validarFor, ejecutado);

                }

                if (validarFor)
                {
                    switch (operador)
                    {
                        case "*=":
                            if (EvaluaNumero(getValor(nombre) * guardar) <= getTipo(nombre))
                            {
                                ModificaValor(nombre, getValor(nombre) * guardar);
                                if(!ejecutado)
                                {
                                    asm.WriteLine("MUL " + nombre + ", " + guardar);

                                }
                            }
                            else
                            {
                                throw new Error("Error de semantica, no podemos asignar un: < " + EvaluaNumero(getValor(nombre) * guardar) + " > a un: " + getTipo(nombre) + " > en linea " + linea, log);
                            }
                            break;
                        case "/=":
                            if (EvaluaNumero(getValor(nombre) / guardar) <= getTipo(nombre))
                            {
                                ModificaValor(nombre, getValor(nombre) / guardar);
                                if(!ejecutado)
                                {
                                    asm.WriteLine("DIV " + nombre + ", " + guardar);

                                }
                            }
                            else
                            {
                                throw new Error("Error de semantica, no podemos asignar un: < " + EvaluaNumero(getValor(nombre) / guardar) + " > a un: " + getTipo(nombre) + " > en linea " + linea, log);
                            }
                            break;
                        case "%=":
                            if (EvaluaNumero(getValor(nombre) % guardar) <= getTipo(nombre))
                            {
                                ModificaValor(nombre, getValor(nombre) % guardar);
                                if(!ejecutado)
                                {
                                    asm.WriteLine("MOD " + nombre + ", " + guardar);

                                }
                            }
                            else
                            {
                                throw new Error("Error de semantica, no podemos asignar un: < " + EvaluaNumero(getValor(nombre) % guardar) + " > a un: " + getTipo(nombre) + " > en linea " + linea, log);
                            }
                            break;
                        case "++":
                            if (EvaluaNumero(getValor(nombre) + guardar) <= getTipo(nombre))
                            {
                                ModificaValor(nombre, getValor(nombre) + guardar);
                                if(!ejecutado)
                                {
                                    asm.WriteLine("INC " + nombre );

                                }
                            }
                            else
                            {
                                throw new Error("Error de semantica, no podemos asignar un: < " + EvaluaNumero(getValor(nombre) + guardar) + " > a un: " + getTipo(nombre) + " > en linea " + linea, log);
                            }
                            break;
                        case "--":
                            if (EvaluaNumero(getValor(nombre) + guardar) <= getTipo(nombre))
                            {
                                ModificaValor(nombre, getValor(nombre) + guardar);
                                if(!ejecutado)
                                {
                                    asm.WriteLine("DEC " + nombre );

                                }
                            }
                            else
                            {
                                throw new Error("Error de semantica, no podemos asignar un: < " + EvaluaNumero(getValor(nombre) + guardar) + " > a un: " + getTipo(nombre) + " > en linea " + linea, log);
                            }
                            break;
                        case "+=":
                            if (EvaluaNumero(getValor(nombre) + guardar) <= getTipo(nombre))
                            {
                                ModificaValor(nombre, getValor(nombre) + guardar);
                                if(!ejecutado)
                                {
                                    asm.WriteLine("ADD " + nombre + ", " + guardar);

                                }
                            }
                            else
                            {
                                throw new Error("Error de semantica, no podemos asignar un: < " + EvaluaNumero(getValor(nombre) + guardar) + " > a un: " + getTipo(nombre) + " > en linea " + linea, log);
                            }
                            break;
                        case "-=":
                            if (EvaluaNumero(getValor(nombre) + guardar) <= getTipo(nombre))
                            {
                                ModificaValor(nombre, getValor(nombre) + guardar);
                                if(!ejecutado)
                                {
                                    asm.WriteLine("SUB " + nombre + ", " + guardar);

                                }
                            }
                            else
                            {
                                throw new Error("Error de semantica, no podemos asignar un: < " + EvaluaNumero(getValor(nombre) + guardar) + " > a un: " + getTipo(nombre) + " > en linea " + linea, log);
                            }
                            break;
                    }
                    archivo.DiscardBufferedData();
                    archivo.BaseStream.Seek(cont_1 + 1, SeekOrigin.Begin);
                    NextToken();
                    setCont(cont_1);
                    setLinea(linea_1);
                    //Console.WriteLine("despues del incremento  "+getContenido());
                }
                if(!ejecutado)
                {
                    asm.WriteLine("JMP " + etiquetaInicioFor);
                    asm.WriteLine(etiquetaFinFor + ":");
                    ejecutado = true;
                }
            } while (validarFor);
            
            //C) Regresar a la posicion de la lectura del archivo
            //d) Sacar otro token
        }

        //Incremento -> Identificador ++ | --
        private void Incremento(string variable, bool evaluacion, bool ejecutado)
        {
            if (existeVariable(variable))
            {
                if (getContenido() == "++")
                {
                    match("++");
                    if (evaluacion)
                    {

                        if (EvaluaNumero(getValor(variable) + 1) <= getTipo(variable))
                        {
                            ModificaValor(variable, getValor(variable) + 1);
                            if(!ejecutado)
                                {
                                    asm.WriteLine("INC " + variable);

                                }
                        }
                        else
                        {
                            throw new Error("Error de semantica, no podemos asignar un: < " + EvaluaNumero(getValor(variable) + 1) + " > a un: " + getTipo(variable) + " > en linea " + linea, log);
                        }

                    }
                }
                else if (getContenido() == "+=")
                {
                    match("+=");
                    if (evaluacion)
                    {
                        Expresion(ejecutado);
                        float resultado = stack.Pop();
                        if (EvaluaNumero(getValor(variable) + resultado) <= getTipo(variable))
                        {
                            ModificaValor(variable, getValor(variable) + resultado);
                            if(!ejecutado)
                            {
                                asm.WriteLine("ADD " + variable + ", " + resultado);

                            }
                            
                        }
                        else
                        {
                            throw new Error("Error de semantica, no podemos asignar un: < " + EvaluaNumero(getValor(variable) + resultado) + " > a un: " + getTipo(variable) + " > en linea " + linea, log);
                        }
                    }
                }
                else if (getContenido() == "-=")
                {
                    match("-=");
                    if (evaluacion)
                    {
                        Expresion(ejecutado);
                        float resultado = stack.Pop();
                        if (EvaluaNumero(getValor(variable) - resultado) <= getTipo(variable))
                        {
                            ModificaValor(variable, getValor(variable) - resultado);
                            if(!ejecutado)
                            {
                                    asm.WriteLine("SUB " + variable + ", " + resultado);

                            }
                        }
                        else
                        {
                            throw new Error("Error de semantica, no podemos asignar un: < " + EvaluaNumero(getValor(variable) - resultado) + " > a un: " + getTipo(variable) + " > en linea " + linea, log);
                        }
                    }
                }
                else if (getContenido() == "*=")
                {
                    match("*=");
                    if (evaluacion)
                    {
                        Expresion(ejecutado);
                        float resultado = stack.Pop();
                        if (EvaluaNumero(getValor(variable) * resultado) <= getTipo(variable))
                        {
                            ModificaValor(variable, getValor(variable) * resultado);
                            if(!ejecutado)
                            {
                                asm.WriteLine("MUL " + variable + ", " + resultado);

                            }
                        }
                        else
                        {
                            throw new Error("Error de semantica, no podemos asignar un: < " + EvaluaNumero(getValor(variable) * resultado) + " > a un: " + getTipo(variable) + " > en linea " + linea, log);
                        }
                    }
                }
                else if (getContenido() == "/=")
                {
                    match("/=");
                    if (evaluacion)
                    {
                        Expresion(ejecutado);
                        float resultado = stack.Pop();
                        if (EvaluaNumero(getValor(variable) / resultado) <= getTipo(variable))
                        {
                            ModificaValor(variable, getValor(variable) / resultado);
                            if(!ejecutado)
                            {
                                asm.WriteLine("DIV " + variable + ", " + resultado);

                            }
                        }
                        else
                        {
                            throw new Error("Error de semantica, no podemos asignar un: < " + EvaluaNumero(getValor(variable) / resultado) + " > a un: " + getTipo(variable) + " > en linea " + linea, log);
                        }
                    }
                }
                else if (getContenido() == "%=")
                {
                    match("%=");
                    if (evaluacion)
                    {
                        Expresion(ejecutado);
                        float resultado = stack.Pop();
                        if (EvaluaNumero(getValor(variable) % resultado) <= getTipo(variable))
                        {
                            ModificaValor(variable, getValor(variable) % resultado);
                            if(!ejecutado)
                            {
                                asm.WriteLine("MOD " + variable + ", " + resultado);

                            }
                        }
                        else
                        {
                            throw new Error("Error de semantica, no podemos asignar un: < " + EvaluaNumero(getValor(variable) % resultado) + " > a un: " + getTipo(variable) + " > en linea " + linea, log);
                        }
                    }
                }
                else
                {
                    match("--");
                    if (evaluacion)
                    {
                        if (EvaluaNumero(getValor(variable) - 1) <= getTipo(variable))
                        {
                            ModificaValor(variable, getValor(variable) - 1);
                            if(!ejecutado)
                            {
                                asm.WriteLine("DEC " + variable);

                            }
                        }
                        else
                        {
                            throw new Error("Error de semantica, no podemos asignar un: < " + EvaluaNumero(getValor(variable) - 1) + " > a un: " + getTipo(variable) + " > en linea " + linea, log);
                        }
                    }
                }
            }
            else
            {
                throw new Error("Error de sintaxis, no existe la variable <" + getContenido() + "> en linea: " + linea, log);
            }
        }
        private float Incremento(bool ejecutado)
        {
            if (getContenido() == "++")
            {
                match("++");
                return 1;
            }
            else if (getContenido() == "+=")
            {
                match("+=");
                Expresion(ejecutado);
                float resultado = stack.Pop();
                return resultado;
            }
            else if (getContenido() == "-=")
            {
                match("-=");
                Expresion(ejecutado);
                float resultado = stack.Pop();
                return resultado * (-1);
            }
            else if (getContenido() == "*=")
            {
                match("*=");
                Expresion(ejecutado);
                float resultado = stack.Pop();
                return resultado;
            }
            else if (getContenido() == "/=")
            {
                match("/=");
                Expresion(ejecutado);
                float resultado = stack.Pop();
                return resultado;
            }
            else if (getContenido() == "%=")
            {
                match("%=");
                Expresion(ejecutado);
                float resultado = stack.Pop();
                return resultado;
            }
            else
            {
                match("--");
                return -1;
            }
        }

        //Switch -> switch (Expresion) {Lista de casos} | (default: )
        private void Switch(bool evaluacion, bool ejecutado)
        {
            match("switch");
            match("(");
            Expresion(ejecutado);
            stack.Pop();
            if(!ejecutado)
            {
                asm.WriteLine("POP AX");
            }
            match(")");
            match("{");
            ListaDeCasos(evaluacion, ejecutado);
            if (getContenido() == "default")
            {
                match("default");
                match(":");
                if (getContenido() == "{")
                {
                    BloqueInstrucciones(evaluacion, ejecutado);
                }
                else
                {
                    Instruccion(evaluacion, ejecutado);
                }
            }
            match("}");
        }

        //ListaDeCasos -> case Expresion: listaInstruccionesCase (break;)? (ListaDeCasos)?
        private void ListaDeCasos(bool evaluacion, bool ejecutado)
        {
            match("case");
            Expresion(ejecutado);
            stack.Pop();
            if(!ejecutado)
            {
                asm.WriteLine("POP AX");
            }
            match(":");
            ListaInstruccionesCase(evaluacion, ejecutado);
            if (getContenido() == "break")
            {
                match("break");
                match(";");
            }
            if (getContenido() == "case")
            {
                ListaDeCasos(evaluacion, ejecutado);
            }
        }

        //Condicion -> Expresion operador relacional Expresion
        private bool Condicion(string etiqueta, bool ejecutado)
        {
            Expresion(ejecutado);
            string operador = getContenido();
            match(Tipos.OperadorRelacional);
            Expresion(ejecutado);
            float e2 = stack.Pop();
            if(!ejecutado)
            {
                asm.WriteLine("POP BX");
            }
            float e1 = stack.Pop();
            if(!ejecutado)
            {
                asm.WriteLine("POP AX");
                asm.WriteLine("CMP AX, BX");
            }
            switch (operador)
            {
                case "==":
                    if(!ejecutado)
                    {
                        asm.WriteLine("JNE " + etiqueta);
                    }
                    return e1 == e2;
                case "<=":
                    if(!ejecutado)
                    {
                        asm.WriteLine("JG " + etiqueta);
                    }
                    return e1 <= e2;
                case ">=":
                    if(!ejecutado)
                    {
                        asm.WriteLine("JL " + etiqueta);
                    }
                    return e1 >= e2;
                case "<":
                if(!ejecutado)
                    {
                        asm.WriteLine("JGE " + etiqueta);
                    }
                    return e1 < e2;
                case ">":
                    if(!ejecutado)
                    {
                        asm.WriteLine("JLE " + etiqueta);
                    }
                    return e1 > e2;
                default:
                    if(!ejecutado)
                    {
                        asm.WriteLine("JE " + etiqueta);
                    }
                    return e1 != e2;
            }
        }

        //If -> if(Condicion) bloque de instrucciones (else bloque de instrucciones)?
        private void If(bool evaluacion, bool ejecutado)
        {
            string etiquetaIf = "if" + (++cIf);
            string etiquetaElse = "else" + (cIf);
            match("if");
            match("(");

            bool validarIf = Condicion(etiquetaIf,ejecutado);
            if (!evaluacion)
            {
                validarIf = false;
            }
            //Console.WriteLine(Condicion());
            match(")");
            if (getContenido() == "{")
            {
                BloqueInstrucciones(validarIf, ejecutado);
            }
            else
            {
                Instruccion(validarIf, ejecutado);
            }
            if(!ejecutado)
            {
                asm.WriteLine("JMP " + etiquetaElse);
                asm.WriteLine(etiquetaIf + ":");
            }
            if (getContenido() == "else")
            {
                match("else");
                if (!evaluacion)
                {
                    validarIf = true;
                }
                //Requerimiento 4
                if (getContenido() == "{")
                {
                    BloqueInstrucciones(!validarIf, ejecutado);

                }
                else
                {
                    Instruccion(!validarIf, ejecutado);
                }
            }
            if(!ejecutado)
            {
                asm.WriteLine(etiquetaElse + ":");
            }
            
        }

        //Printf -> printf(cadena|expresion);
        private void Printf(bool evaluacion, bool ejecutado)
        {
            match("printf");
            match("(");
            string arreglo = getContenido();
            string contenido = "";
            if (getClasificacion() == Tipos.Cadena)
            {
                if (arreglo.Contains("\\n"))
                {
                    arreglo = arreglo.Replace("\\n", "\n");
                }
                if (arreglo.Contains("\\t"))
                {
                    arreglo = arreglo.Replace("\\t", "\t");
                }
                if (evaluacion)
                {
                    for (int i = 1; i < arreglo.Length - 1; i++)
                    {
                        contenido += arreglo[i];
                    }
                    Console.Write(contenido);
                }
                if(!ejecutado)
                {
                    asm.WriteLine("PRINTN \"" + contenido + "\"");
                }
                match(Tipos.Cadena);
            }
            else
            {
                Expresion(ejecutado);
                float resultado = stack.Pop();
                if(!ejecutado)
                {
                    asm.WriteLine("POP AX");
                }
                if (evaluacion)
                    Console.Write(resultado);
                //Codigo ensamblador para imprimir una variable
            }
            match(")");
            match(";");
        }

        //Scanf -> scanf(cadena, &, identf);
        private void Scanf(bool evaluacion, bool ejecutado)
        {
            match("scanf");
            match("(");
            match(Tipos.Cadena);
            match(",");
            match("&");
            if (existeVariable(getContenido()))
            {
                string nombre = getContenido();
                //ModificaValor(getContenido(),float.Parse(val));
                match(Tipos.Identificador);
                if (evaluacion)
                {
                    string val = "" + Console.ReadLine();

                    try
                    {
                        float valorFloat = float.Parse(val);
                        ModificaValor(nombre, valorFloat);
                    }
                    catch (Exception)
                    {
                        throw new Error("Error de sintaxis, no se puede asignar <" + val + "> en linea: " + linea, log);
                    }
                }
                match(")");
                match(";");
                if(!ejecutado)
                {
                    asm.WriteLine("CALL SCAN_NUM");
                    asm.WriteLine("MOV " + nombre + ", CX");
                }
            }
            else
            {
                throw new Error("Error de sintaxis, no existe la variable <" + getContenido() + "> en linea: " + linea, log);
            }
        }

        //Main      -> void main() Bloque de instrucciones
        //Expresion -> Termino MasTermino
        private void Expresion(bool ejecutado)
        {
            Termino(ejecutado);
            MasTermino(ejecutado);
        }
        //MasTermino -> (OperadorTermino Termino)?
        private void MasTermino(bool ejecutado)
        {
            if (getClasificacion() == Tipos.OperadorTermino)
            {
                string operador = getContenido();
                match(Tipos.OperadorTermino);
                Termino(ejecutado);
                log.Write(operador + " ");
                float n1 = stack.Pop();
                if(!ejecutado)
                {
                    asm.WriteLine("POP BX");
                }
                float n2 = stack.Pop();
                if(!ejecutado)
                {
                    asm.WriteLine("POP AX");
                }
                switch (operador)
                {
                    case "+":
                        stack.Push(n2 + n1);
                        if(!ejecutado)
                        {
                            asm.WriteLine("ADD AX, BX");
                            asm.WriteLine("PUSH AX");
                        }
                        break;
                    case "-":
                        stack.Push(n2 - n1);
                        if(!ejecutado)
                        {
                            asm.WriteLine("SUB AX, BX");
                            asm.WriteLine("PUSH AX");
                        }
                        break;
                }
            }
        }
        //Termino -> Factor PorFactor
        private void Termino(bool ejecutado)
        {
            Factor(ejecutado);
            PorFactor(ejecutado);
        }
        //PorFactor -> (OperadorFactor Factor)? 
        private void PorFactor(bool ejecutado)
        {
            if (getClasificacion() == Tipos.OperadorFactor)
            {
                string operador = getContenido();
                match(Tipos.OperadorFactor);
                Factor(ejecutado);
                log.Write(operador + " ");
                float n1 = stack.Pop();
                if(!ejecutado)
                {
                    asm.WriteLine("POP BX");
                }
                float n2 = stack.Pop();
                if(!ejecutado)
                {
                    asm.WriteLine("POP AX");
                }
                switch (operador)
                {
                    case "*":
                        stack.Push(n2 * n1);
                        if(!ejecutado)
                        {
                            asm.WriteLine("MUL BX");
                            asm.WriteLine("PUSH AX");
                        }
                        break;
                    case "/":
                        stack.Push(n2 / n1);
                        if(!ejecutado)
                        {
                            asm.WriteLine("DIV BX");
                            asm.WriteLine("PUSH AX");
                        }
                        break;
                    case "%":
                        stack.Push(n2 % n1);
                        if(!ejecutado)
                        {
                            asm.WriteLine("DIV BX");
                            asm.WriteLine("PUSH DX");
                        }
                        break;
                }
            }
        }
        //Factor -> numero | identificador | (Expresion)
        private void Factor(bool ejecutado)
        {
            if (getClasificacion() == Tipos.Numero)
            {
                log.Write(getContenido() + " ");
                log.Write(" ");
                if (dominante < EvaluaNumero(float.Parse(getContenido())))
                {
                    dominante = EvaluaNumero(float.Parse(getContenido()));
                }
                stack.Push(float.Parse(getContenido()));
                if(!ejecutado)
                {
                    asm.WriteLine("MOV AX, " + getContenido());
                    asm.WriteLine("PUSH AX");
                }
                match(Tipos.Numero);
            }
            else if (getClasificacion() == Tipos.Identificador)
            {

                if(!ejecutado)
                {
                    asm.WriteLine("MOV AX, " + getContenido());
                    asm.WriteLine("PUSH AX");
                }
                if (existeVariable(getContenido()))
                {
                    log.Write(getContenido() + " ");
                    //Requerimiento 1
                    log.Write(" ");
                    dominante = getTipo((getContenido()));
                    stack.Push(getValor(getContenido()));
                    //Requerimiento 3
                    match(Tipos.Identificador);
                }

                else
                {
                    throw new Error("Error de sintaxis, no existe la variable <" + getContenido() + "> en linea: " + linea, log);
                }
            }
            else
            {
                bool huboCasteo = false;
                Variable.TipoDato casteo = Variable.TipoDato.Char;
                match("(");
                if (getClasificacion() == Tipos.TipoDato)
                {
                    huboCasteo = true;
                    switch (getContenido())
                    {
                        case "char":
                            casteo = Variable.TipoDato.Char;
                            break;
                        case "int":
                            casteo = Variable.TipoDato.Int;
                            break;
                        case "float":
                            casteo = Variable.TipoDato.Float;
                            break;
                    }
                    match(Tipos.TipoDato);
                    match(")");
                    match("(");
                }
                Expresion(ejecutado);
                match(")");
                if (huboCasteo)
                {
                    dominante = casteo;
                    
                    float valores = stack.Pop();
                    if(!ejecutado)
                    {
                        asm.WriteLine("POP AX");
                    }

                    if (dominante != Variable.TipoDato.Float && valores % 1 != 0)
                    {
                        valores = MathF.Truncate(valores);
                    }
                    valores = Convertir(valores, dominante);
                    if(!ejecutado)
                    {
                        asm.WriteLine("MOV AX, " + valores);
                        asm.WriteLine("PUSH AX");
                    }
                    stack.Push(valores);

                }
            }
        }
    }
}