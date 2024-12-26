using System;
using System.Collections.Generic;

namespace ServerInteraction.Payload
{
    [Serializable]
    public class MultiHistoryMatch
    {
        public double timeElapsed;
        public string dateStartTime;
        public List<string> playerNames;
        public int deathCount;
    }
}