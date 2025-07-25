﻿using Repository.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Models.DTOs.Response
{
    public class CustomerResponse
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string phoneNumber { get; set; }
        public string? FullName { get; set; }
        public DateTime? BirthDate { get; set; }
        public Gender? gender { get; set; }
        public decimal? Wallet { get; set; }
    }
}
