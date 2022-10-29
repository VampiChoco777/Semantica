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

        public Lenguaje()
        {
            cIf = cFOR = 0;
        }
        public Lenguaje(string nombre) : base(nombre)
        {
            cIf = cFOR = 0;
        }
        ~Lenguaje()
        {
            Console.WriteLine("Destructor");
            cerrar();

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
            asm.WriteLine("make COM");
            asm.WriteLine("include 'emu 8086.inc'");
            asm.WriteLine("ORG 100h");
            Libreria();
            Variables();
            VariableAsm();
            Main();
            displayVariables();
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
            BloqueInstrucciones(true);
        }
        //Bloque de instrucciones -> {listaIntrucciones?}
        private void BloqueInstrucciones(bool evaluacion)
        {
            match("{");
            if (getContenido() != "}")
            {
                ListaInstrucciones(evaluacion);
            }
            match("}");
        }
        //ListaInstrucciones -> Instruccion ListaInstrucciones?
        private void ListaInstrucciones(bool evaluacion)
        {
            Instruccion(evaluacion);
            if (getContenido() != "}")
            {
                ListaInstrucciones(evaluacion);
            }
        }
        //ListaInstruccionesCase -> Instruccion ListaInstruccionesCase?
        private void ListaInstruccionesCase(bool evaluacion)
        {
            Instruccion(evaluacion);
            if (getContenido() != "case" && getContenido() != "break" && getContenido() != "default" && getContenido() != "}")
            {
                ListaInstruccionesCase(evaluacion);
            }
        }
        //Instruccion -> Printf | Scanf | If | While | do while | For | Switch | Asignacion
        private void Instruccion(bool evaluacion)
        {
            if (getContenido() == "printf")
            {
                Printf(evaluacion);
            }
            else if (getContenido() == "scanf")
            {
                Scanf(evaluacion);
            }
            else if (getContenido() == "if")
            {
                If(evaluacion);
            }
            else if (getContenido() == "while")
            {
                While(evaluacion);
            }
            else if (getContenido() == "do")
            {
                Do(evaluacion);
            }
            else if (getContenido() == "for")
            {
                For(evaluacion);
            }
            else if (getContenido() == "switch")
            {
                Switch(evaluacion);
            }
            else
            {
                Asignacion(evaluacion);
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
        private void Asignacion(bool evaluacion)
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
                    Incremento(nombre, evaluacion);
                    match(";");
                    //Requetimiento 1.c
                }
                else
                {
                    match(Tipos.Asignacion);
                    Expresion();
                    match(";");
                    float resultado = stack.Pop();
                    asm.WriteLine("POP AX");
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
                    asm.WriteLine("MOV " + nombre + ", AX");
                }

            }
            else
            {
                throw new Error("Error de sintaxis, no existe la variable < " + getContenido() + " > en linea: " + linea, log);
            }
        }

        //While -> while(Condicion) bloque de instrucciones | instruccion
        private void While(bool evaluacion)
        {
            match("while");
            match("(");
            bool validarWhile = false;
            int linea_1 = getLinea();
            int cont_1 = getCont();
            do{
                validarWhile = Condicion("");

                if (!evaluacion)
                {
                validarWhile = false;
                }   
                match(")");
                if (getContenido() == "{")
                {
                BloqueInstrucciones(validarWhile);
                }
                else
                {
                Instruccion(validarWhile);
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
            }while(validarWhile);
        }

        //Do -> do bloque de instrucciones | intruccion while(Condicion)
        private void Do(bool evaluacion)
        {
            bool validarDo = true;
            match("do");
            int linea_2 = getLinea();
            int cont_2 = getCont();

            do
            {
                if (getContenido() == "{")
                {
                    BloqueInstrucciones(validarDo && evaluacion);
                }
                else
                {
                    Instruccion(validarDo && evaluacion);
                }

                match("while");
                match("(");
 
                validarDo = Condicion("") ;

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
            }while(validarDo && evaluacion);
            

            
            
            match(";");
        }
        //For -> for(Asignacion Condicion; Incremento) BloqueInstruccones | Intruccion 
        private void For(bool evaluacion)
        {
            string nombre = "", operador = "";
            float guardar = 0;
            string etiquetaInicioFor = "inicioFor" + cFOR;
            string etiquetaFinFor = "finFor" + cFOR++;
            asm.WriteLine(etiquetaInicioFor + ":");
            bool validarFor;
            match("for");
            match("(");
            Asignacion(evaluacion);
            //Requerimiento 4
            //Requerimiento 6: 
            int cont_1 = getCont();
            int linea_1 = getLinea();

            //a) Necesito guardar la posicion del archivo de texto 
            //Console.WriteLine("antes condicion "+getContenido());
            do
            {
                //Console.WriteLine("Evaluar condi  "+getContenido());
                validarFor = Condicion("");
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
                guardar = Incremento();
                match(")");

                if (getContenido() == "{")
                {

                    BloqueInstrucciones(validarFor);

                }
                else
                {
                    Instruccion(validarFor);

                }

                if (validarFor)
                {
                    switch (operador)
                    {
                        case "*=":
                            if (EvaluaNumero(getValor(nombre) * guardar) <= getTipo(nombre))
                            {
                                ModificaValor(nombre, getValor(nombre) * guardar);
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
                            }
                            else
                            {
                                throw new Error("Error de semantica, no podemos asignar un: < " + EvaluaNumero(getValor(nombre) % guardar) + " > a un: " + getTipo(nombre) + " > en linea " + linea, log);
                            }
                            break;
                        case "++":
                        case "--":
                        case "+=":
                        case "-=":
                            if (EvaluaNumero(getValor(nombre) + guardar) <= getTipo(nombre))
                            {
                                ModificaValor(nombre, getValor(nombre) + guardar);
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

            } while (validarFor);
            asm.WriteLine(etiquetaFinFor + ":");
            //C) Regresar a la posicion de la lectura del archivo
            //d) Sacar otro token
        }

        //Incremento -> Identificador ++ | --
        private void Incremento(string variable, bool evaluacion)
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
                        Expresion();
                        float resultado = stack.Pop();
                        if (EvaluaNumero(getValor(variable) + resultado) <= getTipo(variable))
                        {
                            ModificaValor(variable, getValor(variable) + resultado);
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
                        Expresion();
                        float resultado = stack.Pop();
                        if (EvaluaNumero(getValor(variable) - resultado) <= getTipo(variable))
                        {
                            ModificaValor(variable, getValor(variable) - resultado);
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
                        Expresion();
                        float resultado = stack.Pop();
                        if (EvaluaNumero(getValor(variable) * resultado) <= getTipo(variable))
                        {
                            ModificaValor(variable, getValor(variable) * resultado);
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
                        Expresion();
                        float resultado = stack.Pop();
                        if (EvaluaNumero(getValor(variable) / resultado) <= getTipo(variable))
                        {
                            ModificaValor(variable, getValor(variable) / resultado);
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
                        Expresion();
                        float resultado = stack.Pop();
                        if (EvaluaNumero(getValor(variable) % resultado) <= getTipo(variable))
                        {
                            ModificaValor(variable, getValor(variable) % resultado);
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
        private float Incremento()
        {
            if (getContenido() == "++")
            {
                match("++");
                return 1;
            }
            else if (getContenido() == "+=")
            {
                match("+=");
                Expresion();
                float resultado = stack.Pop();
                return resultado;
            }
            else if (getContenido() == "-=")
            {
                match("-=");
                Expresion();
                float resultado = stack.Pop();
                return resultado * (-1);
            }
            else if (getContenido() == "*=")
            {
                match("*=");
                Expresion();
                float resultado = stack.Pop();
                return resultado;
            }
            else if (getContenido() == "/=")
            {
                match("/=");
                Expresion();
                float resultado = stack.Pop();
                return resultado;
            }
            else if (getContenido() == "%=")
            {
                match("%=");
                Expresion();
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
        private void Switch(bool evaluacion)
        {
            match("switch");
            match("(");
            Expresion();
            stack.Pop();
            asm.WriteLine("POP AX");
            match(")");
            match("{");
            ListaDeCasos(evaluacion);
            if (getContenido() == "default")
            {
                match("default");
                match(":");
                if (getContenido() == "{")
                {
                    BloqueInstrucciones(evaluacion);
                }
                else
                {
                    Instruccion(evaluacion);
                }
            }
            match("}");
        }

        //ListaDeCasos -> case Expresion: listaInstruccionesCase (break;)? (ListaDeCasos)?
        private void ListaDeCasos(bool evaluacion)
        {
            match("case");
            Expresion();
            stack.Pop();
            asm.WriteLine("POP AX");
            match(":");
            ListaInstruccionesCase(evaluacion);
            if (getContenido() == "break")
            {
                match("break");
                match(";");
            }
            if (getContenido() == "case")
            {
                ListaDeCasos(evaluacion);
            }
        }

        //Condicion -> Expresion operador relacional Expresion
        private bool Condicion(string etiqueta)
        {
            Expresion();
            string operador = getContenido();
            match(Tipos.OperadorRelacional);
            Expresion();
            float e2 = stack.Pop();
            asm.WriteLine("POP BX");
            float e1 = stack.Pop();
            asm.WriteLine("POP AX");
            asm.WriteLine("CMP AX, BX");
            switch (operador)
            {
                case "==":
                    asm.WriteLine("JNE " + etiqueta);
                    return e1 == e2;
                case "<=":
                    asm.WriteLine("JG " + etiqueta);
                    return e1 <= e2;
                case ">=":
                    asm.WriteLine("JL " + etiqueta);
                    return e1 >= e2;
                case "<":
                    asm.WriteLine("JGE " + etiqueta);
                    return e1 < e2;
                case ">":
                    asm.WriteLine("JLE " + etiqueta);
                    return e1 > e2;
                default:
                    asm.WriteLine("JE " + etiqueta);
                    return e1 != e2;
            }
        }

        //If -> if(Condicion) bloque de instrucciones (else bloque de instrucciones)?
        private void If(bool evaluacion)
        {
            string etiquetaIf = "if" + (++cIf);
            match("if");
            match("(");

            bool validarIf = Condicion(etiquetaIf);
            if (!evaluacion)
            {
                validarIf = false;
            }
            //Console.WriteLine(Condicion());
            match(")");
            if (getContenido() == "{")
            {
                BloqueInstrucciones(validarIf);
            }
            else
            {
                Instruccion(validarIf);
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
                    BloqueInstrucciones(!validarIf);

                }
                else
                {
                    Instruccion(!validarIf);
                }
            }

            asm.WriteLine(etiquetaIf + ":");
        }

        //Printf -> printf(cadena|expresion);
        private void Printf(bool evaluacion)
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
                asm.WriteLine("PRINTN \"" + contenido + "\"");
                match(Tipos.Cadena);
            }
            else
            {
                Expresion();
                float resultado = stack.Pop();
                asm.WriteLine("POP AX");
                if (evaluacion)
                    Console.Write(resultado);
                //Codigo ensamblador para imprimir una variable
            }
            match(")");
            match(";");
        }

        //Scanf -> scanf(cadena, &, identf);
        private void Scanf(bool evaluacion)
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
                asm.WriteLine("CALL SCAN_NUM");
                asm.WriteLine("MOV " + nombre + ", CX");
            }
            else
            {
                throw new Error("Error de sintaxis, no existe la variable <" + getContenido() + "> en linea: " + linea, log);
            }
        }

        //Main      -> void main() Bloque de instrucciones
        //Expresion -> Termino MasTermino
        private void Expresion()
        {
            Termino();
            MasTermino();
        }
        //MasTermino -> (OperadorTermino Termino)?
        private void MasTermino()
        {
            if (getClasificacion() == Tipos.OperadorTermino)
            {
                string operador = getContenido();
                match(Tipos.OperadorTermino);
                Termino();
                log.Write(operador + " ");
                float n1 = stack.Pop();
                asm.WriteLine("POP BX");
                float n2 = stack.Pop();
                asm.WriteLine("POP AX");
                switch (operador)
                {
                    case "+":
                        stack.Push(n2 + n1);
                        asm.WriteLine("ADD AX,BX");
                        asm.WriteLine("PUSH AX");
                        break;
                    case "-":
                        stack.Push(n2 - n1);
                        asm.WriteLine("SUB AX,BX");
                        asm.WriteLine("PUSH AX");
                        break;
                }
            }
        }
        //Termino -> Factor PorFactor
        private void Termino()
        {
            Factor();
            PorFactor();
        }
        //PorFactor -> (OperadorFactor Factor)? 
        private void PorFactor()
        {
            if (getClasificacion() == Tipos.OperadorFactor)
            {
                string operador = getContenido();
                match(Tipos.OperadorFactor);
                Factor();
                log.Write(operador + " ");
                float n1 = stack.Pop();
                asm.WriteLine("POP BX");
                float n2 = stack.Pop();
                asm.WriteLine("POP AX");
                //Requerimiento 3
                switch (operador)
                {
                    case "*":
                        stack.Push(n2 * n1);
                        asm.WriteLine("MUL BX");
                        asm.WriteLine("PUSH AX");
                        break;
                    case "/":
                        stack.Push(n2 / n1);
                        asm.WriteLine("DIV BX");
                        asm.WriteLine("PUSH AX");
                        break;
                    case "%":
                        stack.Push(n2 % n1);
                        asm.WriteLine("DIV BX");
                        asm.WriteLine("PUSH DX");
                        break;
                }
            }
        }
        //Factor -> numero | identificador | (Expresion)
        private void Factor()
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
                asm.WriteLine("MOV AX, " + getContenido());
                asm.WriteLine("PUSH AX");
                match(Tipos.Numero);
            }
            else if (getClasificacion() == Tipos.Identificador)
            {

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
                Expresion();
                match(")");
                if (huboCasteo)
                {
                    dominante = casteo;

                    float valores = stack.Pop();
                    asm.WriteLine("POP AX");
                    if (dominante != Variable.TipoDato.Float && valores % 1 != 0)
                    {
                        valores = MathF.Truncate(valores);
                    }
                    valores = Convertir(valores, dominante);
                    stack.Push(valores);

                }
            }
        }
    }
}