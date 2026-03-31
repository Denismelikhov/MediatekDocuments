using System;

namespace MediaTekDocuments.model
{
    /// <summary>
    /// Classe métier Exemplaire (exemplaire d'une revue)
    /// </summary>
    public class Exemplaire
    {
        public int Numero { get; set; }
        public string Photo { get; set; }
        public DateTime DateAchat { get; set; }
        public string IdEtat { get; set; }
        public string Id { get; set; }

        public string Etat
        {
            get
            {
                switch (IdEtat)
                {
                    case "00001": return "neuf";
                    case "00002": return "usagé";
                    case "00003": return "détérioré";
                    case "00004": return "inutilisable";
                    default: return "";
                }
            }
        }

        public Exemplaire(int numero, DateTime dateAchat, string photo, string idEtat, string idDocument)
        {
            this.Numero = numero;
            this.DateAchat = dateAchat;
            this.Photo = photo;
            this.IdEtat = idEtat;
            this.Id = idDocument;
        }
    }
}
