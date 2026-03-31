using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaTekDocuments.model
{
    /// <summary>
    /// Classe métier CommandeDocument
    /// </summary>
    public class CommandeDocument
    {
        public string Id { get; set; }
        public string DateCommande { get; set; }
        public double Montant { get; set; }
        public int NbExemplaire { get; set; }
        public string IdLivreDvd { get; set; }
        public string IdSuivi { get; set; }
        public string LibelleSuivi { get; set; }

        public CommandeDocument(string id, string dateCommande, double montant, int nbExemplaire, string idLivreDvd, string idSuivi, string libelleSuivi)
        {
            Id = id;
            DateCommande = dateCommande;
            Montant = montant;
            NbExemplaire = nbExemplaire;
            IdLivreDvd = idLivreDvd;
            IdSuivi = idSuivi;
            LibelleSuivi = libelleSuivi;
        }
    }
}
