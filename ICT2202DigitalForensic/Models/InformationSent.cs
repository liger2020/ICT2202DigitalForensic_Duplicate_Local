using System;
using System.Collections.Generic;
using System.Web;

namespace ICT2202DigitalForensic.Models
{
    public class InformationSent
    {
        public string Caseid { get; set;}
        public IEnumerable<meta_data> metaDatas { get; set; }
        public IEnumerable<log> logs { get; set; }
    }

    public class meta_data
    {
        public string FileHash { get; set; }
        public string FileName { get; set; }
        public string ModifiedDate { get; set; }
        public string CreationDate { get; set; }
    }

    public class log
    {
        public string Action { get; set; }
        public string Username { get; set; }
    }
}