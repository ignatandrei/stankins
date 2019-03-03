using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Stankins.Rest;
using StankinsCommon;
using StankinsObjects;
using System;
using System.Collections.Generic;
using System.Data;

namespace Stankins.Trello
{
    public class ReceiverFromTrello: ReceiveRest
    {
        public ReceiverFromTrello(CtorDictionary dataNeeded):base(dataNeeded)
        {

        }
        protected override IEnumerable<DataTable> FromJSon(string json)
        {
            TrelloJSON trello= JsonConvert.DeserializeObject<TrelloJSON>(json);
            var cards= trello.Cards;
            var lists=trello.Lists;
            
            foreach(var l in lists)
            {
                
            }
            return null;

        }

    }
}
