using System;
using System.Collections.Generic;
using MelonLoader;
using UnityEngine;
using LabFusion.Network;
using LabFusion.Representation;
using LabFusion.Extensions;
using BoneLib.BoneMenu;
using BoneLib.BoneMenu.Elements;
using TMPro;

[assembly: MelonInfo(typeof(ModRolesFusion.Main), "RolesPermanentsFusion", "1.1.0", "LaruERP")]
[assembly: MelonGame("Stress Level Zero", "BONELAB")]

namespace ModRolesFusion
{
    public class Main : MelonMod
    {
        public static Dictionary<ulong, string> RolesJoueurs = new Dictionary<ulong, string>();
        private static readonly List<string> ListeRolesDisponibles = new List<string> { "Citoyen", "Policier", "Medecin", "Criminel", "Admin" };
        private static MenuCategory _categoriePrincipale;

        public override void OnInitializeMelon()
        {
            MelonLogger.Msg("Mod de Roles avec Menu initialise !");
            _categoriePrincipale = MenuManager.CreateCategory("Gestion des Roles", Color.cyan);
            _categoriePrincipale.CreateFunctionElement("Rafraichir la liste", Color.white, () => GenererMenuJoueurs());
        }

        public override void OnUpdate()
        {
            if (NetworkInfo.HasServer && PlayerIdManager.HasLocalId)
            {
                foreach (var activePlayer in PlayerIdManager.PlayerIds)
                {
                    ulong userId = activePlayer.SmallId;
                    
                    if (!RolesJoueurs.ContainsKey(userId))
                    {
                        RolesJoueurs[userId] = "Citoyen";
                    }

                    UpdatePlayerTextDisplay(activePlayer, RolesJoueurs[userId]);
                }
            }
        }

        private void UpdatePlayerTextDisplay(PlayerId player, string texteRole)
        {
            GameObject nameTagObj = GameObject.Find(player.Metadata.GetUsername() + "_NameTag");
            
            if (nameTagObj != null)
            {
                TextMeshPro textComponent = nameTagObj.GetComponent<TextMeshPro>();
                if (textComponent != null)
                {
                    textComponent.text = $"[{texteRole}] {player.Metadata.GetUsername()}";
                    textComponent.color = Color.cyan;
                }
            }
        }

        private static void GenererMenuJoueurs()
        {
            _categoriePrincipale.Elements.Clear();
            _categoriePrincipale.CreateFunctionElement("Rafraichir la liste", Color.white, () => GenererMenuJoueurs());

            if (!NetworkInfo.HasServer)
            {
                _categoriePrincipale.CreateFunctionElement("Hors ligne - Aucun joueur", Color.red, null);
                return;
            }

            foreach (var player in PlayerIdManager.PlayerIds)
            {
                ulong idJoueur = player.SmallId;
                string nomJoueur = player.Metadata.GetUsername();
                
                MenuCategory menuJoueur = _categoriePrincipale.CreateCategory(nomJoueur, Color.yellow);
                
                foreach (string role in ListeRolesDisponibles)
                {
                    menuJoueur.CreateFunctionElement($"Attribuer: {role}", Color.green, () => {
                        ModifierRole(idJoueur, role);
                    });
                }
            }
        }

        public static void ModifierRole(ulong userId, string nouveauRole)
        {
            if (RolesJoueurs.ContainsKey(userId))
            {
                RolesJoueurs[userId] = nouveauRole;
                MelonLogger.Msg("Role mis a jour via le menu.");
            }
        }
    }
}
