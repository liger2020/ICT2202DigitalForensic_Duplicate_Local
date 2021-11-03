using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebRole1.Models
{
    public class Rootobject
    {
        public Block[] Blocks { get; set; }
    }

    public class Block
    {
        public string block_hash { get; set; }
        public int block_number { get; set; }
        public string id { get; set; }
        public string log { get; set; }
        public string meta_data { get; set; }
        public string previous_block_hash { get; set; }
        public bool status { get; set; }
        public string timestamp { get; set; }
    }


    public class CaseInfo
    {
        public string id { get; set; }
        public Meta_Data meta_data { get; set; }
        public Log log { get; set; }
    }

    public class Outside_Pool
    {
        public List<Pool> Pool { get; set; }
    }

    public class Pool
    {
        public string case_id { get; set; }
        public Meta_Data meta_data { get; set; }
        public Log log { get; set; }
    }

    public class Meta_Data
    {
        public string File_Hash { get; set; }
        public string File_Name { get; set; }
        public string Modified_Date { get; set; }
        public string Creation_Date { get; set; }
    }

    public class Log
    {
        public string Action { get; set; }
        public List<string> Username { get; set; }
    }

}