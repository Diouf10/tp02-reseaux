using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

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

        public void GererClient(TcpClient client)
        {

            // Utiliser un try catch pour les erreurs

            NetworkStream stream = client.GetStream();

            // Lire la requete du client
            HttpRequete.Parse(stream);

            // Gérer la session


            // Générer la réponse


            // Compression
        }
    }
}
