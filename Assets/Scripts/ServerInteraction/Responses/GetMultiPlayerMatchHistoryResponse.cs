using System;
using System.Collections.Generic;

namespace ServerInteraction.Responses
{
    public class GetMultiPlayerMatchHistoryResponse
    {
        public List<MultiPlayerMatchHistory> multiPlayerMatchHistoryList;
    }
    
    public class MultiPlayerMatchHistory
    {
        public string teamName;
    }
}