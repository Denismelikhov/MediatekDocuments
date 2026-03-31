using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace MediaTekDocuments.manager
{
    /// <summary>
    /// Classe indépendante d'accès à une api rest avec éventuellement une "basic authorization"
    /// </summary>
    class ApiRest
    {
        /// <summary>
        /// unique instance de la classe
        /// </summary>
        private static ApiRest instance = null;
        /// <summary>
        /// Objet de connexion à l'api
        /// </summary>
        private readonly HttpClient httpClient;
        /// <summary>
        /// Canal http pour l'envoi du message et la récupération de la réponse
        /// </summary>
        private HttpResponseMessage httpResponse;

        /// <summary>
        /// Constructeur privé pour préparer la connexion (éventuellement sécurisée)
        /// </summary>
        /// <param name="uriApi">adresse de l'api</param>
        /// <param name="authentificationString">chaîne d'authentification</param>
        private ApiRest(String uriApi, String authentificationString = "")
        {
            httpClient = new HttpClient() { BaseAddress = new Uri(uriApi) };
            SetAuthentification(authentificationString);
        }

        /// <summary>
        /// Crée une instance unique de la classe
        /// </summary>
        /// <param name="uriApi">adresse de l'api</param>
        /// <param name="authentificationString">chaîne d'authentification (login:pwd)</param>
        /// <returns></returns>
        public static ApiRest GetInstance(String uriApi, String authentificationString = "")
        {
            if (instance == null)
            {
                instance = new ApiRest(uriApi, authentificationString);
            }
            else
            {
                instance.SetAuthentification(authentificationString);
            }
            return instance;
        }

        /// <summary>
        /// Méthode d'authentification à l'api : si la chaîne d'authentification est vide, aucune authentification n'est utilisée
        /// </summary>
        /// <param name="authentificationString">chaîne d'authentificatio (login:pwd)</param>
        /// <returns></returns>
        public void SetAuthentification(string authentificationString)
        {
            httpClient.DefaultRequestHeaders.Authorization = null;

            if (!String.IsNullOrEmpty(authentificationString))
            {
                String base64EncodedAuthentificationString =
                    Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(authentificationString));
                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Basic", base64EncodedAuthentificationString);
            }
        }

        /// <summary>
        /// Envoi une demande à l'API et récupère la réponse
        /// </summary>
        /// <param name="methode">verbe http (GET, POST, PUT, DELETE)</param>
        /// <param name="message">message à envoyer dans l'URL</param>
        /// <param name="parametres">contenu de variables à mettre dans body</param>
        /// <returns>liste d'objets (select) ou liste vide (ok) ou null si erreur</returns>
        public JObject RecupDistant(string methode, string message, String parametres)
        {
            StringContent content = null;
            if (!(parametres is null))
            {
                content = new StringContent(parametres, System.Text.Encoding.UTF8, "application/x-www-form-urlencoded");
            }

            switch (methode)
            {
                case "GET":
                    httpResponse = httpClient.GetAsync(message).Result;
                    break;
                case "POST":
                    httpResponse = httpClient.PostAsync(message, content).Result;
                    break;
                case "PUT":
                    httpResponse = httpClient.PutAsync(message, content).Result;
                    break;
                case "DELETE":
                    httpResponse = httpClient.DeleteAsync(message).Result;
                    break;
                default:
                    return new JObject();
            }

            var json = httpResponse.Content.ReadAsStringAsync().Result;
            return JObject.Parse(json);
        }
    }
}
