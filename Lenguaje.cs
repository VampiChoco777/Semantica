//Marco Adrián Domínguez Jiménez
using System;
using System.Collections.Generic;
//Requerimiento 1.- Actualizar el dominante para variables en la expresion
//                  Ejemplo = float x; char y; y = x;
//Requerimiento 2.- Actualizar el dominante para el casteo y el valor de la sub expresion
//Requerimiento 3.- Programar un metodo de connversion de un valor a un tipo de dato
//                  Funcion Convierte : private float Convert (float valor, string tipoDato)
//                  Deberan de usar el residuo de la division %255 entre %65535
namespace Semantica
{
    public class Lenguaje : Sintaxis
    {
        List <Variable> variables = new List<Variable>();
        Stack<float> stack = new Stack<float>();
        Variable.TipoDato dominante;

        public Lenguaje()
        {

        }
        public Lenguaje(string nombre) : base(nombre)
        {

        }
        private void addVariable(String nombre,Variable.TipoDato tipo)
        {
            variables.Add(new Variable(nombre, tipo));
        }
        private void displayVariables()
        {
            log.WriteLine(" ");
            log.WriteLine("Variables: ");
            foreach (Variable v in variables)
            {
                log.WriteLine(v.getNombre()+" "+v.getTipo()+" "+v.getValor());
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
            foreach(Variable v in variables)
            {
                if(v.getNombre().Equals(nombre))
                {
                    v.setValor(nuevoValor);
                }
            }

        }
        private float getValor(string nombreVariable)
        {
            foreach (Variable v in variables)
            {
                if(v.getNombre().Equals(nombreVariable))
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
                if(v.getNombre().Equals(nombreVariable))
                {
                    return v.getTipo();
                }
            }
            return Variable.TipoDato.Char;
        }
        //Programa  -> Librerias? Variables? Main
        public void Programa()
        {
            Libreria();
            Variables();
            Main();
            displayVariables();
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
                    throw new Error("Error de sintaxis, variable duplicada <" +getContenido()+"> en linea: "+linea, log);
                }
            }
            match(Tipos.Identificador);
            if (getContenido() == ",")
            {
                match(",");
                Lista_identificadores(tipo);
            }
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
            if (getContenido() != "case" && getContenido() !=  "break" && getContenido() != "default" && getContenido() != "}")
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
            else if(getContenido() == "do")
            {
                Do(evaluacion);
            }
            else if(getContenido() == "for")
            {
                For(evaluacion);
            }
            else if(getContenido() == "switch")
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
            if(resultado != 0)
            {
                return Variable.TipoDato.Float;
            }
            if(resultado <=255)
            {
                return Variable.TipoDato.Char;
            }else if(resultado <= 65535)
            {
                return Variable.TipoDato.Int;
            }
            return Variable.TipoDato.Float;
        }
        private  bool EvaluaSemantica(string variable, float resultado)
        {
            Variable.TipoDato tipoDato = getTipo(variable);
            
            return false;
        }

        //Asignacion -> identificador = cadena | Expresion;
        private void Asignacion(bool evaluacion)
        {
            if(existeVariable(getContenido()))
            {
                log.WriteLine();
                log.Write(getContenido()+" = ");
                string nombre = getContenido();
                match(Tipos.Identificador);
                match(Tipos.Asignacion);
                dominante = Variable.TipoDato.Char;
                Expresion();
                match(";");
                float resultado = stack.Pop();
                log.Write("= "+resultado);
                log.WriteLine();
                Console.Write(dominante);
                Console.WriteLine(EvaluaNumero(resultado));
                if(dominante < EvaluaNumero(resultado))
                {
                    dominante = EvaluaNumero(resultado);
                }
                if(dominante <= getTipo(nombre))
                {
                    if(evaluacion)
                    {
                        ModificaValor(nombre, resultado);
                    }
                }
                else
                {
                    throw new Error("Error de sintaxis, no podemos asignar un: < " +dominante+" > a un: " + getTipo(nombre)+ " > en linea " + linea,log);
                }
            }
            else
            {
                throw new Error("Error de sintaxis, no existe la variable < " +getContenido()+" > en linea: "+linea, log);
            }
        }

        //While -> while(Condicion) bloque de instrucciones | instruccion
        private void While(bool evaluacion)
        {
            match("while");
            match("(");
            Condicion();
            match(")");
            if (getContenido() == "{") 
            {
                BloqueInstrucciones(evaluacion);
            }
            else
            {
                Instruccion(evaluacion);
            }
        }

        //Do -> do bloque de instrucciones | intruccion while(Condicion)
        private void Do(bool evaluacion)
        {
            match("do");
            if (getContenido() == "{")
            {
                BloqueInstrucciones(evaluacion);
            }
            else
            {
                Instruccion(evaluacion);
            } 
            match("while");
            match("(");
            Condicion();
            match(")");
            match(";");
        }
        //For -> for(Asignacion Condicion; Incremento) BloqueInstruccones | Intruccion 
        private void For(bool evaluacion)
        {
            match("for");
            match("(");
            Asignacion(evaluacion);
            Condicion();
            match(";");
            Incremento(evaluacion);
            match(")");
            if (getContenido() == "{")
            {
                BloqueInstrucciones(evaluacion);  
            }
            else
            {
                Instruccion(evaluacion);
            }
        }

