using System.Collections.Generic;
using MediaTekDocuments.model;
using MediaTekDocuments.dal;

namespace MediaTekDocuments.controller
{
    /// <summary>
    /// Contrôleur lié à FrmMediatek
    /// </summary>
    class FrmMediatekController
    {
        /// <summary>
        /// Objet d'accès aux données
        /// </summary>
        private readonly Access access;

        /// <summary>
        /// Récupération de l'instance unique d'accès aux données
        /// </summary>
        public FrmMediatekController()
        {
            access = Access.GetInstance();
        }

        /// <summary>
        /// getter sur la liste des genres
        /// </summary>
        /// <returns>Liste d'objets Genre</returns>
        public List<Categorie> GetAllGenres()
        {
            return access.GetAllGenres();
        }

        /// <summary>
        /// getter sur la liste des livres
        /// </summary>
        /// <returns>Liste d'objets Livre</returns>
        public List<Livre> GetAllLivres()
        {
            return access.GetAllLivres();
        }

        /// <summary>
        /// getter sur la liste des Dvd
        /// </summary>
        /// <returns>Liste d'objets dvd</returns>
        public List<Dvd> GetAllDvd()
        {
            return access.GetAllDvd();
        }

        /// <summary>
        /// getter sur la liste des revues
        /// </summary>
        /// <returns>Liste d'objets Revue</returns>
        public List<Revue> GetAllRevues()
        {
            return access.GetAllRevues();
        }

        /// <summary>
        /// getter sur les rayons
        /// </summary>
        /// <returns>Liste d'objets Rayon</returns>
        public List<Categorie> GetAllRayons()
        {
            return access.GetAllRayons();
        }

        /// <summary>
        /// getter sur les publics
        /// </summary>
        /// <returns>Liste d'objets Public</returns>
        public List<Categorie> GetAllPublics()
        {
            return access.GetAllPublics();
        }


        /// <summary>
        /// récupère les exemplaires d'une revue
        /// </summary>
        /// <param name="idDocuement">id de la revue concernée</param>
        /// <returns>Liste d'objets Exemplaire</returns>
        public List<Exemplaire> GetExemplairesRevue(string idDocuement)
        {
            return access.GetExemplairesRevue(idDocuement);
        }

        /// <summary>
        /// Crée un exemplaire d'une revue dans la bdd
        /// </summary>
        /// <param name="exemplaire">L'objet Exemplaire concerné</param>
        /// <returns>True si la création a pu se faire</returns>
        public bool CreerExemplaire(Exemplaire exemplaire)
        {
            return access.CreerExemplaire(exemplaire);
        }

        /// <summary>
        /// Supprimer un document dans la bdd
        /// </summary>
        /// <param name="idDocument">L'objet Document concerné</param>
        /// <returns>True si la supression a pu se faire</returns>
        public bool SupprimerDocument(string idDocument)
        {
            return access.SupprimerDocument(idDocument);
        }

        /// <summary>
        /// récupère les commandes d'un livre ou dvd
        /// </summary>
        /// <param name="idDocument">id du document concerné</param>
        /// <returns>Liste d'objets CommandeDocument</returns>
        public List<CommandeDocument> GetCommandesDocument(string idDocument)
        {
            return access.GetCommandesDocument(idDocument);
        }

        /// <summary>
        /// indique si un document peut être supprimé
        /// </summary>
        /// <param name="document">document concerné</param>
        /// <returns>true si suppression autorisée</returns>
        public bool DocumentSupprimable(Document document)
        {
            return access.DocumentSupprimable(document);
        }

        /// <summary>
        /// demande la suppression d'un livre
        /// </summary>
        /// <param name="idLivre">id du livre</param>
        /// <returns>true si suppression effectuée</returns>
        public bool SupprimerLivre(string idLivre)
        {
            return access.SupprimerLivre(idLivre);
        }

        /// <summary>
        /// demande la suppression d'un dvd
        /// </summary>
        /// <param name="idDvd">id du dvd</param>
        /// <returns>true si suppression effectuée</returns>
        public bool SupprimerDvd(string idDvd)
        {
            return access.SupprimerDvd(idDvd);
        }

