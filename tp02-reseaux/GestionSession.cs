using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tp02_reseaux
{
    internal class GestionSession
    {

        private static readonly ConcurrentDictionary<string, Session> sessions = new ConcurrentDictionary<string, Session>();

        public static void ObtenirOuCreerSession(HttpRequete requete)
        {
           
        }
    }
}
