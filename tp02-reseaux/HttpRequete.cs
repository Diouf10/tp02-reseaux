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
        private string method;
        private string url;
        private string protocol;
        private Dictionary<string, string> headers = new();
        private string body;
        private string? newIdsession = null;

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
                throw new Exception("Requete vide");
            }

            var parts = line.Split(' ');
            if(parts.Length < 3)
            {
                throw new Exception("Requete mal formée");
            }

            requete.method = parts[0];
            requete.url = parts[1];
            requete.protocol = parts[2];


            // Lire les headers
            while (!string.IsNullOrEmpty(line = reader.ReadLine()))
            {
                int separeteur = line.IndexOf(';');
                if (separeteur == -1)
                {
                    continue;
                }

                string nom = line.Substring(0, separeteur).Trim();
                string valeur = line.Substring(separeteur + 1).Trim();
                requete.headers[nom] = valeur;
            }

            // Lire le body selon la requete (post)

            if (requete.method == "POST" && requete.headers.ContainsKey("Content-Length"))
            {
                Console.WriteLine("POST");

                int contentLength = int.Parse(requete.headers["Content-Length"]);
                char[] buffer = new char[contentLength];
                reader.Read(buffer, 0, contentLength);
                requete.body = new string(buffer);

                
                Console.WriteLine(requete.body);
            }

            // Lire le body selon la requete (get)

            if (requete.method.StartsWith("GET"))
            {
                // Répondre à une requête HTTP simple
                // Construction de la réponse HTTP (headers + contenu HTML).
                Console.WriteLine("GET");

                string html = "<html><body><h1>Bienvenue sur le serveur De Momo et Prince C#</h1></body></html>";
                string response = "HTTP/1.1 200 OK\r\n" +
                "Content-Type: text/html; charset=UTF-8\r\n" +
                $"Content-Length: {html.Length}\r\n" +
                "\r\n" + html;

                byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                stream.Write(responseBytes, 0, responseBytes.Length);

                Console.WriteLine(response);
                
            }

            return requete;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nom"></param>
        /// <returns></returns>
        public string? ObtenirCookie(string nom)
        {
            if(headers.TryGetValue("Cookie", out string cookies)) 
            {
                var cookiePairs = cookies.Split(';');

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
