# Altman Z-Score

## Overview

The Altman Z-Score is a classic multivariate formula used to predict the probability that a firm will go bankrupt within
two years. Originally developed by Edward Altman in 1968, this specific version is calibrated for **public manufacturing
companies**.

## Mathematical Formula

The model calculates a score based on five key financial ratios:

$$Z = 1.2X_1 + 1.4X_2 + 3.3X_3 + 0.6X_4 + 1.0X_5$$

## Financial Ratios

| Ratio  | Description                             | Formula                                     |
|:-------|:----------------------------------------|:--------------------------------------------|
| **X1** | Working Capital / Total Assets          | Measures liquid assets in relation to size. |
| **X2** | Retained Earnings / Total Assets        | Measures cumulative profitability.          |
| **X3** | EBIT / Total Assets                     | Measures operating efficiency.              |
| **X4** | Market Value Equity / Total Liabilities | Measures market dimension of solvency.      |
| **X5** | Sales / Total Assets                    | Measures asset turnover (efficiency).       |

## Interpretation (Risk Levels)

| Score (Z)           | Risk Level   | Description                                                               |
|:--------------------|:-------------|:--------------------------------------------------------------------------|
| **Z > 2.99**        | **Low**      | Indicates a low probability of financial distress; contractor is healthy. |
| **1.81 ≤ Z ≤ 2.99** | **Moderate** | Indicates an uncertain financial situation; requires further monitoring.  |
| **Z < 1.81**        | **Critical** | Indicates a high probability of bankruptcy or severe financial problems.  |
