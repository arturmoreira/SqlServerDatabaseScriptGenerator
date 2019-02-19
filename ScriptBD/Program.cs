using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ScriptBD
{
    class Program
    {
        static void Main(string[] args)
        {
            if (LinhaComando.ProcessarArgumentos(ref args))
            {
                Stopwatch c = Stopwatch.StartNew();

                Console.WriteLine(string.Format("A gerar o script da base de dados \"{0}\"...", Script.BaseDados));

                if (Script.GerarScript())
                {
                    Console.WriteLine("Script da base de dados gerado com sucesso:");
                    Console.WriteLine(Script.CaminhoFicheiroScript);
                    Console.WriteLine(c.ElapsedMilliseconds.ToString("0 ms"));
                }
            }
        }
    }
}