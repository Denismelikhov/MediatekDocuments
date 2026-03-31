using System;
using System.Windows.Forms;
using MediaTekDocuments.model;
using MediaTekDocuments.controller;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.IO;

namespace MediaTekDocuments.view

{
    /// <summary>
    /// Classe d'affichage
    /// </summary>
    public partial class FrmMediatek : Form
    {
        #region Commun
        private readonly FrmMediatekController controller;
        private readonly BindingSource bdgGenres = new BindingSource();
        private readonly BindingSource bdgPublics = new BindingSource();
        private readonly BindingSource bdgRayons = new BindingSource();

        /// <summary>
        /// Constructeur : création du contrôleur lié à ce formulaire
        /// </summary>
        internal FrmMediatek()
        {
            InitializeComponent();
            this.controller = new FrmMediatekController();
            InitialiserOngletCommandes();
            InitialiserOngletAbonnements();
        }

        private Livre RecupererLivreSelectionne()
        {
            if (dgvLivresListe.CurrentCell != null && bdgLivresListe.Position >= 0)
            {
                return (Livre)bdgLivresListe.List[bdgLivresListe.Position];
            }
            return null;
        }

        private Dvd RecupererDvdSelectionne()
        {
            if (dgvDvdListe.CurrentCell != null && bdgDvdListe.Position >= 0)
            {
                return (Dvd)bdgDvdListe.List[bdgDvdListe.Position];
            }
            return null;
        }

        private Revue RecupererRevueSelectionnee()
        {
            if (dgvRevuesListe.CurrentCell != null && bdgRevuesListe.Position >= 0)
            {
                return (Revue)bdgRevuesListe.List[bdgRevuesListe.Position];
            }
            return null;
        }

        private void RechargerLivres()
        {
            lesLivres = controller.GetAllLivres();
            RemplirLivresListeComplete();
        }

        private void RechargerDvd()
        {
            lesDvd = controller.GetAllDvd();
            RemplirDvdListeComplete();
        }

        private void RechargerRevues()
        {
            lesRevues = controller.GetAllRevues();
            RemplirRevuesListeComplete();
        }

        private string TrouverIdGenre(string libelle)
        {
            Categorie genre = controller.GetAllGenres()
                .Find(g => g.Libelle.Trim().ToLower().Equals(libelle.Trim().ToLower()));
            return genre == null ? null : genre.Id;
        }

        private string TrouverIdPublic(string libelle)
        {
            Categorie lePublic = controller.GetAllPublics()
                .Find(p => p.Libelle.Trim().ToLower().Equals(libelle.Trim().ToLower()));
            return lePublic == null ? null : lePublic.Id;
        }

        private string TrouverIdRayon(string libelle)
        {
            Categorie rayon = controller.GetAllRayons()
                .Find(r => r.Libelle.Trim().ToLower().Equals(libelle.Trim().ToLower()));
            return rayon == null ? null : rayon.Id;
        }

        /// <summary>
        /// Rempli un des 3 combo (genre, public, rayon)
        /// </summary>
        /// <param name="lesCategories">liste des objets de type Genre ou Public ou Rayon</param>
        /// <param name="bdg">bindingsource contenant les informations</param>
        /// <param name="cbx">combobox à remplir</param>
        public void RemplirComboCategorie(List<Categorie> lesCategories, BindingSource bdg, ComboBox cbx)
        {
            bdg.DataSource = lesCategories;
            cbx.DataSource = bdg;
            if (cbx.Items.Count > 0)
            {
                cbx.SelectedIndex = -1;
            }
        }
        #endregion

        #region Onglet Livres
        private readonly BindingSource bdgLivresListe = new BindingSource();
        private List<Livre> lesLivres = new List<Livre>();
        private bool livresModeAjout = false;
        private bool livresModeModification = false;

        private void ViderChampsLivre()
        {
            txbLivresNumero.Text = "";
            txbLivresTitre.Text = "";
            txbLivresAuteur.Text = "";
            txbLivresCollection.Text = "";
            txbLivresIsbn.Text = "";
            txbLivresGenre.Text = "";
            txbLivresPublic.Text = "";
            txbLivresRayon.Text = "";
            txbLivresImage.Text = "";
            pcbLivresImage.Image = null;
        }

        private void ActiverEditionLivre(bool actif)
        {
            txbLivresNumero.ReadOnly = !actif;
            txbLivresTitre.ReadOnly = !actif;
            txbLivresAuteur.ReadOnly = !actif;
            txbLivresCollection.ReadOnly = !actif;
            txbLivresIsbn.ReadOnly = !actif;
            txbLivresGenre.ReadOnly = !actif;
            txbLivresPublic.ReadOnly = !actif;
            txbLivresRayon.ReadOnly = !actif;
            txbLivresImage.ReadOnly = !actif;
        }

        private void ActiverCommandeLivre(bool edition)
        {
            btnLivresAjouter.Enabled = !edition;
            btnLivresModifier.Enabled = !edition;
            btnLivresSupprimer.Enabled = !edition;

            btnLivresEnregistrer.Enabled = edition;
            btnLivresAnnuler.Enabled = edition;
        }

        private void InitialiserModeLivre()
        {
            livresModeAjout = false;
            livresModeModification = false;

            ActiverEditionLivre(false);
            ActiverCommandeLivre(false);
        }

