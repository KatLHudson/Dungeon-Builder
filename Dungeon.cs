using System;

/// <summary>
/// A representation of a dungeon for roleplay purposes. Contains a map and a name.
/// </summary>
public class Dungeon
{
	public int[,] dungeonMap;
	public String dungeonName;

	/// <summary>
	/// Generates a default 5x5 dungeon named "THE DUNGEON".
	/// </summary>
	public Dungeon()
	{
		// Quickbuild with default settings
		dungeonMap = Build();
		dungeonName = "THE DUNGEON";
	}
	
	/// <summary>
	/// Generates a custom dungeon with user inputs
	/// </summary>
	/// <param name="width">Number of columns</param>
	/// <param name="height">Number of rows</param>
	/// <param name="tileTypes">Unique Tile Identifiers</param>
	/// <param name="entranceMax">Maximum entrances possible</param>
	/// <param name="exitMax">Maximum exits possible</param>
	/// <param name="name">Dungeon Name</param>
	public Dungeon(int width, int height, int tileTypes, int entranceMax, int exitMax, String name)
    {
		// Custom build with user input
		dungeonMap = Build(width, height, tileTypes, entranceMax, exitMax);
		dungeonName = name;
    }

	/// <summary>
	/// Generates a dungeon with the provided map array.  Errors in map array will force a reset with corrected parameters.
	/// </summary>
	/// <param name="myDungeon">Integer Array for the dungeon</param>
	/// <param name="name">Dungeon Name</param>
	public Dungeon(int[,] myDungeon, String name)
	{
		// For the instance where someone might have already pre-built an array
		//Array will be checked for validity before being assigned to dungeonMap
		//Failed array will be transformed to a fresh build with corrections to invalid parameters
		int[] newDungeonParams = new int[5];
		int[] adjustedParams;
		int typeCount = 0;
		List<int> typeRegistry = new List<int> { };
		int entCount = 0;
		int extCount = 0;
		bool sameArray = true;

		//Dungeon Width & Height
		newDungeonParams[0] = myDungeon.GetLength(1);
		newDungeonParams[1] = myDungeon.GetLength(0);
		
		for(int x = 0; x < newDungeonParams[1]; x++)
        {
			for(int y = 0; y < newDungeonParams[0]; y++)
            {
				int temp = myDungeon[x, y];
				bool newInstance = true;

				//Dungeon TileTypes - Count Unique values
				foreach (int i in typeRegistry)
				{
					if (i == temp)
                    {
						newInstance = false;
                    }
				}
				if (newInstance)
                {
					typeRegistry.Add(temp);
					typeCount++;
                }

				//Dungeon Entrance Count - See how many 0s are present
				if (temp == 0)
                {
					entCount++;
                }
				//Dungeon Exit Count - See how many 1s are present
				if (temp == 1)
                {
					extCount++;
                }
			}
		}
		
		newDungeonParams[2] = typeCount;
        newDungeonParams[3] = entCount;
		newDungeonParams[4] = extCount;
		
		//Parameter Check
		adjustedParams = ParamAdjust(newDungeonParams);
		for (int i=0; i<5; i++)
        {
			if (adjustedParams[i] != newDungeonParams[i])
            {
				sameArray = false;
				break;
            }
        }

        if (!sameArray)
        {
			//Throw error Message
			Console.WriteLine("Invalid Dungeon Map submitted. Map re-rolled.");
			dungeonMap = Build(adjustedParams[0], adjustedParams[1], adjustedParams[2], adjustedParams[3], adjustedParams[4]);
        }
		else
        {
			dungeonMap = myDungeon;
		}
		dungeonName = name;
    }

