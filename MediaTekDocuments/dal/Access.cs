using MediaTekDocuments.manager;
using MediaTekDocuments.model;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace MediaTekDocuments.dal
{
    /// <summary>
    /// Classe d'accès aux données
    /// </summary>
    public class Access
    {
        /// <summary>
        /// adresse de l'API
        /// </summary>
        private static readonly string uriApi = ConfigurationManager.AppSettings["uriApi"];

        /// <summary>
        /// login d'accès à l'API
        /// </summary>
        private static readonly string apiLogin = ConfigurationManager.AppSettings["apiLogin"];

        /// <summary>
        /// mot de passe d'accès à l'API
        /// </summary>
        private static readonly string apiPassword = ConfigurationManager.AppSettings["apiPassword"];

        /// <summary>
        /// chemin du fichier de logs
        /// </summary>
        private static readonly string logFilePath = ConfigurationManager.AppSettings["logFilePath"];

        /// <summary>
        /// instance unique de la classe
        /// </summary>
        private static Access instance = null;

        /// <summary>
        /// instance de ApiRest pour envoyer des demandes vers l'api et recevoir la réponse
        /// </summary>
        private readonly ApiRest api = null;

        /// <summary>
        /// chaîne d'authentification transmise à l'API
        /// </summary>
        private string authentificationString = "";

        /// <summary>
        /// méthode HTTP pour select
        /// </summary>
        private const string GET = "GET";

        /// <summary>
        /// méthode HTTP pour insert
        /// </summary>
        private const string POST = "POST";

        /// <summary>
        /// méthode HTTP pour update
        /// </summary>
        private const string PUT = "PUT";

        /// <summary>
        /// méthode HTTP pour delete
        /// </summary>
        private const string DELETE = "DELETE";

        /// <summary>
        /// Méthode privée pour créer un singleton
        /// initialise l'accès à l'API
        /// </summary>
        private Access()
        {
            try
            {
                authentificationString = apiLogin + ":" + apiPassword;
                api = ApiRest.GetInstance(uriApi, authentificationString);
            }
            catch (Exception e)
            {
                LogErreur(e.Message);
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Création et retour de l'instance unique de la classe
        /// </summary>
        /// <returns>instance unique de la classe</returns>
        public static Access GetInstance()
        {
            if(instance == null)
            {
                instance = new Access();
            }
            return instance;
        }

        /// <summary>
        /// met à jour la chaîne d'authentification utilisée pour accéder à l'API
        /// </summary>
        /// <param name="login">login de l'utilisateur</param>
        /// <param name="pwd">mot de passe de l'utilisateur</param>
        public void SetAuthentification(string login, string pwd)
        {
            authentificationString = login + ":" + pwd;
            api.SetAuthentification(authentificationString);
        }

        /// <summary>
        /// teste si le login et le mot de passe permettent d'accéder à l'API
        /// </summary>
        /// <param name="login">login de l'utilisateur</param>
        /// <param name="pwd">mot de passe de l'utilisateur</param>
        /// <returns>true si l'authentification réussit</returns>
        public bool TesterAuthentification(string login, string pwd)
        {
            try
            {
                SetAuthentification(login, pwd);
                List<Categorie> genres = GetAllGenres();
                return genres != null;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// récupère l'utilisateur connecté à partir de ses identifiants
        /// </summary>
        /// <param name="login">login de l'utilisateur</param>
        /// <param name="pwd">mot de passe de l'utilisateur</param>
        /// <returns>objet Utilisateur si l'authentification réussit, sinon null</returns>
        public Utilisateur GetUtilisateur(string login, string pwd)
        {
            try
            {
                SetAuthentification(login, pwd);
                JObject retour = api.RecupDistant(GET, "utilisateurconnecte", null);
                string code = (string)retour["code"];

                if (code != "200")
                {
                    return null;
                }

                string resultString = JsonConvert.SerializeObject(retour["result"]);
                return JsonConvert.DeserializeObject<Utilisateur>(resultString);
            }
            catch (Exception ex)
            {
                LogErreur(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Retourne tous les genres à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Genre</returns>
        public List<Categorie> GetAllGenres()
        {
            IEnumerable<Genre> lesGenres = TraitementRecup<Genre>(GET, "genre", null);
            return new List<Categorie>(lesGenres);
        }

        /// <summary>
        /// Retourne tous les rayons à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Rayon</returns>
        public List<Categorie> GetAllRayons()
        {
            IEnumerable<Rayon> lesRayons = TraitementRecup<Rayon>(GET, "rayon", null);
            return new List<Categorie>(lesRayons);
        }

        /// <summary>
        /// Retourne toutes les catégories de public à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Public</returns>
        public List<Categorie> GetAllPublics()
        {
            IEnumerable<Public> lesPublics = TraitementRecup<Public>(GET, "public", null);
            return new List<Categorie>(lesPublics);
        }

        /// <summary>
        /// Retourne toutes les livres à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Livre</returns>
        public List<Livre> GetAllLivres()
        {
            List<Livre> lesLivres = TraitementRecup<Livre>(GET, "livre", null);
            return lesLivres;
        }

        /// <summary>
        /// Retourne toutes les dvd à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Dvd</returns>
        public List<Dvd> GetAllDvd()
        {
            List<Dvd> lesDvd = TraitementRecup<Dvd>(GET, "dvd", null);
            return lesDvd;
        }

        /// <summary>
        /// Retourne toutes les revues à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Revue</returns>
        public List<Revue> GetAllRevues()
        {
            List<Revue> lesRevues = TraitementRecup<Revue>(GET, "revue", null);
            return lesRevues;
        }


        /// <summary>
        /// Retourne les exemplaires d'une revue
        /// </summary>
        /// <param name="idDocument">id de la revue concernée</param>
        /// <returns>Liste d'objets Exemplaire</returns>
        public List<Exemplaire> GetExemplairesRevue(string idDocument)
        {
            String jsonIdDocument = convertToJson("id", idDocument);
            List<Exemplaire> lesExemplaires = TraitementRecup<Exemplaire>(GET, "exemplaire/" + jsonIdDocument, null);
            return lesExemplaires;
        }

        /// <summary>
        /// ecriture d'un exemplaire en base de données
        /// </summary>
        /// <param name="exemplaire">exemplaire à insérer</param>
        /// <returns>true si l'insertion a pu se faire (retour != null)</returns>
        public bool CreerExemplaire(Exemplaire exemplaire)
        {
            String jsonExemplaire = JsonConvert.SerializeObject(exemplaire, new CustomDateTimeConverter());
            try
            {
                List<Exemplaire> liste = TraitementRecup<Exemplaire>(POST, "exemplaire", "champs=" + jsonExemplaire);
                return (liste != null);
            }
            catch (Exception ex)
            {
                LogErreur(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Traitement de la récupération du retour de l'api, avec conversion du json en liste pour les select (GET)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="methode">verbe HTTP (GET, POST, PUT, DELETE)</param>
        /// <param name="message">information envoyée dans l'url</param>
        /// <param name="parametres">paramètres à envoyer dans le body, au format "chp1=val1&chp2=val2&..."</param>
        /// <returns>liste d'objets récupérés (ou liste vide)</returns>
        private List<T> TraitementRecup<T>(string methode, string message, string parametres)
        {
            try
            {
                JObject retour = api.RecupDistant(methode, message, parametres);
                string code = (string)retour["code"];

                if (code == "200")
                {
                    if (methode.Equals(GET))
                    {
                        string resultString = JsonConvert.SerializeObject(retour["result"]);
                        return JsonConvert.DeserializeObject<List<T>>(resultString, new CustomBooleanJsonConverter());
                    }

                    return new List<T>();
                }
                else
                {
                    LogErreur("code erreur = " + code + " message = " + (string)retour["message"]);
                    return null;
                }
            }
            catch (Exception e)
            {
                LogErreur("Erreur lors de l'accès à l'API : " + e.Message);
                return null;
            }
        }

        /// <summary>
        /// Convertit en json un couple nom/valeur
        /// </summary>
        /// <param name="nom"></param>
        /// <param name="valeur"></param>
        /// <returns>couple au format json</returns>
        private String convertToJson(Object nom, Object valeur)
        {
            Dictionary<Object, Object> dictionary = new Dictionary<Object, Object>();
            dictionary.Add(nom, valeur);
            return JsonConvert.SerializeObject(dictionary);
        }

        /// <summary>
        /// Modification du convertisseur Json pour gérer le format de date
        /// </summary>
        private sealed class CustomDateTimeConverter : IsoDateTimeConverter
        {
            public CustomDateTimeConverter()
            {
                base.DateTimeFormat = "yyyy-MM-dd";
            }
        }

        /// <summary>
        /// Modification du convertisseur Json pour prendre en compte les booléens
        /// classe trouvée sur le site :
        /// https://www.thecodebuzz.com/newtonsoft-jsonreaderexception-could-not-convert-string-to-boolean/
        /// </summary>
        private sealed class CustomBooleanJsonConverter : JsonConverter<bool>
        {
            public override bool ReadJson(JsonReader reader, Type objectType, bool existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                return Convert.ToBoolean(reader.ValueType == typeof(string) ? Convert.ToByte(reader.Value) : reader.Value);
            }

            public override void WriteJson(JsonWriter writer, bool value, JsonSerializer serializer)
            {
                serializer.Serialize(writer, value);
            }
        }

        /// <summary>
        /// Indique si un document peut être supprimé
        /// Revue : pas d'exemplaire
        /// Livre/DVD : pas de commande
        /// </summary>
        /// <param name="document">document concerné</param>
        /// <returns>true si suppression autorisée</returns>
        public bool DocumentSupprimable(Document document)
        {
            if (document is Revue)
            {
                return !GetExemplairesRevue(document.Id).Any();
            }
            return !GetCommandesDocument(document.Id).Any();
        }

        public bool SupprimerDocument(string idDocument)
        {
            string jsonIdDocument = convertToJson("id", idDocument);
            try
            {
                List<Document> liste = TraitementRecup<Document>(DELETE, "document/" + jsonIdDocument, null);
                return (liste != null);
            }
            catch (Exception ex)
            {
                LogErreur(ex.Message);
            }
            return false;
        }

        public List<CommandeDocument> GetCommandesDocument(string idDocument)
        {
            String jsonIdDocument = convertToJson("id", idDocument);
            List<CommandeDocument> lesCommandes = TraitementRecup<CommandeDocument>(GET, "commandedocument/" + jsonIdDocument, null);
            return lesCommandes;
        }

        /// <summary>
        /// Demande la suppression d'un livre
        /// </summary>
        /// <param name="idLivre">id du livre</param>
        /// <returns>true si suppression effectuée</returns>
        public bool SupprimerLivre(string idLivre)
        {
            String jsonIdLivre = convertToJson("id", idLivre);
            List<Livre> liste = TraitementRecup<Livre>(DELETE, "livre/" + jsonIdLivre, null);
            return (liste != null);
        }

        /// <summary>
        /// Demande la suppression d'un dvd
        /// </summary>
        /// <param name="idDvd">id du dvd</param>
        /// <returns>true si suppression effectuée</returns>
        public bool SupprimerDvd(string idDvd)
        {
            String jsonIdDvd = convertToJson("id", idDvd);
            List<Dvd> liste = TraitementRecup<Dvd>(DELETE, "dvd/" + jsonIdDvd, null);
            return (liste != null);
        }

        /// <summary>
        /// Demande la suppression d'une revue
        /// </summary>
        /// <param name="idRevue">id de la revue</param>
        /// <returns>true si suppression effectuée</returns>
        public bool SupprimerRevue(string idRevue)
        {
            String jsonIdRevue = convertToJson("id", idRevue);
            List<Revue> liste = TraitementRecup<Revue>(DELETE, "revue/" + jsonIdRevue, null);
            return (liste != null);
        }

        /// <summary>
        /// Demande la création d'un livre
        /// </summary>
        /// <param name="livre">objet Livre à créer</param>
        /// <returns>true si création effectuée</returns>
        public bool CreerLivre(Livre livre)
        {
            var data = new
            {
                id = livre.Id,
                titre = livre.Titre,
                image = livre.Image,
                idGenre = livre.IdGenre,
                idPublic = livre.IdPublic,
                idRayon = livre.IdRayon,
                isbn = livre.Isbn,
                auteur = livre.Auteur,
                collection = livre.Collection
            };

            string jsonLivre = JsonConvert.SerializeObject(data);
            string parametres = "champs=" + Uri.EscapeDataString(jsonLivre);

            try
            {
                List<Livre> liste = TraitementRecup<Livre>(POST, "livre", parametres);
                return (liste != null);
            }
            catch (Exception ex)
            {
                LogErreur(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Demande la création d'un dvd
        /// </summary>
        /// <param name="dvd">objet Dvd à créer</param>
        /// <returns>true si création effectuée</returns>
        public bool CreerDvd(Dvd dvd)
        {
            var data = new
            {
                id = dvd.Id,
                titre = dvd.Titre,
                image = dvd.Image,
                idGenre = dvd.IdGenre,
                idPublic = dvd.IdPublic,
                idRayon = dvd.IdRayon,
                duree = dvd.Duree,
                realisateur = dvd.Realisateur,
                synopsis = dvd.Synopsis
            };

            string jsonDvd = JsonConvert.SerializeObject(data);
            string parametres = "champs=" + Uri.EscapeDataString(jsonDvd);

            try
            {
                List<Dvd> liste = TraitementRecup<Dvd>(POST, "dvd", parametres);
                return (liste != null);
            }
            catch (Exception ex)
            {
                LogErreur(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Demande la création d'une revue
        /// </summary>
        /// <param name="revue">objet Revue à créer</param>
        /// <returns>true si création effectuée</returns>
        public bool CreerRevue(Revue revue)
        {
            var data = new
            {
                id = revue.Id,
                titre = revue.Titre,
                image = revue.Image,
                idGenre = revue.IdGenre,
                idPublic = revue.IdPublic,
                idRayon = revue.IdRayon,
                periodicite = revue.Periodicite,
                delaiMiseADispo = revue.DelaiMiseADispo
            };

            string jsonRevue = JsonConvert.SerializeObject(data);
            string parametres = "champs=" + Uri.EscapeDataString(jsonRevue);

            try
            {
                List<Revue> liste = TraitementRecup<Revue>(POST, "revue", parametres);
                return (liste != null);
            }
            catch (Exception ex)
            {
                LogErreur(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Demande la modification d'un livre
        /// </summary>
        /// <param name="livre">objet Livre modifié</param>
        /// <returns>true si modification effectuée</returns>
        public bool ModifierLivre(Livre livre)
        {
            var data = new
            {
                id = livre.Id,
                titre = livre.Titre,
                image = livre.Image,
                idGenre = livre.IdGenre,
                idPublic = livre.IdPublic,
                idRayon = livre.IdRayon,
                isbn = livre.Isbn,
                auteur = livre.Auteur,
                collection = livre.Collection
            };

            string jsonLivre = JsonConvert.SerializeObject(data);
            string parametres = "champs=" + Uri.EscapeDataString(jsonLivre);

            try
            {
                List<Livre> liste = TraitementRecup<Livre>(PUT, "livre/" + livre.Id, parametres);
                return (liste != null);
            }
            catch (Exception ex)
            {
                LogErreur(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Demande la modification d'un dvd
        /// </summary>
        /// <param name="dvd">objet Dvd modifié</param>
        /// <returns>true si modification effectuée</returns>
        public bool ModifierDvd(Dvd dvd)
        {
            var data = new
            {
                id = dvd.Id,
                titre = dvd.Titre,
                image = dvd.Image,
                idGenre = dvd.IdGenre,
                idPublic = dvd.IdPublic,
                idRayon = dvd.IdRayon,
                duree = dvd.Duree,
                realisateur = dvd.Realisateur,
                synopsis = dvd.Synopsis
            };

            string jsonDvd = JsonConvert.SerializeObject(data);
            string parametres = "champs=" + Uri.EscapeDataString(jsonDvd);

            try
            {
                List<Dvd> liste = TraitementRecup<Dvd>(PUT, "dvd/" + dvd.Id, parametres);
                return (liste != null);
            }
            catch (Exception ex)
            {
                LogErreur(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Demande la modification d'une revue
        /// </summary>
        /// <param name="revue">objet Revue modifiée</param>
        /// <returns>true si modification effectuée</returns>
        public bool ModifierRevue(Revue revue)
        {
            var data = new
            {
                id = revue.Id,
                titre = revue.Titre,
                image = revue.Image,
                idGenre = revue.IdGenre,
                idPublic = revue.IdPublic,
                idRayon = revue.IdRayon,
                periodicite = revue.Periodicite,
                delaiMiseADispo = revue.DelaiMiseADispo
            };

            string jsonRevue = JsonConvert.SerializeObject(data);
            string parametres = "champs=" + Uri.EscapeDataString(jsonRevue);

            try
            {
                List<Revue> liste = TraitementRecup<Revue>(PUT, "revue/" + revue.Id, parametres);
                return (liste != null);
            }
            catch (Exception ex)
            {
                LogErreur(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Demande la création d'une commande de livre ou dvd
        /// </summary>
        /// <param name="commande">objet CommandeDocument à créer</param>
        /// <returns>true si création effectuée</returns>
        public bool CreerCommandeDocument(CommandeDocument commande)
        {
            var data = new
            {
                id = commande.Id,
                dateCommande = commande.DateCommande,
                montant = commande.Montant,
                nbExemplaire = commande.NbExemplaire,
                idLivreDvd = commande.IdLivreDvd
            };

            string jsonCommande = JsonConvert.SerializeObject(data, new CustomDateTimeConverter());
            string parametres = "champs=" + Uri.EscapeDataString(jsonCommande);

            try
            {
                List<CommandeDocument> liste = TraitementRecup<CommandeDocument>(POST, "commandedocument", parametres);
                return (liste != null);
            }
            catch (Exception ex)
            {
                LogErreur(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Demande la modification d'une commande de livre ou dvd
        /// </summary>
        /// <param name="commande">objet CommandeDocument à modifier</param>
        /// <returns>true si modification effectuée</returns>
        public bool ModifierCommandeDocument(CommandeDocument commande)
        {
            var data = new
            {
                id = commande.Id,
                dateCommande = commande.DateCommande,
                montant = commande.Montant,
                nbExemplaire = commande.NbExemplaire,
                idLivreDvd = commande.IdLivreDvd,
                idSuivi = commande.IdSuivi
            };

            string jsonCommande = JsonConvert.SerializeObject(data, new CustomDateTimeConverter());
            string parametres = "champs=" + Uri.EscapeDataString(jsonCommande);

            try
            {
                List<CommandeDocument> liste = TraitementRecup<CommandeDocument>(PUT, "commandedocument/" + commande.Id, parametres);
                return (liste != null);
            }
            catch (Exception ex)
            {
                LogErreur(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Demande la suppression d'une commande de livre ou dvd
        /// </summary>
        /// <param name="idCommande">id de la commande</param>
        /// <returns>true si suppression effectuée</returns>
        public bool SupprimerCommandeDocument(string idCommande)
        {
            string jsonIdCommande = convertToJson("id", idCommande);
            try
            {
                List<CommandeDocument> liste = TraitementRecup<CommandeDocument>(DELETE, "commandedocument/" + jsonIdCommande, null);
                return (liste != null);
            }
            catch (Exception ex)
            {
                LogErreur(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Récupère les abonnements d'une revue
        /// </summary>
        /// <param name="idRevue">id de la revue concernée</param>
        /// <returns>Liste d'objets CommandeRevue</returns>
        public List<CommandeRevue> GetCommandesRevue(string idRevue)
        {
            string jsonIdRevue = convertToJson("id", idRevue);
            List<CommandeRevue> lesCommandes = TraitementRecup<CommandeRevue>(GET, "commanderevue/" + jsonIdRevue, null);
            return lesCommandes;
        }

        /// <summary>
        /// Demande la création d'un abonnement de revue
        /// </summary>
        /// <param name="commande">objet CommandeRevue à créer</param>
        /// <returns>true si création effectuée</returns>
        public bool CreerCommandeRevue(CommandeRevue commande)
        {
            var data = new
            {
                id = commande.Id,
                dateCommande = commande.DateCommande,
                montant = commande.Montant,
                dateFinAbonnement = commande.DateFinAbonnement,
                idRevue = commande.IdRevue
            };

            string jsonCommande = JsonConvert.SerializeObject(data, new CustomDateTimeConverter());
            string parametres = "champs=" + Uri.EscapeDataString(jsonCommande);

            try
            {
                List<CommandeRevue> liste = TraitementRecup<CommandeRevue>(POST, "commanderevue", parametres);
                return (liste != null);
            }
            catch (Exception ex)
            {
                LogErreur(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Demande la modification d'un abonnement de revue
        /// </summary>
        /// <param name="commande">objet CommandeRevue à modifier</param>
        /// <returns>true si modification effectuée</returns>
        public bool ModifierCommandeRevue(CommandeRevue commande)
        {
            var data = new
            {
                id = commande.Id,
                dateCommande = commande.DateCommande,
                montant = commande.Montant,
                dateFinAbonnement = commande.DateFinAbonnement,
                idRevue = commande.IdRevue
            };

            string jsonCommande = JsonConvert.SerializeObject(data, new CustomDateTimeConverter());
            string parametres = "champs=" + Uri.EscapeDataString(jsonCommande);

            try
            {
                List<CommandeRevue> liste = TraitementRecup<CommandeRevue>(PUT, "commanderevue/" + commande.Id, parametres);
                return (liste != null);
            }
            catch (Exception ex)
            {
                LogErreur(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Demande la suppression d'un abonnement de revue
        /// </summary>
        /// <param name="idCommande">id de la commande</param>
        /// <returns>true si suppression effectuée</returns>
        public bool SupprimerCommandeRevue(string idCommande)
        {
            string jsonIdCommande = convertToJson("id", idCommande);
            try
            {
                List<CommandeRevue> liste = TraitementRecup<CommandeRevue>(DELETE, "commanderevue/" + jsonIdCommande, null);
                return (liste != null);
            }
            catch (Exception ex)
            {
                LogErreur(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Récupère les abonnements de revue qui expirent dans moins de 30 jours
        /// </summary>
        /// <returns>Liste d'objets CommandeRevue</returns>
        public List<CommandeRevue> GetCommandesRevueExpirationProche()
        {
            List<CommandeRevue> lesCommandes = TraitementRecup<CommandeRevue>(GET, "commanderevueexpiration", null);
            return lesCommandes;
        }

        /// <summary>
        /// récupère les exemplaires d'un document
        /// </summary>
        /// <param name="idDocument">id du document concerné</param>
        /// <returns>Liste d'objets Exemplaire</returns>
        public List<Exemplaire> GetExemplairesDocument(string idDocument)
        {
            string jsonIdDocument = convertToJson("id", idDocument);
            List<Exemplaire> lesExemplaires = TraitementRecup<Exemplaire>(GET, "exemplaire/" + jsonIdDocument, null);
            return lesExemplaires;
        }

        /// <summary>
        /// récupère tous les états possibles
        /// </summary>
        /// <returns>Liste d'objets Categorie</returns>
        public List<Categorie> GetAllEtats()
        {
            List<Etat> lesEtats = TraitementRecup<Etat>(GET, "etat", null);

            return lesEtats
                .Select(e => new Categorie(e.Id, e.Libelle))
                .ToList();
        }

        /// <summary>
        /// modifie l'état d'un exemplaire
        /// </summary>
        /// <param name="exemplaire">exemplaire concerné</param>
        /// <returns>true si la modification a pu se faire</returns>
        public bool ModifierExemplaire(Exemplaire exemplaire)
        {
            var data = new
            {
                id = exemplaire.Id,
                numero = exemplaire.Numero,
                dateAchat = exemplaire.DateAchat,
                photo = exemplaire.Photo,
                idEtat = exemplaire.IdEtat
            };

            string jsonExemplaire = JsonConvert.SerializeObject(data, new CustomDateTimeConverter());
            string parametres = "champs=" + Uri.EscapeDataString(jsonExemplaire);

            try
            {
                List<Exemplaire> liste = TraitementRecup<Exemplaire>(PUT, "exemplaire/" + exemplaire.Id, parametres);
                return (liste != null);
            }
            catch (Exception ex)
            {
                LogErreur(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// supprime un exemplaire
        /// </summary>
        /// <param name="idDocument">id du document</param>
        /// <param name="numero">numéro de l'exemplaire</param>
        /// <returns>true si la suppression a pu se faire</returns>
        public bool SupprimerExemplaire(string idDocument, int numero)
        {
            var data = new
            {
                id = idDocument,
                numero = numero
            };

            string jsonIdDocument = convertToJson("id", idDocument);
            string jsonExemplaire = JsonConvert.SerializeObject(data);
            string parametres = "champs=" + Uri.EscapeDataString(jsonExemplaire);

            try
            {
                List<Exemplaire> liste = TraitementRecup<Exemplaire>(DELETE, "exemplaire/" + jsonIdDocument, parametres);
                return (liste != null);
            }
            catch (Exception ex)
            {
                LogErreur(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// écrit un message dans la console et dans le fichier de logs
        /// </summary>
        /// <param name="message">message à enregistrer</param>
        private void Log(string message)
        {
            string messageLog = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " [INFO] " + message;
            Console.WriteLine(messageLog);
            EcrireDansFichierLog(messageLog);
        }

        /// <summary>
        /// écrit une erreur dans la console et dans le fichier de logs
        /// </summary>
        /// <param name="message">message d'erreur</param>
        private void LogErreur(string message)
        {
            string messageLog = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " [ERREUR] " + message;
            Console.WriteLine(messageLog);
            EcrireDansFichierLog(messageLog);
        }

        /// <summary>
        /// écrit dans le fichier de logs
        /// </summary>
        /// <param name="message">message à enregistrer</param>
        private void EcrireDansFichierLog(string message)
        {
            try
            {
                string cheminLog = logFilePath;

                if (!Path.IsPathRooted(cheminLog))
                {
                    cheminLog = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, cheminLog);
                }

                string dossier = Path.GetDirectoryName(cheminLog);
                if (!string.IsNullOrEmpty(dossier) && !Directory.Exists(dossier))
                {
                    Directory.CreateDirectory(dossier);
                }

                File.AppendAllText(cheminLog, message + Environment.NewLine);
            }
            catch
            {
                // pas de blocage de l'application si échec de l'écriture dans le fichier de logs
            }
        }
    }
}
