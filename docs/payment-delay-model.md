# Weighted Payment Delay Model

## Overview

The Weighted Payment Delay model assesses the behavioral risk of a contractor by analyzing their historical invoice
payment performance. Unlike financial statement models, this model focuses on operational reliability and cash flow
discipline.

## Calculation Logic

The model evaluates invoices that are either already paid or are currently past their due date. It calculates a weighted
average of delay days, where the "weight" is the invoice amount converted to a base currency.

### Delay Calculation per Invoice

For each relevant invoice, the delay is determined as follows:

- **Paid Invoices**: $\text{Delay} = \max(0, \text{Payment Date} - \text{Due Date})$
- **Unpaid (Past Due) Invoices**: $\text{Delay} = \max(0, \text{Current Date} - \text{Due Date})$

### Mathematical Formula

The final score ($D_w$) represents the weighted average delay in days:

$$D_w = \frac{\sum_{i=1}^{n} (\text{Delay}_i \cdot \text{Amount}_{i, \text{base}})}{\sum_{i=1}^{n} \text{Amount}_{i, \text{base}}}$$

Where:

* **$\text{Delay}_i$**: Number of days past the due date for invoice $i$.
* **$\text{Amount}_{i, \text{base}}$**: The invoice amount normalized to the base currency using historical exchange
  rates.

## Interpretation (Risk Levels)

The calculated weighted average delay is mapped to a specific risk category based on the following thresholds:

| Average Delay ($D_w$)     | Risk Level   | Description                                                 |
|:--------------------------|:-------------|:------------------------------------------------------------|
| **$D_w < 5$ days**        | **Low**      | Excellent payment discipline; minor or no delays.           |
| **$5 \le D_w < 14$ days** | **Moderate** | Noticeable delays; requires monitoring of liquidity.        |
| **$D_w \ge 14$ days**     | **Critical** | Chronic payment issues; high risk of default or insolvency. |

## Implementation Details

- **Currency Normalization**: Uses an external exchange rate service to ensure large invoices have a proportional impact
  on the score, regardless of the currency used.
- **Empty History Handling**: If no relevant invoices are found, the model defaults to a **Moderate** risk level to
  indicate a lack of behavioral data for a definitive assessment.
