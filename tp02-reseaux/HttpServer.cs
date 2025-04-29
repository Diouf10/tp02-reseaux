using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using static tp02_reseaux.HttpReponse;

namespace tp02_reseaux
{
    /**
     * Le serveur, qui accepte les connexions entrantes.
     * doit atre async (pour multithreading)
     */
    internal class HttpServer
    {
        // Pour écouter les connexions
        private TcpListener listener;
        private bool estEnCours = false;

        private int port = 8088;
       
        // Constructeur
        public HttpServer(TcpListener listener) 
        { 
            this.listener = listener; 
        }

        public void Start()
        {
            listener.Start();
            estEnCours = true;

            Console.WriteLine("Le serveur est en marche !!");
            
            // si le serveur est en cours !!!
            while(estEnCours)
            {
                TcpClient client = listener.AcceptTcpClient();

                Console.WriteLine("Client connecté !!!");

                // Thread pour chaque client
                Thread clientThread = new Thread(() => GererClient(client));

                // Start
                clientThread.Start();
            }
        }

        /// <summary>
        /// Permet de gérer chaque client au serveur de manière asynchrone.
        /// </summary>
        /// <param name="client"></param>
        public void GererClient(TcpClient client)
        {
            using ( NetworkStream stream = client.GetStream() )
            {
                try
                {

                    // Lire la requete du client
                    HttpRequete? requete = HttpRequete.Parse(stream);

                    if (requete == null)
                    {
                        Console.WriteLine("Connexion vide ignorée.");
                        return; // Ne pas planter, juste arrêter là
                    }

                    // Gérer la session
                    Session session = GestionSession.ObtenirOuCreerSession(requete);
                    HttpResponse reponse = new HttpResponse();

                    if (requete.method == "GET" && requete.url == "/")
                    {
                        string filePath = "index.html";
                        if (File.Exists(filePath))
                        {
                            string content = File.ReadAllText(filePath);
                            reponse.SetBody(content, "text/html");
                        }

                        else if (requete.method == "POST")
                        {
                            var formData = ParseFormData(requete.body);

                            if (formData.ContainsKey("nom"))
                            {
                                session.data["nom"] = formData["nom"];
                            }

                            reponse.SetBody("<html><body><h1>Données reçues</h1></body></html>", "text/html");
                        }
                    }

                    else
                    {
                        HttpResponse.EnvoyerErreur(stream, 400, "Bad Request");
                        return;
                    }

                    // Compression
                    Compresseur.AppliquerCompression(requete, reponse);
                    reponse.Envoyer(stream);
                }

                catch (FileNotFoundException)
                {
                    HttpResponse.EnvoyerErreur(stream, 404, "Not Found");
                }

                catch (Exception ex)
                {
                    Console.WriteLine("Erreur : " + ex.Message);
                    HttpResponse.EnvoyerErreur(stream, 500, "Internal Server Error");
                }
            }
        }

        /// <summary>
        /// Parse les données.
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        private static Dictionary<string, string> ParseFormData(string body)
        {
            var dict = new Dictionary<string, string>();
            var pairs = body.Split('&');
            foreach (var pair in pairs)
            {
                var kv = pair.Split('=');
                if (kv.Length == 2)
                {
                    dict[kv[0]] = Uri.UnescapeDataString(kv[1]);
                }
            }
            return dict;
        }
    }
}
