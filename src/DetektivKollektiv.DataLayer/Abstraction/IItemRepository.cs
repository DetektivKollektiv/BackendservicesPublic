using System.Collections.Generic;
using System.Threading.Tasks;

namespace DetektivKollektiv.DataLayer.Abstraction
{
    public interface IItemRepository
    {
        /// <summary>
        /// Creates an <see cref="Item"/>
        /// </summary>
        /// <param name="item">The item to create.</param>
        /// <returns>The created item.</returns>
        Task<Item> CreateItem(Item item);
        
        /// <summary>
        /// Returns a list of all <see cref="Item"/>
        /// </summary>
        /// <returns><see cref="IEnumerable{T}"/> of <see cref="Item"/></returns>
        Task<IEnumerable<Item>> GetAllItems();

        /// <summary>
        /// Returns a random <see cref="Item"/>.
        /// </summary>
        /// <returns><see cref="Item"/></returns>
        Task<Item> GetRandomItem();
    }
}