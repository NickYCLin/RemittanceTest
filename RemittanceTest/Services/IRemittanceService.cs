namespace RemittanceTest.Services
{
    public interface IRemittanceService
    {
        // 回傳 Tuple，包含是否成功與訊息
        (bool IsSuccess, string Message) CancelRemittance(int id);
    }
}