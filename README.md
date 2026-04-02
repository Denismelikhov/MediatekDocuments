<h1>Présentation de l'application MediatekDocuments</h1>

Cette application est basée sur le dépôt d’origine suivant :<br>
https://github.com/CNED-SLAM/MediaTekDocuments<br><br>

Le dépôt d’origine contient, dans son readme, la présentation de l’application de base.<br>
Dans ce dépôt, je présente uniquement les <strong>fonctionnalités ajoutées</strong> ainsi que le <strong>mode opératoire</strong> pour installer et utiliser l’application en local.<br>

<h1>Technologies utilisées</h1>

L’application a été développée en <strong>C#</strong> avec <strong>Windows Forms</strong> sous <strong>Visual Studio</strong>.<br>
Elle communique avec une <strong>API REST en PHP</strong> afin d’accéder à une base de données <strong>MySQL</strong>.<br>

<h1>Fonctionnalités ajoutées</h1>

Par rapport à l’application d’origine, les fonctionnalités suivantes ont été ajoutées :<br>

<ul>
  <li><strong>Mission 1 :</strong> ajout, modification et suppression des <strong>livres</strong>, <strong>DVD</strong> et <strong>revues</strong> ;</li>
  <li><strong>Mission 2 :</strong> gestion des <strong>commandes de livres et DVD</strong> avec suivi d’état ;</li>
  <li><strong>Mission 2 :</strong> gestion des <strong>commandes de revues</strong> et des abonnements ;</li>
  <li><strong>Mission 3 :</strong> ajout du <strong>suivi de l’état des exemplaires</strong> et possibilité de suppression ;</li>
  <li><strong>Mission 4 :</strong> ajout d’une <strong>fenêtre d’authentification</strong> et gestion des <strong>droits selon le service</strong> ;</li>
  <li><strong>Mission 5 :</strong> correction de problèmes de sécurité, ajout de <strong>logs</strong> et amélioration de la qualité du code ;</li>
  <li><strong>Mission 6 :</strong> réalisation de <strong>tests de l’API avec Postman</strong> ;</li>
  <li><strong>Mission 7 :</strong> adaptation de l’application pour l’accès à une <strong>API distante</strong> et création d’un <strong>installateur avec Inno Setup</strong>.</li>
</ul>

<h1>Fonctionnement général</h1>

L’application démarre désormais sur une <strong>fenêtre d’authentification</strong>.<br>
Selon le service de l’utilisateur connecté :
<ul>
  <li><strong>Administratif / Administrateur</strong> : accès complet ;</li>
  <li><strong>Prêts</strong> : consultation uniquement ;</li>
  <li><strong>Culture</strong> : accès refusé.</li>
</ul>
<img width="589" height="366" alt="Screenshot 2026-04-02 045729" src="https://github.com/user-attachments/assets/b70b38a3-5e9a-4e04-90c0-41e1056d5ee9" />


L’interface est organisée en plusieurs onglets :
<ul>
  <li>Livres</li>
  <li>DVD</li>
  <li>Revues</li>
  <li>Parutions des revues</li>
  <li>Commandes</li>
  <li>Abonnements</li>
</ul>
<img width="1659" height="1335" alt="Screenshot 2026-04-02 041816" src="https://github.com/user-attachments/assets/fd49263e-14bd-42a6-a661-009eb452189d" />


<h1>Installation en local</h1>

Pour installer et exécuter l’application en local, suivre les étapes suivantes :<br>
<ul>
  <li>installer <strong>Visual Studio</strong> ;</li>
  <li>ouvrir la solution <strong>MediaTekDocuments.sln</strong> ;</li>
  <li>vérifier que les dépendances nécessaires sont installées, notamment <strong>Newtonsoft.Json</strong> ;</li>
  <li>installer et configurer l’API REST locale ;</li>
  <li>créer la base de données <strong>mediatek86</strong> et importer le script SQL ;</li>
  <li>vérifier l’URL de l’API dans la configuration de l’application ;</li>
  <li>compiler puis lancer l’application depuis Visual Studio.</li>
</ul>

<h1>Utilisation de l’application</h1>

Une fois l’application installé et lancée :
<ul>
  <li>se connecter avec un compte utilisateur valide ;</li>
  <li>consulter ou gérer les documents selon les droits ;</li>
  <li>ajouter, modifier ou supprimer un document ;</li>
  <li>gérer les commandes et les abonnements ;</li>
  <li>mettre à jour l’état des exemplaires ;</li>
  <li>consulter les alertes de fin d’abonnement si le profil le permet.</li>
</ul>

<h1>Déploiement</h1>

L’API a été mise en ligne sur <strong>Alwaysdata</strong> afin d’utiliser un hébergement gratuit.<br>
L’application C# a ensuite été adaptée pour utiliser l’API distante, puis un installateur a été créé avec <strong>Inno Setup</strong>.<br>
