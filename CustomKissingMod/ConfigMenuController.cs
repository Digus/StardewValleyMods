using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using CustomKissingMod.integrations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.Characters;

namespace CustomKissingMod
{
    internal class ConfigMenuController
    {
        private const string NewNpcConfirmationFieldId = "newNameConfirmation";
        private const string DeleteNpcConfirmationFieldId = "deleteNameConfirmation";
        private const string NpcNameFieldId = "newName";

        private static GenericModConfigMenuApi _api;
        private static ITranslationHelper _i18n;

        private static string _npcNameOperations = "";
        private static string _npcOperationMessage = "";
        private static readonly Dictionary<string, int> PageFrameValue = new Dictionary<string, int>();
        private static readonly Dictionary<string, int> PageBeachFrameValue = new Dictionary<string, int>();

        internal static void CreateConfigMenu()
        {
            _i18n = DataLoader.I18N;
            IManifest manifest = CustomKissingModEntry.Manifest;
            if (GetApi() is not { } api) return;
            api.Register(manifest, () => { DataLoader.ModConfig = new ModConfig(); DataLoader.LoadContentPacks();}, () => DataLoader.Helper.WriteConfig(DataLoader.ModConfig));

            api.AddSectionTitle(manifest, GetText("RequirementsSection"), GetTooltip("RequirementsSection"));
            api.AddBoolOption(manifest, () => !DataLoader.ModConfig.DisableDatingRequirement, (bool val) => DataLoader.ModConfig.DisableDatingRequirement = !val, GetText("DisableDatingRequirement"), GetTooltip("DisableDatingRequirement"));
            api.AddBoolOption(manifest, () => !DataLoader.ModConfig.DisableEventRequirement, (bool val) => DataLoader.ModConfig.DisableEventRequirement = !val, GetText("DisableEventRequirement"), GetTooltip("DisableEventRequirement"));
            api.AddNumberOption(manifest, () => DataLoader.ModConfig.RequiredFriendshipLevel,
                (int val) => DataLoader.ModConfig.RequiredFriendshipLevel = val, GetText("RequiredFriendshipLevel"), GetTooltip("RequiredFriendshipLevel"));

            api.AddSectionTitle(manifest, GetText("JealousySection"), GetTooltip("JealousySection"));
            api.AddBoolOption(manifest, () => !DataLoader.ModConfig.DisableJealousy, (bool val) => DataLoader.ModConfig.DisableJealousy = !val, GetText("DisableJealousy"), GetTooltip("DisableJealousy"));
            api.AddNumberOption(manifest, () => DataLoader.ModConfig.JealousyFriendshipPoints, (int val) => DataLoader.ModConfig.JealousyFriendshipPoints = val, GetText("JealousyFriendshipPoints"), GetTooltip("JealousyFriendshipPoints"));

            api.AddSectionTitle(manifest, GetText("BenefitsSection"), GetTooltip("BenefitsSection"));
            api.AddBoolOption(manifest, () => !DataLoader.ModConfig.DisableExhaustionReset, (bool val) => DataLoader.ModConfig.DisableExhaustionReset = !val, GetText("DisableExhaustionReset"), GetTooltip("DisableExhaustionReset"));
            api.AddNumberOption(manifest, () => DataLoader.ModConfig.KissingFriendshipPoints, (int val) => DataLoader.ModConfig.KissingFriendshipPoints = val, GetText("KissingFriendshipPoints"), GetTooltip("KissingFriendshipPoints"));


            api.AddSectionTitle(manifest, GetText("GeneralSection"), GetTooltip("GeneralSection"));
            api.AddBoolOption(manifest, () => DataLoader.ModConfig.EnableContentPacksOverrides, (bool val) => DataLoader.ModConfig.EnableContentPacksOverrides = val, GetText("EnableContentPacksOverrides"), GetTooltip("EnableContentPacksOverrides"));
            api.AddBoolOption(manifest, () => !DataLoader.ModConfig.DisableCursorKissingIndication, (bool val) => DataLoader.ModConfig.DisableCursorKissingIndication = !val, GetText("DisableCursorKissingIndication"), GetTooltip("DisableCursorKissingIndication"));

            if (DataLoader.ModConfig.NpcConfigs.Count > 0)
            {
                api.AddSectionTitle(manifest, GetText("NpcListSection"), GetTooltip("NpcListSection"));

                foreach (var npcConfig in DataLoader.ModConfig.NpcConfigs)
                {
                    api.AddPageLink(manifest, $"Npc{npcConfig.Name}", () => npcConfig.Name, () => NPC.GetDisplayName(npcConfig.Name));
                }

                foreach (var npcConfig in DataLoader.ModConfig.NpcConfigs)
                {
                    CreateNpcPage(npcConfig);
                }
            }

            api.AddPage(manifest, "", null);
            api.AddSectionTitle(manifest, GetText("NpcOperationsSection"), GetTooltip("NpcOperationsSection"));
            api.AddComplexOption(manifest, () => "", drawMessage, height: () => 0);
            api.AddTextOption(manifest, () => _npcNameOperations, (string val) => _npcNameOperations = val, GetText("NpcOperations.NameNpc"), GetTooltip("NpcOperations.NameNpc"), fieldId: NpcNameFieldId);
            api.AddBoolOption(manifest, () => false, (bool val) => { }, GetText("NpcOperations.AddNpc"), GetTooltip("NpcOperations.AddNpc"), fieldId: NewNpcConfirmationFieldId);
            api.AddBoolOption(manifest, () => false, (bool val) => { }, GetText("NpcOperations.RemoveNpc"), GetTooltip("NpcOperations.RemoveNpc"), fieldId: DeleteNpcConfirmationFieldId);

            api.OnFieldChanged(manifest, (string id, object value) =>
            {
                if (id.EndsWith(":Frame"))
                {
                    if (!api.TryGetCurrentMenu(out var mod, out var page)) return;
                    if (mod == CustomKissingModEntry.Manifest)
                    {
                        PageFrameValue[page] = (int)value;
                    }
                }
                else if (id.EndsWith(":BeachFrame"))
                {
                    if (!api.TryGetCurrentMenu(out var mod, out var page)) return;
                    if (mod == CustomKissingModEntry.Manifest)
                    {
                        PageBeachFrameValue[page] = (int)value;
                    }
                }
                else if (id == NpcNameFieldId)
                {
                    _npcNameOperations = value.ToString();
                }
                else if (id == NewNpcConfirmationFieldId)
                {
                    if (ValidateNameEmpty()) return;
                    if (ValidateNameConfigExists()) return;
                    if (ValidateNameNpcExists()) return;
                    Game1.characterData.TryGetValue(_npcNameOperations, out CharacterData npcData);
                    DataLoader.ModConfig.NpcConfigs.Add(new NpcConfig
                    {
                        Name = _npcNameOperations,
                        Frame = npcData?.KissSpriteIndex ?? 0,
                        FrameDirectionRight = npcData?.KissSpriteFacingRight ?? false,
                        BeachFrame = 12
                    });

                    ResetConfigMenu();
                }
                else if (id == DeleteNpcConfirmationFieldId)
                {
                    if (ValidateNameEmpty()) return;
                    if (ValidateNameNotExists()) return;

                    DataLoader.ModConfig.NpcConfigs.RemoveAll(n => n.Name == _npcNameOperations);

                    ResetConfigMenu();
                }
            });
        }

