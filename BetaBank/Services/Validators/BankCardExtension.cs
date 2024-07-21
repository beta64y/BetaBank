using BetaBank.Contexts;
using BetaBank.Models;
using Microsoft.EntityFrameworkCore;

namespace BetaBank.Services.Validators
{
    public static class BankCardExtension
    {
        public static async Task<bool> IsCardType(this BankCard bankCard, BetaBankDbContext _context , string typeName)
        {
            BankCardType cardType = await _context.BankCardTypes.FirstOrDefaultAsync(x => x.CardId == bankCard.Id);
            return (await _context.BankCardTypeModels.FirstOrDefaultAsync(x => x.Id == cardType.TypeId)).Name == typeName;
        }
        public static async Task<bool> CanUseCard(this BankCard bankCard, BetaBankDbContext _context)
        {
            BankCardType cardType = await _context.BankCardTypes.FirstOrDefaultAsync(x => x.CardId == bankCard.Id);
            if ((await _context.BankCardTypeModels.FirstOrDefaultAsync(x => x.Id == cardType.TypeId)).Name == "BetaCard")
            {
                var today = DateTime.UtcNow.Date;
                var usageCount = await _context.CardUsages.Where(x => x.CardId == bankCard.Id && x.UsageDate >= today).CountAsync();

                return usageCount < 5;
            }
            else
            {
                return true;
            }

        }
        public async static Task<bool> IsBlocked(this BankCard bankCard , BetaBankDbContext _context)
        {
            BankCardStatus cardStatus = await _context.BankCardStatuses.FirstOrDefaultAsync(x => x.CardId == bankCard.Id);
            BankCardStatusModel bankCardStatusModel = await _context.BankCardStatusModels.FirstOrDefaultAsync(x => x.Id == cardStatus.StatusId);
            return bankCardStatusModel.Name == "Blocked";
        }
        public async static Task<bool> IsDisabled(this BankCard bankCard, BetaBankDbContext _context)
        {
            BankCardStatus cardStatus = await _context.BankCardStatuses.FirstOrDefaultAsync(x => x.CardId == bankCard.Id);
            BankCardStatusModel bankCardStatusModel = await _context.BankCardStatusModels.FirstOrDefaultAsync(x => x.Id == cardStatus.StatusId);
            return bankCardStatusModel.Name == "Disabled";
        }

    }
}
