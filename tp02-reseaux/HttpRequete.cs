using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace tp02_reseaux
{
    /**
     * Permet de parser les requetes entrantes.
     * 
     * Lis les méthodes qu'on va utiliser
     * 
     */
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
        public static HttpRequete Parse(NetworkStream stream)
        {
            StreamReader reader = new StreamReader(stream, Encoding.ASCII, leaveOpen: true);
            HttpRequete requete = new HttpRequete();

            string? line = reader.ReadLine();
            if (string.IsNullOrEmpty(line))
            {
                return null;
            }

            var parts = line.Split(' ');
            if (parts.Length < 3)
            {
                throw new Exception("Requete mal formée");
            }

            requete.method = parts[0];
            requete.url = parts[1];
            requete.protocol = parts[2];

            // Lire les headers correctement
            while (!string.IsNullOrEmpty(line = reader.ReadLine()))
            {
                int separator = line.IndexOf(':'); 
                if (separator == -1) continue;

                string name = line.Substring(0, separator).Trim();
                string value = line.Substring(separator + 1).Trim();
                requete.headers[name] = value;
            }


            // TODO a utiliser ca au lieu du get en dessous !!!!!

        //    // Lire le corps si POST
        //    if (requete.method == "POST" && requete.headers.ContainsKey("Content-Length"))
        //    {
        //        int contentLength = int.Parse(requete.headers["Content-Length"]);
        //        char[] buffer = new char[contentLength];
        //        reader.Read(buffer, 0, contentLength);
        //        requete.body = new string(buffer);
        //    }

        //    // Gérer les query parameters pour GET
        //    if (requete.url.Contains('?'))
        //    {
        //        var urlParts = requete.url.Split('?', 2);
        //        requete.url = urlParts[0];
        //        var query = urlParts[1];

        //        var pairs = query.Split('&');
        //        foreach (var pair in pairs)
        //        {
        //            var kv = pair.Split('=');
        //            if (kv.Length == 2)
        //            {
        //                requete.QueryParameters[kv[0]] = Uri.UnescapeDataString(kv[1]);
        //            }
        //        }
        //    }

        //    Console.WriteLine("requête : ");
        //    Console.WriteLine(requete);

        //    return requete;
        //}


        // Lire le body selon la requete (get)

            if (requete.method.StartsWith("GET"))
            {
                // Répondre à une requête HTTP simple
                // Construction de la réponse HTTP (headers + contenu HTML).
                Console.WriteLine("GET");

                string html = "<html><body><h1>Bienvenue sur le serveur De Momo, Prince et Zacky C#</h1></body></html>";
                string response = "HTTP/1.1 200 OK\r\n" +
                "Content-Type: text/html; charset=UTF-8\r\n" +
                $"Content-Length: {html.Length}\r\n" +
                "\r\n" + html;

                byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                stream.Write(responseBytes, 0, responseBytes.Length);

                Console.WriteLine(response);

            }

            if (requete.method == "POST" && requete.headers.ContainsKey("Content-Length"))
                    {
                        int contentLength = int.Parse(requete.headers["Content-Length"]);
                        char[] buffer = new char[contentLength];
                        reader.Read(buffer, 0, contentLength);
                        requete.body = new string(buffer);
                    }

                return requete;
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
                var cookiePairs = cookies.Split(';');
            if(headers.TryGetValue("Cookie", out string cookies)) 

                foreach (var cookie in cookiePairs)
                {
                    var trimme = cookie.Trim();
                    if(trimme.StartsWith(nom + "="))
                    {
                        return trimme.Substring((nom + "=").Length);
                    }
                }
            }

            return null;
        }  
    }
}
