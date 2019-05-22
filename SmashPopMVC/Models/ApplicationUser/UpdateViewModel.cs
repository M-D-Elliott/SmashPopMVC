using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmashPopMVC.Models.ApplicationUser
{
    public class UpdateViewModel
    {
        public string UserID { get; set; }
        public int? MainID { get; set; }
        public int? AltID { get; set; }
    }
}
