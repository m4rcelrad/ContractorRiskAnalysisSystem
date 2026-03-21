# Altman Z''-Score (Double Prime)

## Overview

The Z''-Score is a modified version of the original Altman model, specifically adapted for **private companies** and *
*non-manufacturing service firms**. It removes the asset turnover ratio ($X_5$) and uses book value instead of market
value
to make it applicable to companies not listed on stock exchanges.

## Mathematical Formula

The score is derived using four financial ratios:

$$Z'' = 6.56X_1 + 3.26X_2 + 6.72X_3 + 1.05X_4$$

## Financial Ratios

| Ratio  | Formula                               | Description                            |
|:-------|:--------------------------------------|:---------------------------------------|
| **X1** | Working Capital / Total Assets        | Liquidity measure.                     |
| **X2** | Retained Earnings / Total Assets      | Leverage/Profitability measure.        |
| **X3** | EBIT / Total Assets                   | Operating efficiency.                  |
| **X4** | Book Value Equity / Total Liabilities | Solvency measure for private entities. |

## Interpretation (Risk Levels)

| Score (Z'')           | Risk Level   | Description                                                            |
|:----------------------|:-------------|:-----------------------------------------------------------------------|
| **Z'' > 2.60**        | **Low**      | Solid financial standing with low probability of distress.             |
| **1.10 ≤ Z'' ≤ 2.60** | **Moderate** | Uncertain financial situation requiring manual analysis or monitoring. |
| **Z'' < 1.10**        | **Critical** | High probability of financial failure and high risk for cooperation.   |
