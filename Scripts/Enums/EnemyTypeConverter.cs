/// File Name: EnemyTypeConverter.cs
/// File Author(s): Lincoln Schroeder
/// File Purpose: The EnemyTypeConverter is a static class used for converting between EnemyTypes and other
/// object types such as integer or string.
/// 
/// Date Last Updated: November 8, 2019

public static class EnemyTypeConverter
{
    /// <summary>
    /// Converts the given token over to an EnemyType. Can convert strings and integers. Throws exceptions for unknown/invalid tokens.
    /// </summary>
    /// <param name="convert">The object token to be converted to an EnemyType.</param>
    /// <returns>The EnemyType that is associated with the given token.</returns>
    public static EnemyType ConvertToEnemyType(object convert)
    {
        if (convert is string)
        {
            convert = convert.ToString().ToLower();
            switch (convert)
            {
                case "shredder":
                    return EnemyType.SHREDDER;
                case "pursuer":
                    return EnemyType.PURSUER;
                case "buster":
                    return EnemyType.BUSTER;
                default:
                    throw new System.Exception("Unknown string. (EnemyTypeConverter.ConvertToEnemyType(UNKNOWN_STRING)): " + convert.ToString());
            }
        }
        else if (convert is int)
        {
            switch (convert)
            {
                case 0:
                    return EnemyType.SHREDDER;
                case 1:
                    return EnemyType.PURSUER;
                case 2:
                    return EnemyType.BUSTER;
                default:
                    throw new System.Exception("Integer out of bounds. (EnemyTypeConverter.ConvertToEnemyType(INVALID_INT)): " + convert.ToString());
            }
        }
        else
            throw new System.Exception("Invalid convert type. (EnemyTypeConverter.ConvertToEnemyType(INVALID_OBJECT)): " + convert.ToString());
    }

    /// <summary>
    /// Converts the given EnemyType to the desired object type. Throws exceptions for unknown EnemyTypes and unknown/invalid object types.
    /// </summary>
    /// <param name="enemyType">The EnemyType to be converted.</param>
    /// <param name="type">The object type to be converted to.</param>
    /// <returns>The correct token associating with the provided EnemyType and desired object type.</returns>
    public static object ConvertFromEnemyType(EnemyType enemyType, string type)
    {
        if (type.Equals("int"))
        {
            switch (enemyType)
            {
                case EnemyType.SHREDDER:
                    return 0;
                case EnemyType.PURSUER:
                    return 1;
                case EnemyType.BUSTER:
                    return 2;
                default:
                    throw new System.Exception("Invalid EnemyType. (EnemyTypeConverter.ConvertFromEnemyType(INVALID_ENEMYTYPE, string type))");
            }
        }
        else if (type.Equals("string"))
        {
            switch (enemyType)
            {
                case EnemyType.SHREDDER:
                    return "shredder";
                case EnemyType.PURSUER:
                    return "pursuer";
                case EnemyType.BUSTER:
                    return "buster";
                default:
                    throw new System.Exception("Invalid EnemyType. (EnemyTypeConverter.ConvertFromEnemyType(INVALID_ENEMYTYPE, string type))");
            }
        }
        else
            throw new System.Exception("Unkown/Invalid convert type. (EnemyTypeConverter.ConvertFromEnemyType(EnemyType enemyType, UNKNOWN_TYPE)): " + type);
    }
}


