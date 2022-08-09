using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameTest
{
    public class Settings
    {
        public int frameRate;
        public int updateFrequency;
        public float playerSpeedChange;
        public float platformSpeedChange;
        public bool isAiTraining;

        public Settings(float playerSpeedChange, float platformSpeedChange, int frameRate = 120, int updateFrequency = 120, 
                bool isAiTraining = false)
            {
            this.frameRate = frameRate;
            this.updateFrequency = updateFrequency;
            this.isAiTraining = isAiTraining;
            this.playerSpeedChange = platformSpeedChange;
            this.platformSpeedChange = platformSpeedChange;
        }
    }
}