using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tp02_reseaux
{
    /// <summary>
    /// 
    /// 
    /// @author : Mouhammad Wagane Diouf, Prince Elonga Kiese et Zackary Ouirzane
    /// </summary>
    internal class GestionSession
    {

        private static readonly ConcurrentDictionary<string, Session> sessions = new ConcurrentDictionary<string, Session>();

        /// <summary>
        /// Obtient ou crée une session.
        /// </summary>
        /// <param name="requete"></param>
        /// <returns></returns>
        public static Session ObtenirOuCreerSession(HttpRequete requete)
        {
            string sessionId = null;

            if (requete.headers.ContainsKey("Cookie")) 
            {
                var cookies = requete.headers["Cookie"].Split(';');
                foreach (var cookie in cookies)
                {
                    var trimmed = cookie.Trim();
                    if (trimmed.StartsWith("sessionId="))
                    {
                        sessionId = trimmed.Substring("sessionId=".Length);
                        break;
                    }
                }
            }

            if(sessionId != null && sessions.ContainsKey(sessionId))
            {
                return sessions[sessionId];
            }

            string newSessionId = Guid.NewGuid().ToString();
            var session = new Session(newSessionId);
            sessions[newSessionId] = session;


            requete.newIdsession = newSessionId;

            return session;
        }
    }
}