        private static bool ValidateName(Func<bool> validation, string message)
        {
            if (validation.Invoke()) return false;
            _npcOperationMessage = GetMessage($"NpcOperations.{message}");
            return true;
        }

        private static bool ValidateNameNotExists()
        {
            return ValidateName(()=>DataLoader.ModConfig.NpcConfigs.Any(n => n.Name.Equals(_npcNameOperations)),"NotExistsList");
        }

        private static bool ValidateNameNpcExists()
        {
            return ValidateName(()=>Game1.characterData.TryGetValue(_npcNameOperations, out CharacterData npcData),"NotExistsGame");
        }

        private static bool ValidateNameConfigExists()
        {
            return ValidateName(()=>!DataLoader.ModConfig.NpcConfigs.Any(n => n.Name.Equals(_npcNameOperations)),"ExistsList");
        }

        private static bool ValidateNameEmpty()
        {
            return ValidateName(()=>!string.IsNullOrWhiteSpace(_npcNameOperations),"Empty");
        }

        private static void ResetConfigMenu()
        {
            _npcNameOperations = "";
            _npcOperationMessage = "";
            DeleteConfigMenu();
            CreateConfigMenu();
            GetApi()?.OpenModMenu(CustomKissingModEntry.Manifest);
        }

        private static Func<string> GetText(string id)
        {
            return () => _i18n.Get($"CustomKissingMod.ConfigMenu.{id}.Text");
        }

