Sur la page Github du projet sont téléchargeables dans le dossier "Builds" les builds du projet pour Windows, Linux et MacOS.
On peut ouvrir le projet en lancant simplement l'application.
On peut interagir avec l'application ainsi:
- Cliquer sur les boutons (changer la configuration, afficher les infos...)
-Tourner la caméra autour des satellites avec les flèches directionnelles 
-Zoomer/Dézoomer avec les touches Z et S
-Afficher un satellite avec un clic gauche dessus
-Afficher la distance entre deux satellites en les sélectionnant avec clic droit
- Retourner au menu (en cas de bug par exemple) avec la touche échap

Pour le calcul des caractéristiques du graphe, le seul fichier pertinent à regarder dans le code source est la classe Graph située dans le fichier Graph.cs, où sont contenues toutes les fonctions de calcul. Le reste des fichiers est relatif à Unity et au traitement graphique et ne montre pas de pertinence pour l'aspect théorique des graphes.