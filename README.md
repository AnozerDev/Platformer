##STATUT == HS
# Platformer
Decouverte de C# avec MonoGame dans le cadre de la gameJam 8 de gameCodeur.
###### Consigne
- Utilliser LUA ou c#
- Realiser un platformer

## Credits
- Assets
    - Skeleton character by [MoonStar2d](https://moonstar2d.itch.io/)
    - Terrain tiles by [Kenney Vleugels](www.kenney.nl)
- Externe Library
    - TiledSharp by [Marshall Ward](tiledsharp@marshallward.org) (under the Apache License 2.0)
      > Use to import .tmx files in c#


#### TODO
- [X] Afficher un perso
- [X] Deplacer le perso, le faire sauter
- [x] Creer un decors
- [x] Gerer collision avec decors
- [x] Revoir le saut ( velocity && gravity)

- [ ] Demarrer à partir d' un menu
- [ ] Add musique && bruitages

- [ ] \+ si affinité :notes:
- [ ] Utiliser le sprite humain pour le joueur
- [ ] Ajouter des mobs

###### Refactoring && ameliorations...
- [ ] __Grosse refonte logique && d' organisation avant toute autre implementation !!__ (--Non c'est pas brouillon ..--)

- [ ] extraire la logique de sprite animé de la classe perso.
- [ ] un même spritesheet pour toute les animes du perso.
- [ ] n' embarquer que les tuiles utiles dans le tileset ( Terrain)
- [x] trouver un sprite avec une anime de jump.. un platformer qui n'saute pas... bah...
- [ ] <del>incorporer la vitesse d' animation dans l' enum d' etat du perso ( ++ pour walk ou possibilité de faire courir..) </del> ..bofbof les enum en c#.. :fp:
- [ ] Pixel perfect pour les collisions.
- [ ] Mon propre parser XML plutot que de caster des classes existante dans tiedSharp... :fp:
- [ ] Nettoyer le brin mis en tatonnant sur les collisions :D + class static pour les gerer separement !
- [ ] Faire une carte plus grande que la fenetre.. la faire defiler avec le joueur