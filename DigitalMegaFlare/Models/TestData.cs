using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DigitalMegaFlare.Models
{
    public class TestData : UserData
    {
        [Key]
        public long Id { get; set; }

        public string Name { get; set; }

    }
}
