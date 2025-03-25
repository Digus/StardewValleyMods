using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using MailFrameworkMod.ContentPack;
using MailFrameworkMod.integrations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Extensions;
using StardewValley.GameData.Objects;
using StardewValley.Objects;
using StardewValley.Tools;
using Attachment = MailFrameworkMod.ContentPack.Attachment;
using MailDataAssetType = System.Collections.Generic.Dictionary<string, MailFrameworkMod.ContentPack.MailItem>;

namespace MailFrameworkMod
{
    public partial class DataLoader
    {
        internal const string MailDataAssetName = "DIGUS.MailFrameworkMod/MailData";
        internal const string MailAssetName = "Data/mail";
        internal const string ValidTagSuffix = "::Valid";

        private static readonly Dictionary<Tuple<string,string>, Texture2D> _contentPackAssets = new Dictionary<Tuple<string, string>, Texture2D>();
        private static readonly HashSet<IContentPack> RegisteredContentPacks = new HashSet<IContentPack>(new ContentPackComparer());
        private static MailDataAssetType MailData = new MailDataAssetType();

        private static bool _wasMailDataInvalidate;

        private static readonly List<string> NoUpgradeLevelTools = new List<string>() {"Scythe", "Shears", "Milk Pail", "Fishing Rod", "Golden Scythe", "Pan", "Return Scepter" };
        private static readonly List<string> SlingshotIndexes = new List<string>() {"32", "33", "34", "(W)32", "(W)33", "(W)34" };

        public static IModHelper Helper;
        public static IDynamicGameAssetsApi DgaApi;
        public static IConditionsChecker ConditionsCheckerApi;

        public DataLoader(IModHelper helper)
        {
            Helper = helper;
            Helper.Events.Content.AssetRequested += this.AssetRequested;
            Helper.Events.Content.AssetReady += this.AssetReady;
            Helper.Events.Content.AssetsInvalidated += this.AssetInvalidated;

            DgaApi = MailFrameworkModEntry.ModHelper.ModRegistry.GetApi<IDynamicGameAssetsApi>("spacechase0.DynamicGameAssets");
            ConditionsCheckerApi = Helper.ModRegistry.GetApi<IConditionsChecker>("Cherry.ExpandedPreconditionsUtility");
            ConditionsCheckerApi?.Initialize(false, MailFrameworkModEntry.Manifest.UniqueID);
            LoadContentPacks();
        }

