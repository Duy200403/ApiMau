using ApiWebsite.Models.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiWebsite.Models
{
  public class AccountRespone
  {
    public AccountRespone()
    {
    }
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string FullName { get; set; }
    public string PhoneNumber { get; set; }
    public string Pseudonym { get; set; }
    public string Email { get; set; }
    public string Roles { get; set; }
    public bool IsActive { get; set; }
  }
}