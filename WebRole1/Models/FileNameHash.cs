using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebRole1.Models
{
    public class RootFileNameHash
    {
        public Files[] Files {get; set;}
    }

    public class Files
    {
        public string File_name { get; set; }
        public string File_Hash { get; set; }
    }
}