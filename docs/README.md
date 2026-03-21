# Risk Methodology Documentation

This directory contains detailed technical specifications for the risk assessment models implemented in the **Contractor
Risk Analysis System**.

## Table of Contents


1. **[Altman Z-Score (Original)](altman-z-score.md)** – Calibrated for public manufacturing companies.
2. **[Altman Z''-Score (Double Prime)](altman-z-double-prime-score.md)** – Adapted for private and non-manufacturing
   firms.
3. **[Ohlson O-Score](ohlson-o-score.md)** – A probabilistic model using logistic regression.
4. **[Weighted Payment Delay](payment-delay-model.md)** – A behavioral model based on real-time invoice history.

---

## Risk Level Definitions

Across all models, the system standardizes results into three core categories defined in the `RiskLevel` enumeration:

| Level        | Technical Meaning                                   | Business Implication                              |
|:-------------|:----------------------------------------------------|:--------------------------------------------------|
| **Low**      | High financial health; low probability of distress. | Approved for standard cooperation terms.          |
| **Moderate** | Uncertain or "Grey Zone" situation.                 | Requires manual review or increased monitoring.   |
| **Critical** | High probability of bankruptcy or default.          | High risk; restrictive payment terms recommended. |

---

## Risk Aggregation Strategy

The system uses a **Decoupled Aggregation Engine**. While each model provides an independent assessment, the final
`OverallRisk` is calculated using a strategy-based approach.

### Current Implementation: Majority Vote

By default, the system employs the `MajorityVoteAggregation` strategy. It collects results from all active models and
selects the risk level that appears most frequently. This prevents a single outlier model from disproportionately
affecting the final assessment.

**Note:** The architecture allows for easy swapping of strategies via Dependency Injection.

---

## Data Requirements

All financial models rely on the `FinancialStatement` entity, which requires the following key metrics for a successful
assessment:

- **Liquidity**: Working Capital, Current Assets, Current Liabilities.
- **Profitability**: Retained Earnings, EBIT, Net Income.
- **Solvency**: Total Assets, Total Liabilities, Market/Book Value of Equity.
- **Efficiency**: Annual Sales.