        /// <summary>
        /// demande la suppression d'une revue
        /// </summary>
        /// <param name="idRevue">id de la revue</param>
        /// <returns>true si suppression effectuée</returns>
        public bool SupprimerRevue(string idRevue)
        {
            return access.SupprimerRevue(idRevue);
        }

        /// <summary>
        /// Demande la création d'un livre
        /// </summary>
        public bool CreerLivre(Livre livre)
        {
            return access.CreerLivre(livre);
        }

        /// <summary>
        /// Demande la création d'un dvd
        /// </summary>
        public bool CreerDvd(Dvd dvd)
        {
            return access.CreerDvd(dvd);
        }

        /// <summary>
        /// Demande la création d'une revue
        /// </summary>
        public bool CreerRevue(Revue revue)
        {
            return access.CreerRevue(revue);
        }

        /// <summary>
        /// Demande la modification d'un livre
        /// </summary>
        public bool ModifierLivre(Livre livre)
        {
            return access.ModifierLivre(livre);
        }

        /// <summary>
        /// Demande la modification d'un dvd
        /// </summary>
        public bool ModifierDvd(Dvd dvd)
        {
            return access.ModifierDvd(dvd);
        }

        /// <summary>
        /// Demande la modification d'une revue
        /// </summary>
        public bool ModifierRevue(Revue revue)
        {
            return access.ModifierRevue(revue);
        }

        /// <summary>
        /// Demande la création d'une commande de livre ou dvd
        /// </summary>
        /// <param name="commande">commande concernée</param>
        /// <returns>true si création effectuée</returns>
        public bool CreerCommandeDocument(CommandeDocument commande)
        {
            return access.CreerCommandeDocument(commande);
        }

        /// <summary>
        /// Demande la modification d'une commande de livre ou dvd
        /// </summary>
        /// <param name="commande">commande concernée</param>
        /// <returns>true si modification effectuée</returns>
        public bool ModifierCommandeDocument(CommandeDocument commande)
        {
            return access.ModifierCommandeDocument(commande);
        }

        /// <summary>
        /// Demande la suppression d'une commande de livre ou dvd
        /// </summary>
        /// <param name="idCommande">id de la commande</param>
        /// <returns>true si suppression effectuée</returns>
        public bool SupprimerCommandeDocument(string idCommande)
        {
            return access.SupprimerCommandeDocument(idCommande);
        }

        /// <summary>
        /// récupère les commandes d'une revue
        /// </summary>
        /// <param name="idRevue">id de la revue concernée</param>
        /// <returns>Liste d'objets CommandeRevue</returns>
        public List<CommandeRevue> GetCommandesRevue(string idRevue)
        {
            return access.GetCommandesRevue(idRevue);
        }

        /// <summary>
        /// crée une commande de revue dans la bdd
        /// </summary>
        /// <param name="commande">objet CommandeRevue concerné</param>
        /// <returns>true si la création a pu se faire</returns>
        public bool CreerCommandeRevue(CommandeRevue commande)
        {
            return access.CreerCommandeRevue(commande);
        }

        /// <summary>
        /// modifie une commande de revue dans la bdd
        /// </summary>
        /// <param name="commande">objet CommandeRevue concerné</param>
        /// <returns>true si la modification a pu se faire</returns>
        public bool ModifierCommandeRevue(CommandeRevue commande)
        {
            return access.ModifierCommandeRevue(commande);
        }

        /// <summary>
        /// supprime une commande de revue dans la bdd
        /// </summary>
        /// <param name="idCommande">id de la commande concernée</param>
        /// <returns>true si la suppression a pu se faire</returns>
        public bool SupprimerCommandeRevue(string idCommande)
        {
            return access.SupprimerCommandeRevue(idCommande);
        }

        /// <summary>
        /// récupère les commandes de revue dont l'abonnement expire dans moins de 30 jours
        /// </summary>
        /// <returns>Liste d'objets CommandeRevue</returns>
        public List<CommandeRevue> GetCommandesRevueExpirationProche()
        {
            return access.GetCommandesRevueExpirationProche();
        }
    }
}
