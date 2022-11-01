//Marco Adrián Domínguez Jiménez
using System;
using System.IO;

namespace Semantica
{
    public class Program
    {
        static void Main(string[] args)
        {
            try
            {
                instancia();
                GC.Collect();
                Console.WriteLine("\n.:Se libera el espacio en memoria:.");
                /*a.match("#");
                a.match("include");
                a.match("<");
                a.match(Token.Tipos.Identificador);
                a.match(".");
                a.match("h");
                a.match(">"); */
                
                //while(!a.FinArchivo())
                //{
                  //  a.NextToken();
                //}
                //a.cerrar();
                /*a = null;
                System.GC.Collect();
                GC.WaitForPendingFinalizers();*/
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        static void instancia()
        {
            Lenguaje a = new Lenguaje();
            a.Programa();
        }
    }
}