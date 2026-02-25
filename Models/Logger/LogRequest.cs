using System;
using ApiWebsite.Helper;
using FluentValidation;

namespace ApiWebsite.Models
{
    public class LogRequest
    {
        public LogRequest()
        {

        }
        public DateTime StartDay { get; set; }
        public DateTime EndDay { get; set; }
    }
}