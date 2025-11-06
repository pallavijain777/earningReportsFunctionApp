# 💼 Earnings Reports Analyzer  
### _AI-Powered Financial Insight Engine — Built with Azure OpenAI, Semantic Kernel & Serverless Functions_

---

## 🧭 Overview
**Earnings Reports Analyzer** is a cloud-native, serverless AI system that automatically processes company earnings reports (PDFs), summarizes financial performance, and performs sentiment analysis using **Azure OpenAI + Semantic Kernel**.  
All secrets and connection strings are securely managed via **Azure Key Vault**, and results are persisted in **Azure SQL Database** for downstream analytics and reporting.

---

## 🚀 Tech Stack
| Layer | Technologies |
|:------|:--------------|
| **Compute / Orchestration** | Azure Functions (.NET 8 Timer Trigger) |
| **AI Orchestration** | Microsoft Semantic Kernel + Azure OpenAI (GPT-4 family) |
| **Storage** | Azure Blob Storage (Reports) |
| **Database** | Azure SQL Server (EF Core) |
| **Secrets & Config** | Azure Key Vault |
| **PDF Parsing** | PdfPig .NET Library |

---

## 🌟 Key Features
| ✅ Feature | 💡 Description |
|:-----------|:---------------|
| **Serverless AI Processing** | Automatically detects & processes new reports via a timer-triggered Azure Function |
| **Semantic Kernel Integration** | Uses SK plugins for Summarization & Sentiment Analysis |
| **Secure Configuration** | All secrets stored and fetched from Azure Key Vault |
| **Data Persistence** | Stores metadata, summaries & sentiment in Azure SQL Server (via EF Core) |
| **Idempotent Execution** | Prevents re-processing of already analyzed reports (using blob metadata tags) |
| **Cloud-Native Stack** | Built with Azure Functions, OpenAI, SQL, Blob Storage & Key Vault |

---

## 🏗️ System Architecture

![System Architecture](A_flowchart-style_digital_diagram_presents_a_cloud.png)

### **Processing Flow**
1. **Upload** company reports (`company/quarter/report.pdf`) to **Azure Blob Storage**.  
2. **Azure Function (Orchestrator)** triggers automatically (e.g., daily at 12 PM UTC).  
3. Function extracts text from PDF using **PdfPig**.  
4. Text is passed to **Semantic Kernel**, which calls Azure OpenAI (GPT-4) via two plugins:  
   - 🧩 Summarize Earnings Report Plugin  
   - 💬 Sentiment Analysis Plugin  
5. Results (summary + sentiment score) are stored in **Azure SQL Database** through EF Core.  
6. Secrets and connection strings are securely resolved from **Azure Key Vault**.  
7. Processed reports are marked to prevent re-processing.

---

## 🧠 AI Capabilities
| Capability | Description |
|:------------|:-------------|
| **Summarization** | Generates 8–10 line financial summaries highlighting revenue, profit/loss, and future outlook |
| **Sentiment Scoring** | Scores each report from 1–10 and provides a brief analysis rationale |
| **Self-Healing Retries** | Handles transient failures and logs Processing Runs for diagnostics |
| **Extensible Plugin Design** | Easy to add custom Semantic Kernel functions for new AI tasks |

---

## 🔐 Security
- Secrets and API keys never hard-coded — all resolved from **Azure Key Vault**  
- **Managed Identity** support for production deployments  
- **EF Core + Parameterized Queries** prevent SQL injection  
- Blob metadata used for idempotency and audit logging  

---

## ⚙️ Deployment
1. Create an **Azure Function App** (.NET 8 Isolated Process).  
2. Set up **Azure Blob Storage**, **Azure SQL Database**, and **Azure Key Vault**.  
3. Add these secrets in Key Vault:  
   - `AZURE-OPENAI-KEY`  
   - `AZURE-OPENAI-ENDPOINT`  
   - `AZURE-OPENAI-DEPLOYMENT`  
   - `SQL-CONNECTION-STRING`  
   - `AZURE-STORAGE-CONNECTION`  
4. Deploy via Visual Studio or GitHub Actions using `func azure functionapp publish`.  
5. Verify logs in Azure Monitor for function executions.  

---

## 📊 Sample Output
```json
{
  "score": 8,
  "brief": "The earnings report reflects strong performance with rising revenues, improved margins, and positive management commentary on AI investments."
}
