using SmashPopMVC.Data.Models;
using System;
using System.Collections.Generic;

namespace SmashPopMVC.Data.Sync
{
    public interface ITally
    {
        Tally Get(int ID);
        void New(Tally tally);

        IEnumerable<Tally> GetAll();
        Tally GetByDateCreated(DateTime now);
        Tally GetByDateCreatedOrCreate(DateTime now);
    }
}
