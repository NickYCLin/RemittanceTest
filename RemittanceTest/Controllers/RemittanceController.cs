using Microsoft.AspNetCore.Mvc;
using RemittanceTest.Models;

namespace RemittanceTest.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RemittanceController : ControllerBase
    {
        // TODO: 1. 請透過建構子注入 (Constructor Injection) 引入 IRemittanceService

        [HttpPost("{id}/cancel")]
        public IActionResult Cancel(int id)
        {
            // TODO: 2. 呼叫 Service 執行取消邏輯
            // TODO: 3. 根據 Service 回傳的結果，回傳相對應的 HTTP 狀態碼 (Ok / BadRequest / NotFound)

            return Ok();
        }
    }
}