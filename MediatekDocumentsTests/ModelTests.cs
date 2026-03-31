using Microsoft.VisualStudio.TestTools.UnitTesting;
using MediaTekDocuments.model;
using System;

namespace MediaTekDocumentsTests.model
{
    [TestClass]
    public class ModelTests
    {
        [TestMethod]
        public void Categorie_Constructor_Initialise_Properties()
        {
            Categorie categorie = new Categorie("100", "Roman");

            Assert.AreEqual("100", categorie.Id);
            Assert.AreEqual("Roman", categorie.Libelle);
        }

        [TestMethod]
        public void Categorie_ToString_Returns_Libelle()
        {
            Categorie categorie = new Categorie("100", "Roman");

            Assert.AreEqual("Roman", categorie.ToString());
        }

        [TestMethod]
        public void Genre_Inherits_Categorie_And_Initialises_Properties()
        {
            Genre genre = new Genre("G1", "Policier");

            Assert.AreEqual("G1", genre.Id);
            Assert.AreEqual("Policier", genre.Libelle);
            Assert.IsInstanceOfType(genre, typeof(Categorie));
        }

        [TestMethod]
        public void Public_Inherits_Categorie_And_Initialises_Properties()
        {
            Public lePublic = new Public("P1", "Adulte");

            Assert.AreEqual("P1", lePublic.Id);
            Assert.AreEqual("Adulte", lePublic.Libelle);
            Assert.IsInstanceOfType(lePublic, typeof(Categorie));
        }

        [TestMethod]
        public void Rayon_Inherits_Categorie_And_Initialises_Properties()
        {
            Rayon rayon = new Rayon("R1", "Fiction");

            Assert.AreEqual("R1", rayon.Id);
            Assert.AreEqual("Fiction", rayon.Libelle);
            Assert.IsInstanceOfType(rayon, typeof(Categorie));
        }

        [TestMethod]
        public void Document_Constructor_Initialise_Properties()
        {
            Document document = new Document(
                "D1", "Titre test", "image.jpg",
                "G1", "Roman",
                "P1", "Adulte",
                "R1", "Fiction"
            );

            Assert.AreEqual("D1", document.Id);
            Assert.AreEqual("Titre test", document.Titre);
            Assert.AreEqual("image.jpg", document.Image);
            Assert.AreEqual("G1", document.IdGenre);
            Assert.AreEqual("Roman", document.Genre);
            Assert.AreEqual("P1", document.IdPublic);
            Assert.AreEqual("Adulte", document.Public);
            Assert.AreEqual("R1", document.IdRayon);
            Assert.AreEqual("Fiction", document.Rayon);
        }

        [TestMethod]
        public void Livre_Constructor_Initialise_Properties_And_Inheritance()
        {
            Livre livre = new Livre(
                "L1", "1984", "1984.jpg",
                "9782070368228", "George Orwell", "Folio",
                "G1", "Roman",
                "P1", "Adulte",
                "R1", "Littérature"
            );

            Assert.AreEqual("L1", livre.Id);
            Assert.AreEqual("1984", livre.Titre);
            Assert.AreEqual("1984.jpg", livre.Image);
            Assert.AreEqual("9782070368228", livre.Isbn);
            Assert.AreEqual("George Orwell", livre.Auteur);
            Assert.AreEqual("Folio", livre.Collection);
            Assert.AreEqual("G1", livre.IdGenre);
            Assert.AreEqual("Roman", livre.Genre);
            Assert.AreEqual("P1", livre.IdPublic);
            Assert.AreEqual("Adulte", livre.Public);
            Assert.AreEqual("R1", livre.IdRayon);
            Assert.AreEqual("Littérature", livre.Rayon);

            Assert.IsInstanceOfType(livre, typeof(LivreDvd));
            Assert.IsInstanceOfType(livre, typeof(Document));
        }