	/// <summary>
	/// Builds a dungeon array with entered parameters.  Default parameters exist.
	/// </summary>
	/// <param name="width">Number of Columns</param>
	/// <param name="height">Number of Rows</param>
	/// <param name="tileTypes">Unique Tile Identifiers</param>
	/// <param name="entranceMax">Maximum entrances possible</param>
	/// <param name="exitMax">Maximum exits possible</param>
	/// <returns>Integer array for Dungeon Map</returns>
	public static int[,] Build(int width = 5, int height = 5, int tileTypes = 6, int entranceMax = 1, int exitMax = 1)
	{
		// Builds the dungeon map based on given criteria
		// Defaults are provided
		// Forces at least 1 entrance and 1 exit should none be rolled.
		// 0 = entrance, 1 = exit

		// width/height must produce a 2x2 array at a minimum
		// entranceMax, and exitMax must be at or above default values to proceed
		// tileTypes must be greater than 2 to allow for a dungeon exit, entrance, and dungeon tile
		int[] paramUpdate = ParamAdjust(width, height, tileTypes, entranceMax, exitMax);
		width = paramUpdate[0];
		height = paramUpdate[1];
		tileTypes = paramUpdate[2];
		entranceMax = paramUpdate[3];
		exitMax = paramUpdate[4];

		// Initializing counters, generator, and final dungeon array
		int entCount = 0;
		int extCount = 0;
		Random generator = new Random();
		int[,] finalMap = new int[height, width];

		// Start of map loop, goes from max width to max height
		for (int x = 0; x < height; x++)
		{
			for (int y = 0; y < width; y++)
			{
				int testValue = generator.Next(0, tileTypes);
				if (testValue == 0 || testValue == 1)
				{
					//  Do a max entrance/max exit check. Re-roll for failure.
					if ((entCount >= entranceMax && testValue == 0) || (extCount >= exitMax && testValue == 1))
					{
						// This case must be re-rolled until the condition is no longer present.
						do
						{
							testValue = generator.Next(0, tileTypes);
						}
						while ((entCount >= entranceMax && testValue == 0) || (extCount >= exitMax && testValue == 1));
					}
					// Now that the prior condition is clear, we can continue.
					switch (testValue)
					{
						case 0:
							entCount++;
							break;
						case 1:
							extCount++;
							break;
						default:
							break;
					}
				}
				finalMap[x, y] = testValue;
			}
		}

		// Validate that the array has at least one entrance and exit.
		if (entCount == 0 || extCount == 0)
        {
			Console.WriteLine("Missing Entrance and/or Exit. Repairing.");
			int x = generator.Next(0, height);
			int y = generator.Next(0, width);
			int oldValue = finalMap[x, y];
			int newValue = finalMap[x, y];
			if (entCount == 0)
            {
				newValue = 0;

				// If there is only 1 exit, we cannot compromise it.
				if (entCount == 0 && extCount < 2)
				{
					do
					{
						x = generator.Next(0, height);
						y = generator.Next(0, width);
						oldValue = finalMap[x, y];
					}
					while (oldValue == 1);
				}
				//Replace the new value here in case an exit is also missing
				finalMap[x, y] = newValue;
			}
			if (extCount == 0)
            {
				newValue = 1;
				
				// If there is only 1 entrance, we cannot compromise it.
				if (entCount < 2 && extCount == 0)
				{
					do
					{
						x = generator.Next(0, height);
						y = generator.Next(0, width);
						oldValue = finalMap[x, y];
					}
					while (oldValue == 0);
				}
				//Replace the new value here in case we also had to replace an entrance.
				finalMap[x, y] = newValue;
			}
        }
		// finalMap will be returned after all checks are complete.
		return finalMap;
	}

	/// <summary>
	/// Verifies the minimum parameters are met.  At smallest, a 2x2 dungeon with 3 tiles, 1 entrance, and 1 exit can exist.
	/// </summary>
	/// <param name="width">Number of Columns</param>
	/// <param name="height">Number of Rows</param>
	/// <param name="tileTypes">Unique Tile Identifiers</param>
	/// <param name="entranceMax">Maximum entrances possible</param>
	/// <param name="exitMax">Maximum exits possible</param>
	/// <returns>Returns an array of verified parameters in the order they were entered into the method.</returns>
	private static int[] ParamAdjust(int width, int height, int tileTypes, int entranceMax, int exitMax)
    {
		//This function is responsible for assuring integrity in the dungeon layout.
		//Swaps erroneous values with the default presets.
		int[] adjustedParam = { width, height, tileTypes, entranceMax, exitMax };
		if (width < 2 || height < 2 || tileTypes < 3 || entranceMax < 1 || exitMax < 1)
		{
			String errEntry = "\t";
			Console.WriteLine("Invalid Parameters Found");
			if (width < 2)
			{
				adjustedParam[0] = 5;
				errEntry += "Width\t";
			}
			if (height < 2)
			{
				adjustedParam[1] = 5;
				errEntry += "Height\t";
			}
			if (tileTypes < 3)
			{
				adjustedParam[2] = 6;
				errEntry += "Tile Types\t";
			}
			if (entranceMax < 1)
			{
				adjustedParam[3] = 1;
				errEntry += "Entrance\t";
			}
			if (exitMax < 1)
			{
				adjustedParam[4] = 1;
				errEntry += "Exit\t";
			}
			Console.WriteLine("\tCorrected Parameters:");
			Console.WriteLine(errEntry);
		}
		return adjustedParam;
	}

	/// <summary>
	/// Verifies the minimum parameters are met.  At smallest, a 2x2 dungeon with 3 tiles, 1 entrance, and 1 exit can exist.
	/// </summary>
	/// <param name="paramCheck">An integer array with parameters in the following order: width, height, tileTypes, entranceMax, exitMax</param>
	/// <returns>Returns an array of verified parameters in the order they were entered into the method.</returns>
	private static int[] ParamAdjust(int[] paramCheck)
    {
		//For use with pre-generated dungeon array submission.
		return ParamAdjust(paramCheck[0], paramCheck[1], paramCheck[2], paramCheck[3], paramCheck[4]);
    }

	/// <summary>
	/// Creates an extended String, showing the Dungeon name followed by a grid of the map.
	/// </summary>
	/// <returns>Extended String with the dungeon name and map displayed.</returns>
	public override String ToString()
    {
		String dungeonString = dungeonName+"\n\n";

		for(int x=-1; x<dungeonMap.GetLength(0); x++)
        {
			dungeonString += "\t\t";
			if (x == -1)
			{
				// Add Label Row
				dungeonString += " ";
				for (int y = 0; y < dungeonMap.GetLength(1); y++)
                {
					int tempLabel = y + 1;
					dungeonString += "\tC" + tempLabel;
                }
			}
			else
			{
				for (int y = -1; y < dungeonMap.GetLength(1); y++)
				{
					if (y == -1)
                    {
						//Add Label
						int tempLabel = x + 1;
						dungeonString += "R"+tempLabel;
                    }
                    else
                    {
						//Add Value
						dungeonString += "\t" + dungeonMap[x, y];
                    }
				}
			}
			dungeonString += "\n";
        }

		return dungeonString;
	}

}