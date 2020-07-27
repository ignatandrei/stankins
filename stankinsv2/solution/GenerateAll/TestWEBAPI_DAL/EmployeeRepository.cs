@model Stankins.Interfaces.IDataToSent
@{

    var dt= Model.DataToBeSentFurther[0];
    string repo= @dt.TableName  + "_Repository";
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using TestWEBAPI_DAL;
using TestWebAPI_BL;

namespace TestWebAPI_DAL
{
    public partial class @repo
    {
        private readonly DatabaseContext databaseContext;

        public @repo (DatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }
        public Task<@(dt.TableName)[]> GetAll_@(dt.TableName)()
        {
            return databaseContext.@(dt.TableName).ToArrayAsync();
        }
        public Task<@dt.TableName> FindAfterId(long id)
        {
            var data = databaseContext.@(dt.TableName).FirstOrDefaultAsync(it => it.ID == id);
            return data;
        }
        public Task<@dt.TableName> FindSingle(Func<@dt.TableName ,bool> f)
        {
            var data = databaseContext.@(dt.TableName).FirstOrDefaultAsync(it=>f(it));
            return data;
        }
        public Task<@(dt.TableName)[]> FindMultiple(Func<@dt.TableName, bool> f)
        {
            var data = databaseContext.@(dt.TableName).Where(it=>f(it));
            return data.ToArrayAsync();
        }
        public async Task<@dt.TableName> Insert(@dt.TableName p)
        {
            databaseContext.@(dt.TableName).Add(p);
            await databaseContext.SaveChangesAsync();
            return p;
        }
        public async Task<@dt.TableName> Update(@dt.TableName p)
        {
            var original = await FindAfterId(p.ID);
            original.CopyPropertiesFrom(other: p, withID: true);                        
            await databaseContext.SaveChangesAsync();
            return p;
        }
        public async Task<@dt.TableName> Delete(@dt.TableName p)
        {
            var original = await FindAfterId(p.ID);
            databaseContext.@(dt.TableName).Remove(original);
            await databaseContext.SaveChangesAsync();
            return p;
        }

    }
}
