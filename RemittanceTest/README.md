# 企業整批匯款管理系統 - 核心邏輯測試 (.NET 8 & Vue 3)

## 專案背景與架構規範
這是一個簡化版的銀行匯款管理系統。為了符合企業級開發規範，本專案要求**嚴格的職責分離（Separation of Concerns）**：
- **Controller**：只負責接收 HTTP Request、驗證輸入、呼叫 Service，以及回傳正確的 HTTP Status Code (200, 400, 404)。
- **Service**：負責處理核心商業邏輯與資料庫狀態檢核。

## 核心任務與商業邏輯
當匯款資料尚未送往主機前，經辦人員可執行取消。
- `Status` 定義：`0: 待覆核`, `1: 交易中`, `2: 已完成`, `9: 已取消`。
- **絕對規則**：**只有狀態為 `0` 的資料才可以被取消（更新為 `9`）**。
- **資安要求**：必須防範前端連點造成的併發（Concurrency）問題，避免重複扣款或狀態異常。

## 您的實作範圍 (TODOs)
1. **依賴注入配置 (`Program.cs`)**
   - 請將 Service 註冊到 DI 容器中。
2. **服務層 (`Services/RemittanceService.cs`)**
   - 實作 `IRemittanceService` 介面，完成 `CancelRemittance` 的商業邏輯與防呆。
3. **控制層 (`Controllers/RemittanceController.cs`)**
   - 透過建構子注入 Service。
   - 呼叫 Service 並根據結果回傳相對應的 `IActionResult`。
4. **前端串接 (`wwwroot/index.html`)**
   - 完成 Vue 內的 `cancelItem(id)` 函數，並根據 API 回傳結果更新 UI。
5. **架構問答 (請直接回答於下方)**
   - 雖然本測試使用 In-Memory List 模擬資料庫，但在正式的 SQL Server 環境中，您會如何撰寫 T-SQL 或 Entity Framework Core 程式碼，來確保「多個使用者同時對同一筆資料按下取消」時，不會發生 Race Condition？
   - **您的回答：** (請在此作答...)
T-SQL: 我會用條件式處理取消動作，讓資料庫在同一個UPDATE陳述式裡同時完成狀態檢查與狀態的更新。只有目前狀態仍是0的資料，才會被更新為9，再透過筆數判斷是否成功。
```
BEGIN TRANSACTION;

UPDATE Remittances WITH (UPDLOCK, ROWLOCK)
SET
	Status = 9,
	UpdatedAt = SYSUTCDATETIME()
WHERE Id = @Id
AND Status = 0;

IF @@ROWCOUNT = 1
BEGIN
	COMMIT TRANSACTION;
	SELECT 1 AS IsSuccess, N'匯款資料已成功取消。' AS Message;
END
ELSE
BEGIN
	ROLLBACK TRANSACTION;

	IF EXISTS (SELECT 1 FROM Remittances WHERE Id = @Id)
		SELECT 0 AS IsSuccess, N'只有待覆核狀態的匯款資料可以取消。' AS Message;
	ELSE
		SELECT 0 AS IsSuccess, N'找不到指定的匯款資料。' AS Message;
END
```
Entity Framework Core: 我會使用ExecuteUpdateAsync產生同樣概念的條件式更新
```
var affectedRows = await dbContext.Remittances
    .Where(remittance => remittance.Id == id && remittance.Status == 0)
    .ExecuteUpdateAsync(setters => setters
        .SetProperty(remittance => remittance.Status, 9)
        .SetProperty(remittance => remittance.UpdatedAt, DateTime.UtcNow));

if (affectedRows == 1)
{
    return CancelRemittanceStatus.Success;
}
```
做法的重點是不要先查詢再更新，因為查詢完成與更新送出之間，可能被其他請求插隊，所以將條件放入`UPDATE ... WHERE Status = 0`
讓SQL Server同一時間只有第一個成功符合的請求才能完成取消，其餘的請求會因影響筆數為0，而被視為狀態已變更或是不可取消。

## 提交要求
1. 請使用 GitHub Public Repository 提交。
2. **請務必多次 Commit**（例如：API完成、前端完成、Bug修復），讓我們了解您的開發思路。