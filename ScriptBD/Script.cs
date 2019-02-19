using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace ScriptBD
{
    static class Script
    {
        public static string Servidor { get; set; }
        public static string BaseDados { get; set; }
        public static string Utilizador { get; set; }
        public static string Password { get; set; }
        public static string CaminhoScript { get; set; }
        public static string CaminhoFicheiroScript { get; set; }
        public static string FicheiroFiltro { get; internal set; }

        private static List<string> ObjetosParaScript;

        public static bool GerarScript()
        {
            if (!string.IsNullOrEmpty(FicheiroFiltro))
            {
                PreencherFiltroObjetosParaScript();
            }

            if (string.IsNullOrEmpty(CaminhoScript))
            {
                CaminhoScript = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            }
            CaminhoFicheiroScript = Path.Combine(CaminhoScript, BaseDados + "_Script.sql");

            try
            {
                ServerConnection srvCon = new ServerConnection();

                srvCon.ServerInstance = Servidor;
                srvCon.LoginSecure = string.IsNullOrEmpty(Utilizador);
                if (!srvCon.LoginSecure)
                {
                    srvCon.Login = Utilizador;
                    srvCon.Password = Password;
                }

                Server srv = new Server(srvCon);

                Database db = srv.Databases[BaseDados];

                ScriptingOptions scrpo = new ScriptingOptions();
                scrpo.IncludeHeaders = true;
                scrpo.ScriptDrops = false;
                scrpo.WithDependencies = true;
                scrpo.Indexes = true;
                scrpo.DriAllConstraints = true;
                scrpo.Triggers = true;
                scrpo.FullTextIndexes = true;
                scrpo.NoCollation = false;
                scrpo.Bindings = true;
                scrpo.IncludeIfNotExists = false;
                scrpo.ScriptBatchTerminator = true;
                scrpo.ExtendedProperties = true;
                scrpo.ToFileOnly = true;
                scrpo.FileName = CaminhoFicheiroScript;

                Transfer scriptTransfer = new Transfer(db);
                scriptTransfer.Options = scrpo;

                scriptTransfer.CopyAllObjects = false;
                scriptTransfer.ObjectList = new System.Collections.ArrayList() { RetornarObjeto(db, "Log_Erros") };
                scriptTransfer.DropDestinationObjectsFirst = true;

                scriptTransfer.ScriptTransfer();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERRO: " + ex.Message);
                return false;
            }
        }

        private static void PreencherFiltroObjetosParaScript()
        {
            string linha;

            using (var sr = new StreamReader(FicheiroFiltro))
            {
                while ((linha = sr.ReadLine()) != null)
                {
                    linha = linha.Trim();
                    if (!string.IsNullOrEmpty(linha))
                    {
                        if (ObjetosParaScript == null)
                        {
                            ObjetosParaScript = new List<string>();
                        }
                        ObjetosParaScript.Add(linha);
                    }
                }
            }
        }

        private static object RetornarObjeto(Database db, string nomeObjeto)
        {
            if (db.Tables.Contains(nomeObjeto))
                return db.Tables[nomeObjeto];

            if (db.Views.Contains(nomeObjeto))
                return db.Views[nomeObjeto];

            if (db.StoredProcedures.Contains(nomeObjeto))
                return db.StoredProcedures[nomeObjeto];

            if (db.UserDefinedFunctions.Contains(nomeObjeto))
                return db.UserDefinedFunctions[nomeObjeto];

            if (db.PartitionFunctions.Contains(nomeObjeto))
                return db.PartitionFunctions[nomeObjeto];

            return null;
        }
    }
}