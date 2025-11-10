# 💼 Earnings Reports Analyzer  
### _AI-Powered Financial Insight Engine — Azure OpenAI • Semantic Kernel • Serverless Functions_

---

## 🧠 Overview
**Earnings Reports Analyzer** is an end-to-end **AI automation** that turns unstructured PDF earnings reports into **structured, queryable insights**.  
It watches an Azure Blob container, extracts PDF text, orchestrates Azure OpenAI via **Semantic Kernel plugins** to produce a clean **summary** and **sentiment score**, then persists the results to **Azure SQL**.  
All sensitive values are pulled securely from **Azure Key Vault**.

---

## ⚙️ Tech Stack

| Layer | Technologies |
|:--|:--|
| **AI Reasoning** | Azure OpenAI via Semantic Kernel |
| **AI Orchestration** | Custom SK Plugins (Summarization, Sentiment) |
| **Compute** | Azure Functions (.NET 8, Timer Trigger) |
| **Storage** | Azure Blob Storage |
| **Database** | Azure SQL + EF Core 8 |
| **Secrets & Config** | Azure Key Vault |
| **PDF Parsing** | PdfPig (.NET) |
| **Observability** | Azure Application Insights |

---

## 🌟 AI Features

| 🤖 Feature | 💡 Description |
|:--|:--|
| **LLM Summarization** | Generates concise, analyst-style summaries with key metrics, risks, and outlook. |
| **Sentiment Intelligence** | Computes a 1–10 sentiment score with a brief rationale. |
| **Prompt Engineering** | Deterministic prompts with JSON-schema outputs for safe ingestion. |
| **Plugin Architecture** | Add new SK functions (e.g., KPI extraction, trend detection) with minimal wiring. |
| **Idempotent Processing** | Blob metadata prevents re-processing of already analyzed reports. |

---

## 🏗️ System Architecture

![System Architecture](https://github.com/pallavijain777/earningReportsFunctionApp/blob/master/ArchitectureDiagram.png?raw=true)

**Flow**
1. Upload `company/quarter/report.pdf` to **Azure Blob Storage**  
2. **Azure Function** (timer) runs once daily at **12:00 UTC**  
3. **PdfPig** extracts text from the PDF  
4. **Semantic Kernel** calls Azure OpenAI using two plugins:  
   - `SummarizeReport()`  
   - `AnalyzeSentiment()`  
5. Persist summary + sentiment to **Azure SQL** via EF Core  
6. Resolve secrets and connection strings from **Azure Key Vault**  
7. Tag blob to mark it **processed** (idempotency)

---

## 🧩 Semantic Kernel Plugin Layer

| Plugin | Function | Description |
|:--|:--|:--|
| **FinancialAnalysisPlugin** | `SummarizeReport()` | Produces structured, concise summaries suited for BI/analytics. |
| **FinancialAnalysisPlugin** | `AnalyzeSentiment()` | Returns sentiment score between 1-10 |

Each function is **prompt-engineered** for consistency and downstream safety.

---

## 🗄️ Database Schema Overview

| Table | Description |
|:--|:--|
| **Company** | Stores company info & identifiers |
| **FiscalQuarter** | Quarter/year metadata |
| **Report** | PDF upload record & processing status |
| **ReportInsight** | AI-generated summary & sentiment |
| **ProcessingRun** | Tokens, cost, latency, status for observability |

---

## 🔐 Security Highlights
- **No secrets in code** — everything comes from **Azure Key Vault**  
- Supports **Managed Identity** in production  
- EF Core with parameterization to **avoid injection**  
- Blob metadata used for **idempotency** and auditability  
- Deterministic JSON outputs to **reduce hallucination risk**

---

## 📊 Sample AI Output

```json
{
  "summary": "The company reported 12% YoY revenue growth driven by AI services demand. Margins improved despite FX pressures. Management expects continued investment in cloud and AI expansion.",
  "sentimentscore": 8
}
```

---

## 🚀 Deploy

1. **Provision Azure resources**
   - Function App (.NET 8)
   - Azure SQL Database
   - Azure Storage (Blob)
   - Azure Key Vault
   - Azure OpenAI resource

2. **Add Key Vault secrets**
   ```
   AZURE-OPENAI-KEY
   AZURE-OPENAI-ENDPOINT
   AZURE-OPENAI-DEPLOYMENT
   SQL-CONNECTION-STRING
   AZURE-STORAGE-CONNECTION
   ```

3. **App settings**
   - `KEY_VAULT_URL` → your vault URI  
   - Enable **System Assigned Managed Identity** for the Function App and grant **Key Vault Secrets User** (or appropriate) access.

4. **Publish the function**
   ```bash
   func azure functionapp publish <your-function-app-name>
   ```

5. **Schedule (host.json / attribute)**
   - Cron: `0 0 12 * * *`  → daily at **12:00 UTC**

6. **Monitor**
   - Azure Application Insights → traces, dependencies, exceptions

---

## 👤 Author
**Pallavi Jain** — Engineering Leader • Full-Stack Architect • Applied AI  
LinkedIn: https://linkedin.com/in/pallavijain7

> _Turning unstructured earnings reports into structured, explainable insights with Azure OpenAI and serverless AI orchestration._
