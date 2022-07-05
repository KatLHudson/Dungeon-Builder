namespace DungeonAPI.Services
{
    using DungeonAPI.Models;
    using Microsoft.Extensions.Caching.Memory;
    using System.Linq;

    /// <summary>
    /// A collection of Loot. Maintains data in cache storage.
    /// </summary>
    public class LootPile
    {
        private const string CacheKey = "LootStore";
        private IMemoryCache _LootCache = new MemoryCache(new MemoryCacheOptions());

        /// <summary>
        /// Starts up a basic instance with a simple void entry
        /// </summary>
        public LootPile()
        {
            var status = _LootCache.Get(CacheKey);

            if (_LootCache != null)
            {
                if(status == null)
                {
                    var newLoot = new List<Loot> { new Loot() };
                    _LootCache.Set(CacheKey, newLoot);
                }
            }
        }

        /// <summary>
        /// Returns the values stored in the cache
        /// </summary>
        /// <returns>Loot collection in list format</returns>
        public List<Loot> GetAllLoot()
        {
            var status = _LootCache.Get(CacheKey);

            if (status != null)
            {
                return (List<Loot>)status;
            }

            return new List<Loot> { new Loot() };
        }

        /// <summary>
        /// Appends a new value to the end of the list.
        /// </summary>
        /// <param name="newLoot">New value to add</param>
        /// <returns>A boolean to represent whether or not the operation succeeded</returns>
        public bool AddNew(Loot newLoot)
        {
            var status = _LootCache.Get(CacheKey);

            if (status != null)
            {
                try
                {
                    List<Loot> currentData = (List<Loot>)status;
                    currentData.Add(newLoot);
                    _LootCache.Set(CacheKey, currentData);

                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// Allows one to swap a value in the list.
        /// </summary>
        /// <param name="oldEntry">Value to be changed</param>
        /// <param name="newEntry">Value to change to</param>
        /// <returns>Throws a boolean to represent the method's success</returns>
        public bool ChangeEntry(Loot oldEntry, Loot newEntry)
        {
            var status = _LootCache.Get(CacheKey);

            if (status != null)
            {
                List<Loot> currentData = (List<Loot>)status;
                int indexCount = 0;

                foreach (Loot i in currentData)
                {
                    //Once the old entry is located, we attempt to swap in the new one.
                    if (i.IsEqual(oldEntry))
                    {
                        try
                        {
                            Loot[] tempArray = currentData.ToArray();
                            tempArray[indexCount] = newEntry;
                            List<Loot> toList = tempArray.ToList();
                            currentData = toList;
                            _LootCache.Set(CacheKey, currentData);
                            return true;
                        }
                        catch(Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                            return false;
                        }
                    }
                    indexCount++;
                }
                //In this instance, we didn't find an entry to replace.
                return false;
            }
            else
            {
                //There's nothing in the cache, so this fails.
                return false;
            }
        }

        /// <summary>
        /// Attempts to remove a specified entry.
        /// </summary>
        /// <param name="oldEntry">Value to be removed</param>
        /// <returns>Throws a boolean to represent success or failure.</returns>
        public bool RemoveEntry(Loot oldEntry)
        {
            var status = _LootCache.Get(CacheKey);

            if (status != null)
            {
                List<Loot> currentData = (List<Loot>)status;

                try
                {
                    currentData.Remove(oldEntry);
                    _LootCache.Set(CacheKey, currentData);
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }
            }
            else
            {
                //There's nothing in the cache, so this fails.
                return false;
            }
        }
    }
}
