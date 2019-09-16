using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RICONS.Web.Models
{
    public class SendMailModel
    {
        public string AgreeLink { get; set; }
        public string DisagreeLink { get; set; }

        public string Hotenlanhdao { get; set; }
        public string chucvulanhdao { get; set; }
        public string phongbancongtruongten { get; set; }

        public string TieuDe { get; set; }
        public System.DateTime TuNgay { get; set; }
        public System.DateTime DenNgay { get; set; }
        public string NoiCongTac { get; set; }
        public string MoTa { get; set; }

    }
}