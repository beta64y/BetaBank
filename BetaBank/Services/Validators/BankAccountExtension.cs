using BetaBank.Contexts;
using BetaBank.Models;
using Microsoft.EntityFrameworkCore;

namespace BetaBank.Services.Validators
{
    public static class BankAccountExtension
    {
        public async static Task<bool> IsSuspended(this BankAccount bankAccount, BetaBankDbContext _context)
        {
            BankAccountStatus bankAccountStatus = await _context.BankAccountStatuses.FirstOrDefaultAsync(x => x.AccountId == bankAccount.Id);
            BankAccountStatusModel bankAccountStatusModel = await _context.BankAccountStatusModels.FirstOrDefaultAsync(x => x.Id == bankAccountStatus.StatusId);
            return bankAccountStatusModel.Name == "Suspended";
        }
    }
}
