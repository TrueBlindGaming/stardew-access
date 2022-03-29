﻿using StardewValley;
using StardewValley.Menus;
using static StardewValley.Menus.LoadGameMenu;

namespace stardew_access.Patches
{
    internal class TitleMenuPatches
    {
        private static int saveGameIndex = -1;
        private static bool isRunning = false;
        private const int MAX_COMPONENTS = 20;

        internal static void CoopMenuPatch(CoopMenu __instance, CoopMenu.Tab ___currentTab)
        {
            try
            {
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true);
                string toSpeak = " ";

                #region Join/Host Button (Important! This should be checked before checking other buttons)
                if (__instance.slotButtons[0].containsPoint(x, y))
                {
                    if (___currentTab == CoopMenu.Tab.JOIN_TAB)
                        toSpeak = "Join lan game";
                    if (___currentTab == CoopMenu.Tab.HOST_TAB)
                        toSpeak = "Host new farm";
                }
                #endregion

                #region Other Buttons
                if (__instance.joinTab.containsPoint(x, y))
                {
                    toSpeak = "Join Tab Button";
                }
                else if (__instance.hostTab.containsPoint(x, y))
                {
                    toSpeak = "Host Tab Button";
                }
                else if (__instance.refreshButton.containsPoint(x, y))
                {
                    toSpeak = "Refresh Button";
                }
                #endregion

                if (toSpeak != " ")
                    MainClass.GetScreenReader().SayWithChecker(toSpeak, true);
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }

        internal static void TitleMenuPatch(TitleMenu __instance, bool ___isTransitioningButtons)
        {
            try
            {
                if (___isTransitioningButtons)
                    return;

                string toSpeak = "";

                __instance.buttons.ForEach(component =>
                {
                    if (component.containsPoint(Game1.getMouseX(true), Game1.getMouseY(true)))
                    {
                        string name = component.name;
                        string label = component.label;
                        toSpeak = $"{name} {label} Button";
                    }
                });

                if (__instance.muteMusicButton.containsPoint(Game1.getMouseX(true), Game1.getMouseY(true)))
                {
                    toSpeak = "Mute Music Button";
                }

                if (__instance.aboutButton.containsPoint(Game1.getMouseX(true), Game1.getMouseY(true)))
                {
                    toSpeak = "About Button";
                }

                if (__instance.languageButton.containsPoint(Game1.getMouseX(true), Game1.getMouseY(true)))
                {
                    toSpeak = "Language Button";
                }

                if (__instance.windowedButton.containsPoint(Game1.getMouseX(true), Game1.getMouseY(true)))
                {
                    toSpeak = "Fullscreen: " + ((Game1.isFullscreen) ? "on" : "off");
                }

                if (TitleMenu.subMenu != null && __instance.backButton.containsPoint(Game1.getMouseX(true), Game1.getMouseY(true)))
                {
                    string text = "Back Button";
                    MainClass.GetScreenReader().SayWithChecker(text, true);
                }

                if (TitleMenu.subMenu == null && toSpeak != "")
                    MainClass.GetScreenReader().SayWithChecker(toSpeak, true);
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }

        internal static void LoadGameMenuPatch(SaveFileSlot __instance, LoadGameMenu ___menu, int i)
        {
            try
            {
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true);
                if (___menu.slotButtons[i].containsPoint(x, y))
                {
                    if (__instance.Farmer != null)
                    {
                        #region Farms
                        if (___menu.deleteButtons.Count > 0 && ___menu.deleteButtons[i].containsPoint(x, y))
                        {
                            MainClass.GetScreenReader().SayWithChecker($"Delete {__instance.Farmer.farmName} Farm", true);
                            return;
                        }

                        if (___menu.deleteConfirmationScreen)
                        {
                            // Used diff. functions to narrate to prevent it from speaking the message again on selecting another button.
                            string message = "Really delete farm?";

                            MainClass.GetScreenReader().SayWithChecker(message, true);
                            if (___menu.okDeleteButton.containsPoint(x, y))
                            {
                                MainClass.GetScreenReader().SayWithMenuChecker("Ok Button", false);
                            }
                            else if (___menu.cancelDeleteButton.containsPoint(x, y))
                            {
                                MainClass.GetScreenReader().SayWithMenuChecker("Cancel Button", false);
                            }
                            return;
                        }

                        String farmerName = __instance.Farmer.displayName;
                        String farmName = __instance.Farmer.farmName;
                        String money = __instance.Farmer.Money.ToString();
                        String hoursPlayed = Utility.getHoursMinutesStringFromMilliseconds(__instance.Farmer.millisecondsPlayed);
                        string dateStringForSaveGame = ((!__instance.Farmer.dayOfMonthForSaveGame.HasValue ||
                            !__instance.Farmer.seasonForSaveGame.HasValue ||
                            !__instance.Farmer.yearForSaveGame.HasValue) ? __instance.Farmer.dateStringForSaveGame : Utility.getDateStringFor(__instance.Farmer.dayOfMonthForSaveGame.Value, __instance.Farmer.seasonForSaveGame.Value, __instance.Farmer.yearForSaveGame.Value));

                        string toSpeak = $"{farmName} Farm Selected, \t\n Farmer:{farmerName}, \t\nMoney:{money}, \t\nHours Played:{hoursPlayed}, \t\nDate:{dateStringForSaveGame}";

                        MainClass.GetScreenReader().SayWithChecker(toSpeak, true);
                        #endregion
                    }
                }
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }

        internal static void CharacterCustomizationMenuPatch(CharacterCustomization __instance, bool ___skipIntro)
        {
            try
            {
                bool isNextArrowPressed = Game1.input.GetKeyboardState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Right);
                bool isPrevArrowPressed = Game1.input.GetKeyboardState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Left);

                if (__instance.backButton.containsPoint != null && __instance.backButton.visible && __instance.backButton.containsPoint((int)Game1.getMouseX(true), (int)Game1.getMouseY(true)))
                {
                    // Perform Left Click
                    if (MainClass.Config.LeftClickMainKey.JustPressed() || MainClass.Config.LeftClickAlternateKey.JustPressed())
                    {
                        Game1.activeClickableMenu.receiveLeftClick(Game1.getMouseX(true), Game1.getMouseY(true));
                    }
                }

                if (isNextArrowPressed && !isRunning)
                {
                    isRunning = true;
                    CycleThroughItems(true, __instance, ___skipIntro);
                    Task.Delay(200).ContinueWith(_ => { isRunning = false; });
                }
                else if (isPrevArrowPressed && !isRunning)
                {
                    isRunning = true;
                    CycleThroughItems(false, __instance, ___skipIntro);
                    Task.Delay(200).ContinueWith(_ => { isRunning = false; });
                }
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }

