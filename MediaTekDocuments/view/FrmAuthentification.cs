using MediaTekDocuments.controller;
using MediaTekDocuments.model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MediaTekDocuments.view
{
    public partial class FrmAuthentification : Form
    {
        private readonly AuthentificationController controller;
        public Utilisateur UtilisateurConnecte { get; private set; }

        public FrmAuthentification()
        {
            InitializeComponent();
            controller = new AuthentificationController();
        }

        private void btnConnexion_Click(object sender, EventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string pwd = txtPwd.Text.Trim();

            if (login == "" || pwd == "")
            {
                MessageBox.Show("Veuillez saisir le login et le mot de passe.");
                return;
            }

            Utilisateur utilisateur = controller.ControleAuthentification(login, pwd);

            if (utilisateur == null)
            {
                MessageBox.Show("Authentification incorrecte.");
                txtPwd.Clear();
                txtPwd.Focus();
                return;
            }

            if (utilisateur.AccesRefuse)
            {
                MessageBox.Show("Cette application n'est pas accessible pour les employés du service Culture.");
                txtPwd.Clear();
                return;
            }

            UtilisateurConnecte = utilisateur;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnAnnuler_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
