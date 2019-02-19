using System;
using System.Collections.Generic;
using System.Linq;

namespace ScriptBD
{
    static class LinhaComando
    {
        private static List<string> argumentos;
        private static bool argumentosValidos = true;

        public static bool ProcessarArgumentos(ref string[] args)
        {
            argumentos = args.ToList();

            Script.Servidor = RetornarValorArgumento("-s", true);
            Script.BaseDados = RetornarValorArgumento("-b", true);
            Script.Utilizador = RetornarValorArgumento("-u", false);
            Script.Password = RetornarValorArgumento("-p", false);
            Script.CaminhoScript = RetornarValorArgumento("-c", false);
            Script.FicheiroFiltro = RetornarValorArgumento("-f", false);

            if (!argumentosValidos)
            {
                MostrarArgumentosValidos();
            }

            return argumentosValidos;
        }

        private static string RetornarValorArgumento(string argumento, bool obrigatorio)
        {
            int posicao = argumentos.IndexOf(argumento);
            string valorArg = null;

            if (posicao > -1 && posicao < argumentos.Count - 1)
            {
                valorArg = argumentos[posicao + 1].Trim();
            }

            //Console.WriteLine(argumento[1].ToString() + " ===> " + valorArg);

            if (obrigatorio && string.IsNullOrEmpty(valorArg))
            {
                argumentosValidos = false;
            }

            return valorArg;
        }

        private static void MostrarArgumentosValidos()
        {
            Console.WriteLine();
            Console.WriteLine("Argumentos obrigatórios:");
            Console.WriteLine("    -s Servidor ===> nome e instância (se existir) do servidor SQL Server");
            Console.WriteLine("    -b BaseDados ===> nome da base de dados para gerar o script");
            Console.WriteLine();
            Console.WriteLine("Argumentos opcionais:");
            Console.WriteLine("    -u Utilizador ===> utilizador de acesso à base de dados");
            Console.WriteLine("    -p Password ===> palavra-passe de acesso à base de dados");
            Console.WriteLine("    -c Caminho ===> caminho para gravação do ficheiro de script");
            Console.WriteLine("    -f Ficheiro ===> ficheiro com filtro de objetos para gerar script (um por linha)");
            Console.WriteLine();
        }
    }
}