        private static void CycleThroughItems(bool increase, CharacterCustomization __instance, bool ___skipIntro)
        {
            string toSpeak = " ";

            if (increase)
            {
                saveGameIndex++;
                if (saveGameIndex > MAX_COMPONENTS)
                    saveGameIndex = 1;
            }
            else
            {
                saveGameIndex--;
                if (saveGameIndex < 1)
                    saveGameIndex = MAX_COMPONENTS;
            }


            switch (saveGameIndex)
            {
                case 1:
                    {
                        #region Skip if button is not available
                        if (!__instance.nameBoxCC.visible)
                        {
                            if (increase)
                            {
                                ++saveGameIndex;
                                goto case 2;
                            }
                            else
                            {
                                --saveGameIndex;
                                goto case MAX_COMPONENTS;
                            }
                        }
                        #endregion

                        __instance.nameBoxCC.snapMouseCursorToCenter();
                        toSpeak = "Enter Farmer's Name";
                    }
                    break;

                case 2:
                    {
                        #region Skip if button is not available
                        if (!__instance.farmnameBoxCC.visible)
                        {
                            if (increase)
                            {
                                ++saveGameIndex;
                                goto case 3;
                            }
                            else
                            {
                                --saveGameIndex;
                                goto case 1;
                            }
                        }
                        #endregion

                        __instance.farmnameBoxCC.snapMouseCursorToCenter();
                        toSpeak = "Enter Farm's Name";
                    }
                    break;
                case 3:
                    {
                        #region Skip if button is not available
                        if (!__instance.favThingBoxCC.visible)
                        {
                            if (increase)
                            {
                                ++saveGameIndex;
                                goto case 4;
                            }
                            else
                            {
                                --saveGameIndex;
                                goto case 2;
                            }
                        }
                        #endregion

                        __instance.favThingBoxCC.snapMouseCursorToCenter();
                        toSpeak = "Enter Favourite Thing";
                    }
                    break;
                case 4:
                    {
                        #region Skip if button is not available
                        if (!__instance.skipIntroButton.visible)
                        {
                            if (increase)
                            {
                                ++saveGameIndex;
                                goto case 5;
                            }
                            else
                            {
                                --saveGameIndex;
                                goto case 3;
                            }
                        }
                        #endregion

                        __instance.skipIntroButton.snapMouseCursorToCenter();
                        toSpeak = (___skipIntro ? "Enabled" : "Disabled") + " Skip Intro Button";
                    }
                    break;
                case 5:
                    {
                        #region Skip if button is not available
                        if (!__instance.randomButton.visible)
                        {
                            if (increase)
                            {
                                ++saveGameIndex;
                                goto case 6;
                            }
                            else
                            {
                                --saveGameIndex;
                                goto case 5;
                            }
                        }
                        #endregion

                        __instance.randomButton.snapMouseCursorToCenter();
                        toSpeak = "Random Skin Button";
                        break;
                    }
                case 6:
                    {
                        #region Skip if button is not available
                        if (__instance.genderButtons.Count <= 0)
                        {
                            if (increase)
                            {
                                ++saveGameIndex;
                                goto case 8;
                            }
                            else
                            {
                                --saveGameIndex;
                                goto case 6;
                            }
                        }
                        #endregion

                        __instance.genderButtons[0].snapMouseCursorToCenter();
                        toSpeak = "Gender Male Button";
                        break;
                    }
                case 7:
                    {
                        #region Skip if button is not available
                        if (__instance.genderButtons.Count <= 0)
                        {
                            if (increase)
                            {
                                ++saveGameIndex;
                                goto case 8;
                            }
                            else
                            {
                                --saveGameIndex;
                                goto case 6;
                            }
                        }
                        #endregion

                        __instance.genderButtons[1].snapMouseCursorToCenter();
                        toSpeak = "Gender Female Button";
                        break;
                    }
                case 8:
                    {
                        #region Skip if button is not available
                        if (__instance.farmTypeButtons.Count <= 0)
                        {
                            if (increase)
                            {
                                ++saveGameIndex;
                                goto case 9;
                            }
                            else
                            {
                                --saveGameIndex;
                                goto case 7;
                            }
                        }
                        #endregion

                        __instance.farmTypeButtons[0].snapMouseCursorToCenter();
                        toSpeak = getFarmHoverText(__instance.farmTypeButtons[0]);
                        break;
                    }
                case 9:
                    {
                        #region Skip if button is not available
                        if (__instance.farmTypeButtons.Count <= 0)
                        {
                            if (increase)
                            {
                                ++saveGameIndex;
                                goto case 10;
                            }
                            else
                            {
                                --saveGameIndex;
                                goto case 8;
                            }
                        }
                        #endregion

                        __instance.farmTypeButtons[1].snapMouseCursorToCenter();
                        toSpeak = getFarmHoverText(__instance.farmTypeButtons[1]);
                        break;
                    }
                case 10:
                    {
                        #region Skip if button is not available
                        if (__instance.farmTypeButtons.Count <= 0)
                        {
                            if (increase)
                            {
                                ++saveGameIndex;
                                goto case 11;
                            }
                            else
                            {
                                --saveGameIndex;
                                goto case 9;
                            }
                        }
                        #endregion

                        __instance.farmTypeButtons[2].snapMouseCursorToCenter();
                        toSpeak = getFarmHoverText(__instance.farmTypeButtons[2]);
                        break;
                    }
                case 11:
                    {
                        #region Skip if button is not available
                        if (__instance.farmTypeButtons.Count <= 0)
                        {
                            if (increase)
                            {
                                ++saveGameIndex;
                                goto case 12;
                            }
                            else
                            {
                                --saveGameIndex;
                                goto case 10;
                            }
                        }
                        #endregion

                        __instance.farmTypeButtons[3].snapMouseCursorToCenter();
                        toSpeak = getFarmHoverText(__instance.farmTypeButtons[3]);
                        break;
                    }
                case 12:
                    {
                        #region Skip if button is not available
                        if (__instance.farmTypeButtons.Count <= 0)
                        {
                            if (increase)
                            {
                                ++saveGameIndex;
                                goto case 13;
                            }
                            else
                            {
                                --saveGameIndex;
                                goto case 11;
                            }
                        }
                        #endregion

                        __instance.farmTypeButtons[4].snapMouseCursorToCenter();
                        toSpeak = getFarmHoverText(__instance.farmTypeButtons[4]);
                        break;
                    }
                case 13:
                    {
                        #region Skip if button is not available
                        if (__instance.farmTypeButtons.Count <= 0)
                        {
                            if (increase)
                            {
                                ++saveGameIndex;
                                goto case 14;
                            }
                            else
                            {
                                --saveGameIndex;
                                goto case 12;
                            }
                        }
                        #endregion

                        __instance.farmTypeButtons[5].snapMouseCursorToCenter();
                        toSpeak = getFarmHoverText(__instance.farmTypeButtons[5]);
                        break;
                    }
                case 14:
                    {
                        #region Skip if button is not available
                        if (__instance.farmTypeButtons.Count <= 0)
                        {
                            if (increase)
                            {
                                ++saveGameIndex;
                                goto case 15;
                            }
                            else
                            {
                                --saveGameIndex;
                                goto case 13;
                            }
                        }
                        #endregion

                        __instance.farmTypeButtons[6].snapMouseCursorToCenter();
                        toSpeak = getFarmHoverText(__instance.farmTypeButtons[6]);
                        break;
                    }
                case 15:
                    {
                        #region Skip if button is not available
                        if (__instance.farmTypeNextPageButton == null)
                        {
                            if (increase)
                            {
                                ++saveGameIndex;
                                goto case 16;
                            }
                            else
                            {
                                --saveGameIndex;
                                goto case 14;
                            }
                        }
                        #endregion

                        __instance.farmTypeNextPageButton.snapMouseCursorToCenter();
                        toSpeak = "Next Farm Type Page Button";
                        break;
                    }
                case 16:
                    {
                        #region Skip if button is not available
                        if (__instance.farmTypePreviousPageButton == null)
                        {
                            if (increase)
                            {
                                ++saveGameIndex;
                                goto case 17;
                            }
                            else
                            {
                                --saveGameIndex;
                                goto case 15;
                            }
                        }
                        #endregion

                        __instance.farmTypePreviousPageButton.snapMouseCursorToCenter();
                        toSpeak = "Previous Farm Type Page Button";
                        break;
                    }
                case 17:
                    {
                        #region Skip if button is not available
                        if (__instance.cabinLayoutButtons.Count <= 0)
                        {
                            if (increase)
                            {
                                ++saveGameIndex;
                                goto case 18;
                            }
                            else
                            {
                                --saveGameIndex;
                                goto case 16;
                            }
                        }
                        #endregion

                        __instance.cabinLayoutButtons[0].snapMouseCursorToCenter();
                        toSpeak = "Cabin layout nearby";
                        break;
                    }
                case 18:
                    {
                        #region Skip if button is not available
                        if (__instance.cabinLayoutButtons.Count <= 0)
                        {
                            if (increase)
                            {
                                ++saveGameIndex;
                                goto case 19;
                            }
                            else
                            {
                                --saveGameIndex;
                                goto case 17;
                            }
                        }
                        #endregion

                        __instance.cabinLayoutButtons[1].snapMouseCursorToCenter();
                        toSpeak = "Cabin layout separate";
                        break;
                    }
                case 19:
                    {
                        #region Skip if button is not available
                        if (!__instance.okButton.visible)
                        {
                            if (increase)
                            {
                                ++saveGameIndex;
                                goto case 18;
                            }
                            else
                            {
                                --saveGameIndex;
                                goto case 20;
                            }
                        }
                        #endregion

                        __instance.okButton.snapMouseCursorToCenter();
                        toSpeak = "Ok Button";
                    }
                    break;
                case 20:
                    {
                        #region Exit if button is not available
                        if (!__instance.backButton.visible)
                        {
                            break;
                        }
                        #endregion

                        __instance.backButton.snapMouseCursorToCenter();
                        toSpeak = "Back Button";
                    }
                    break;
            }

            if (toSpeak != " ")
            {
                MainClass.GetScreenReader().Say(toSpeak, true);
            }
        }

        private static string getFarmHoverText(ClickableTextureComponent farm)
        {
            string hoverTitle = " ", hoverText = " ";
            if (!farm.name.Contains("Gray"))
            {
                if (farm.hoverText.Contains('_'))
                {
                    hoverTitle = farm.hoverText.Split('_')[0];
                    hoverText = farm.hoverText.Split('_')[1];
                }
                else
                {
                    hoverTitle = " ";
                    hoverText = farm.hoverText;
                }
            }
            else
            {
                if (farm.name.Contains("Gray"))
                {
                    hoverText = "Reach level 10 " + Game1.content.LoadString("Strings\\UI:Character_" + farm.name.Split('_')[1]) + " to unlock.";
                }
            }

            return $"{hoverTitle}: {hoverText}";
        }
    }
}
