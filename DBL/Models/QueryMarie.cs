using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BITPay.DBL.Models
{
    public class QueryMarie
    {
        [Required]
        [Display(Name = "Tax Group")]       
        public int TaxType { get; set; }

        [Required]
        [Display(Name = "Tax Type")]
        [JsonProperty("typ_code")]
        public int TypeCode { get; set; }

        [Required]
        [Display(Name = "Reference Number")]
        public string RefNo { get; set; }

        [JsonProperty("plot_no")]
        public string PlotNo { get; set; }

        [JsonProperty("plate_no")]
        public string PlateNo { get; set; }

        [JsonProperty("seat_no")]
        public string SeatNo { get; set; }

        [JsonProperty("mkt_name")]
        public string MarketName { get; set; }

        [JsonProperty("act_no")]
        public string ActivityNo { get; set; }

        [JsonProperty("card_no")]
        public string CardNo { get; set; }

        [JsonProperty("ent_typ")]
        public string EntityType { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("id_no")]
        public string NationalID { get; set; }

        [JsonProperty("qtr")]
        public string Quarter { get; set; }

        [JsonProperty("str_avn")]
        public string StreetAvenue { get; set; }

        [JsonProperty("alloc_ref")]
        public string AllocationRef { get; set; }

        [JsonProperty("alloc_no")]
        public string AllocationNo { get; set; }

        [JsonProperty("legal_ref")]
        public string LegalRef { get; set; }

        //---- Tax Notice
        [JsonProperty("tnc_no")]
        public string TaxNoticeNo { get; set; }

        [JsonProperty("tnc_code")]
        public string TaxNoticeCode { get; set; }
    }
}
