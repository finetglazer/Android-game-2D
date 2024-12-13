using System.Collections.Generic;

namespace ServerInteraction.Responses
{
    public class GetAllSoloStatsResponse
    {
        public List<SoloStats> soloStatsList;
    }

    public class SoloStats
    {
        public string matchHistory;
        public float previousPoints;
        public float totalPoints;
        public string userId;
        public string userName;
    }
}