using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace WebRole1.Models
{
    public class UserModel
    {
        public List<SelectListItem> Users { get; set; }
        public int[] UserIds { get; set; }
    }
}