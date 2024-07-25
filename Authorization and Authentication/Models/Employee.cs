using System;
using System.Collections.Generic;

namespace Authorization_and_Authentication.Models;

public partial class Employee
{
    public int EmpId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }
}
