using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmashPopMVC.Models.ApplicationUser
{
    public class UserListingModel
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Joined { get; set; }
        public string MainName { get; set; }
        public string MainImage { get; set; }
        public string AltName { get; set; }
        public string AltImage { get; set; }
    }
}
