﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasteBook.Data.DataTransferObjects
{
    public class LikeDTO
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public int LikerAccountId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool Active { get; set; }
        public string? ProfileImagePath { get; set; }
    }

    public class NewLikeDTO
    {
        public int PostId { get; set; }
        public int LikerAccountId { get; set; }
    }
}
