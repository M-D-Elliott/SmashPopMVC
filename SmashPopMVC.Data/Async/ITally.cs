using SmashPopMVC.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmashPopMVC.Data.Async
{
    public interface ITally
    {
        Task<Tally> Get(int ID);
        Task New(Tally tally);

        Task<IEnumerable<Tally>> GetAll();
        Task<Tally> GetByDateCreated(DateTime now);
        Task<Tally> GetByDateCreatedOrCreate(DateTime now);
    }
}
