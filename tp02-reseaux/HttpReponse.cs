using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace tp02_reseaux
{
    /**
     * Permet de gérer les réponses
     * ajoute les bons en tetes.
     * 
     * Va donner le bonne réponse HTTP
     */
    internal class HttpReponse
    {
        public class HttpResponse
        {
            public string StatutCode { get; set; } = "200 OK";
            public Dictionary<string, string> Headers { get; private set; } = new();
            public byte[] Body { get;  set; } = Array.Empty<byte>();

            /// <summary>
            /// Modifier le body.
            /// </summary>
            /// <param name="content"></param>
            /// <param name="contentType"></param>
            public void SetBody(string content, string contentType)
            {
                Body = Encoding.UTF8.GetBytes(content);
                Headers["Content-Type"] = contentType;
                Headers["Content-Length"] = Body.Length.ToString();
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="stream"></param>
            public void Envoyer(NetworkStream stream)
            {
                using (StreamWriter writer = new StreamWriter(stream, Encoding.ASCII, leaveOpen: true))
                {
                    // Envoyer la ligne de statut
                    writer.WriteLine($"HTTP/1.1 {StatutCode}");

                    // Ajouter DA
                    Headers["X-Etudiant-ID"] = "1936603"; //  DA de Mo
                    Headers["Server"] = "MonServeur/1.0";

                    // Envoyer tous les headers
                    foreach (var header in Headers)
                    {
                        writer.WriteLine($"{header.Key}: {header.Value}");
                    }

                    writer.WriteLine(); // Ligne vide avant le corps
                    writer.Flush();     // Flush obligatoire pour envoyer les headers

                    // Envoyer le corps (body)
                    stream.Write(Body, 0, Body.Length);
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="stream"></param>
            /// <param name="code"></param>
            /// <param name="message"></param>
            public static void EnvoyerErreur(NetworkStream stream, int code, string message)
            {
                var response = new HttpResponse
                {
                    StatutCode = $"{code} {message}"
                };
                string errorHtml = $"<html><body><h1>{code} - {message}</h1></body></html>";
                response.SetBody(errorHtml, "text/html");
                response.Envoyer(stream);
            }
        }

    }
}