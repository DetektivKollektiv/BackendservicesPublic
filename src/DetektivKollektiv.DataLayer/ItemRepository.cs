using Amazon.Lambda.Core;
using DatektivKollektiv.Shared;
using DetektivKollektiv.DataLayer.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DetektivKollektiv.DataLayer
{
    public class ItemRepository : IItemRepository
    {
        private readonly ILambdaLogger _logger;

        /// <summary>
        /// Creates a new instance of the <see cref="ItemRepository"/>
        /// </summary>
        /// <param name="logger">Instance of the <see cref="ILambdaLogger"/></param>
        public ItemRepository(ILambdaLogger logger) 
        {
             _logger = logger;
        }

        /// <summary>
        /// Returns all items from the database
        /// </summary>
        /// <returns>All items in an IEnumerable object</returns>
        public List<Item> GetAllItems()
        {
            _logger.LogInfo("GetAllItems Method initiated");

            using (var itemContext = new ItemContext())
            {
                var items = itemContext.Items.ToList();
                return items;
            }
        }

        /// <summary>
        /// Returns a random <see cref="Item"/>
        /// </summary>
        /// <returns>A random <see cref="Item"/> from the database</returns>
        public Item GetRandomItem()
        {
            _logger.LogInfo("Retrieving random item from database.");

            using (var itemContext = new ItemContext())
            {
                var items = itemContext.Items.ToList();
                Random r = new Random();
                return items[r.Next(items.Count)];
            }
        }

        /// <summary>
        /// Queries the database for the item with the specified id
        /// </summary>
        /// <param name="id">The id of the desired item</param>
        /// <returns>The desired item</returns>
        /// <returns>null, if no obejct was found</returns>
        public Item GetItemById(Guid id)
        {
            using (var itemContext = new ItemContext())
            {
                try
                {
                    var item = itemContext.Items
                        .Single(i => i.ItemId == id);
                    return item;
                }
                catch (InvalidOperationException)
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// Checks the database for items with the specified text.
        /// </summary>
        /// <param name="text"></param>
        /// <returns>The item, if it exists</returns>
        /// <returns>Null, if the item does not exist</returns>
        public Item GetItemByText(string text)
        {
            using (var itemContext = new ItemContext())
            {
                try
                {
                    var item = itemContext.Items.
                        Single(i => i.Text == text);
                    return item;
                }
                catch (InvalidOperationException)
                {
                    return null;
                }
            }
        }          

        public Item CreateItem(string text)
        {
            Item item = new Item
            {
                Text = text,
                ReviewBad = 0,
                ReviewGood = 0,
                ItemId = Guid.NewGuid()
            };

            using (var itemContext = new ItemContext())
            {
                try {
                    itemContext.Add(item);
                    itemContext.SaveChanges();
                    return item;
                }
                catch(Exception)
                {
                    return null;
                }
            }
        }

        public Item Review(Guid id, bool goodReview)
        {
            using (var itemContext = new ItemContext()) {

                try
                {
                    var item = itemContext.Items.Single(i => i.ItemId == id);

                    if (goodReview)
                    {
                        item.ReviewGood++;
                    }

                    else
                    {
                        item.ReviewBad++;
                    }
                    itemContext.SaveChanges();
                    return item;
                }
                catch(Exception e)
                {
                    _logger.LogLine(e.Message);
                    return null;
                }
            }
        }
    }
}
