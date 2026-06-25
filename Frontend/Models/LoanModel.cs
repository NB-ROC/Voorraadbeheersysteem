using System;
using System.Collections.Generic;

namespace Frontend.Models;

public class LoanModel
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public UserModel User { get; set; } = null!;

    public int LenderId { get; set; }
    public UserModel Lender { get; set; } = null!;

    public ICollection<LoanProductModel> Products = null!;
    
    public DateTime LoanedAt { get; set; } = DateTime.UtcNow;
    public DateTime DueAt { get; set; }
    public DateTime? ReturnedAt { get; set; }
}