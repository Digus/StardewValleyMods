using StardewModdingAPI;

namespace MailFrameworkMod.integrations
{
    public interface IDynamicGameAssetsApi
    {
        /// <summary>
        /// Spawn a DGA item, referenced with its full ID ("mod.id/ItemId").
        /// Some items, such as crafting recipes or crops, don't have an item representation.
        /// </summary>
        /// <param name="fullId">The full ID of the item to spawn.</param>
        /// <returns></returns>
        object SpawnDGAItem(string fullId);
    }
}
