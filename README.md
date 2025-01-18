# Tabloulet

Logiciel de création **et** de visualisation de contenus interactifs pour dispositifs tangibles.

## Préréquis (dans le cas où vous voudriez compiler et développer le projet)

- [Godot Engine](https://godotengine.org/download) (version 4.3 .NET)
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Visual Studio](https://visualstudio.microsoft.com/fr/downloads/) (pratique pour ouvrir la solution `.sln` et développer)

## Dépendances

Pour utiliser le logiciel, vous aurez besoin du package `libnfc-bin` pour la communication avec les lecteurs RFID.

## Utilisation

Pour utiliser le logiciel, téléchargez le fichier `tabloulet.x86_64` (Linux) dans la section [Releases](https://github.com/etave/Tabloulet/releases).
L'application nécessite de définir deux mots de passe pour accéder à l'administration et à l'application. Pour cela, le plus simple est de créer un fichier `.env` à côté de l'exécutable avec le contenu suivant :
*
```env
// Le mot de passe doit être composé uniquement de chiffres de 0 à 9
PASSWORD=<mot de passe>
// Si vous avez un lecteur RFID, vous pouvez définir un mot de passe RFID.
// Si vous êtes sur Windows ou si vous n'avez pas de lecteur RFID, mettez 00000000-0000-0000-0000-000000000000.
PASSWORD_RFID=<mot de passe>
```

Le mot de passe RFID se base sur l'UID de la carte RFID, pour le récupérer, placez la carte sur le lecteur et lancez la commande suivante :
```bash
sudo nfc-list

// Exemple de sortie
NFC reader: pn532_uart:/dev/ttyS0 opened
1 ISO14443A passive target(s) found:
ISO/IEC 14443A (106 kbps) target:
    ATQA (SENS_RES): 00  04
    UID (NFCID1): 3e 39 ab 7d // UID de la carte
    SAK (SEL_RES): 08
```

Avec cette sortie, le mot de passe RFID sera `3e39ab7d`, soit `3e39ab7d-0000-0000-0000-000000000000` dans le fichier `.env`.


## Compilation

Cette commande ne produit qu’un build pour Linux. Si Godot n’est pas dans votre PATH, téléchargez l’exécutable (https://godotengine.org/download) puis utilisez-le ainsi :

```bash
// Windows
C:/chemin/vers/Godot_v4.3.exe --headless --verbose --export-release "Linux" tabloulet.x86_64

// Linux
/path/to/Godot_v4.3 --headless --verbose --export-release "Linux" tabloulet.x86_64
```

## Développement

Pour développer, ouvrez le projet dans Godot et lancez-le. Pour déboguer, utilisez Visual Studio en ouvrant le fichier `Tabloulet.sln`.
Il faudra également créer un fichier `.env` dans la racine du projet (suivre les instructions de la section Utilisation).

## Licence

[MIT](https://choosealicense.com/licenses/mit/)

## Auteurs

- [Nathan Etave](https://www.github.com/etave)
- [Ibrahim Ozocak](https://www.github.com/ibrahimOzocak)
- [Lebeaupin Louis](https://www.github.com/LouisL18)
- [Maridat Ethan](https://www.github.com/Ethan-Maridat)
- [Mechain Romain](https://www.github.com/RomainMechain)