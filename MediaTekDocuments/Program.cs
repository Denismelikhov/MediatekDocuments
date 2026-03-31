using MediaTekDocuments.model;
using MediaTekDocuments.view;
using System;
using System.Windows.Forms;

namespace MediaTekDocuments
{
    static class Program
    {
        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            FrmAuthentification frmAuthentification = new FrmAuthentification();
            
            if (frmAuthentification.ShowDialog() == DialogResult.OK)
            {
                Utilisateur utilisateur = frmAuthentification.UtilisateurConnecte;
                Application.Run(new FrmMediatek(utilisateur));
            }
        }
    }
}
