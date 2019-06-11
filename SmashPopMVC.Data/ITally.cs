using SmashPopMVC.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmashPopMVC.Data
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
