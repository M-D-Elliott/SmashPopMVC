using SmashPopMVC.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SmashPopMVC.Data.Models
{
    public class FriendRelationship
    {
        [Key]
        public int RelationshipID { get; set; }

        public ApplicationUser Friend { get; set; }
    }

}
