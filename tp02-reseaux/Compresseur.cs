using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static tp02_reseaux.HttpReponse;

namespace tp02_reseaux
{
    /// <summary>
    /// 
    /// 
    /// @author : Mouhammad Wagane Diouf, Prince Elonga Kiese et Zackary Ouirzane
    /// </summary>
    internal class Compresseur
    {
        /// <summary>
        /// Appliquer une compression.
        /// </summary>
        /// <param name="requete"></param>
        /// <param name="reponse"></param>
        public static void AppliquerCompression(HttpRequete requete, HttpResponse reponse) 
        {
            if (!requete.headers.ContainsKey("Accept-Encoding"))
                return;

            string encoding = requete.headers["Accept-Encoding"];

            Console.WriteLine("\n(Compression) Accept-Encoding reçu : " + encoding);

            if (encoding.Contains("gzip"))
            {
                reponse.Body = CompressGzip(reponse.Body);
                reponse.Headers["Content-Encoding"] = "gzip";
                reponse.Headers["Content-Length"] = reponse.Body.Length.ToString();
                Console.WriteLine("\nCompression GZIP appliquée.");
            }
            else if (encoding.Contains("br"))
            {
                reponse.Body = CompressBrotli(reponse.Body);
                reponse.Headers["Content-Encoding"] = "br";
                reponse.Headers["Content-Length"] = reponse.Body.Length.ToString();
                Console.WriteLine("\nCompression Brotli appliquée.");
            }
        }
    
        /// <summary>
        /// Compresser avec Gzip.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] CompressGzip(byte[] data)
        {
            using MemoryStream compressedStream = new();
            using (var gzip = new GZipStream(compressedStream, CompressionMode.Compress))
            {
                gzip.Write(data, 0, data.Length);
            }
            return compressedStream.ToArray();
        }

        /// <summary>
        /// Compresser avec Brotli.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] CompressBrotli(byte[] data)
        {
            using MemoryStream compressedStream = new();
            using (var brotli = new BrotliStream(compressedStream, CompressionMode.Compress))
            {
                brotli.Write(data, 0, data.Length);
            }
            return compressedStream.ToArray();
        }
    }
}
