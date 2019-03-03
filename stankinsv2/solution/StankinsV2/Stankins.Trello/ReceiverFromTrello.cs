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
        public ReceiverFromTrello(string adress) : this(new CtorDictionary()
        {
            {nameof(adress),adress },
          

        })
        {

        }
        protected override IEnumerable<DataTable> FromJSon(string json)
        {
            var trello= JsonConvert.DeserializeObject<BoardTrello>(json);
            var cards= trello.Cards;
            var lists=trello.Lists;
            
            var dtList=new DataTable();
            dtList.TableName="list";
            dtList.Columns.Add("id",typeof(string));
            dtList.Columns.Add("name",typeof(string));
            dtList.Columns.Add("idboard",typeof(string));

            var dtCards=new DataTable();
            dtCards.TableName="card";
            dtCards.Columns.Add("id",typeof(string));
            dtCards.Columns.Add("name",typeof(string));
            dtCards.Columns.Add("idlist",typeof(string));
            dtCards.Columns.Add("url",typeof(string));
            
            var dtComments=new DataTable();
            dtComments.TableName="comment";
            dtComments.Columns.Add("id",typeof(string));
            dtComments.Columns.Add("name",typeof(string));
            dtComments.Columns.Add("idcard",typeof(string));
            dtComments.Columns.Add("url",typeof(string));
            
            
            foreach(var l in lists)
            {
                dtList.Rows.Add(l.Id,l.Name,l.IdBoard);
            }
            yield return dtList;
            foreach (var c in cards)
            {
                dtCards.Rows.Add(c.Id,c.Name,c.IdList,c.Url);
                
            }
            foreach(var act in trello.Actions)
            {
                if(!string.Equals(act.Type,"commentCard", StringComparison.CurrentCultureIgnoreCase))
                    continue;
                dtComments.Rows.Add(act.Id,act.Data.Text,act.Data.Card.Id);

            }

            yield return dtCards;
            yield return dtComments;

        }

    }
}
