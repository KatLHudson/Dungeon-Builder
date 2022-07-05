using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DungeonAPI.Models;
using DungeonAPI.Services;

namespace DungeonAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DungeonController : ControllerBase
    {
        private LootPile dungeonRewards;
        
        /// <summary>
        /// Creates an instance of a Loot List model
        /// </summary>
        public DungeonController()
        {
            dungeonRewards = new LootPile();
        }

        /// <summary>
        /// Returns all the loot stored in cache memory
        /// </summary>
        /// <returns>List format of the Loot</returns>
        public List<Loot> Get()
        {
            //Get Resource
            return dungeonRewards.GetAllLoot();
        }

        /// <summary>
        /// Add a loot to the list
        /// </summary>
        /// <param name="newLoot">Loot to be added</param>
        public void Post(Loot newLoot)
        {
            //Add resource
            bool postResult = dungeonRewards.AddNew(newLoot);
            if (!postResult)
            {
                Console.WriteLine("Post failure");
            }

        }

        /// <summary>
        /// Modify a loot in the list
        /// </summary>
        /// <param name="oldLoot">Loot to change</param>
        /// <param name="newLoot">Loot to change to</param>
        public void Put(Loot oldLoot, Loot newLoot)
        {
            //modify the first instance found
            bool putResult = dungeonRewards.ChangeEntry(oldLoot, newLoot);
            if (!putResult)
            {
                Console.WriteLine("Put failure");
            }
        }

        /// <summary>
        /// Remove an instance of loot from the list
        /// </summary>
        /// <param name="oldLoot">Loot to be removed</param>
        public void Delete(Loot oldLoot)
        {
            //remove the first instance found
            bool delResult = dungeonRewards.RemoveEntry(oldLoot);
            if (!delResult)
            {
                Console.WriteLine("Delete failure");
            }
        }
    }
}