        [TestMethod]
        public void Dvd_Constructor_Initialise_Properties_And_Inheritance()
        {
            Dvd dvd = new Dvd(
                "DVD1", "Inception", "inception.jpg",
                148, "Christopher Nolan", "Un thriller de science-fiction",
                "G2", "Science-fiction",
                "P2", "Tout public",
                "R2", "Cinéma"
            );

            Assert.AreEqual("DVD1", dvd.Id);
            Assert.AreEqual("Inception", dvd.Titre);
            Assert.AreEqual("inception.jpg", dvd.Image);
            Assert.AreEqual(148, dvd.Duree);
            Assert.AreEqual("Christopher Nolan", dvd.Realisateur);
            Assert.AreEqual("Un thriller de science-fiction", dvd.Synopsis);
            Assert.AreEqual("G2", dvd.IdGenre);
            Assert.AreEqual("Science-fiction", dvd.Genre);
            Assert.AreEqual("P2", dvd.IdPublic);
            Assert.AreEqual("Tout public", dvd.Public);
            Assert.AreEqual("R2", dvd.IdRayon);
            Assert.AreEqual("Cinéma", dvd.Rayon);

            Assert.IsInstanceOfType(dvd, typeof(LivreDvd));
            Assert.IsInstanceOfType(dvd, typeof(Document));
        }

        [TestMethod]
        public void Revue_Constructor_Initialise_Properties()
        {
            Revue revue = new Revue(
                "REV1", "Science & Vie", "sciencevie.jpg",
                "G3", "Sciences",
                "P3", "Ados",
                "R3", "Revues",
                "Mensuel", 30
            );

            Assert.AreEqual("REV1", revue.Id);
            Assert.AreEqual("Science & Vie", revue.Titre);
            Assert.AreEqual("sciencevie.jpg", revue.Image);
            Assert.AreEqual("G3", revue.IdGenre);
            Assert.AreEqual("Sciences", revue.Genre);
            Assert.AreEqual("P3", revue.IdPublic);
            Assert.AreEqual("Ados", revue.Public);
            Assert.AreEqual("R3", revue.IdRayon);
            Assert.AreEqual("Revues", revue.Rayon);
            Assert.AreEqual("Mensuel", revue.Periodicite);
            Assert.AreEqual(30, revue.DelaiMiseADispo);

            Assert.IsInstanceOfType(revue, typeof(Document));
        }

        [TestMethod]
        public void Etat_Constructor_Initialise_Properties()
        {
            Etat etat = new Etat("00001", "neuf");

            Assert.AreEqual("00001", etat.Id);
            Assert.AreEqual("neuf", etat.Libelle);
        }

        [TestMethod]
        public void Exemplaire_Constructor_Initialise_Properties()
        {
            DateTime dateAchat = new DateTime(2025, 1, 15);

            Exemplaire exemplaire = new Exemplaire(12, dateAchat, "photo.jpg", "00001", "REV1");

            Assert.AreEqual(12, exemplaire.Numero);
            Assert.AreEqual(dateAchat, exemplaire.DateAchat);
            Assert.AreEqual("photo.jpg", exemplaire.Photo);
            Assert.AreEqual("00001", exemplaire.IdEtat);
            Assert.AreEqual("REV1", exemplaire.Id);
        }

        [TestMethod]
        public void Exemplaire_Etat_Returns_Neuf_For_00001()
        {
            Exemplaire exemplaire = new Exemplaire(1, DateTime.Today, "", "00001", "REV1");

            Assert.AreEqual("neuf", exemplaire.Etat);
        }

        [TestMethod]
        public void Exemplaire_Etat_Returns_Usage_For_00002()
        {
            Exemplaire exemplaire = new Exemplaire(1, DateTime.Today, "", "00002", "REV1");

            Assert.AreEqual("usagé", exemplaire.Etat);
        }

        [TestMethod]
        public void Exemplaire_Etat_Returns_Deteriore_For_00003()
        {
            Exemplaire exemplaire = new Exemplaire(1, DateTime.Today, "", "00003", "REV1");

            Assert.AreEqual("détérioré", exemplaire.Etat);
        }

        [TestMethod]
        public void Exemplaire_Etat_Returns_Inutilisable_For_00004()
        {
            Exemplaire exemplaire = new Exemplaire(1, DateTime.Today, "", "00004", "REV1");

            Assert.AreEqual("inutilisable", exemplaire.Etat);
        }

