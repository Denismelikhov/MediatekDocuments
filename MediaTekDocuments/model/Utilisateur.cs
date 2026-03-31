using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaTekDocuments.model
{
    /// <summary>
    /// Classe métier Utilisateur : contient des propriétés spécifiques aux utilisateurs
    /// </summary>
    public class Utilisateur
    {
        public string id { get; set; }
        public string login { get; set; }
        public string idService { get; set; }
        public string service { get; set; }

        public bool AccesTotal => service == "Administratif" || service == "Administrateur";
        public bool ConsultationSeulement => service == "Prêts";
        public bool AccesRefuse => service == "Culture";
    }
}