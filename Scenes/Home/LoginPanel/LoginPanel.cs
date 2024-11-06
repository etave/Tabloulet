using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DotNetEnv;
using Godot;
using Tabloulet.Helpers;
using Tabloulet.Scenes.HomeNS;

namespace Tabloulet.Scenes.HomeNS.LoginPanelNS
{
    public partial class LoginPanel : Control
    
        // Référence aux éléments de l'interface
        Panel myPanel;
        Label loginError;
        LineEdit passwordField;
        Button loginButton;
        Button backspaceButton;
        Button deleteAllButton;
        List<Button> numberButtons = [];
        Color originalTextColor;
        Button quitButton;
        Timer timer;
        Guid passwordGuid;

        public override void _Ready()
        {
            // Initialiser les éléments
            myPanel = GetNode<Panel>("Panel");

            loginError = GetNode<Label>(
                "Panel/VBoxContainer/HBoxContainer/PaveNumerique/GridContainer/VBoxContainer/LabelError"
            );
            originalTextColor = loginError.GetThemeColor("font_color");

            // Initialiser les éléments
            passwordField = GetNode<LineEdit>(
                "Panel/VBoxContainer/HBoxContainer/PaveNumerique/GridContainer/VBoxContainer/LineEdit"
            );
            passwordField.Secret = true; // Cacher les caractères

            backspaceButton = GetNode<Button>(
                "Panel/VBoxContainer/HBoxContainer/PaveNumerique/GridContainer/HBoxContainer4/ButtonBackspace"
            );

            deleteAllButton = GetNode<Button>(
                "Panel/VBoxContainer/HBoxContainer/PaveNumerique/GridContainer/HBoxContainer4/ButtonX"
            );

            loginButton = GetNode<Button>(
                "Panel/VBoxContainer/HBoxContainer/PaveNumerique/GridContainer/HBoxContainer5/ButtonLogin"
            );

            var gridContainer = GetNode<GridContainer>(
                "Panel/VBoxContainer/HBoxContainer/PaveNumerique/GridContainer"
            );

            // Connecter le bouton "QUIT" à la méthode de gestion
            quitButton = GetNode<Button>(
                "Panel/VBoxContainer/PanelLeave/MarginContainer/ButtonLeave"
            );

            Env.Load(); // Charger le fichier .env
            string myPassword = Env.GetString("PASSWORD"); // Récupérer la valeur du mot de passe
          
            Env.Load(); // Charger le fichier .env
            string myPassword = Env.GetString("PASSWORD"); // Récupérer la valeur du mot de passe
            passwordGuid = new(Env.GetString("PASSWORD_RFID")); // Récupérer la valeur du GUID

            // Connecter le bouton "BACKSPACE" à la méthode de gestion
            backspaceButton.Pressed += () => Backspace();

            // Connecter le bouton "X" à la méthode de gestion
            deleteAllButton.Pressed += () => DeleteAll();

            // Connecter le bouton "LOGIN" à la méthode de gestion
            loginButton.Pressed += () => OnLoginButtonPressed(myPassword);

            // Récupérer les boutons numériques
            numberButtons = GetMyButtons(gridContainer);

            // Connecter le bouton "QUIT" à la méthode de gestion
            quitButton.Pressed += OnQuitButtonPressed;

            // Timer pour la lecture de la carte RFID
            timer = GetNode<Timer>("RFIDTimer");
            timer.Timeout += OnRFIDTimerTimeout;
            timer.Start();
        }

        public List<Button> GetMyButtons(GridContainer gridContainer)
        {
            // Récupérer tous les boutons dans l'arbre
            foreach (var child in gridContainer.GetChildren())
            {
                if (child is HBoxContainer hbox)
                {
                    if (hbox.Name != "HBoxContainer5") // Remplacez "HBoxContainer5" par le nom que vous recherchez
                    {
                        foreach (var btn in hbox.GetChildren())
                        {
                            if (
                                btn is Button myButton
                                && myButton.Name != "ButtonBackspace"
                                && myButton.Name != "ButtonX"
                            ) // Remplacez "ButtonBackspace" par le nom que vous recherchez
                            {
                                myButton.FocusMode = Control.FocusModeEnum.None;
                                numberButtons.Add(myButton);
                                myButton.Pressed += () => OnNumberButtonPressed(myButton);
                            }
                        }
                    }
                }
            }

            return numberButtons;
        }

        // Méthode appelée lorsqu'un bouton numérique est appuyé
        private void OnNumberButtonPressed(Button pressedButton)
        {
            if (pressedButton != null) // Vérifie si le bouton est valide
            {
                passwordField.Text += pressedButton.Text; // Ajoute le texte du bouton au LineEdit
            }
        }

        // Méthode appelée lorsqu'on appuie sur le bouton "LOGIN"
        private void OnLoginButtonPressed(string myPassword)
        {
            string password = passwordField.Text;

            // Exemple simple de validation
            if (password == myPassword) // Remplace par ta logique de validation
            {
                passwordField.Text = ""; // Réinitialise le champ mot de passe

                // Restaurer la couleur d'origine (par défaut)
                loginError.AddThemeColorOverride("font_color", originalTextColor);

                Visible = false;
                var home = GetParent<Control>() as Home;
                home.ChangeToAdmin();
            }
            else
            {
                passwordField.Text = ""; // Réinitialise le champ mot de passe

                // Changer la couleur du texte à rouge vif pour indiquer une erreur
                loginError.AddThemeColorOverride("font_color", new Color("ff4c4c"));
            }
        }

        // Optionnel : Si tu veux ajouter une fonction pour supprimer des caractères (backspace)
        private void Backspace()
        {
            if (passwordField.Text.Length > 0)
            {
                passwordField.Text = passwordField.Text.Remove(passwordField.Text.Length - 1);
            }
        }

        private void DeleteAll()
        {
            passwordField.Text = "";
        }

        private void OnQuitButtonPressed()
        {
            Visible = false;
        }
    }

    private void OnRFIDTimerTimeout()
    {
        RFID.GetUIDAsync()
            .ContinueWith(
                task =>
                {
                    if (task.Result == passwordGuid)
                    {
                        Visible = false;
                        var home = GetParent<Control>() as Home;
                        home.ChangeToAdmin();
                    }
                },
                TaskScheduler.FromCurrentSynchronizationContext()
            );
    }
}
