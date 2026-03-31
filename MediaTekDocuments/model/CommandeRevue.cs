using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaTekDocuments.model
{
    public class CommandeRevue
    {
        public string Id { get; set; }
        public string DateCommande { get; set; }
        public string DateFinAbonnement { get; set; }
        public double Montant { get; set; }
        public string IdRevue { get; set; }

        public CommandeRevue(string id, string dateCommande, string dateFinAbonnement, double montant, string idRevue)
        {
            Id = id;
            DateCommande = dateCommande;
            DateFinAbonnement = dateFinAbonnement;
            Montant = montant;
            IdRevue = idRevue;
        }
    }
}
