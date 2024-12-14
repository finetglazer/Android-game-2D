namespace Models
{
    public class TeamMatchHistoryModel
    {
        public string id;
        public int duration;
        public string startTime;   // dd-mm-yyyy hh:mm
        public TeamMember[] teamMembers;
        public string teamName;
        public int teamSize;
    }
}