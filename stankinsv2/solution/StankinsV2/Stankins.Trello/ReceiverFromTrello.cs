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
            
            var dtList=new DataTable();
            dtList.Columns.Add("id",typeof(string));
            dtList.Columns.Add("name",typeof(string));
            dtList.Columns.Add("idboard",typeof(string));

            var dtCards=new DataTable();
            dtCards.Columns.Add("id",typeof(string));
            dtCards.Columns.Add("name",typeof(string));
            dtCards.Columns.Add("idlist",typeof(string));
            
            
            foreach(var l in lists)
            {
                dtList.Rows.Add(l.Id,l.Name,l.IdBoard);
            }
            yield return dtList;
            foreach (var c in cards)
            {
                dtCards.Rows.Add(c.Id,c.Name,c.IdList);
                
            }

            yield return dtCards;

        }

    }
}
