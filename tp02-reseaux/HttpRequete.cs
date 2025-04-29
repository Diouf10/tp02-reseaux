using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace tp02_reseaux
{


    /// <summary>
    /// 
    /// Permet de parser les requetes entrantes.
    ///
    /// Lis les méthodes qu'on va utiliser
    ///
    /// @author : Mouhammad Wagane Diouf, Prince Elonga Kiese et Zackary Ouirzane 
    /// </summary>
    internal class HttpRequete
    {
        public string method { get; private set; }
        public string url { get; private set; }
        public string protocol { get; private set; }
        public Dictionary<string, string> headers { get; private set; } = new();
        public string body { get; private set; } = "";
        public string? newIdsession { get; set; } = null;
        public Dictionary<string, string> QueryParameters { get; private set; } = new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static HttpRequete? Parse(NetworkStream stream)
        {
            StreamReader reader = new StreamReader(stream, Encoding.ASCII, leaveOpen: true);
            string? ligne = reader.ReadLine();
            if (string.IsNullOrEmpty(ligne)) return null;

            var parts = ligne.Split(' ');
            if (parts.Length < 3) throw new Exception("Requête mal formée");

            HttpRequete req = new HttpRequete
            {
                method = parts[0],
                url = parts[1],
                protocol = parts[2]
            };

            string? line;
            while (!string.IsNullOrEmpty(line = reader.ReadLine()))
            {
                int sep = line.IndexOf(':');
                if (sep == -1) continue;
                string nom = line.Substring(0, sep).Trim();
                string val = line.Substring(sep + 1).Trim();
                req.headers[nom] = val;
            }

            if (req.method == "POST" && req.headers.TryGetValue("Content-Length", out string? lenStr) && int.TryParse(lenStr, out int length))
            {
                char[] buffer = new char[length];
                reader.Read(buffer, 0, length);
                req.body = new string(buffer);
            }

            Console.WriteLine("\nRequête reçue et parsée avec succès.");

            return req;
        }





        /// <summary>
        /// 
        /// </summary>
        /// <param name="nom"></param>
        /// <returns></returns>
        /// 
        public string? ObtenirCookie(string nom)
        {
            {

                if (headers.TryGetValue("Cookie", out string cookies)) return null;

                foreach (var cookie in cookies.Split(";"))
                {
                    var trimme = cookie.Trim();
                    if(trimme.StartsWith(nom + "="))
                    {
                        return trimme.Substring(nom.Length + 1);
                    }
                }
            }

            return null;
        }  
    }
}
