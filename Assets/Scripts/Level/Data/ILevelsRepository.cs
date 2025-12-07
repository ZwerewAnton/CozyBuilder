using System.Collections.Generic;
using _1_LEVEL_REWORK.New.Data;

namespace Level
{
    public interface ILevelsRepository
    {
        List<LevelData> Levels { get; }
        bool TryGetLevel(string levelName, out LevelData level);
    }
}