        /// <summary>
        /// Ouverture de l'onglet Livres : 
        /// appel des méthodes pour remplir le datagrid des livres et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabLivres_Enter(object sender, EventArgs e)
        {
            lesLivres = controller.GetAllLivres();
            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxLivresGenres);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxLivresPublics);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxLivresRayons);
            RemplirLivresListeComplete();
            InitialiserModeLivre();
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="livres">liste de livres</param>
        private void RemplirLivresListe(List<Livre> livres)
        {
            bdgLivresListe.DataSource = livres;
            dgvLivresListe.DataSource = bdgLivresListe;
            dgvLivresListe.Columns["isbn"].Visible = false;
            dgvLivresListe.Columns["idRayon"].Visible = false;
            dgvLivresListe.Columns["idGenre"].Visible = false;
            dgvLivresListe.Columns["idPublic"].Visible = false;
            dgvLivresListe.Columns["image"].Visible = false;
            dgvLivresListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvLivresListe.Columns["id"].DisplayIndex = 0;
            dgvLivresListe.Columns["titre"].DisplayIndex = 1;
        }

        /// <summary>
        /// Recherche et affichage du livre dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbLivresNumRecherche.Text.Equals(""))
            {
                txbLivresTitreRecherche.Text = "";
                cbxLivresGenres.SelectedIndex = -1;
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
                Livre livre = lesLivres.Find(x => x.Id.Equals(txbLivresNumRecherche.Text));
                if (livre != null)
                {
                    List<Livre> livres = new List<Livre>() { livre };
                    RemplirLivresListe(livres);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirLivresListeComplete();
                }
            }
            else
            {
                RemplirLivresListeComplete();
            }
        }

        /// <summary>
        /// Recherche et affichage des livres dont le titre matche acec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxbLivresTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbLivresTitreRecherche.Text.Equals(""))
            {
                cbxLivresGenres.SelectedIndex = -1;
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
                txbLivresNumRecherche.Text = "";
                List<Livre> lesLivresParTitre;
                lesLivresParTitre = lesLivres.FindAll(x => x.Titre.ToLower().Contains(txbLivresTitreRecherche.Text.ToLower()));
                RemplirLivresListe(lesLivresParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxLivresGenres.SelectedIndex < 0 && cbxLivresPublics.SelectedIndex < 0 && cbxLivresRayons.SelectedIndex < 0
                    && txbLivresNumRecherche.Text.Equals(""))
                {
                    RemplirLivresListeComplete();
                }
            }
        }

        /// <summary>
        /// Affichage des informations du livre sélectionné
        /// </summary>
        /// <param name="livre">le livre</param>
        private void AfficheLivresInfos(Livre livre)
        {
            txbLivresAuteur.Text = livre.Auteur;
            txbLivresCollection.Text = livre.Collection;
            txbLivresImage.Text = livre.Image;
            txbLivresIsbn.Text = livre.Isbn;
            txbLivresNumero.Text = livre.Id;
            txbLivresGenre.Text = livre.Genre;
            txbLivresPublic.Text = livre.Public;
            txbLivresRayon.Text = livre.Rayon;
            txbLivresTitre.Text = livre.Titre;
            string image = livre.Image;
            try
            {
                pcbLivresImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbLivresImage.Image = null;
            }
        }

        /// <summary>
        /// Vide les zones d'affichage des informations du livre
        /// </summary>
        private void VideLivresInfos()
        {
            txbLivresAuteur.Text = "";
            txbLivresCollection.Text = "";
            txbLivresImage.Text = "";
            txbLivresIsbn.Text = "";
            txbLivresNumero.Text = "";
            txbLivresGenre.Text = "";
            txbLivresPublic.Text = "";
            txbLivresRayon.Text = "";
            txbLivresTitre.Text = "";
            pcbLivresImage.Image = null;
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxLivresGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLivresGenres.SelectedIndex >= 0)
            {
                txbLivresTitreRecherche.Text = "";
                txbLivresNumRecherche.Text = "";
                Genre genre = (Genre)cbxLivresGenres.SelectedItem;
                List<Livre> livres = lesLivres.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirLivresListe(livres);
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxLivresPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLivresPublics.SelectedIndex >= 0)
            {
                txbLivresTitreRecherche.Text = "";
                txbLivresNumRecherche.Text = "";
                Public lePublic = (Public)cbxLivresPublics.SelectedItem;
                List<Livre> livres = lesLivres.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirLivresListe(livres);
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresGenres.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxLivresRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLivresRayons.SelectedIndex >= 0)
            {
                txbLivresTitreRecherche.Text = "";
                txbLivresNumRecherche.Text = "";
                Rayon rayon = (Rayon)cbxLivresRayons.SelectedItem;
                List<Livre> livres = lesLivres.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirLivresListe(livres);
                cbxLivresGenres.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des informations du livre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgvLivresListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvLivresListe.CurrentCell != null)
            {
                try
                {
                    Livre livre = (Livre)bdgLivresListe.List[bdgLivresListe.Position];
                    AfficheLivresInfos(livre);
                }
                catch
                {
                    VideLivresZones();
                }
            }
            else
            {
                VideLivresInfos();
            }
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des livres
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirLivresListeComplete()
        {
            RemplirLivresListe(lesLivres);
            VideLivresZones();
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideLivresZones()
        {
            cbxLivresGenres.SelectedIndex = -1;
            cbxLivresRayons.SelectedIndex = -1;
            cbxLivresPublics.SelectedIndex = -1;
            txbLivresNumRecherche.Text = "";
            txbLivresTitreRecherche.Text = "";
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgvLivresListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideLivresZones();
            string titreColonne = dgvLivresListe.Columns[e.ColumnIndex].HeaderText;
            List<Livre> sortedList = new List<Livre>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesLivres.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesLivres.OrderBy(o => o.Titre).ToList();
                    break;
                case "Collection":
                    sortedList = lesLivres.OrderBy(o => o.Collection).ToList();
                    break;
                case "Auteur":
                    sortedList = lesLivres.OrderBy(o => o.Auteur).ToList();
                    break;
                case "Genre":
                    sortedList = lesLivres.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesLivres.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesLivres.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirLivresListe(sortedList);
        }

        private void btnLivresSupprimer_Click(object sender, EventArgs e)
        {
            Livre livre = RecupererLivreSelectionne();

            if (livre == null)
            {
                MessageBox.Show("Veuillez sélectionner un livre.");
                return;
            }

            if (!controller.DocumentSupprimable(livre))
            {
                MessageBox.Show("Suppression impossible : le livre possède des dépendances.");
                return;
            }

            DialogResult rep = MessageBox.Show(
                "Voulez-vous vraiment supprimer ce livre ?",
                "Confirmation",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (rep != DialogResult.Yes)
            {
                return;
            }

            if (controller.SupprimerLivre(livre.Id))
            {
                MessageBox.Show("Suppression effectuée.");
                RechargerLivres();
            }
            else
            {
                MessageBox.Show("Erreur lors de la suppression.");
            }
        }

        private void btnLivresAjouter_Click(object sender, EventArgs e)
        {
            livresModeAjout = true;
            livresModeModification = false;

            ViderChampsLivre();
            ActiverEditionLivre(true);

            txbLivresNumero.ReadOnly = false;
            txbLivresNumero.Focus();

            ActiverCommandeLivre(true);
        }

        private void btnLivresModifier_Click(object sender, EventArgs e)
        {
            Livre livre = RecupererLivreSelectionne();

            if (livre == null)
            {
                MessageBox.Show("Veuillez sélectionner un livre.");
                return;
            }

            livresModeAjout = false;
            livresModeModification = true;

            ActiverEditionLivre(true);

            // le numéro ne doit pas être modifiable
            txbLivresNumero.ReadOnly = true;
            txbLivresTitre.Focus();

            ActiverCommandeLivre(true);
        }

        private void btnLivresAnnuler_Click(object sender, EventArgs e)
        {
            InitialiserModeLivre();

            Livre livre = RecupererLivreSelectionne();
            if (livre != null)
            {
                AfficheLivresInfos(livre);
            }
            else
            {
                ViderChampsLivre();
            }
        }

        private void btnLivresEnregistrer_Click(object sender, EventArgs e)
        {
            try
            {
                string id = txbLivresNumero.Text.Trim();
                string titre = txbLivresTitre.Text.Trim();
                string image = txbLivresImage.Text.Trim();
                string isbn = txbLivresIsbn.Text.Trim();
                string auteur = txbLivresAuteur.Text.Trim();
                string collection = txbLivresCollection.Text.Trim();

                string idGenre = TrouverIdGenre(txbLivresGenre.Text.Trim());
                string idPublic = TrouverIdPublic(txbLivresPublic.Text.Trim());
                string idRayon = TrouverIdRayon(txbLivresRayon.Text.Trim());

                if (id == "" || titre == "" || isbn == "" || auteur == "" || collection == ""
                    || idGenre == null || idPublic == null || idRayon == null)
                {
                    MessageBox.Show("Tous les champs obligatoires doivent être renseignés.");
                    return;
                }

                Livre livre = new Livre(
                    id,
                    titre,
                    image,
                    isbn,
                    auteur,
                    collection,
                    idGenre,
                    txbLivresGenre.Text.Trim(),
                    idPublic,
                    txbLivresPublic.Text.Trim(),
                    idRayon,
                    txbLivresRayon.Text.Trim()
                );

                bool ok = false;

                if (livresModeAjout)
                {
                    ok = controller.CreerLivre(livre);
                }
                else if (livresModeModification)
                {
                    ok = controller.ModifierLivre(livre);
                }

                if (ok)
                {
                    MessageBox.Show("Enregistrement effectué.");
                    RechargerLivres();
                    InitialiserModeLivre();
                }
                else
                {
                    MessageBox.Show("Erreur lors de l'enregistrement.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur : " + ex.Message);
            }
        }
        #endregion

        #region Onglet Dvd
        private readonly BindingSource bdgDvdListe = new BindingSource();
        private List<Dvd> lesDvd = new List<Dvd>();
        private bool dvdModeAjout = false;
        private bool dvdModeModification = false;

        private void ViderChampsDvd()
        {
            txbDvdNumero.Text = "";
            txbDvdTitre.Text = "";
            txbDvdDuree.Text = "";
            txbDvdRealisateur.Text = "";
            txbDvdSynopsis.Text = "";
            txbDvdGenre.Text = "";
            txbDvdPublic.Text = "";
            txbDvdRayon.Text = "";
            txbDvdImage.Text = "";
            pcbDvdImage.Image = null;
        }

        private void ActiverEditionDvd(bool actif)
        {
            txbDvdNumero.ReadOnly = !actif;
            txbDvdTitre.ReadOnly = !actif;
            txbDvdDuree.ReadOnly = !actif;
            txbDvdRealisateur.ReadOnly = !actif;
            txbDvdSynopsis.ReadOnly = !actif;
            txbDvdGenre.ReadOnly = !actif;
            txbDvdPublic.ReadOnly = !actif;
            txbDvdRayon.ReadOnly = !actif;
            txbDvdImage.ReadOnly = !actif;
        }

        private void ActiverCommandeDvd(bool edition)
        {
            btnDvdAjouter.Enabled = !edition;
            btnDvdModifier.Enabled = !edition;
            btnDvdSupprimer.Enabled = !edition;

            btnDvdEnregistrer.Enabled = edition;
            btnDvdAnnuler.Enabled = edition;
        }

        private void InitialiserModeDvd()
        {
            dvdModeAjout = false;
            dvdModeModification = false;

            ActiverEditionDvd(false);
            ActiverCommandeDvd(false);
        }

        /// <summary>
        /// Ouverture de l'onglet Dvds : 
        /// appel des méthodes pour remplir le datagrid des dvd et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabDvd_Enter(object sender, EventArgs e)
        {
            lesDvd = controller.GetAllDvd();
            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxDvdGenres);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxDvdPublics);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxDvdRayons);
            RemplirDvdListeComplete();
            InitialiserModeDvd();
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="Dvds">liste de dvd</param>
        private void RemplirDvdListe(List<Dvd> Dvds)
        {
            bdgDvdListe.DataSource = Dvds;
            dgvDvdListe.DataSource = bdgDvdListe;
            dgvDvdListe.Columns["idRayon"].Visible = false;
            dgvDvdListe.Columns["idGenre"].Visible = false;
            dgvDvdListe.Columns["idPublic"].Visible = false;
            dgvDvdListe.Columns["image"].Visible = false;
            dgvDvdListe.Columns["synopsis"].Visible = false;
            dgvDvdListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvDvdListe.Columns["id"].DisplayIndex = 0;
            dgvDvdListe.Columns["titre"].DisplayIndex = 1;
        }

        /// <summary>
        /// Recherche et affichage du Dvd dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbDvdNumRecherche.Text.Equals(""))
            {
                txbDvdTitreRecherche.Text = "";
                cbxDvdGenres.SelectedIndex = -1;
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
                Dvd dvd = lesDvd.Find(x => x.Id.Equals(txbDvdNumRecherche.Text));
                if (dvd != null)
                {
                    List<Dvd> Dvd = new List<Dvd>() { dvd };
                    RemplirDvdListe(Dvd);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirDvdListeComplete();
                }
            }
            else
            {
                RemplirDvdListeComplete();
            }
        }

        /// <summary>
        /// Recherche et affichage des Dvd dont le titre matche acec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbDvdTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbDvdTitreRecherche.Text.Equals(""))
            {
                cbxDvdGenres.SelectedIndex = -1;
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
                txbDvdNumRecherche.Text = "";
                List<Dvd> lesDvdParTitre;
                lesDvdParTitre = lesDvd.FindAll(x => x.Titre.ToLower().Contains(txbDvdTitreRecherche.Text.ToLower()));
                RemplirDvdListe(lesDvdParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxDvdGenres.SelectedIndex < 0 && cbxDvdPublics.SelectedIndex < 0 && cbxDvdRayons.SelectedIndex < 0
                    && txbDvdNumRecherche.Text.Equals(""))
                {
                    RemplirDvdListeComplete();
                }
            }
        }

        /// <summary>
        /// Affichage des informations du dvd sélectionné
        /// </summary>
        /// <param name="dvd">le dvd</param>
        private void AfficheDvdInfos(Dvd dvd)
        {
            txbDvdRealisateur.Text = dvd.Realisateur;
            txbDvdSynopsis.Text = dvd.Synopsis;
            txbDvdImage.Text = dvd.Image;
            txbDvdDuree.Text = dvd.Duree.ToString();
            txbDvdNumero.Text = dvd.Id;
            txbDvdGenre.Text = dvd.Genre;
            txbDvdPublic.Text = dvd.Public;
            txbDvdRayon.Text = dvd.Rayon;
            txbDvdTitre.Text = dvd.Titre;
            string image = dvd.Image;
            try
            {
                pcbDvdImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbDvdImage.Image = null;
            }
        }

        /// <summary>
        /// Vide les zones d'affichage des informations du dvd
        /// </summary>
        private void VideDvdInfos()
        {
            txbDvdRealisateur.Text = "";
            txbDvdSynopsis.Text = "";
            txbDvdImage.Text = "";
            txbDvdDuree.Text = "";
            txbDvdNumero.Text = "";
            txbDvdGenre.Text = "";
            txbDvdPublic.Text = "";
            txbDvdRayon.Text = "";
            txbDvdTitre.Text = "";
            pcbDvdImage.Image = null;
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxDvdGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxDvdGenres.SelectedIndex >= 0)
            {
                txbDvdTitreRecherche.Text = "";
                txbDvdNumRecherche.Text = "";
                Genre genre = (Genre)cbxDvdGenres.SelectedItem;
                List<Dvd> Dvd = lesDvd.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirDvdListe(Dvd);
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxDvdPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxDvdPublics.SelectedIndex >= 0)
            {
                txbDvdTitreRecherche.Text = "";
                txbDvdNumRecherche.Text = "";
                Public lePublic = (Public)cbxDvdPublics.SelectedItem;
                List<Dvd> Dvd = lesDvd.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirDvdListe(Dvd);
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdGenres.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxDvdRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxDvdRayons.SelectedIndex >= 0)
            {
                txbDvdTitreRecherche.Text = "";
                txbDvdNumRecherche.Text = "";
                Rayon rayon = (Rayon)cbxDvdRayons.SelectedItem;
                List<Dvd> Dvd = lesDvd.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirDvdListe(Dvd);
                cbxDvdGenres.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des informations du dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvDvdListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvDvdListe.CurrentCell != null)
            {
                try
                {
                    Dvd dvd = (Dvd)bdgDvdListe.List[bdgDvdListe.Position];
                    AfficheDvdInfos(dvd);
                }
                catch
                {
                    VideDvdZones();
                }
            }
            else
            {
                VideDvdInfos();
            }
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des Dvd
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirDvdListeComplete()
        {
            RemplirDvdListe(lesDvd);
            VideDvdZones();
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideDvdZones()
        {
            cbxDvdGenres.SelectedIndex = -1;
            cbxDvdRayons.SelectedIndex = -1;
            cbxDvdPublics.SelectedIndex = -1;
            txbDvdNumRecherche.Text = "";
            txbDvdTitreRecherche.Text = "";
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvDvdListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideDvdZones();
            string titreColonne = dgvDvdListe.Columns[e.ColumnIndex].HeaderText;
            List<Dvd> sortedList = new List<Dvd>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesDvd.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesDvd.OrderBy(o => o.Titre).ToList();
                    break;
                case "Duree":
                    sortedList = lesDvd.OrderBy(o => o.Duree).ToList();
                    break;
                case "Realisateur":
                    sortedList = lesDvd.OrderBy(o => o.Realisateur).ToList();
                    break;
                case "Genre":
                    sortedList = lesDvd.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesDvd.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesDvd.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirDvdListe(sortedList);
        }

        private void btnDvdSupprimer_Click(object sender, EventArgs e)
        {
            Dvd dvd = RecupererDvdSelectionne();

            if (dvd == null)
            {
                MessageBox.Show("Veuillez sélectionner un dvd.");
                return;
            }

            if (!controller.DocumentSupprimable(dvd))
            {
                MessageBox.Show("Suppression impossible : le dvd possède des dépendances.");
                return;
            }

            DialogResult rep = MessageBox.Show(
                "Voulez-vous vraiment supprimer ce dvd ?",
                "Confirmation",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (rep != DialogResult.Yes)
            {
                return;
            }

            if (controller.SupprimerDvd(dvd.Id))
            {
                MessageBox.Show("Suppression effectuée.");
                RechargerDvd();
            }
            else
            {
                MessageBox.Show("Erreur lors de la suppression.");
            }
        }

        private void btnDvdAjouter_Click(object sender, EventArgs e)
        {
            dvdModeAjout = true;
            dvdModeModification = false;

            ViderChampsDvd();
            ActiverEditionDvd(true);

            txbDvdNumero.ReadOnly = false;
            txbDvdNumero.Focus();

            ActiverCommandeDvd(true);
        }

        private void btnDvdModifier_Click(object sender, EventArgs e)
        {
            Dvd dvd = RecupererDvdSelectionne();

            if (dvd == null)
            {
                MessageBox.Show("Veuillez sélectionner un dvd.");
                return;
            }

            dvdModeAjout = false;
            dvdModeModification = true;

            ActiverEditionDvd(true);
            txbDvdNumero.ReadOnly = true;
            txbDvdTitre.Focus();

            ActiverCommandeDvd(true);
        }

        private void btnDvdAnnuler_Click(object sender, EventArgs e)
        {
            InitialiserModeDvd();

            Dvd dvd = RecupererDvdSelectionne();
            if (dvd != null)
            {
                AfficheDvdInfos(dvd);
            }
            else
            {
                ViderChampsDvd();
            }
        }

        private void btnDvdEnregistrer_Click(object sender, EventArgs e)
        {
            try
            {
                string id = txbDvdNumero.Text.Trim();
                string titre = txbDvdTitre.Text.Trim();
                string image = txbDvdImage.Text.Trim();
                string synopsis = txbDvdSynopsis.Text.Trim();
                string realisateur = txbDvdRealisateur.Text.Trim();

                string idGenre = TrouverIdGenre(txbDvdGenre.Text.Trim());
                string idPublic = TrouverIdPublic(txbDvdPublic.Text.Trim());
                string idRayon = TrouverIdRayon(txbDvdRayon.Text.Trim());

                if (id == "" || titre == "" || synopsis == "" || realisateur == ""
                    || idGenre == null || idPublic == null || idRayon == null)
                {
                    MessageBox.Show("Tous les champs obligatoires doivent être renseignés.");
                    return;
                }

                int duree;
                if (!int.TryParse(txbDvdDuree.Text.Trim(), out duree))
                {
                    MessageBox.Show("La durée doit être numérique.");
                    return;
                }

                Dvd dvd = new Dvd(
                    id,
                    titre,
                    image,
                    duree,
                    realisateur,
                    synopsis,
                    idGenre,
                    txbDvdGenre.Text.Trim(),
                    idPublic,
                    txbDvdPublic.Text.Trim(),
                    idRayon,
                    txbDvdRayon.Text.Trim()
                );

                bool ok = false;

                if (dvdModeAjout)
                {
                    ok = controller.CreerDvd(dvd);
                }
                else if (dvdModeModification)
                {
                    ok = controller.ModifierDvd(dvd);
                }

                if (ok)
                {
                    MessageBox.Show("Enregistrement effectué.");
                    RechargerDvd();
                    InitialiserModeDvd();
                }
                else
                {
                    MessageBox.Show("Erreur lors de l'enregistrement.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur : " + ex.Message);
            }
        }
        #endregion

        #region Onglet Revues
        private readonly BindingSource bdgRevuesListe = new BindingSource();
        private List<Revue> lesRevues = new List<Revue>();
        private bool revuesModeAjout = false;
        private bool revuesModeModification = false;

        private void ViderChampsRevue()
        {
            txbRevuesNumero.Text = "";
            txbRevuesTitre.Text = "";
            txbRevuesPeriodicite.Text = "";
            txbRevuesDateMiseADispo.Text = "";
            txbRevuesGenre.Text = "";
            txbRevuesPublic.Text = "";
            txbRevuesRayon.Text = "";
            txbRevuesImage.Text = "";
            pcbRevuesImage.Image = null;
        }

        private void ActiverEditionRevue(bool actif)
        {
            txbRevuesNumero.ReadOnly = !actif;
            txbRevuesTitre.ReadOnly = !actif;
            txbRevuesPeriodicite.ReadOnly = !actif;
            txbRevuesDateMiseADispo.ReadOnly = !actif;
            txbRevuesGenre.ReadOnly = !actif;
            txbRevuesPublic.ReadOnly = !actif;
            txbRevuesRayon.ReadOnly = !actif;
            txbRevuesImage.ReadOnly = !actif;
        }

        private void ActiverCommandeRevue(bool edition)
        {
            btnRevuesAjouter.Enabled = !edition;
            btnRevuesModifier.Enabled = !edition;
            btnRevuesSupprimer.Enabled = !edition;

            btnRevuesEnregistrer.Enabled = edition;
            btnRevuesAnnuler.Enabled = edition;
        }

        private void InitialiserModeRevue()
        {
            revuesModeAjout = false;
            revuesModeModification = false;

            ActiverEditionRevue(false);
            ActiverCommandeRevue(false);
        }

        /// <summary>
        /// Ouverture de l'onglet Revues : 
        /// appel des méthodes pour remplir le datagrid des revues et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabRevues_Enter(object sender, EventArgs e)
        {
            lesRevues = controller.GetAllRevues();
            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxRevuesGenres);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxRevuesPublics);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxRevuesRayons);
            RemplirRevuesListeComplete();
            InitialiserModeRevue();
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="revues"></param>
        private void RemplirRevuesListe(List<Revue> revues)
        {
            bdgRevuesListe.DataSource = revues;
            dgvRevuesListe.DataSource = bdgRevuesListe;
            dgvRevuesListe.Columns["idRayon"].Visible = false;
            dgvRevuesListe.Columns["idGenre"].Visible = false;
            dgvRevuesListe.Columns["idPublic"].Visible = false;
            dgvRevuesListe.Columns["image"].Visible = false;
            dgvRevuesListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvRevuesListe.Columns["id"].DisplayIndex = 0;
            dgvRevuesListe.Columns["titre"].DisplayIndex = 1;
        }

        /// <summary>
        /// Recherche et affichage de la revue dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbRevuesNumRecherche.Text.Equals(""))
            {
                txbRevuesTitreRecherche.Text = "";
                cbxRevuesGenres.SelectedIndex = -1;
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
                Revue revue = lesRevues.Find(x => x.Id.Equals(txbRevuesNumRecherche.Text));
                if (revue != null)
                {
                    List<Revue> revues = new List<Revue>() { revue };
                    RemplirRevuesListe(revues);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirRevuesListeComplete();
                }
            }
            else
            {
                RemplirRevuesListeComplete();
            }
        }

        /// <summary>
        /// Recherche et affichage des revues dont le titre matche acec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbRevuesTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbRevuesTitreRecherche.Text.Equals(""))
            {
                cbxRevuesGenres.SelectedIndex = -1;
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
                txbRevuesNumRecherche.Text = "";
                List<Revue> lesRevuesParTitre;
                lesRevuesParTitre = lesRevues.FindAll(x => x.Titre.ToLower().Contains(txbRevuesTitreRecherche.Text.ToLower()));
                RemplirRevuesListe(lesRevuesParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxRevuesGenres.SelectedIndex < 0 && cbxRevuesPublics.SelectedIndex < 0 && cbxRevuesRayons.SelectedIndex < 0
                    && txbRevuesNumRecherche.Text.Equals(""))
                {
                    RemplirRevuesListeComplete();
                }
            }
        }

        /// <summary>
        /// Affichage des informations de la revue sélectionné
        /// </summary>
        /// <param name="revue">la revue</param>
        private void AfficheRevuesInfos(Revue revue)
        {
            txbRevuesPeriodicite.Text = revue.Periodicite;
            txbRevuesImage.Text = revue.Image;
            txbRevuesDateMiseADispo.Text = revue.DelaiMiseADispo.ToString();
            txbRevuesNumero.Text = revue.Id;
            txbRevuesGenre.Text = revue.Genre;
            txbRevuesPublic.Text = revue.Public;
            txbRevuesRayon.Text = revue.Rayon;
            txbRevuesTitre.Text = revue.Titre;
            string image = revue.Image;
            try
            {
                pcbRevuesImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbRevuesImage.Image = null;
            }
        }

        /// <summary>
        /// Vide les zones d'affichage des informations de la reuve
        /// </summary>
        private void VideRevuesInfos()
        {
            txbRevuesPeriodicite.Text = "";
            txbRevuesImage.Text = "";
            txbRevuesDateMiseADispo.Text = "";
            txbRevuesNumero.Text = "";
            txbRevuesGenre.Text = "";
            txbRevuesPublic.Text = "";
            txbRevuesRayon.Text = "";
            txbRevuesTitre.Text = "";
            pcbRevuesImage.Image = null;
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxRevuesGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxRevuesGenres.SelectedIndex >= 0)
            {
                txbRevuesTitreRecherche.Text = "";
                txbRevuesNumRecherche.Text = "";
                Genre genre = (Genre)cbxRevuesGenres.SelectedItem;
                List<Revue> revues = lesRevues.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirRevuesListe(revues);
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxRevuesPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxRevuesPublics.SelectedIndex >= 0)
            {
                txbRevuesTitreRecherche.Text = "";
                txbRevuesNumRecherche.Text = "";
                Public lePublic = (Public)cbxRevuesPublics.SelectedItem;
                List<Revue> revues = lesRevues.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirRevuesListe(revues);
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesGenres.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxRevuesRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxRevuesRayons.SelectedIndex >= 0)
            {
                txbRevuesTitreRecherche.Text = "";
                txbRevuesNumRecherche.Text = "";
                Rayon rayon = (Rayon)cbxRevuesRayons.SelectedItem;
                List<Revue> revues = lesRevues.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirRevuesListe(revues);
                cbxRevuesGenres.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des informations de la revue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvRevuesListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvRevuesListe.CurrentCell != null)
            {
                try
                {
                    Revue revue = (Revue)bdgRevuesListe.List[bdgRevuesListe.Position];
                    AfficheRevuesInfos(revue);
                }
                catch
                {
                    VideRevuesZones();
                }
            }
            else
            {
                VideRevuesInfos();
            }
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des revues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des revues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des revues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des revues
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirRevuesListeComplete()
        {
            RemplirRevuesListe(lesRevues);
            VideRevuesZones();
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideRevuesZones()
        {
            cbxRevuesGenres.SelectedIndex = -1;
            cbxRevuesRayons.SelectedIndex = -1;
            cbxRevuesPublics.SelectedIndex = -1;
            txbRevuesNumRecherche.Text = "";
            txbRevuesTitreRecherche.Text = "";
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvRevuesListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideRevuesZones();
            string titreColonne = dgvRevuesListe.Columns[e.ColumnIndex].HeaderText;
            List<Revue> sortedList = new List<Revue>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesRevues.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesRevues.OrderBy(o => o.Titre).ToList();
                    break;
                case "Periodicite":
                    sortedList = lesRevues.OrderBy(o => o.Periodicite).ToList();
                    break;
                case "DelaiMiseADispo":
                    sortedList = lesRevues.OrderBy(o => o.DelaiMiseADispo).ToList();
                    break;
                case "Genre":
                    sortedList = lesRevues.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesRevues.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesRevues.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirRevuesListe(sortedList);
        }

        private void btnRevuesSupprimer_Click(object sender, EventArgs e)
        {
            Revue revue = RecupererRevueSelectionnee();

            if (revue == null)
            {
                MessageBox.Show("Veuillez sélectionner une revue.");
                return;
            }

            if (!controller.DocumentSupprimable(revue))
            {
                MessageBox.Show("Suppression impossible : la revue possède des dépendances.");
                return;
            }

            DialogResult rep = MessageBox.Show(
                "Voulez-vous vraiment supprimer cette revue ?",
                "Confirmation",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (rep != DialogResult.Yes)
            {
                return;
            }

            if (controller.SupprimerRevue(revue.Id))
            {
                MessageBox.Show("Suppression effectuée.");
                RechargerRevues();
            }
            else
            {
                MessageBox.Show("Erreur lors de la suppression.");
            }
        }
        private void btnRevuesAjouter_Click(object sender, EventArgs e)
        {
            revuesModeAjout = true;
            revuesModeModification = false;

            ViderChampsRevue();
            ActiverEditionRevue(true);

            txbRevuesNumero.ReadOnly = false;
            txbRevuesNumero.Focus();

            ActiverCommandeRevue(true);
        }

        private void btnRevuesModifier_Click(object sender, EventArgs e)
        {
            Revue revue = RecupererRevueSelectionnee();

            if (revue == null)
            {
                MessageBox.Show("Veuillez sélectionner une revue.");
                return;
            }

            revuesModeAjout = false;
            revuesModeModification = true;

            ActiverEditionRevue(true);
            txbRevuesNumero.ReadOnly = true;
            txbRevuesTitre.Focus();

            ActiverCommandeRevue(true);
        }

        private void btnRevuesAnnuler_Click(object sender, EventArgs e)
        {
            InitialiserModeRevue();

            Revue revue = RecupererRevueSelectionnee();
            if (revue != null)
            {
                AfficheRevuesInfos(revue);
            }
            else
            {
                ViderChampsRevue();
            }
        }

        private void btnRevuesEnregistrer_Click(object sender, EventArgs e)
        {
            try
            {
                string id = txbRevuesNumero.Text.Trim();
                string titre = txbRevuesTitre.Text.Trim();
                string image = txbRevuesImage.Text.Trim();
                string periodicite = txbRevuesPeriodicite.Text.Trim();

                string idGenre = TrouverIdGenre(txbRevuesGenre.Text.Trim());
                string idPublic = TrouverIdPublic(txbRevuesPublic.Text.Trim());
                string idRayon = TrouverIdRayon(txbRevuesRayon.Text.Trim());

                if (id == "" || titre == "" || periodicite == ""
                    || idGenre == null || idPublic == null || idRayon == null)
                {
                    MessageBox.Show("Tous les champs obligatoires doivent être renseignés.");
                    return;
                }

                int delaiMiseADispo;
                if (!int.TryParse(txbRevuesDateMiseADispo.Text.Trim(), out delaiMiseADispo))
                {
                    MessageBox.Show("Le délai de mise à disposition doit être numérique.");
                    return;
                }

                Revue revue = new Revue(
                    id,
                    titre,
                    image,
                    idGenre,
                    txbRevuesGenre.Text.Trim(),
                    idPublic,
                    txbRevuesPublic.Text.Trim(),
                    idRayon,
                    txbRevuesRayon.Text.Trim(),
                    periodicite,
                    delaiMiseADispo
                );

                bool ok = false;

                if (revuesModeAjout)
                {
                    ok = controller.CreerRevue(revue);
                }
                else if (revuesModeModification)
                {
                    ok = controller.ModifierRevue(revue);
                }

                if (ok)
                {
                    MessageBox.Show("Enregistrement effectué.");
                    RechargerRevues();
                    InitialiserModeRevue();
                }
                else
                {
                    MessageBox.Show("Erreur lors de l'enregistrement.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur : " + ex.Message);
            }
        }
        #endregion

        #region Onglet Paarutions
        private readonly BindingSource bdgExemplairesListe = new BindingSource();
        private List<Exemplaire> lesExemplaires = new List<Exemplaire>();

        private string idDocumentExemplaireSelectionne = "";
        const string ETATNEUF = "00001";

        /// <summary>
        /// Ouverture de l'onglet : récupère les documents et vide tous les champs.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabReceptionRevue_Enter(object sender, EventArgs e)
        {
            lesRevues = controller.GetAllRevues();

            idDocumentExemplaireSelectionne = "";
            txbReceptionRevueNumero.Text = "";
            txbReceptionRevueTitre.Text = "";
            txbReceptionRevueGenre.Text = "";
            txbReceptionRevuePublic.Text = "";
            txbReceptionRevueRayon.Text = "";
            txbReceptionRevueImage.Text = "";
            txbReceptionRevuePeriodicite.Text = "";
            txbReceptionRevueDelaiMiseADispo.Text = "";

            pcbReceptionRevueImage.Image = null;
            pcbReceptionExemplaireRevueImage.Image = null;

            dgvReceptionExemplairesListe.DataSource = null;
            cbxReceptionExemplaireEtat.SelectedIndex = -1;

            cbxTypeDocumentExemplaire.SelectedItem = "Revue";

            AccesReceptionExemplaireGroupBox(false);
        }

        /// <summary>
        /// Remplit le dategrid des exemplaires avec la liste reçue en paramètre
        /// </summary>
        /// <param name="exemplaires">liste d'exemplaires</param>
        private void RemplirReceptionExemplairesListe(List<Exemplaire> exemplaires)
        {
            if (exemplaires != null)
            {
                bdgExemplairesListe.DataSource = exemplaires;
                dgvReceptionExemplairesListe.DataSource = bdgExemplairesListe;

                if (dgvReceptionExemplairesListe.Columns.Contains("IdEtat"))
                    dgvReceptionExemplairesListe.Columns["IdEtat"].Visible = false;

                if (dgvReceptionExemplairesListe.Columns.Contains("Id"))
                    dgvReceptionExemplairesListe.Columns["Id"].Visible = false;

                if (dgvReceptionExemplairesListe.Columns.Contains("Etat"))
                    dgvReceptionExemplairesListe.Columns["Etat"].HeaderText = "Etat";

                dgvReceptionExemplairesListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

                if (dgvReceptionExemplairesListe.Columns.Contains("Numero"))
                    dgvReceptionExemplairesListe.Columns["Numero"].DisplayIndex = 0;

                if (dgvReceptionExemplairesListe.Columns.Contains("DateAchat"))
                    dgvReceptionExemplairesListe.Columns["DateAchat"].DisplayIndex = 1;

                if (dgvReceptionExemplairesListe.Columns.Contains("Etat"))
                    dgvReceptionExemplairesListe.Columns["Etat"].DisplayIndex = 2;

                if (dgvReceptionExemplairesListe.Columns.Contains("Photo"))
                    dgvReceptionExemplairesListe.Columns["Photo"].DisplayIndex = 3;
            }
            else
            {
                bdgExemplairesListe.DataSource = null;
                dgvReceptionExemplairesListe.DataSource = null;
            }
        }

        /// <summary>
        /// Recherche d'un numéro de revue OU d'un document de type livre/dvd et affiche ses informations
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionRechercher_Click(object sender, EventArgs e)
        {
            string recherche = txbReceptionRevueNumero.Text.Trim().ToLower();

            if (cbxTypeDocumentExemplaire.Text == "")
            {
                MessageBox.Show("Sélectionnez un type de document.");
                return;
            }

            if (cbxTypeDocumentExemplaire.Text == "Revue")
            {
                Revue revue = controller.GetAllRevues()
                    .Find(r => r.Id.ToLower().Equals(recherche));

                if (revue != null)
                {
                    AfficherInfosDocumentExemplaire(
                        revue.Id,
                        revue.Titre,
                        revue.Genre,
                        revue.Public,
                        revue.Rayon,
                        revue.Image,
                        revue.Periodicite,
                        revue.DelaiMiseADispo.ToString()
                    );

                    idDocumentExemplaireSelectionne = revue.Id;
                    AfficheReceptionExemplairesDocument();
                }
                else
                {
                    MessageBox.Show("Document introuvable.");
                }
            }
            else if (cbxTypeDocumentExemplaire.Text == "Livre")
            {
                Livre livre = controller.GetAllLivres()
                    .Find(l => l.Id.ToLower().Equals(recherche));

                if (livre != null)
                {
                    AfficherInfosDocumentExemplaire(
                        livre.Id,
                        livre.Titre,
                        livre.Genre,
                        livre.Public,
                        livre.Rayon,
                        livre.Image,
                        "",
                        ""
                    );

                    idDocumentExemplaireSelectionne = livre.Id;
                    AfficheReceptionExemplairesDocument();
                }
                else
                {
                    MessageBox.Show("Document introuvable.");
                }
            }
            else if (cbxTypeDocumentExemplaire.Text == "DVD")
            {
                Dvd dvd = controller.GetAllDvd()
                    .Find(d => d.Id.ToLower().Equals(recherche));

                if (dvd != null)
                {
                    AfficherInfosDocumentExemplaire(
                        dvd.Id,
                        dvd.Titre,
                        dvd.Genre,
                        dvd.Public,
                        dvd.Rayon,
                        dvd.Image,
                        "",
                        ""
                    );

                    idDocumentExemplaireSelectionne = dvd.Id;
                    AfficheReceptionExemplairesDocument();
                }
                else
                {
                    MessageBox.Show("Document introuvable.");
                }
            }
        }

        private void AfficherInfosDocumentExemplaire(
            string id,
            string titre,
            string genre,
            string lePublic,
            string rayon,
            string image,
            string periodicite,
            string delaiMiseADispo)
        {
            txbReceptionRevueNumero.Text = id;
            txbReceptionRevueTitre.Text = titre;
            txbReceptionRevueGenre.Text = genre;
            txbReceptionRevuePublic.Text = lePublic;
            txbReceptionRevueRayon.Text = rayon;
            txbReceptionRevueImage.Text = image;
            txbReceptionRevuePeriodicite.Text = periodicite;
            txbReceptionRevueDelaiMiseADispo.Text = delaiMiseADispo;

            try
            {
                pcbReceptionRevueImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbReceptionRevueImage.Image = null;
            }
        }

        private void cbxTypeDocumentExemplaire_SelectedIndexChanged(object sender, EventArgs e)
        {
            idDocumentExemplaireSelectionne = "";
            txbReceptionRevueNumero.Text = "";
            txbReceptionRevueTitre.Text = "";
            txbReceptionRevueGenre.Text = "";
            txbReceptionRevuePublic.Text = "";
            txbReceptionRevueRayon.Text = "";
            txbReceptionRevueImage.Text = "";
            txbReceptionRevuePeriodicite.Text = "";
            txbReceptionRevueDelaiMiseADispo.Text = "";
            pcbReceptionRevueImage.Image = null;

            dgvReceptionExemplairesListe.DataSource = null;
            AccesReceptionExemplaireGroupBox(false);
        }

        /// <summary>
        /// Si le numéro de revue est modifié, la zone de l'exemplaire est vidée et inactive
        /// les informations de la revue son aussi effacées
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbReceptionRevueNumero_TextChanged(object sender, EventArgs e)
        {
            txbReceptionRevuePeriodicite.Text = "";
            txbReceptionRevueImage.Text = "";
            txbReceptionRevueDelaiMiseADispo.Text = "";
            txbReceptionRevueGenre.Text = "";
            txbReceptionRevuePublic.Text = "";
            txbReceptionRevueRayon.Text = "";
            txbReceptionRevueTitre.Text = "";
            pcbReceptionRevueImage.Image = null;
            RemplirReceptionExemplairesListe(null);
            AccesReceptionExemplaireGroupBox(false);
        }

        /// <summary>
        /// Affichage des informations de la revue sélectionnée et les exemplaires
        /// </summary>
        /// <param name="revue">la revue</param>
        private void AfficheReceptionRevueInfos(Revue revue)
        {
            // informations sur la revue
            txbReceptionRevuePeriodicite.Text = revue.Periodicite;
            txbReceptionRevueImage.Text = revue.Image;
            txbReceptionRevueDelaiMiseADispo.Text = revue.DelaiMiseADispo.ToString();
            txbReceptionRevueNumero.Text = revue.Id;
            txbReceptionRevueGenre.Text = revue.Genre;
            txbReceptionRevuePublic.Text = revue.Public;
            txbReceptionRevueRayon.Text = revue.Rayon;
            txbReceptionRevueTitre.Text = revue.Titre;
            string image = revue.Image;
            try
            {
                pcbReceptionRevueImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbReceptionRevueImage.Image = null;
            }
            // affiche la liste des exemplaires de la revue
            AfficheReceptionExemplairesDocument();
        }

        /// <summary>
        /// Récupère et affiche les exemplaires d'un document
        /// </summary>
        private void AfficheReceptionExemplairesDocument()
        {
            string idDocument = idDocumentExemplaireSelectionne.Trim();

            if (idDocument == "")
            {
                lesExemplaires = new List<Exemplaire>();
                RemplirReceptionExemplairesListe(lesExemplaires);
                AccesReceptionExemplaireGroupBox(false);
                return;
            }

            lesExemplaires = controller.GetExemplairesRevue(idDocument);
            RemplirReceptionExemplairesListe(lesExemplaires);
            AccesReceptionExemplaireGroupBox(true);
        }

        /// <summary>
        /// Permet ou interdit l'accès à la gestion de la réception d'un exemplaire
        /// et vide les objets graphiques
        /// </summary>
        /// <param name="acces">true ou false</param>
        private void AccesReceptionExemplaireGroupBox(bool acces)
        {
            grpReceptionExemplaire.Enabled = acces;
            txbReceptionExemplaireImage.Text = "";
            txbReceptionExemplaireNumero.Text = "";
            pcbReceptionExemplaireImage.Image = null;
            dtpReceptionExemplaireDate.Value = DateTime.Now;
        }

        /// <summary>
        /// Recherche image sur disque (pour l'exemplaire à insérer)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionExemplaireImage_Click(object sender, EventArgs e)
        {
            string filePath = "";
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                // positionnement à la racine du disque où se trouve le dossier actuel
                InitialDirectory = Path.GetPathRoot(Environment.CurrentDirectory),
                Filter = "Files|*.jpg;*.bmp;*.jpeg;*.png;*.gif"
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                filePath = openFileDialog.FileName;
            }
            txbReceptionExemplaireImage.Text = filePath;
            try
            {
                pcbReceptionExemplaireImage.Image = Image.FromFile(filePath);
            }
            catch
            {
                pcbReceptionExemplaireImage.Image = null;
            }
        }

        /// <summary>
        /// Enregistrement du nouvel exemplaire
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionExemplaireValider_Click(object sender, EventArgs e)
        {
            if (!txbReceptionExemplaireNumero.Text.Equals(""))
            {
                try
                {
                    int numero = int.Parse(txbReceptionExemplaireNumero.Text);
                    DateTime dateAchat = dtpReceptionExemplaireDate.Value;
                    string photo = txbReceptionExemplaireImage.Text;
                    string idEtat = ETATNEUF;
                    string idDocument = txbReceptionRevueNumero.Text;
                    Exemplaire exemplaire = new Exemplaire(numero, dateAchat, photo, idEtat, idDocument);
                    if (controller.CreerExemplaire(exemplaire))
                    {
                        AfficheReceptionExemplairesDocument();
                    }
                    else
                    {
                        MessageBox.Show("numéro de publication déjà existant", "Erreur");
                    }
                }
                catch
                {
                    MessageBox.Show("le numéro de parution doit être numérique", "Information");
                    txbReceptionExemplaireNumero.Text = "";
                    txbReceptionExemplaireNumero.Focus();
                }
            }
            else
            {
                MessageBox.Show("numéro de parution obligatoire", "Information");
            }
        }

        /// <summary>
        /// Tri sur une colonne
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvExemplairesListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string titreColonne = dgvReceptionExemplairesListe.Columns[e.ColumnIndex].HeaderText;
            List<Exemplaire> sortedList = new List<Exemplaire>();
            switch (titreColonne)
            {
                case "Numero":
                    sortedList = lesExemplaires.OrderBy(o => o.Numero).Reverse().ToList();
                    break;
                case "DateAchat":
                    sortedList = lesExemplaires.OrderBy(o => o.DateAchat).Reverse().ToList();
                    break;
                case "Photo":
                    sortedList = lesExemplaires.OrderBy(o => o.Photo).ToList();
                    break;
            }
            RemplirReceptionExemplairesListe(sortedList);
        }

        /// <summary>
        /// affichage de l'image de l'exemplaire suite à la sélection d'un exemplaire dans la liste
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvReceptionExemplairesListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvReceptionExemplairesListe.CurrentCell != null && bdgExemplairesListe.Position >= 0)
            {
                try
                {
                    Exemplaire exemplaire = (Exemplaire)bdgExemplairesListe.List[bdgExemplairesListe.Position];

                    try
                    {
                        pcbReceptionExemplaireRevueImage.Image = Image.FromFile(exemplaire.Photo);
                    }
                    catch
                    {
                        pcbReceptionExemplaireRevueImage.Image = null;
                    }

                    switch (exemplaire.IdEtat)
                    {
                        case "00001":
                            cbxReceptionExemplaireEtat.SelectedIndex = 0;
                            break;
                        case "00002":
                            cbxReceptionExemplaireEtat.SelectedIndex = 1;
                            break;
                        case "00003":
                            cbxReceptionExemplaireEtat.SelectedIndex = 2;
                            break;
                        case "00004":
                            cbxReceptionExemplaireEtat.SelectedIndex = 3;
                            break;
                        default:
                            cbxReceptionExemplaireEtat.SelectedIndex = -1;
                            break;
                    }
                }
                catch
                {
                    pcbReceptionExemplaireRevueImage.Image = null;
                    cbxReceptionExemplaireEtat.SelectedIndex = -1;
                }
            }
            else
            {
                pcbReceptionExemplaireRevueImage.Image = null;
                cbxReceptionExemplaireEtat.SelectedIndex = -1;
            }
        }

        private void btnModifierEtatExemplaire_Click(object sender, EventArgs e)
        {
            if (dgvReceptionExemplairesListe.CurrentCell == null || bdgExemplairesListe.Position < 0)
            {
                MessageBox.Show("Sélectionnez un exemplaire.");
                return;
            }

            if (cbxReceptionExemplaireEtat.SelectedIndex < 0)
            {
                MessageBox.Show("Sélectionnez un état.");
                return;
            }

            Exemplaire exemplaire = (Exemplaire)bdgExemplairesListe.List[bdgExemplairesListe.Position];

            string idEtat = "";
            switch (cbxReceptionExemplaireEtat.SelectedIndex)
            {
                case 0:
                    idEtat = "00001";
                    break;
                case 1:
                    idEtat = "00002";
                    break;
                case 2:
                    idEtat = "00003";
                    break;
                case 3:
                    idEtat = "00004";
                    break;
                default:
                    MessageBox.Show("Etat invalide.");
                    return;
            }

            Exemplaire exemplaireModifie = new Exemplaire(
                exemplaire.Numero,
                exemplaire.DateAchat,
                exemplaire.Photo,
                idEtat,
                exemplaire.Id
            );

            bool ok = controller.ModifierExemplaire(exemplaireModifie);

            if (ok)
            {
                MessageBox.Show("Modification effectuée.");
                AfficheReceptionExemplairesDocument();
            }
            else
            {
                MessageBox.Show("Erreur lors de la modification.");
            }
        }

        private void btnSupprimerExemplaire_Click(object sender, EventArgs e)
        {
            if (dgvReceptionExemplairesListe.CurrentCell == null || bdgExemplairesListe.Position < 0)
            {
                MessageBox.Show("Sélectionnez un exemplaire.");
                return;
            }

            Exemplaire exemplaire = (Exemplaire)bdgExemplairesListe.List[bdgExemplairesListe.Position];

            DialogResult rep = MessageBox.Show(
                "Voulez-vous supprimer cet exemplaire ?",
                "Confirmation",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (rep == DialogResult.Yes)
            {
                bool ok = controller.SupprimerExemplaire(exemplaire.Id, exemplaire.Numero);

                if (ok)
                {
                    MessageBox.Show("Suppression effectuée.");
                    AfficheReceptionExemplairesDocument();
                }
                else
                {
                    MessageBox.Show("Erreur lors de la suppression.");
                }
            }
        }
        #endregion

        #region Onglet Commandes

        private bool enCoursAjoutCommande = false;
        private bool enCoursModificationCommande = false;
        private string idDocumentSelectionne = "";

        private void InitialiserOngletCommandes()
        {
            InitOngletCommandes();
        }

        private void InitOngletCommandes()
        {
            txtNumeroCommande.Text = "";
            txtMontantCommande.Text = "";
            txtNbExemplairesCommande.Text = "";

            dgvDocumentsCommande.DataSource = null;
            dgvCommandesDocument.DataSource = null;

            ActiverSaisieCommande(false);

            btnAjouterCommande.Enabled = false;
            btnModifierCommande.Enabled = false;
            btnSupprimerCommande.Enabled = false;
            btnEnregistrerCommande.Enabled = false;
            btnAnnulerCommande.Enabled = false;
        }

        private void ConfigurerGrillesCommandes()
        {
            dgvDocumentsCommande.ReadOnly = true;
            dgvDocumentsCommande.MultiSelect = false;
            dgvDocumentsCommande.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvDocumentsCommande.AllowUserToAddRows = false;
            dgvDocumentsCommande.AllowUserToDeleteRows = false;
            dgvDocumentsCommande.AllowUserToResizeRows = false;
            dgvDocumentsCommande.RowHeadersVisible = false;
            dgvDocumentsCommande.AutoGenerateColumns = true;
            dgvDocumentsCommande.ClearSelection();

            dgvCommandesDocument.ReadOnly = true;
            dgvCommandesDocument.MultiSelect = false;
            dgvCommandesDocument.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCommandesDocument.AllowUserToAddRows = false;
            dgvCommandesDocument.AllowUserToDeleteRows = false;
            dgvCommandesDocument.AllowUserToResizeRows = false;
            dgvCommandesDocument.RowHeadersVisible = false;
            dgvCommandesDocument.AutoGenerateColumns = true;
            dgvCommandesDocument.ClearSelection();
        }

        private void ActiverSaisieCommande(bool actif)
        {
            txtNumeroCommande.Enabled = actif;
            txtMontantCommande.Enabled = actif;
            txtNbExemplairesCommande.Enabled = actif;

            cbxSuiviCommande.Enabled = actif;
            dtpDateCommande.Enabled = actif;

            txtMontantCommande.ReadOnly = !actif;
            txtNbExemplairesCommande.ReadOnly = !actif;

            txtNumeroCommande.ReadOnly = !enCoursAjoutCommande;
        }

        private void ReinitialiserCommande()
        {
            enCoursAjoutCommande = false;
            enCoursModificationCommande = false;

            ViderChampsCommande();

            txtNumeroCommande.ReadOnly = true;
            ActiverSaisieCommande(false);

            btnEnregistrerCommande.Enabled = false;
            btnAnnulerCommande.Enabled = false;
            btnModifierCommande.Enabled = false;
            btnSupprimerCommande.Enabled = false;
            btnAjouterCommande.Enabled = (idDocumentSelectionne != "");
        }

        private bool CommandeLivreeOuReglee()
        {
            string suivi = cbxSuiviCommande.Text.Trim().ToLower();
            return suivi == "livrée" || suivi == "réglée";
        }

        private bool CommandeReglee()
        {
            string suivi = cbxSuiviCommande.Text.Trim().ToLower();
            return suivi == "réglée";
        }

        private void ChargerCommandesDocument(string idDocument)
        {
            dgvCommandesDocument.DataSource = controller.GetCommandesDocument(idDocument);

            if (dgvCommandesDocument.Columns.Count > 0)
            {
                if (dgvCommandesDocument.Columns.Contains("IdLivreDvd"))
                    dgvCommandesDocument.Columns["IdLivreDvd"].Visible = false;

                if (dgvCommandesDocument.Columns.Contains("IdSuivi"))
                    dgvCommandesDocument.Columns["IdSuivi"].Visible = false;

                dgvCommandesDocument.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            }

            btnAjouterCommande.Enabled = true;

            if (dgvCommandesDocument.Rows.Count > 0)
            {
                dgvCommandesDocument.ClearSelection();
                dgvCommandesDocument.Rows[0].Selected = true;
                dgvCommandesDocument.CurrentCell = dgvCommandesDocument.Rows[0].Cells["Id"];

                AfficherCommandeSelectionnee();
            }
            else
            {
                ViderChampsCommande();
                btnModifierCommande.Enabled = false;
                btnSupprimerCommande.Enabled = false;
            }
        }
        
        private void AfficherCommandeSelectionnee()
        {
            if (dgvCommandesDocument.CurrentRow == null || dgvCommandesDocument.CurrentRow.Cells["Id"].Value == null)
            {
                return;
            }

            txtNumeroCommande.Text = dgvCommandesDocument.CurrentRow.Cells["Id"].Value.ToString();

            if (DateTime.TryParse(dgvCommandesDocument.CurrentRow.Cells["DateCommande"].Value.ToString(), out DateTime dateCommande))
            {
                dtpDateCommande.Value = dateCommande;
            }

            txtMontantCommande.Text = dgvCommandesDocument.CurrentRow.Cells["Montant"].Value.ToString();
            txtNbExemplairesCommande.Text = dgvCommandesDocument.CurrentRow.Cells["NbExemplaire"].Value.ToString();

            if (dgvCommandesDocument.Columns.Contains("LibelleSuivi"))
            {
                cbxSuiviCommande.Text = dgvCommandesDocument.CurrentRow.Cells["LibelleSuivi"].Value.ToString();
            }

            btnModifierCommande.Enabled = !CommandeReglee();
            btnSupprimerCommande.Enabled = !CommandeLivreeOuReglee();
        }

        private void AfficherCommandesDuDocumentSelectionne(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= dgvDocumentsCommande.Rows.Count)
            {
                idDocumentSelectionne = "";
                dgvCommandesDocument.DataSource = null;
                ReinitialiserCommande();
                return;
            }

            DataGridViewRow row = dgvDocumentsCommande.Rows[rowIndex];

            if (row.Cells["Id"].Value == null)
            {
                idDocumentSelectionne = "";
                dgvCommandesDocument.DataSource = null;
                ReinitialiserCommande();
                return;
            }

            dgvDocumentsCommande.ClearSelection();
            row.Selected = true;
            dgvDocumentsCommande.CurrentCell = row.Cells["Id"];

            idDocumentSelectionne = row.Cells["Id"].Value.ToString();

            ChargerCommandesDocument(idDocumentSelectionne);

            btnAjouterCommande.Enabled = true;
            btnModifierCommande.Enabled = false;
            btnSupprimerCommande.Enabled = false;
        }

        private void ConfigurerGrilleDocuments()
        {
            if (dgvDocumentsCommande.Columns.Count > 0)
            {
                if (dgvDocumentsCommande.Columns.Contains("Image"))
                    dgvDocumentsCommande.Columns["Image"].Visible = false;

                if (dgvDocumentsCommande.Columns.Contains("Id"))
                    dgvDocumentsCommande.Columns["Id"].HeaderText = "Numéro";

                if (dgvDocumentsCommande.Columns.Contains("Titre"))
                    dgvDocumentsCommande.Columns["Titre"].HeaderText = "Titre";

                dgvDocumentsCommande.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            }
        }

        private void dgvCommandesDocument_SelectionChanged(object sender, EventArgs e)
        {
            AfficherCommandeSelectionnee();
        }

        private void dgvCommandesDocument_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            AfficherCommandeSelectionnee();
        }

        private void dgvDocumentsCommande_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            AfficherCommandesDuDocumentSelectionne(e.RowIndex);
        }

        private void dgvCommandesDocument_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            DataGridViewRow row = dgvCommandesDocument.Rows[e.RowIndex];

            if (row.Cells["Id"].Value == null)
            {
                return;
            }

            dgvCommandesDocument.ClearSelection();
            row.Selected = true;
            dgvCommandesDocument.CurrentCell = row.Cells["Id"];

            AfficherCommandeSelectionnee();
        }

        private bool TransitionSuiviValide(string ancienSuivi, string nouveauSuivi)
        {
            if (ancienSuivi == "réglée" && nouveauSuivi != "réglée")
                return false;

            if (ancienSuivi == "livrée" && (nouveauSuivi == "en cours" || nouveauSuivi == "relancée"))
                return false;

            return true;
        }

        private void ViderChampsCommande()
        {
            txtNumeroCommande.Text = "";
            txtMontantCommande.Text = "";
            txtNbExemplairesCommande.Text = "";
            cbxSuiviCommande.SelectedIndex = -1;
            dtpDateCommande.Value = DateTime.Now;
        }

        private void btnAjouterCommande_Click(object sender, EventArgs e)
        {
            if (idDocumentSelectionne == "")
            {
                MessageBox.Show("Sélectionnez d'abord un document.");
                return;
            }

            enCoursAjoutCommande = true;
            enCoursModificationCommande = false;

            ViderChampsCommande();
            ActiverSaisieCommande(true);

            cbxSuiviCommande.Text = "en cours";

            btnAjouterCommande.Enabled = false;
            btnModifierCommande.Enabled = false;
            btnSupprimerCommande.Enabled = false;
            btnEnregistrerCommande.Enabled = true;
            btnAnnulerCommande.Enabled = true;
        }

        private void btnModifierCommande_Click(object sender, EventArgs e)
        {
            if (txtNumeroCommande.Text.Trim() == "")
            {
                MessageBox.Show("Sélectionnez une commande.");
                return;
            }

            enCoursAjoutCommande = false;
            enCoursModificationCommande = true;

            ActiverSaisieCommande(true);

            btnAjouterCommande.Enabled = false;
            btnModifierCommande.Enabled = false;
            btnSupprimerCommande.Enabled = false;
            btnEnregistrerCommande.Enabled = true;
            btnAnnulerCommande.Enabled = true;
        }

        private string GetIdSuiviFromLibelle(string libelle)
        {
            switch (libelle)
            {
                case "en cours": return "00001";
                case "relancée": return "00002";
                case "livrée": return "00003";
                case "réglée": return "00004";
                default: return "";
            }
        }

        private void btnEnregistrerCommande_Click(object sender, EventArgs e)
        {
            if (idDocumentSelectionne == "")
            {
                MessageBox.Show("Aucun document sélectionné.");
                return;
            }

            if (txtNumeroCommande.Text.Trim() == "" ||
                txtMontantCommande.Text.Trim() == "" ||
                txtNbExemplairesCommande.Text.Trim() == "" ||
                cbxSuiviCommande.Text.Trim() == "")
            {
                MessageBox.Show("Tous les champs sont obligatoires.");
                return;
            }

            if (!double.TryParse(txtMontantCommande.Text.Trim(), out double montant))
            {
                MessageBox.Show("Le montant est invalide.");
                return;
            }

            if (!int.TryParse(txtNbExemplairesCommande.Text.Trim(), out int nbExemplaires) || nbExemplaires <= 0)
            {
                MessageBox.Show("Le nombre d'exemplaires est invalide.");
                return;
            }

            string idSuivi = GetIdSuiviFromLibelle(cbxSuiviCommande.Text.Trim());

            if (idSuivi == "")
            {
                MessageBox.Show("Le suivi est invalide.");
                return;
            }

            CommandeDocument commande = new CommandeDocument(
                txtNumeroCommande.Text.Trim(),
                dtpDateCommande.Value.ToString("yyyy-MM-dd"),
                montant,
                nbExemplaires,
                idDocumentSelectionne,
                idSuivi,
                cbxSuiviCommande.Text.Trim()
            );

            bool ok = false;

            if (enCoursAjoutCommande)
            {
                ok = controller.CreerCommandeDocument(commande);
            }
            else if (enCoursModificationCommande)
            {
                ok = controller.ModifierCommandeDocument(commande);
            }

            if (ok)
            {
                MessageBox.Show("Enregistrement effectué.");
                ChargerCommandesDocument(idDocumentSelectionne);
                ReinitialiserCommande();
            }
            else
            {
                MessageBox.Show("Erreur lors de l'enregistrement.");
            }
        }

        private void btnSupprimerCommande_Click(object sender, EventArgs e)
        {
            if (txtNumeroCommande.Text.Trim() == "")
            {
                MessageBox.Show("Sélectionnez une commande.");
                return;
            }

            DialogResult rep = MessageBox.Show(
                "Voulez-vous supprimer cette commande ?",
                "Confirmation",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (rep == DialogResult.Yes)
            {
                bool ok = controller.SupprimerCommandeDocument(txtNumeroCommande.Text);

                if (ok)
                {
                    MessageBox.Show("Suppression effectuée.");
                    ChargerCommandesDocument(idDocumentSelectionne);
                    ReinitialiserCommande();
                }
                else
                {
                    MessageBox.Show("Suppression impossible.");
                }
            }
        }

        private void btnAnnulerCommande_Click(object sender, EventArgs e)
        {
            ReinitialiserCommande();
        }

        private void btnRechercherDocumentCommande_Click(object sender, EventArgs e)
        {
            string recherche = txtRechercheDocumentCommande.Text.Trim().ToLower();

            if (cbxTypeDocumentCommande.Text == "")
            {
                MessageBox.Show("Sélectionnez un type de document.");
                return;
            }

            if (cbxTypeDocumentCommande.Text == "Livre")
            {
                var livres = controller.GetAllLivres();

                var resultat = livres.Where(l =>
                    l.Id.ToLower().Contains(recherche) ||
                    l.Titre.ToLower().Contains(recherche)
                ).ToList();

                dgvDocumentsCommande.DataSource = resultat;
            }
            else if (cbxTypeDocumentCommande.Text == "DVD")
            {
                var dvds = controller.GetAllDvd();

                var resultat = dvds.Where(d =>
                    d.Id.ToLower().Contains(recherche) ||
                    d.Titre.ToLower().Contains(recherche)
                ).ToList();

                dgvDocumentsCommande.DataSource = resultat;
            }

            ConfigurerGrilleDocuments();

            dgvCommandesDocument.DataSource = null;
            idDocumentSelectionne = "";

            ReinitialiserCommande();

            if (dgvDocumentsCommande.Rows.Count > 0)
            {
                AfficherCommandesDuDocumentSelectionne(0);
            }
            else
            {
                dgvCommandesDocument.DataSource = null;
                idDocumentSelectionne = "";
                ReinitialiserCommande();
            }
        }

        #endregion

        #region Onglet Abonnements

        private bool enCoursAjoutCommandeRevue = false;
        private bool enCoursModificationCommandeRevue = false;
        private string idRevueSelectionnee = "";

        private void InitialiserOngletAbonnements()
        {
            ConfigurerGrillesAbonnements();
            InitOngletAbonnements();
        }
        private void tabAbonnements_Enter(object sender, EventArgs e)
        {
            InitOngletAbonnements();
        }

        private void ActiverSaisieCommandeRevue(bool actif)
        {
            txtNumeroCommandeRevue.Enabled = actif;
            txtMontantCommandeRevue.Enabled = actif;
            dtpDateCommandeRevue.Enabled = actif;
            dtpDateFinAbonnement.Enabled = actif;

            txtMontantCommandeRevue.ReadOnly = !actif;
            txtNumeroCommandeRevue.ReadOnly = !enCoursAjoutCommandeRevue;
        }

        private void ViderChampsCommandeRevue()
        {
            txtNumeroCommandeRevue.Text = "";
            txtMontantCommandeRevue.Text = "";
            dtpDateCommandeRevue.Value = DateTime.Now;
            dtpDateFinAbonnement.Value = DateTime.Now;
        }

        private void ReinitialiserCommandeRevue()
        {
            enCoursAjoutCommandeRevue = false;
            enCoursModificationCommandeRevue = false;

            ViderChampsCommandeRevue();
            ActiverSaisieCommandeRevue(false);

            btnEnregistrerCommandeRevue.Enabled = false;
            btnAnnulerCommandeRevue.Enabled = false;
            btnModifierCommandeRevue.Enabled = false;
            btnSupprimerCommandeRevue.Enabled = false;
            btnAjouterCommandeRevue.Enabled = (idRevueSelectionnee != "");
        }

        private void InitOngletAbonnements()
        {
            txtNumeroCommandeRevue.Text = "";
            txtMontantCommandeRevue.Text = "";

            dgvRevuesAbonnement.DataSource = null;
            dgvCommandesRevue.DataSource = null;

            ActiverSaisieCommandeRevue(false);

            btnAjouterCommandeRevue.Enabled = false;
            btnModifierCommandeRevue.Enabled = false;
            btnSupprimerCommandeRevue.Enabled = false;
            btnEnregistrerCommandeRevue.Enabled = false;
            btnAnnulerCommandeRevue.Enabled = false;
        }

        private void ConfigurerGrillesAbonnements()
        {
            dgvRevuesAbonnement.ReadOnly = true;
            dgvRevuesAbonnement.MultiSelect = false;
            dgvRevuesAbonnement.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvRevuesAbonnement.AllowUserToAddRows = false;
            dgvRevuesAbonnement.AllowUserToDeleteRows = false;
            dgvRevuesAbonnement.AllowUserToResizeRows = false;
            dgvRevuesAbonnement.RowHeadersVisible = false;
            dgvRevuesAbonnement.AutoGenerateColumns = true;
            dgvRevuesAbonnement.ClearSelection();

            dgvCommandesRevue.ReadOnly = true;
            dgvCommandesRevue.MultiSelect = false;
            dgvCommandesRevue.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCommandesRevue.AllowUserToAddRows = false;
            dgvCommandesRevue.AllowUserToDeleteRows = false;
            dgvCommandesRevue.AllowUserToResizeRows = false;
            dgvCommandesRevue.RowHeadersVisible = false;
            dgvCommandesRevue.AutoGenerateColumns = true;
            dgvCommandesRevue.ClearSelection();
        }

        private void btnRechercherRevueAbonnement_Click(object sender, EventArgs e)
        {
            string recherche = txtRechercheRevueAbonnement.Text.Trim().ToLower();

            var revues = controller.GetAllRevues();

            var resultat = revues.Where(r =>
                r.Id.ToLower().Contains(recherche) ||
                r.Titre.ToLower().Contains(recherche)
            ).ToList();

            dgvRevuesAbonnement.DataSource = resultat;

            if (dgvRevuesAbonnement.Columns.Count > 0)
            {
                if (dgvRevuesAbonnement.Columns.Contains("Image"))
                    dgvRevuesAbonnement.Columns["Image"].Visible = false;

                dgvRevuesAbonnement.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            }

            dgvCommandesRevue.DataSource = null;
            idRevueSelectionnee = "";
            ReinitialiserCommandeRevue();

            if (dgvRevuesAbonnement.Rows.Count > 0)
            {
                dgvRevuesAbonnement.ClearSelection();
                dgvRevuesAbonnement.Rows[0].Selected = true;
                dgvRevuesAbonnement.CurrentCell = dgvRevuesAbonnement.Rows[0].Cells["Id"];

                idRevueSelectionnee = dgvRevuesAbonnement.Rows[0].Cells["Id"].Value.ToString();

                ChargerCommandesRevue(idRevueSelectionnee);

                btnAjouterCommandeRevue.Enabled = true;
                btnModifierCommandeRevue.Enabled = false;
                btnSupprimerCommandeRevue.Enabled = false;
            }
            else
            {
                idRevueSelectionnee = "";
                dgvCommandesRevue.DataSource = null;
                ReinitialiserCommandeRevue();
            }
        }

        private void ChargerCommandesRevue(string idRevue)
        {
            dgvCommandesRevue.DataSource = controller.GetCommandesRevue(idRevue);

            if (dgvCommandesRevue.Columns.Count > 0)
            {
                if (dgvCommandesRevue.Columns.Contains("IdRevue"))
                    dgvCommandesRevue.Columns["IdRevue"].Visible = false;

                dgvCommandesRevue.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            }

            btnAjouterCommandeRevue.Enabled = true;

            if (dgvCommandesRevue.Rows.Count > 0)
            {
                dgvCommandesRevue.Rows[0].Selected = true;
                dgvCommandesRevue.CurrentCell = dgvCommandesRevue.Rows[0].Cells["Id"];
                AfficherCommandeRevueSelectionnee();
            }
            else
            {
                ViderChampsCommandeRevue();
                btnModifierCommandeRevue.Enabled = false;
                btnSupprimerCommandeRevue.Enabled = false;
            }
        }  
                        
        private void dgvRevuesAbonnement_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = dgvRevuesAbonnement.Rows[e.RowIndex];
            if (row.Cells["Id"].Value == null) return;

            dgvRevuesAbonnement.ClearSelection();
            row.Selected = true;
            dgvRevuesAbonnement.CurrentCell = row.Cells["Id"];

            idRevueSelectionnee = row.Cells["Id"].Value.ToString();
            ChargerCommandesRevue(idRevueSelectionnee);

            btnAjouterCommandeRevue.Enabled = true;
            btnModifierCommandeRevue.Enabled = false;
            btnSupprimerCommandeRevue.Enabled = false;
        }

        private void AfficherCommandeRevueSelectionnee()
        {
            if (dgvCommandesRevue.CurrentRow == null || dgvCommandesRevue.CurrentRow.Cells["Id"].Value == null)
            {
                return;
            }

            txtNumeroCommandeRevue.Text = dgvCommandesRevue.CurrentRow.Cells["Id"].Value.ToString();

            if (DateTime.TryParse(dgvCommandesRevue.CurrentRow.Cells["DateCommande"].Value.ToString(), out DateTime dateCommande))
            {
                dtpDateCommandeRevue.Value = dateCommande;
            }

            txtMontantCommandeRevue.Text = dgvCommandesRevue.CurrentRow.Cells["Montant"].Value.ToString();

            if (DateTime.TryParse(dgvCommandesRevue.CurrentRow.Cells["DateFinAbonnement"].Value.ToString(), out DateTime dateFin))
            {
                dtpDateFinAbonnement.Value = dateFin;
            }

            btnModifierCommandeRevue.Enabled = true;
            btnSupprimerCommandeRevue.Enabled = true;
        }

        private void dgvCommandesRevue_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = dgvCommandesRevue.Rows[e.RowIndex];
            if (row.Cells["Id"].Value == null) return;

            dgvCommandesRevue.ClearSelection();
            row.Selected = true;
            dgvCommandesRevue.CurrentCell = row.Cells["Id"];

            AfficherCommandeRevueSelectionnee();
        }

        private void btnAjouterCommandeRevue_Click(object sender, EventArgs e)
        {
            if (idRevueSelectionnee == "")
            {
                MessageBox.Show("Sélectionnez d'abord une revue.");
                return;
            }

            enCoursAjoutCommandeRevue = true;
            enCoursModificationCommandeRevue = false;

            ViderChampsCommandeRevue();
            ActiverSaisieCommandeRevue(true);

            btnAjouterCommandeRevue.Enabled = false;
            btnModifierCommandeRevue.Enabled = false;
            btnSupprimerCommandeRevue.Enabled = false;
            btnEnregistrerCommandeRevue.Enabled = true;
            btnAnnulerCommandeRevue.Enabled = true;
        }

        private void btnModifierCommandeRevue_Click(object sender, EventArgs e)
        {
            if (txtNumeroCommandeRevue.Text.Trim() == "")
            {
                MessageBox.Show("Sélectionnez une commande.");
                return;
            }

            enCoursAjoutCommandeRevue = false;
            enCoursModificationCommandeRevue = true;

            ActiverSaisieCommandeRevue(true);

            btnAjouterCommandeRevue.Enabled = false;
            btnModifierCommandeRevue.Enabled = false;
            btnSupprimerCommandeRevue.Enabled = false;
            btnEnregistrerCommandeRevue.Enabled = true;
            btnAnnulerCommandeRevue.Enabled = true;
        }

        private void btnSupprimerCommandeRevue_Click(object sender, EventArgs e)
        {
            if (txtNumeroCommandeRevue.Text.Trim() == "")
            {
                MessageBox.Show("Sélectionnez une commande.");
                return;
            }

            DialogResult rep = MessageBox.Show(
                "Voulez-vous supprimer cet abonnement ?",
                "Confirmation",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (rep == DialogResult.Yes)
            {
                bool ok = controller.SupprimerCommandeRevue(txtNumeroCommandeRevue.Text.Trim());

                if (ok)
                {
                    MessageBox.Show("Suppression effectuée.");
                    ChargerCommandesRevue(idRevueSelectionnee);
                    ReinitialiserCommandeRevue();
                }
                else
                {
                    MessageBox.Show("Suppression impossible.");
                }
            }
        }

        private void btnEnregistrerCommandeRevue_Click(object sender, EventArgs e)
        {
            if (idRevueSelectionnee == "")
            {
                MessageBox.Show("Aucune revue sélectionnée.");
                return;
            }

            if (txtNumeroCommandeRevue.Text.Trim() == "" || txtMontantCommandeRevue.Text.Trim() == "")
            {
                MessageBox.Show("Tous les champs sont obligatoires.");
                return;
            }

            if (!double.TryParse(txtMontantCommandeRevue.Text.Trim(), out double montant))
            {
                MessageBox.Show("Le montant est invalide.");
                return;
            }

            CommandeRevue commande = new CommandeRevue(
                txtNumeroCommandeRevue.Text.Trim(),
                dtpDateCommandeRevue.Value.ToString("yyyy-MM-dd"),
                dtpDateFinAbonnement.Value.ToString("yyyy-MM-dd"),
                montant,
                idRevueSelectionnee
            );

            bool ok = false;

            if (enCoursAjoutCommandeRevue)
            {
                ok = controller.CreerCommandeRevue(commande);
            }
            else if (enCoursModificationCommandeRevue)
            {
                ok = controller.ModifierCommandeRevue(commande);
            }

            if (ok)
            {
                MessageBox.Show("Enregistrement effectué.");
                ChargerCommandesRevue(idRevueSelectionnee);
                ReinitialiserCommandeRevue();
            }
            else
            {
                MessageBox.Show("Erreur lors de l'enregistrement.");
            }
        }

        private void btnAnnulerCommandeRevue_Click(object sender, EventArgs e)
        {
            ReinitialiserCommandeRevue();
        }
        #endregion
    }
}