        private static void CreateNpcPage(NpcConfig npcConfig)
        {
            IManifest manifest = CustomKissingModEntry.Manifest;
            var name = npcConfig.Name;
            var pageId = $"Npc{name}";
            var pageFrame = $"Npc{name}:Frame";
            var pageBeachFrame = $"Npc{name}:BeachFrame";
            var max = 100;
            var maxBeach = 100;
            bool hasBeachAttire = false;

            if (Game1.characterData.TryGetValue(name, out CharacterData npcData))
            {
                var texture = DataLoader.Helper.GameContent.Load<Texture2D>($"Characters/{npcData.TextureName ?? name}");
                max = (texture.ActualHeight / 32) * 4;
                
                if (DataLoader.Helper.GameContent.DoesAssetExist<Texture2D>(DataLoader.Helper.GameContent.ParseAssetName($"Characters/{npcData.TextureName ?? name}_Beach")))
                {
                    var textureBeach = DataLoader.Helper.GameContent.Load<Texture2D>($"Characters/{npcData.TextureName ?? name}_Beach");
                    hasBeachAttire = true;
                    maxBeach = (textureBeach.ActualHeight / 32) * 4;
                }

            }

            if (GetApi() is not { } api) return;
            api.AddPage(manifest, pageId, () => name);
            api.AddNumberOption(manifest, () => npcConfig.Frame, (int val) => npcConfig.Frame = val,  GetText("NpcConfig.Frame"), GetTooltip("NpcConfig.Frame"), fieldId: pageFrame, min: 0, max: max);
            PageFrameValue[pageId] = npcConfig.Frame;
            if (hasBeachAttire)
            {
                api.AddNumberOption(manifest, () => npcConfig.BeachFrame, (int val) => npcConfig.BeachFrame = val,  GetText("NpcConfig.BeachFrame"), GetTooltip("NpcConfig.BeachFrame"), fieldId: pageBeachFrame, min: 0, max: maxBeach);
                PageBeachFrameValue[pageId] = npcConfig.BeachFrame;
            }
            api.AddComplexOption(manifest, () => "", drawFrame, height: () => 0);
            if (hasBeachAttire) api.AddComplexOption(manifest, () => "", drawBeachFrame, height: () => 0);
            api.AddBoolOption(manifest, () => npcConfig.FrameDirectionRight, (bool val) => npcConfig.FrameDirectionRight = val, GetText("NpcConfig.FrameDirectionRight"), GetTooltip("NpcConfig.FrameDirectionRight"));
            if (hasBeachAttire) api.AddBoolOption(manifest, () => npcConfig.BeachFrameDirectionRight, (bool val) => npcConfig.BeachFrameDirectionRight = val, GetText("NpcConfig.BeachFrameDirectionRight"), GetTooltip("NpcConfig.BeachFrameDirectionRight"));
            
            api.AddTextOption(manifest, () => npcConfig.RequiredEvent ?? "", (string val) => npcConfig.RequiredEvent = String.IsNullOrWhiteSpace(val) ? null : val, GetText("NpcConfig.RequiredEvent"), GetTooltip("NpcConfig.RequiredEvent"));
            api.AddPageLink(manifest, "", GetText("NpcConfig.Back"), GetTooltip("NpcConfig.Back"));
        }

        private static Func<string> GetTooltip(string id)
        {
            return () => _i18n.Get($"CustomKissingMod.ConfigMenu.{id}.Tooltip");
        }

        private static string GetMessage(string id)
        {
            return _i18n.Get($"CustomKissingMod.ConfigMenu.{id}.Message");
        }

        internal static void DeleteConfigMenu()
        {
            GetApi()?.Unregister(CustomKissingModEntry.Manifest);
        }

        private static GenericModConfigMenuApi GetApi()
        {
            return _api ??= DataLoader.Helper.ModRegistry.GetApi<GenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
        }

        public static void drawFrame(SpriteBatch sb, Vector2 v)
        {
            if (GetApi() is not { } api) return;
            if (!api.TryGetCurrentMenu(out var mod, out var page)) return;
            if (mod != CustomKissingModEntry.Manifest) return;
            var name = page.Split("Npc")[1];
            if (!Game1.characterData.TryGetValue(name, out CharacterData npcData)) return;
            var texture = DataLoader.Helper.GameContent.Load<Texture2D>($"Characters/{npcData.TextureName ?? name}");
            Rectangle dest = new((int)v.X + 250, (int)v.Y, 64, 128);
            sb.Draw(
                texture,
                color: Color.White * 1f,
                sourceRectangle: AnimatedSprite.GetSourceRect(texture.Width, 16, 32, PageFrameValue[page]),
                destinationRectangle: dest
            );
        }
        
        public static void drawBeachFrame(SpriteBatch sb, Vector2 v)
        {
            if (GetApi() is not { } api) return;
            if (!api.TryGetCurrentMenu(out var mod, out var page)) return;
            if (mod != CustomKissingModEntry.Manifest) return;
            var name = page.Split("Npc")[1];
            if (!Game1.characterData.TryGetValue(name, out CharacterData npcData)) return;
            var texture = DataLoader.Helper.GameContent.Load<Texture2D>($"Characters/{npcData.TextureName ?? name}_Beach");
            Rectangle dest = new((int)v.X + 350, (int)v.Y - 16, 64, 128);
            sb.Draw(
                texture,
                color: Color.White * 1f,
                sourceRectangle: AnimatedSprite.GetSourceRect(texture.Width, 16, 32, PageBeachFrameValue[page]),
                destinationRectangle: dest
            );
        }

        public static void drawMessage(SpriteBatch sb, Vector2 v)
        {
            Utility.drawTextWithShadow(
                sb,
                _npcOperationMessage,
                Game1.dialogueFont, v + new Vector2(0,-Game1.dialogueFont.MeasureString(_npcOperationMessage).Y),
                Color.DarkRed, 1.0f
            );
        }
    }
}
