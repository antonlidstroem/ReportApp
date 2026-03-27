Det här är en omfattande och professionell projektplan för din PoC (Proof of Concept). Den är utformad för att vara helt fristående – om du klistrar in denna i en ny chattsession eller visar den för en kollega, kommer de att förstå exakt arkitekturen, målen och de tekniska begränsningarna.

---

# Projektplan: "Modern Reporting Engine PoC"

## 1. Övergripande Syfte
Att utvärdera och jämföra tre olika tekniska lösningar för att generera rapporter (PDF, Excel, PowerPoint) från en .NET-backend. Målet är att ersätta en föråldrad `StringBuilder`-baserad lösning med en modern, modulär arkitektur som stödjer grafer, tabeller och användardefinierade mallar.

## 2. Arkitektur & Systemdesign
Systemet byggs med en **Clean Architecture**-ansats för att separera data, affärslogik och presentation.



### Projektstruktur i Visual Studio (Solution)
1.  **`ReportApp.Core` (Class Library):**
    * **Data Models:** Entities för Arbetsmiljö-domänen (t.ex. `Survey`, `Question`, `Answer`, `Company`).
    * **Persistence:** SQLite med Entity Framework Core.
    * **Seed Data:** En robust mock-databas med realistiska värden för grafer och trender.
2.  **`ReportApp.AnalysisEngine` (Class Library):**
    * **Logic:** Beräkningsmotor som aggregerar data (t.ex. genomsnittsbetyg per avdelning, incidentfrekvens).
    * **ViewModels:** DTO:er (Data Transfer Objects) optimerade för rapportgenerering.
3.  **`ReportApp.WebAPI` (ASP.NET Core API):**
    * **Endpoints:** Hanterar anrop från Vue-frontend.
    * **Orchestration:** Dirigerar data till rätt export-provider (IronSuite, OpenSource eller jsreport).
    * **Benchmarking:** Middleware eller Helpers som mäter tid (`Stopwatch`) och minnesanvändning.
4.  **`ReportApp.Frontend` (Vue 3 + Vite):**
    * **Dashboard:** Jämförelsevy med prestandadata och funktionsmatris.
    * **Report Builder:** Gränssnitt där användaren väljer mall och moduler (t.ex. "Visa graf för fråga 1-5").

---

## 3. Testade Tekniska Stackar (De tre vinnarna)

### Spår 1: "The Enterprise Suite" (Iron Software)
* **Verktyg:** `IronPdf`, `IronXL`, `IronPpt`.
* **Strategi:** Enhetlig lösning från en leverantör. Använder HTML-till-PDF för dokument.
* **Utmaning:** Säkerställa att grafer i Excel/PPT blir redigerbara objekt.

### Spår 2: "The Best-of-Breed Mix" (Open Source/Low Cost)
* **Verktyg:** `QuestPDF` (PDF), `ClosedXML` (Excel), `ShapeCrawler` (PPT).
* **Strategi:** Kod-centrerad approach.
* **PDF:** Byggs med C# Fluent API.
* **Excel/PPT:** Manipulerar mallfiler (.xlsx / .pptx) för att behålla snygg design och redigerbara grafer.

### Spår 3: "The Web-Standard Engine" (jsreport)
* **Verktyg:** `jsreport.Local` (NuGet).
* **Strategi:** Server-side rendering av HTML/JS med Chromium.
* **Grafer:** Använder `Chart.js` och `Ag Grid` direkt i rapporten (identiskt med webbvyn).

---

## 4. Funktionskrav för PoC
* **Dynamiska Mallar:** Minst 3 malltyper (Dashboard, Trend, Detalj).
* **Modularitet:** Användaren ska kunna välja vilka datafält/frågor som ska inkluderas via Vue-interfacet.
* **Graf-stöd:** Varje export måste innehålla minst ett cirkeldiagram och ett linjediagram.
* **Prestandamätning:** Varje export ska logga:
    1.  Tid för datahämtning (ms).
    2.  Tid för rendering/generering (ms).
    3.  Total filstorlek (KB).

---

## 5. Implementationssteg (Backlog)

### Fas 1: Fundament (Core & Data)
- [ ] Skapa Solution och projekt i VS 2022/2026.
- [ ] Definiera entiteter i `Core` (Survey, Question, Response).
- [ ] Implementera SQLite + EF Core och skriv en Seed-metod som genererar 100+ rader data.
- [ ] Skapa `AnalysisEngine` med metoder för att räkna ut %-andelar och trender.

### Fas 2: Backend-Exporters (Provider-mönster)
- [ ] Skapa ett interface `IReportGenerator` med metoderna `GeneratePdf`, `GenerateExcel`, `GeneratePpt`.
- [ ] Implementera **Spår 1 (Iron)**.
- [ ] Implementera **Spår 2 (Open Source)**.
- [ ] Implementera **Spår 3 (jsreport)**.

### Fas 3: Webb-API & Benchmarking
- [ ] Skapa controllers för att trigga exporter.
- [ ] Implementera logik för att mäta `ExecutionTime` och returnera detta i API-svaret (header eller JSON-body).

### Fas 4: Vue Frontend
- [ ] Sätt upp Vue 3 med Tailwind CSS (eller liknande) för snabb styling.
- [ ] Bygg "Comparison Dashboard" (Tabell med priser, funktioner, fördelar).
- [ ] Bygg "Report Configurator" (Checkboxes för att välja moduler).
- [ ] Integrera nedladdning och visa prestandagraf (t.ex. ett stapeldiagram som jämför renderingstid).

---

## 6. Definition of Done
PoC:en anses klar när man från Vue-frontenden kan klicka på en knapp för valfri provider och få ut en PDF, Excel och PPT som alla innehåller korrekt aggregerad data och visuella grafer, samt att prestandaskillnaden visas tydligt på skärmen.

---

**Nästa steg för dig:** Spara denna text i en fil (t.ex. `README_POC.md`). När du är redo att börja koda, säg bara till så börjar vi med **Fas 1: Fundament (Core & Data)** och skriver koden för modellerna och databasen!