        [TestMethod]
        public void Exemplaire_Etat_Returns_Empty_For_Unknown_IdEtat()
        {
            Exemplaire exemplaire = new Exemplaire(1, DateTime.Today, "", "99999", "REV1");

            Assert.AreEqual("", exemplaire.Etat);
        }

        [TestMethod]
        public void CommandeDocument_Constructor_Initialise_Properties()
        {
            CommandeDocument commande = new CommandeDocument(
                "CMD1", "2026-03-31", 125.50, 3, "L1", "S1", "en cours"
            );

            Assert.AreEqual("CMD1", commande.Id);
            Assert.AreEqual("2026-03-31", commande.DateCommande);
            Assert.AreEqual(125.50, commande.Montant);
            Assert.AreEqual(3, commande.NbExemplaire);
            Assert.AreEqual("L1", commande.IdLivreDvd);
            Assert.AreEqual("S1", commande.IdSuivi);
            Assert.AreEqual("en cours", commande.LibelleSuivi);
        }

        [TestMethod]
        public void CommandeRevue_Constructor_Initialise_Properties()
        {
            CommandeRevue commande = new CommandeRevue(
                "CR1", "2026-03-31", "2027-03-31", 89.99, "REV1"
            );

            Assert.AreEqual("CR1", commande.Id);
            Assert.AreEqual("2026-03-31", commande.DateCommande);
            Assert.AreEqual("2027-03-31", commande.DateFinAbonnement);
            Assert.AreEqual(89.99, commande.Montant);
            Assert.AreEqual("REV1", commande.IdRevue);
        }

        [TestMethod]
        public void Service_Constructor_Initialise_Properties()
        {
            Service service = new Service("S1", "Administratif");

            Assert.AreEqual("S1", service.Id);
            Assert.AreEqual("Administratif", service.Libelle);
        }

        [TestMethod]
        public void Utilisateur_AccesTotal_Returns_True_For_Administratif()
        {
            Utilisateur utilisateur = new Utilisateur
            {
                id = "U1",
                login = "admin1",
                idService = "S1",
                service = "Administratif"
            };

            Assert.IsTrue(utilisateur.AccesTotal);
            Assert.IsFalse(utilisateur.ConsultationSeulement);
            Assert.IsFalse(utilisateur.AccesRefuse);
        }

        [TestMethod]
        public void Utilisateur_AccesTotal_Returns_True_For_Administrateur()
        {
            Utilisateur utilisateur = new Utilisateur
            {
                id = "U2",
                login = "admin2",
                idService = "S2",
                service = "Administrateur"
            };

            Assert.IsTrue(utilisateur.AccesTotal);
            Assert.IsFalse(utilisateur.ConsultationSeulement);
            Assert.IsFalse(utilisateur.AccesRefuse);
        }

        [TestMethod]
        public void Utilisateur_ConsultationSeulement_Returns_True_For_Prets()
        {
            Utilisateur utilisateur = new Utilisateur
            {
                id = "U3",
                login = "pret1",
                idService = "S3",
                service = "Prêts"
            };

            Assert.IsFalse(utilisateur.AccesTotal);
            Assert.IsTrue(utilisateur.ConsultationSeulement);
            Assert.IsFalse(utilisateur.AccesRefuse);
        }

        [TestMethod]
        public void Utilisateur_AccesRefuse_Returns_True_For_Culture()
        {
            Utilisateur utilisateur = new Utilisateur
            {
                id = "U4",
                login = "culture1",
                idService = "S4",
                service = "Culture"
            };

            Assert.IsFalse(utilisateur.AccesTotal);
            Assert.IsFalse(utilisateur.ConsultationSeulement);
            Assert.IsTrue(utilisateur.AccesRefuse);
        }

        [TestMethod]
        public void Utilisateur_All_Access_Flags_Return_False_For_Unknown_Service()
        {
            Utilisateur utilisateur = new Utilisateur
            {
                id = "U5",
                login = "autre1",
                idService = "S5",
                service = "Inconnu"
            };

            Assert.IsFalse(utilisateur.AccesTotal);
            Assert.IsFalse(utilisateur.ConsultationSeulement);
            Assert.IsFalse(utilisateur.AccesRefuse);
        }
    }
}