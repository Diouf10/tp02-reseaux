using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using static tp02_reseaux.HttpReponse;

namespace tp02_reseaux
{
    /// <summary>
    ///
    /// Le serveur, qui accepte les connexions entrantes.
    /// 
    /// Doit atre async(pour multithreading)
    ///
    /// @author : Mouhammad Wagane Diouf, Prince Elonga Kiese et Zackary Ouirzane
    /// </summary>
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

                Console.WriteLine("\nClient connecté !!!");

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

                    Console.WriteLine("\n--- Nouvelle requête ---");

                    if (requete == null)
                    {
                        Console.WriteLine("\nConnexion vide ignorée.");
                        return; // Ne pas planter, juste arrêter là
                    }
                    else
                    {
                        Console.WriteLine($"Méthode : {requete.method}");
                        Console.WriteLine($"URL demandée : {requete.url}");
                        Console.WriteLine($"Protocole : {requete.protocol}");

                        Console.WriteLine("En-têtes reçus :");
                        foreach (var h in requete.headers)
                        {
                            Console.WriteLine($"  {h.Key}: {h.Value}");
                        }
                        Console.WriteLine("\n");
                    }

                    // Gérer la session
                    Session session = GestionSession.ObtenirOuCreerSession(requete);
                    HttpResponse reponse = new HttpResponse();

                    string pageDemandee = requete.url == "/" ? "/index.html" : requete.url;
                    if (requete.method == "GET")
                    {
                        string path = requete.url == "/" ? "/index.html" : requete.url;
                        reponse = ServirFichierStatique(path);
                    }
                    else if (requete.method == "POST" && requete.url == "/formulaire")
                    {
                        Console.WriteLine("\nContenu POST : " + requete.body);

                        var formData = ParseFormData(requete.body);
                        if (formData.ContainsKey("nom")) { 
                            session.data["nom"] = formData["nom"];
                            Console.WriteLine($"\nNom enregistré en session : {session.data["nom"]}");
                        }


                        string html = $"<html><body><h1>Bienvenue {session.data["nom"]}</h1></body></html>";
                        reponse = new HttpResponse();

                        Console.WriteLine("Données reçues dans le body :");
                        Console.WriteLine(requete.body);

                        reponse.SetBody(html, "text/html");
                    }
                    else
                    {
                        reponse = new HttpResponse();
                        reponse.StatutCode = "400 Bad Request";
                        reponse.SetBody("<h1>Requête invalide</h1>", "text/html");
                    }

                    if (requete.newIdsession != null)
                        reponse.Headers["Set-Cookie"] = $"sessionId={requete.newIdsession}; HttpOnly";

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cheminRelatif"></param>
        /// <returns></returns>
        private HttpResponse ServirFichierStatique(string cheminRelatif)
        {
            string basePath = "wwwroot";
            string fullPath = Path.Combine(basePath, cheminRelatif.TrimStart('/'));

            HttpResponse response = new HttpResponse();

            Console.WriteLine($"\nTentative de lecture du fichier : {fullPath}");

            if (!File.Exists(fullPath))
            {
                Console.WriteLine("\nFichier non trouvé.");
                response.StatutCode = "404 Not Found";
                response.SetBody("<html><body><h1>404 - Fichier non trouvé</h1></body></html>", "text/html");
                Console.WriteLine("GET - Fichier non trouvé\n");
                return response;
            }
            else
            {
                Console.WriteLine("\nFichier trouvé, envoi au client.");
            }

            string extension = Path.GetExtension(fullPath).ToLower();
            string contentType = extension switch
            {
                ".html" => "text/html",
                ".css" => "text/css",
                ".js" => "application/javascript",
                ".png" => "image/png",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".ico" => "image/x-icon",
                _ => "application/octet-stream"
            };

            byte[] contenu = File.ReadAllBytes(fullPath);
            response.SetBody(contenu, contentType);
            return response;
        }
    }
}
