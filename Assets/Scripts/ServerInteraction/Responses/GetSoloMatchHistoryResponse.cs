using UnityEngine;

namespace ServerInteraction.Responses
{
    public class GetSoloMatchHistoryResponse
    {
        public SoloMatchHistoryModel[] matchHistoryList;
    }
    public class SoloMatchHistoryModel
    {
        public string id;
        public int duration;
        public PlayerStats player1;
        public PlayerStats player2;
        public string startTime;   // dd-mm-yyyy hh:mm
    }

    public class PlayerStats
    {
        public string userId;
        public string username;
        public bool won;
        public float points;
        public float damageDealt;
        public float damageTaken;
    }
}