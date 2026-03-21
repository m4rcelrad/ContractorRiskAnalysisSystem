# Ohlson O-Score

## Overview

Developed by James Ohlson in 1980, the O-Score is a probabilistic model based on logistic regression. Unlike the Altman
models which provide a score, the Ohlson model estimates the **actual probability of bankruptcy** ($P$).

## Mathematical Model

The calculation involves a logit value ($O$), which is then converted into a probability:

$$O = -1.32 - 0.407 \cdot SIZE + 6.03 \cdot TLTA - 1.43 \cdot WCTA + 0.0757 \cdot CLCA - 2.37 \cdot NITA - 1.83 \cdot FUTL + 0.285 \cdot INTWO - 1.72 \cdot OENEG$$

### Probability Calculation

The probability of bankruptcy is determined by:
$$P = \frac{1}{1 + e^{-O}}$$

### Components

* **SIZE**: $\log_{10}(\frac{\text{Total Assets}}{\text{GNP Price Index}})$.
* **TLTA**: Total Liabilities / Total Assets.
* **WCTA**: Working Capital / Total Assets.
* **CLCA**: Current Liabilities / Current Assets.
* **NITA**: Net Income / Total Assets.
* **FUTL**: Funds From Operations / Total Liabilities.
* **INTWO**: 1.0 if Net Income was negative for the last two years, else 0.0.
* **OENEG**: 1.0 if Total Liabilities > Total Assets, else 0.0.

## Interpretation (Risk Levels)

| Probability (P)     | Risk Level   | Description                                                         |
|:--------------------|:-------------|:--------------------------------------------------------------------|
| **P < 0.20**        | **Low**      | Financially healthy with very low probability of bankruptcy.        |
| **0.20 ≤ P ≤ 0.50** | **Moderate** | Uncertain financial state; further behavioral analysis recommended. |
| **P > 0.50**        | **Critical** | High risk of bankruptcy; severe financial distress detected.        |
