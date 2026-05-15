using RemittanceTest.Models;

namespace RemittanceTest.Services
{
    public class RemittanceService : IRemittanceService
    {
        // 模擬資料庫 (靜態變數確保跨 Request 資料一致)
        private static readonly List<Remittance> _db = new()
        {
            new Remittance { Id = 1, AccountName = "測試企業A", Amount = 50000, Status = 0 },
            new Remittance { Id = 2, AccountName = "測試企業B", Amount = 12000, Status = 1 }, // 不可取消
            new Remittance { Id = 3, AccountName = "測試企業C", Amount = 30000, Status = 0 }
        };

        // 提示：如何確保多執行緒下的資料安全？
        private static readonly object _lockObj = new object();

        public (bool IsSuccess, string Message) CancelRemittance(int id)
        {
            // TODO: 請在此處實作「取消」的商業邏輯與防併發檢核
            lock (_lockObj)
            {
                var remittance = _db.FirstOrDefault(item => item.Id == id);

                if (remittance is null)
                {
                    return (false, "找不到匯款資料");
                }

                if (remittance.Status != 0)
                {
                    return (false, "只有待覆核的匯款可以取消");
                }

                remittance.Status = 9;

                return (true, "匯款已取消");
            }

            throw new NotImplementedException();
        }
    }
}