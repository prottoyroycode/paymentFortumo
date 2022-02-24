using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Models.Entities
{
    [Table("Channel_Code")]
    public class Channel_Code
    {
        [Key]
        public int Id { get; set; }
        public string ChannelCode { get; set; }
        public string Currency { get; set; }
        public string Country_Code { get; set; }
        public decimal Price { get; set; }
        

    }
}
