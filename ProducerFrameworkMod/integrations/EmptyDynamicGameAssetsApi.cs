using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace ProducerFrameworkMod.integrations
{
    internal class EmptyDynamicGameAssetsApi : IDynamicGameAssetsApi
    {
        /// <summary>
        /// Get the DGA item ID of this item, if it has one.
        /// </summary>
        /// <param name="item">The item to get the DGA item ID of.</param>
        /// <returns>The DGA item ID if it has one, otherwise null.</returns>
        public string GetDGAItemId(object item)
        {
            return null;
        }

        /// <summary>
        /// Get the DGA Fake Index of this item, if it has one.
        /// </summary>
        /// <param name="item">The item to get the DGA item ID of.</param>
        /// <returns>The DGA Fake Index if it has one, otherwise null.</returns>
        public int? GetDGAFakeIndex(object item)
        {
            return null;
        }

        /// <summary>
        /// Get the DGA Fake Index of the item referenced with its full ID ("mod.id/ItemId")..
        /// </summary>
        /// <param name="fullId">The full ID of the item to obtain the Fake Index.</param>
        /// <returns>The DGA Fake Index if it has one, otherwise null.</returns>
        public int? GetDGAFakeIndex(string fullId)
        {
            return null;
        }

        /// <summary>
        /// Get the DGA Fake Object Information of the item referenced with its Fake Index.
        /// </summary>
        /// <param name="fakeIndex">The Fake Index to get the Fake information of.</param>
        /// <returns>The DGA Fake Index if it has one, otherwise null.</returns>
        public string GetDGAFakeObjectInformation(int fakeIndex)
        {
            return null;
        }

        /// <summary>
        /// Spawn a DGA item, referenced with its full ID ("mod.id/ItemId").
        /// Some items, such as crafting recipes or crops, don't have an item representation.
        /// </summary>
        /// <param name="fullId">The full ID of the item to spawn.</param>
        /// <param name="color">The color of the item.</param>
        /// <returns></returns>
        public object SpawnDGAItem(string fullId, Color? color = null)
        {
            return null;
        }
    }
}