        //Incremento -> Identificador ++ | --
        private void Incremento(bool evaluacion)
        {
            string variable = getContenido();
            if(existeVariable(variable)){
                match(Tipos.Identificador);
                if(getContenido() == "++")
                {
                    match("++");
                    if(evaluacion)
                    {
                        ModificaValor(variable,getValor(variable)+1);
                    }
                }
                else
                {
                    match("--");
                    {
                        ModificaValor(variable,getValor(variable)+1);
                    }
                }
            }else{
                throw new Error("Error de sintaxis, no existe la variable <" +getContenido()+"> en linea: "+linea, log);
            }
            
        }

        //Switch -> switch (Expresion) {Lista de casos} | (default: )
        private void Switch(bool evaluacion)
        {
            match("switch");
            match("(");
            Expresion();
            stack.Pop();
            match(")");
            match("{");
            ListaDeCasos(evaluacion);
            if(getContenido() == "default")
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
            match(":");
            ListaInstruccionesCase(evaluacion);
            if(getContenido() == "break")
            {
                match("break");
                match(";");
            }
            if(getContenido() == "case")
            {
                ListaDeCasos(evaluacion);
            }
        }

        //Condicion -> Expresion operador relacional Expresion
        private bool Condicion()
        {
            Expresion();
            string operador = getContenido();
            match(Tipos.OperadorRelacional);
            Expresion();
            float e2 = stack.Pop();
            float e1 = stack.Pop();
            switch (operador)
            {
                case "==":
                    return e1 == e2;
                case "<=":
                    return e1 <= e2;
                case ">=":
                    return e1 >= e2;
                case "<":
                    return e1 < e2;
                case ">":
                    return e1 > e2;
                default:
                    return e1 != e2;
            }
        }

        //If -> if(Condicion) bloque de instrucciones (else bloque de instrucciones)?
        private void If(bool evaluacion)
        {
            match("if");
            match("(");
            bool validarIf = Condicion();
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
                if (getContenido() == "{")
                {
                    BloqueInstrucciones(validarIf);
                }
                else
                {
                    Instruccion(validarIf);
                }
            }
        }

        //Printf -> printf(cadena|expresion);
        private void Printf(bool evaluacion)
        {
            match("printf");
            match("(");
            string arreglo = getContenido();
                  if(getClasificacion() == Tipos.Cadena)
                  { 
                    if(evaluacion)
                    {
                        Console.Clear();
                    }
                    if(arreglo.Contains("\\n"))
                    {
                        arreglo = arreglo.Replace("\\n","\n");
                    }
                    if(arreglo.Contains("\\t"))
                    {
                        arreglo = arreglo.Replace("\\t","\t");
                    }
                    for( int i = 1; i < arreglo.Length-1;i++)
                    {
                        Console.Write(arreglo[i]); 
                    }
                    match(Tipos.Cadena);
                
                  }
                  else
                  {
                    Expresion();
                    float resultado = stack.Pop();
                    Console.Write(stack.Pop());
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
            if(existeVariable(getContenido()))
            {
                string val = "" + Console.ReadLine();
                ModificaValor(getContenido(),float.Parse(val));
                match(Tipos.Identificador);  
                match(")");
                match(";");
            }
            else
            {
                throw new Error("Error de sintaxis, no existe la variable <" +getContenido()+"> en linea: "+linea, log);
            }    
            
        }

        //Main      -> void main() Bloque de instrucciones
        private void Main()
        {
            match("void");
            match("main");
            match("(");
            match(")");
            BloqueInstrucciones(true);
        }

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
                float n2 = stack.Pop();
                switch (operador)
                {
                    case "+":
                        stack.Push(n2 + n1);
                        break;
                    case "-":
                        stack.Push(n2 - n1);
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
                float n2 = stack.Pop();
                switch (operador)
                {
                    case "*":
                        stack.Push(n2 * n1);
                        break;
                    case "/":
                        stack.Push(n2 / n1);
                        break;
                }
            }
        }
        //Factor -> numero | identificador | (Expresion)
        private void Factor()
        {
            if (getClasificacion() == Tipos.Numero)
            {
                log.Write(getContenido() + " " );
                log.Write(" ");
                
                if(dominante < EvaluaNumero(float.Parse(getContenido()))){
                    dominante = EvaluaNumero(float.Parse(getContenido()));
                }
                stack.Push(float.Parse(getContenido()));
                match(Tipos.Numero);
            }
            else if (getClasificacion() == Tipos.Identificador)
            {
                
                if(existeVariable(getContenido()))
                {
                    log.Write(getContenido() + " " );
                    //Requerimiento 1
                    stack.Push(getValor(getContenido()));
                    match(Tipos.Identificador);
                }
                else
                {
                    throw new Error("Error de sintaxis, no existe la variable <" +getContenido()+"> en linea: "+linea, log);
                }            
            }
            else
            {
                bool huboCasteo = false;
                Variable.TipoDato casteo =  Variable.TipoDato.Char;
                match("(");
                if(getClasificacion() == Tipos.TipoDato)
                {
                    huboCasteo = true;
                    switch(getContenido())
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
                if(huboCasteo)
                {
                    //Requerimiento 2#
                    //Saco un elemento del stack
                    //Convierto ese valor al equivalente en casteo
                    //Requerimiento 3#
                    //Ejemplo: si el casteo es Char y el Pop regresa un 256, el valor equivalente es un 0
                    //Meto ese valor al stack 
                }
            }
        }
    }
}