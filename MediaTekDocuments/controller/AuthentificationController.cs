using MediaTekDocuments.dal;
using MediaTekDocuments.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaTekDocuments.controller
{
    class AuthentificationController
    {
        private readonly Access access;

        public AuthentificationController()
        {
            access = Access.GetInstance();
        }

        public Utilisateur ControleAuthentification(string login, string pwd)
        {
            return access.GetUtilisateur(login, pwd);
        }
    }
}