        public void AssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo(MailAssetName))
            {
                e.Edit(asset =>
                {
                    var data = asset.AsDictionary<string, string>().Data;
                    foreach (Letter letter in MailRepository.GetSavedLetters())
                    {
                        if (letter.Title != null && !letter.AutoOpen)
                        {
                            data[letter.Id] = letter.ToMailDataString();
                        }
                    }

                    foreach (string letterId in MailRepository.GetRemovedLetterIds())
                    {
                        if (data.ContainsKey(letterId))
                        {
                            data.Remove(letterId);
                        }
                    }

                    MailRepository.CleanDataToUpdate();
                });
            } if (e.NameWithoutLocale.IsEquivalentTo(MailDataAssetName)) {
                MailFrameworkModEntry.ModMonitor.Log($"Asset {MailDataAssetName} requested.");
                e.LoadFrom(() => MailData, AssetLoadPriority.Exclusive);
            }
        }

        public void AssetReady(object sender, AssetReadyEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo(MailDataAssetName))
            {
                MailFrameworkModEntry.ModMonitor.Log($"Asset {MailDataAssetName} ready.");
                LoadMailDataToLetterRepository(null, null);
            }
        }

        public void AssetInvalidated(object sender, AssetsInvalidatedEventArgs e) {
            foreach (var name in e.NamesWithoutLocale) {
                if (name.IsEquivalentTo(MailDataAssetName)) {
                    MailFrameworkModEntry.ModMonitor.Log($"Asset {MailDataAssetName} invalidated.");
                    MailData.RemoveWhere(i => i.Value.ContentPack == null);
                    _wasMailDataInvalidate = true;
                }
            }
        }

        public static void ReloadAssets()
        {
            Game1.content.Load<MailDataAssetType>(MailDataAssetName);
            _wasMailDataInvalidate = false;
        }

        public static void LoadContentPacks()
        {
            foreach (IContentPack contentPack in MailFrameworkModEntry.ModHelper.ContentPacks.GetOwned())
            {
                LoadContentPack(contentPack);
            }
            foreach (IContentPack contentPack in RegisteredContentPacks)
            {
                LoadContentPack(contentPack);
            }
            Helper.GameContent.InvalidateCache(MailDataAssetName);
        }

        public static bool WasMailDataInvalidated()
        {
            return _wasMailDataInvalidate;
        }

        public static void LoadMailDataToLetterRepository(object sender, EventArgs e)
        {
            MailData = Game1.content.Load<MailDataAssetType>(MailDataAssetName);
            LoadMailData(MailData.Values.ToList());
        }

        public static void LoadContentPack(IContentPack contentPack)
        {
            if (File.Exists(Path.Combine(contentPack.DirectoryPath, "mail.json")))
            {
                MailFrameworkModEntry.ModMonitor.Log(
                    $"Reading content pack: {contentPack.Manifest.Name} {contentPack.Manifest.Version} from {contentPack.DirectoryPath}");
                List<MailItem> mailItems = contentPack.ReadJsonFile<List<MailItem>>("mail.json");
                MailData.RemoveWhere(i => i.Value.ContentPack?.Manifest.UniqueID == contentPack.Manifest.UniqueID);
                mailItems?.ForEach(m =>
                {
                    m.ContentPack = contentPack;
                    MailData[m.Id] = m;
                });
            }
            else
            {
                MailFrameworkModEntry.ModMonitor.Log(
                    $"Ignoring content pack: {contentPack.Manifest.Name} {contentPack.Manifest.Version} from {contentPack.DirectoryPath}\nIt does not have an mail.json file.",
                    LogLevel.Warn);
            }
        }

        private static void LoadMailData(List<MailItem> mailItems)
        {
            foreach (MailItem mailItem in mailItems)
            {
                var contentPack = mailItem.ContentPack;
                bool hasTranslation = contentPack?.Translation.GetTranslations().Any()??false;
                //Populate all Indexes based on the given name. Ignore the letter otherwise.
                if (mailItem.CollectionConditions != null && mailItem.CollectionConditions.Any(c =>
                    {
                        c.Ids ??= new HashSet<string>();
                        if (c.Index != null) c.Ids.Add(c.Index);
                        if (c.Name != null)
                        {
                            if (c.Collection != Collection.Crafting)
                            {
                                KeyValuePair<string, ObjectData> pair = Game1.objectData.FirstOrDefault(o => c.Name.Equals(o.Value.Name));
                                if (pair.Value != null)
                                {
                                    c.Ids.Add(pair.Key);
                                }
                                else
                                {
                                    MailFrameworkModEntry.ModMonitor.Log(
                                        $"No object found with the name '{c.Name}' for a condition for letter '{mailItem.Id}'.\n This letter will be ignored.",
                                        LogLevel.Warn);
                                    MailRepository.RemoveLetter(new Letter(mailItem.Id, null, null));
                                    return true;
                                }
                            }
                            else
                            {
                                c.Ids.Add(c.Name);
                            }
                        }

                        return false;
                    })) continue;

                bool Condition(Letter l) =>
                    ( mailItem.KeepValid && Game1.player.mailReceived.Contains(l.Id + ValidTagSuffix))
                    || (
                        (!Game1.player.mailReceived.Contains(l.Id) || mailItem.Repeatable || (mailItem.Recipe != null && mailItem.Attachments?.Count == 0))
                        && (mailItem.Recipe == null || !(Game1.player.cookingRecipes.ContainsKey(mailItem.Recipe) 
                                                         || Game1.player.craftingRecipes.ContainsKey(mailItem.Recipe)
                                                         || Game1.player.cookingRecipes.ContainsKey(CraftingRecipe.cookingRecipes.Where(r => ItemRegistry.GetData(r.Value.Split("/")[2].Split(" ")[0])?.InternalName == mailItem.Recipe).Select(r=>r.Key).FirstOrDefault()??"")
                                                         || Game1.player.craftingRecipes.ContainsKey(CraftingRecipe.craftingRecipes.Where(r => ItemRegistry.GetData(r.Value.Split("/")[2].Split(" ")[0])?.InternalName == mailItem.Recipe).Select(r => r.Key).FirstOrDefault() ?? "")))
                        && (mailItem.Date == null || SDate.Now() >= new SDate(Convert.ToInt32(mailItem.Date.Split(' ')[0]),
                            mailItem.Date.Split(' ')[1], Convert.ToInt32(mailItem.Date.Split(' ')[2].Replace("Y", ""))))
                        && (mailItem.Days == null || mailItem.Days.Contains(SDate.Now().Day))
                        && (mailItem.Seasons == null || mailItem.Seasons.Exists(s=> string.Equals(s,SDate.Now().Season.ToString(),StringComparison.OrdinalIgnoreCase)))
                        && (mailItem.Weather == null || (Game1.isRaining && "rainy".Equals(mailItem.Weather)) ||
                            (!Game1.isRaining && "sunny".Equals(mailItem.Weather)))
                        && (mailItem.FriendshipConditions == null || (mailItem.FriendshipConditions.TrueForAll(f =>
                                Game1.player.getFriendshipHeartLevelForNPC(f.NpcName) >= f.FriendshipLevel))
                            && mailItem.FriendshipConditions.TrueForAll(f =>
                                f.FriendshipStatus == null || (Game1.player.friendshipData.ContainsKey(f.NpcName) &&
                                                               f.FriendshipStatus.Any(s =>
                                                                   s == Game1.player.friendshipData[f.NpcName].Status))))
                        && (mailItem.SkillConditions == null || mailItem.SkillConditions.TrueForAll(s =>
                            Game1.player.getEffectiveSkillLevel((int) s.SkillName) >= s.SkillLevel))
                        && (mailItem.StatsConditions == null ||
                            (mailItem.StatsConditions.TrueForAll(s =>
                                 s.StatsLabel == null || Game1.player.stats.Get(s.StatsLabel) >= s.Amount) &&
                             mailItem.StatsConditions.TrueForAll(s =>
                                 s.StatsName == null ||
                                 MailFrameworkModEntry.ModHelper.Reflection
                                     .GetProperty<uint>(Game1.player.stats, s.StatsName.ToString()).GetValue() >= s.Amount)))
                        && (mailItem.CollectionConditions == null || (mailItem.CollectionConditions.TrueForAll(c =>
                            (c.Collection == Collection.Shipped && c.Ids.Sum(i => Game1.player.basicShipped.ContainsKey(i) ? Game1.player.basicShipped[i] : 0)   >= c.Amount)
                            || (c.Collection == Collection.Fish && c.Ids.Sum(i => Game1.player.fishCaught.ContainsKey(i) ? Game1.player.fishCaught[i][0] : 0) >= c.Amount)
                            || (c.Collection == Collection.Artifacts && c.Ids.Sum(i => Game1.player.archaeologyFound.ContainsKey(i) ? Game1.player.archaeologyFound[i][0] : 0) >= c.Amount)
                            || (c.Collection == Collection.Minerals && c.Ids.Sum(i => Game1.player.mineralsFound.ContainsKey(i) ? Game1.player.mineralsFound[i] : 0) >= c.Amount)
                            || (c.Collection == Collection.Cooking && c.Ids.Sum(i => Game1.player.recipesCooked.ContainsKey(i) ? Game1.player.recipesCooked[i] : 0) >= c.Amount)
                            || (c.Collection == Collection.Crafting && c.Ids.Sum(i => Game1.player.craftingRecipes.ContainsKey(i) ? Game1.player.craftingRecipes[i] : 0) >= c.Amount)
                        )))
                        && (mailItem.RandomChance == null ||
                            new Random((int) (((ulong) Game1.stats.DaysPlayed * 1000000000000000) +
                                              (((ulong) l.Id.GetHashCode()) % 1000000000 * 1000000) +
                                              Game1.uniqueIDForThisGame % 1000000)).NextDouble() < mailItem.RandomChance)
                        && (mailItem.Buildings == null || (mailItem.RequireAllBuildings
                            ? mailItem.Buildings.TrueForAll(b => Game1.getFarm().isBuildingConstructed(b))
                            : mailItem.Buildings.Any(b => Game1.getFarm().isBuildingConstructed(b))))
                        && (mailItem.MailReceived == null || (mailItem.RequireAllMailReceived
                            ? !mailItem.MailReceived.Except(Game1.player.mailReceived).Any()
                            : mailItem.MailReceived.Intersect(Game1.player.mailReceived).Any()))
                        && (mailItem.MailNotReceived == null ||
                            !mailItem.MailNotReceived.Intersect(Game1.player.mailReceived).Any())
                        && (mailItem.EventsSeen == null || (mailItem.RequireAllEventsSeen
                            ? !mailItem.EventsSeen.Except(Game1.player.eventsSeen).Any()
                            : mailItem.EventsSeen.Intersect(Game1.player.eventsSeen).Any()))
                        && (mailItem.EventsNotSeen == null || !mailItem.EventsNotSeen.Intersect(Game1.player.eventsSeen).Any())
                        && (mailItem.RecipeKnown == null || (mailItem.RequireAllRecipeKnown
                            ? mailItem.RecipeKnown.All(r => Game1.player.knowsRecipe(r))
                            : mailItem.RecipeKnown.Any(r => Game1.player.knowsRecipe(r))))
                        && (mailItem.RecipeNotKnown == null || mailItem.RecipeNotKnown.All(r => !Game1.player.knowsRecipe(r)))
                        && (mailItem.HasMods == null || (mailItem.RequireAllMods
                            ? mailItem.HasMods.All(r => Helper.ModRegistry.IsLoaded(r))
                            : mailItem.HasMods.Any(r => Helper.ModRegistry.IsLoaded(r))))
                        && (mailItem.ExpandedPrecondition == null ||
                            (ConditionsCheckerApi != null &&
                             ConditionsCheckerApi.CheckConditions(mailItem.ExpandedPrecondition)))
                        && (mailItem.ExpandedPreconditions == null ||
                            (ConditionsCheckerApi != null &&
                             ConditionsCheckerApi.CheckConditions(mailItem.ExpandedPreconditions)))
                        && (mailItem.HouseUpgradeLevel == null || mailItem.HouseUpgradeLevel <= Game1.player.HouseUpgradeLevel)
                        && (mailItem.DeepestMineLevel == null || mailItem.DeepestMineLevel <= Game1.player.deepestMineLevel)
                        && (mailItem.CurrentMoney == null || mailItem.CurrentMoney <= Game1.player.Money)
                        && (mailItem.TotalMoneyEarned == null || mailItem.TotalMoneyEarned <= Game1.player.totalMoneyEarned)
                        && (mailItem.SpecialDateCondition == null 
                            || (mailItem.SpecialDateCondition.SpecialDate == SpecialDate.Wedding 
                                && Game1.player.GetSpouseFriendship() != null 
                                && SDate.Now() >= SDate.FromDaysSinceStart(1 + Game1.player.GetSpouseFriendship().WeddingDate.TotalDays + mailItem.SpecialDateCondition.YearsSince * WorldDate.MonthsPerYear * WorldDate.DaysPerMonth)) 
                            || (mailItem.SpecialDateCondition.SpecialDate == SpecialDate.ChildBirth 
                                && Game1.player.getChildrenCount() >= mailItem.SpecialDateCondition.WhichChild 
                                && GetChild(mailItem.SpecialDateCondition.WhichChild).daysOld.Value >= mailItem.SpecialDateCondition.YearsSince * WorldDate.MonthsPerYear * WorldDate.DaysPerMonth))
                    )
                ;

                if (mailItem.CustomTextColorName != null)
                {
                    try
                    {
                        mailItem.CustomTextColor = DataLoader.Helper.Reflection.GetProperty<Color>(typeof(Color), mailItem.CustomTextColorName).GetValue();
                    }
                    catch (Exception)
                    {
                        MailFrameworkModEntry.ModMonitor.Log($"Color '{mailItem.CustomTextColorName}' isn't valid. Check XNA Color Chart for valid names. This color will be ignored.");
                    }
                }

                var contentPackTranslation = hasTranslation ? contentPack.Translation : null;
                Action<Letter> callback = (l) =>
                {
                    Game1.player.mailReceived.Add(l.Id);
                    if (mailItem.AdditionalMailReceived != null) Game1.player.mailReceived.AddRange(mailItem.AdditionalMailReceived);
                    if (mailItem.MailReceivedToRemove != null) Game1.player.mailReceived.RemoveWhere(s => mailItem.MailReceivedToRemove?.Contains(s) ?? false);
                    if (mailItem.ReplyConfig != null) ReplyController.OpenReplyDialog(mailItem.ReplyConfig, contentPackTranslation);
                };

                if (mailItem.Attachments != null && mailItem.Attachments.Count > 0)
                {
                    if (mailItem.RandomlyChooseAttachment != null)
                    {
                        MailRepository.SaveLetter(
                            new Letter(
                                mailItem.Id
                                , mailItem.Text
                                , Condition
                                , callback
                                , mailItem.WhichBG
                            )
                            {
                                TextColor = mailItem.TextColor,
                                CustomTextColor = mailItem.CustomTextColor,
                                Title = mailItem.Title,
                                GroupId = mailItem.GroupId,
                                LetterTexture = mailItem.LetterBG != null
                                    ? GetTextureAsset(contentPack, mailItem.LetterBG)
                                    : null,
                                UpperRightCloseButtonTexture = mailItem.UpperRightCloseButton != null
                                    ? GetTextureAsset(contentPack, mailItem.UpperRightCloseButton)
                                    : null,
                                AutoOpen = mailItem.AutoOpen,
                                I18N = contentPackTranslation,
                                DynamicItems = l =>
                                {
                                    var dynamicItems = new List<Item>();
                                    var random = new Random(SDate.Now().DaysSinceStart + mailItem.Id.GetHashCode() + (int)Game1.uniqueIDForThisGame);

                                    var filteredAttachments = mailItem.Attachments.FindAll(a =>
                                        a.RequireMailReceived == null || (a.RequireAllMailReceived
                                            ? !a.RequireMailReceived.Except(Game1.player.mailReceived).Any()
                                            : a.RequireMailReceived.Intersect(Game1.player.mailReceived).Any()));
                                    foreach (var groupAttachments in filteredAttachments.GroupBy(a => a.RandomGroup).Select(g => new Tuple<string,List<Attachment>>(g.Key,g.ToList())))
                                    {
                                        int? amount = null;
                                        var groupName = groupAttachments.Item1;
                                        if (mailItem.RandomlyChooseAttachmentPerGroup.TryGetValue(groupName, out int value)) amount = value;
                                        var attachments = groupAttachments.Item2;
                                        for (var i = 1; i <= (amount ?? mailItem.RandomlyChooseAttachment) ; i++) if (attachments.Count > 0) if (GetNextItem(mailItem, ref attachments,mailItem.AttachmentGroupWithReplacement.Contains(groupName), random) is {} item) dynamicItems.Add(item);
                                    }
                                    return dynamicItems;
                                }
                            });
                    }
                    else
                    {
                        var attachments = GetAttachments(mailItem);
                        MailRepository.SaveLetter(
                            new Letter(
                                mailItem.Id
                                , mailItem.Text
                                , attachments
                                , Condition
                                , callback
                                , mailItem.WhichBG
                            )
                            {
                                TextColor = mailItem.TextColor,
                                CustomTextColor = mailItem.CustomTextColor,
                                Title = mailItem.Title,
                                GroupId = mailItem.GroupId,
                                LetterTexture = mailItem.LetterBG != null
                                    ? GetTextureAsset(contentPack, mailItem.LetterBG)
                                    : null,
                                UpperRightCloseButtonTexture = mailItem.UpperRightCloseButton != null
                                    ? GetTextureAsset(contentPack, mailItem.UpperRightCloseButton)
                                    : null,
                                AutoOpen = mailItem.AutoOpen,
                                I18N = contentPackTranslation
                            });
                    }
                }
                else
                {
                    MailRepository.SaveLetter(
                        new Letter(
                            mailItem.Id
                            , mailItem.Text
                            , mailItem.Recipe
                            , Condition
                            , callback
                            , mailItem.WhichBG
                        )
                        {
                            TextColor = mailItem.TextColor,
                            CustomTextColor = mailItem.CustomTextColor,
                            Title = mailItem.Title,
                            GroupId = mailItem.GroupId,
                            LetterTexture = mailItem.LetterBG != null
                                ? GetTextureAsset(contentPack, mailItem.LetterBG)
                                : null,
                            UpperRightCloseButtonTexture = mailItem.UpperRightCloseButton != null
                                ? GetTextureAsset(contentPack, mailItem.UpperRightCloseButton)
                                : null,
                            AutoOpen = mailItem.AutoOpen,
                            I18N = contentPackTranslation
                        });
                }
            }
        }

        private static Item GetNextItem(MailItem mailItem, ref List<Attachment> attachments, bool withReplacement, Random random)
        {
            Double chance = random.Next(attachments.Sum(a=> a.ProbabilityWeight));
            Double probabilities = 0;
            Item result = null;
            Attachment chosenAttachment = null;
            foreach (Attachment attachment in attachments)
            {
                probabilities += (double) attachment.ProbabilityWeight;
                if (chance - probabilities < 0)
                {
                    chosenAttachment = attachment;
                    result = CreateItemFromAttachment(attachment,mailItem);
                    break;
                }
            }
            if (!withReplacement) attachments = attachments.FindAll(a => !a.Equals(chosenAttachment));
            return result;
        }

        private static List<Item> GetAttachments(MailItem mailItem)
        {
            var attachments = new List<Item>();
            var filteredAttachments = mailItem.Attachments.FindAll(a =>
                a.RequireMailReceived == null || (a.RequireAllMailReceived
                    ? !a.RequireMailReceived.Except(Game1.player.mailReceived).Any()
                    : a.RequireMailReceived.Intersect(Game1.player.mailReceived).Any()));

            filteredAttachments.ForEach(i => {if (CreateItemFromAttachment(i, mailItem) is { } item) attachments.Add(item);});
            return attachments;
        }

        internal static Item CreateItemFromAttachment(Attachment i, MailItem mailItem)
            {
                if (i == null) return null;
                switch (i.Type)
                {
                    case ItemType.Object:
                        if (i.Name != null)
                        {
                            var pair = Game1.objectData.FirstOrDefault(o => i.Name.Equals(o.Value.Name));
                            if (pair.Value != null)
                            {
                                i.Index = pair.Key;
                            }
                            else
                            {
                                MailFrameworkModEntry.ModMonitor.Log($"No object found with the name {i.Name} for letter {mailItem.Id}.", LogLevel.Warn);
                            }
                        }

                        if (i.Index != null)
                        {
                            return new StardewValley.Object(i.Index, i.Stack ?? 1, quality: i.Quality);
                        }
                        else
                        {
                            MailFrameworkModEntry.ModMonitor.Log($"An index value is required to attach an object for letter {mailItem.Id}.", LogLevel.Warn);
                        }

                        break;
                    case ItemType.BigObject:
                    case ItemType.BigCraftable:
                        if (i.Name != null)
                        {
                            var pair = Game1.bigCraftableData.FirstOrDefault(o => i.Name.Equals(o.Value.Name));
                            if (pair.Value != null)
                            {
                                i.Index = pair.Key;
                            }
                            else
                            {
                                MailFrameworkModEntry.ModMonitor.Log($"No big craftable found with the name {i.Name} for letter {mailItem.Id}.", LogLevel.Warn);
                            }
                        }

                        if (i.Index != null)
                        {
                            Item item = new StardewValley.Object(Vector2.Zero, i.Index);
                            if (i.Stack.HasValue)
                            {
                                item.Stack = i.Stack.Value;
                            }

                            return item;
                        }
                        else
                        {
                            MailFrameworkModEntry.ModMonitor.Log($"An index value is required to attach a big craftable for letter {mailItem.Id}.", LogLevel.Warn);
                        }

                        break;
                    case ItemType.Tool:
                        Tool tool = null;
                        switch (i.Name)
                        {
                            case "Axe":
                                tool = new Axe();
                                break;
                            case "Hoe":
                                tool = new Hoe();
                                break;
                            case "Watering Can":
                                tool = new WateringCan();
                                break;
                            case "Scythe":
                                tool = new MeleeWeapon("47");
                                break;
                            case "Golden Scythe":
                                tool = new MeleeWeapon("53");
                                break;
                            case "Pickaxe":
                                tool = new Pickaxe();
                                break;
                            case "Milk Pail":
                                tool = new MilkPail();
                                break;
                            case "Shears":
                                tool = new Shears();
                                break;
                            case "Fishing Rod":
                                tool = new FishingRod(i.UpgradeLevel ?? 0);
                                break;
                            case "Pan":
                                tool = new Pan();
                                break;
                            case "Return Scepter":
                                tool = new Wand();
                                break;
                            default:
                                MailFrameworkModEntry.ModMonitor.Log($"Tool with name {i.Name} not found for letter {mailItem.Id}.", LogLevel.Warn);
                                break;
                        }

                        if (tool != null)
                        {
                            if (!NoUpgradeLevelTools.Contains(i.Name))
                            {
                                tool.UpgradeLevel = i.UpgradeLevel ?? 0;
                            }

                            return tool;
                        }

                        break;
                    case ItemType.Ring:
                        if (i.Name != null)
                        {
                            KeyValuePair<string, ObjectData> pair = Game1.objectData.FirstOrDefault(o => i.Name.Equals(o.Value.Name));
                            if (pair.Value != null)
                            {
                                i.Index = pair.Key;
                            }
                            else
                            {
                                MailFrameworkModEntry.ModMonitor.Log($"No ring found with the name {i.Name} for letter {mailItem.Id}.", LogLevel.Warn);
                            }
                        }

                        if (i.Index != null)
                        {
                            if (Game1.objectData[i.Index].Type.Equals("Ring"))
                            {
                                return new Ring(i.Index);
                            }
                            else
                            {
                                MailFrameworkModEntry.ModMonitor.Log($"A valid ring is required to attach an ring for letter {mailItem.Id}.", LogLevel.Warn);
                            }
                        }
                        else
                        {
                            MailFrameworkModEntry.ModMonitor.Log($"An index value is required to attach an ring for letter {mailItem.Id}.", LogLevel.Warn);
                        }

                        break;
                    case ItemType.Furniture:
                        if (i.Name != null)
                        {
                            Dictionary<string, string> furnitures = MailFrameworkModEntry.ModHelper.GameContent.Load<Dictionary<string, string>>(PathUtilities.NormalizeAssetName("Data/Furniture"));
                            KeyValuePair<string, string> pair = furnitures.FirstOrDefault(o => o.Value.StartsWith(i.Name + "/"));
                            if (pair.Value != null)
                            {
                                i.Index = pair.Key;
                            }
                            else
                            {
                                MailFrameworkModEntry.ModMonitor.Log($"No furniture found with the name {i.Name} for letter {mailItem.Id}.", LogLevel.Warn);
                            }
                        }

                        if (i.Index != null)
                        {
                            return Furniture.GetFurnitureInstance(i.Index);
                        }
                        else
                        {
                            MailFrameworkModEntry.ModMonitor.Log($"An index value is required to attach a furniture for letter {mailItem.Id}.", LogLevel.Warn);
                        }

                        break;
                    case ItemType.Weapon:
                        if (i.Name != null)
                        {
                            var pair = Game1.weaponData.FirstOrDefault(o => i.Name.Equals(o.Value.Name));
                            if (pair.Value != null)
                            {
                                i.Index = pair.Key;
                            }
                            else
                            {
                                MailFrameworkModEntry.ModMonitor.Log($"No weapon found with the name {i.Name} for letter {mailItem.Id}.", LogLevel.Warn);
                            }
                        }

                        if (i.Index != null)
                        {
                            string index = i.Index;
                            return SlingshotIndexes.Contains(index)
                                ? (Item)new Slingshot(index.Replace("(W)", ""))
                                : (Item)new MeleeWeapon(index);
                        }
                        else
                        {
                            MailFrameworkModEntry.ModMonitor.Log($"An index value is required to attach a weapon for letter {mailItem.Id}.", LogLevel.Warn);
                        }

                        break;
                    case ItemType.Boots:
                        if (i.Name != null)
                        {
                            var boots = MailFrameworkModEntry.ModHelper.GameContent.Load<Dictionary<string, string>>(PathUtilities.NormalizeAssetName("Data/Boots"));
                            var pair = boots.FirstOrDefault(o => o.Value.StartsWith(i.Name + "/"));
                            if (pair.Value != null)
                            {
                                i.Index = pair.Key;
                            }
                            else
                            {
                                MailFrameworkModEntry.ModMonitor.Log($"No boots found with the name {i.Name} for letter {mailItem.Id}.", LogLevel.Warn);
                            }
                        }

                        if (i.Index != null)
                        {
                            return new Boots(i.Index);
                        }
                        else
                        {
                            MailFrameworkModEntry.ModMonitor.Log($"An index value is required to attach a boots for letter {mailItem.Id}.", LogLevel.Warn);
                        }

                        break;
                    case ItemType.DGA:
                        if (DgaApi != null)
                        {
                            try
                            {
                                object dgaObject = DgaApi.SpawnDGAItem(i.Name);
                                if (dgaObject is StardewValley.Item dgaItem)
                                {
                                    if (dgaItem is StardewValley.Object)
                                    {
                                        dgaItem.Stack = i.Stack ?? 1;
                                        dgaItem.Quality = i.Quality;
                                    }
                                    else
                                    {
                                        dgaItem.Stack = 1;
                                    }

                                    return dgaItem;
                                }
                                else
                                {
                                    MailFrameworkModEntry.ModMonitor.Log($"No DGA item found with the ID {i.Name} for letter {mailItem.Id}.", LogLevel.Warn);
                                }
                            }
                            catch (Exception ex)
                            {
                                MailFrameworkModEntry.ModMonitor.Log($"Error trying to create item with the DGA ID {i.Name} for letter {mailItem.Id}.", LogLevel.Warn);
                                MailFrameworkModEntry.ModMonitor.Log(ex.Message, LogLevel.Trace);
                            }
                        }
                        else
                        {
                            MailFrameworkModEntry.ModMonitor.Log($"No DGA API found, so item with the ID {i.Name} for letter {mailItem.Id} will be ignored.", LogLevel.Warn);
                        }

                        break;
                    case ItemType.QualifiedItemId:
                        if (i.Index != null)
                        {
                            Item item = ItemRegistry.Create(i.Index, i.Stack ?? 1, i.Quality);
                            return item;
                        }
                        else
                        {
                            MailFrameworkModEntry.ModMonitor.Log($"An index value is required to attach a FullId Item for letter {mailItem.Id}.", LogLevel.Warn);
                        }

                        break;
                    case ItemType.None:
                        break;
                    default:
                        MailFrameworkModEntry.ModMonitor.Log($"Invalid attachment type '{i.Type}' found in letter {mailItem.Id}.", LogLevel.Warn);
                        break;
                }

                return null;
            }

        private static Child GetChild(int childNumber)
        {
            return Game1.player.getChildren().OrderByDescending(c=> c.daysOld.Value).ToImmutableList()[childNumber - 1];
        }

        public static Texture2D GetTextureAsset(IContentPack contentPack, string textureName)
        {
            if (contentPack != null && contentPack.ModContent.DoesAssetExist<Texture2D>(textureName))
            {
                var key = new Tuple<string, string>(contentPack.Manifest.UniqueID, textureName);
                if (!_contentPackAssets.ContainsKey(key))
                {
                    _contentPackAssets[key] = contentPack.ModContent.Load<Texture2D>(textureName);
                }
                return _contentPackAssets[key];
            }
            else
            {
                var key = new Tuple<string, string>("DIGUS.MailFrameworkMod", textureName);
                if (!_contentPackAssets.ContainsKey(key))
                {
                    _contentPackAssets[key] = MailFrameworkModEntry.ModHelper.GameContent.Load<Texture2D>(textureName);
                }
                return _contentPackAssets[key];
            }
        }

        public static void RegisterContentPack(IContentPack contentPack)
        {
            RegisteredContentPacks.Add(contentPack);
        }
    }
}
