using System;
using ApiWebsite.Helper;
using FluentValidation;

namespace ApiWebsite.Models
{
    public class WelcomeRequest
    {
        public WelcomeRequest()
        {

        }
        public string Ten { get; set; }
        public string Trangthai { get; set; }
    }
}