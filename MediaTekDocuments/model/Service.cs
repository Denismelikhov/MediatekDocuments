using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaTekDocuments.model
{
    /// <summary>
    /// Classe métier Service : contient des propriétés spécifiques aux services des utilisateurs
    /// </summary>
    public class Service
    {
        public string Id { get; }
        public string Libelle { get; }

        public Service(string id, string libelle)
        {
            Id = id;
            Libelle = libelle;
        }
    }
}
