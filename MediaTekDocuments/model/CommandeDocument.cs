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
        public string Id { get; }
        public string DateCommande { get; }
        public double Montant { get; }
        public int NbExemplaire { get; }
        public string IdLivreDvd { get; }

        public CommandeDocument(string id, string dateCommande, double montant, int nbExemplaire, string idLivreDvd)
        {
            Id = id;
            DateCommande = dateCommande;
            Montant = montant;
            NbExemplaire = nbExemplaire;
            IdLivreDvd = idLivreDvd;
        }
    }
}
