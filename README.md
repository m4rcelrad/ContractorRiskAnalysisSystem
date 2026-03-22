# Contractor Risk Analysis System (CRAS)

[![.NET CI](https://github.com/m4rcelrad/contractorriskanalysissystem/actions/workflows/dotnet-ci.yml/badge.svg)](https://github.com/m4rcelrad/contractorriskanalysissystem/actions/workflows/dotnet-ci.yml)
![.NET](https://img.shields.io/badge/.NET-9.0-512bd4)
![Blazor](https://img.shields.io/badge/Blazor-WASM-512bd4)
![Docker](https://img.shields.io/badge/Docker-Ready-2496ed)

**CRAS** is an automated framework for financial risk assessment, designed to evaluate contractor stability through a combination of econometric models and behavioral analysis. The system provides a multi-dimensional view of financial health by processing structured financial statements alongside historical payment discipline.

## Core Capabilities

* **Advanced Risk Engine**: Implements various bankruptcy prediction models, including **Altman Z-Score** and **Ohlson O-Score**, to detect early signs of financial distress.
* **Behavioral Payment Analysis**: Features a weighted delay model that evaluates contractor reliability based on invoice history, utilizing real-time NBP exchange rates for currency normalization.
* **Dynamic Reporting**: Generates detailed, section-based PDF credit reports using QuestPDF, providing clear breakdowns of financial ratios and assessment results.
* **Reactive Monitoring**: A Blazor WebAssembly dashboard providing a unified view of the contractor portfolio, backed by a resilient state management layer.

## Tech Stack

- **Backend**: ASP.NET Core 9 Web API, Entity Framework Core (PostgreSQL).
- **Frontend**: Blazor WebAssembly, Bootstrap 5.
- **Infrastructure**: Docker & Docker Compose, QuestPDF, FluentValidation.
- **Testing & CI**: xUnit, Moq, GitHub Actions.

## Execution

The entire environment is containerized for seamless deployment and testing.

1.  **Start Services**:
    ```bash
    docker compose up -d
    ```
2.  **Endpoints**:
    - **Web Interface**: `http://localhost:5265`
    - **API Documentation**: `http://localhost:5250/swagger`

## Methodology Documentation

For in-depth explanations of the econometric formulas and logic used in the risk assessment process, refer to the [Documentation](./docs/README.md) folder.
