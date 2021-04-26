using System;
using System.Collections.Generic;
using System.Text;

namespace EventFully.Models
{
    public class SearchResponse<T>
    {
        public List<T> Results { get; set; }

        public List<T> AllResults { get; set; }

        public int iTotalRecords { get; set; }

        public int iTotalDisplayRecords { get; set; }
    }
}
