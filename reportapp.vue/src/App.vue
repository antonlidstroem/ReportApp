<script setup>
  import { ref, onMounted } from 'vue';
  import axios from 'axios';

  // Konfiguration för API
  const api = axios.create({ baseURL: 'http://localhost:5000/api' });

  const surveys = ref([]);
  const selectedSurvey = ref('');
  const startDate = ref('');
  const endDate = ref('');
  const metrics = ref([]);

  onMounted(async () => {
    try {
      const res = await api.get('/reports/surveys');
      surveys.value = res.data;
    } catch (e) {
      console.error("Kunde inte ladda enkäter", e);
    }
  });

  const handleExport = async (provider, format) => {
    if (!selectedSurvey.value) return alert("Välj en enkät först!");

    try {
      const res = await api.post(
        `/reports/export/${provider}/${format}/${selectedSurvey.value}`,
        null,
        {
          params: { start: startDate.value, end: endDate.value },
          responseType: 'blob'
        }
      );

      const time = res.headers['x-generation-time-ms'];
      metrics.value.push({ provider, format, time, timestamp: new Date().toLocaleTimeString() });

      // Ladda ner filen
      const url = window.URL.createObjectURL(new Blob([res.data]));
      const link = document.createElement('a');
      link.href = url;
      link.setAttribute('download', `Rapport_${provider}.${format}`);
      document.body.appendChild(link);
      link.click();
    } catch (e) {
      alert("Export misslyckades. Kontrollera att backend körs och att Providern är registrerad.");
    }
  };
</script>

<template>
  <div class="container py-5">
    <header class="mb-5">
      <h1 class="display-5 fw-bold text-primary">Reporting Engine PoC</h1>
      <p class="lead text-muted">Jämförelse av exportteknologier för .NET & Vue</p>
    </header>

    <div class="card shadow-sm mb-4">
      <div class="card-body">
        <div class="row g-3">
          <div class="col-md-4">
            <label class="form-label fw-semibold">Välj Enkät</label>
            <select v-model="selectedSurvey" class="form-select">
              <option value="">-- Välj från databasen --</option>
              <option v-for="s in surveys" :key="s.id" :value="s.id">{{ s.title }}</option>
            </select>
          </div>
          <div class="col-md-4">
            <label class="form-label fw-semibold">Från datum</label>
            <input type="date" v-model="startDate" class="form-control">
          </div>
          <div class="col-md-4">
            <label class="form-label fw-semibold">Till datum</label>
            <input type="date" v-model="endDate" class="form-control">
          </div>
        </div>
      </div>
    </div>

    <div class="row">
      <div class="col-lg-8">

        <div class="card mb-4 border-start border-primary border-4">
          <div class="card-body">
            <div class="d-flex justify-content-between align-items-center mb-3">
              <h5 class="card-title mb-0">Iron Suite</h5>
              <span class="badge bg-primary text-uppercase">Enterprise</span>
            </div>
            <p class="card-text text-muted small">Allt-i-ett bibliotek för PDF, Excel och PowerPoint.</p>
            <div class="btn-group">
              <button @click="handleExport('IronSuite', 'pdf')" class="btn btn-outline-primary">PDF</button>
              <button @click="handleExport('IronSuite', 'excel')" class="btn btn-outline-success">Excel</button>
              <button @click="handleExport('IronSuite', 'ppt')" class="btn btn-outline-danger">PPT</button>
            </div>
          </div>
        </div>

        <div class="card mb-4 border-start border-info border-4">
          <div class="card-body">
            <div class="d-flex justify-content-between align-items-center mb-3">
              <h5 class="card-title mb-0">QuestPDF + Open Source</h5>
              <span class="badge bg-info text-dark text-uppercase">Best-of-breed</span>
            </div>
            <p class="card-text text-muted small">QuestPDF för layout, ClosedXML för Excel, ShapeCrawler för PPT.</p>
            <div class="btn-group">
              <button @click="handleExport('QuestOpenSource', 'pdf')" class="btn btn-outline-primary">PDF</button>
              <button @click="handleExport('QuestOpenSource', 'excel')" class="btn btn-outline-success">Excel</button>
              <button @click="handleExport('QuestOpenSource', 'ppt')" class="btn btn-outline-danger">PPT</button>
            </div>
          </div>
        </div>

        <div class="card mb-4 border-start border-warning border-4">
          <div class="card-body">
            <div class="d-flex justify-content-between align-items-center mb-3">
              <h5 class="card-title mb-0">jsreport</h5>
              <span class="badge bg-warning text-dark text-uppercase">Web-Standard</span>
            </div>
            <p class="card-text text-muted small">Använder Chromium för att rendera HTML/JS (Chart.js).</p>
            <div class="btn-group">
              <button @click="handleExport('jsreport', 'pdf')" class="btn btn-outline-primary">PDF</button>
              <button @click="handleExport('jsreport', 'excel')" class="btn btn-outline-success">Excel</button>
            </div>
          </div>
        </div>
      </div>

      <div class="col-lg-4">
        <div class="card bg-dark text-light shadow">
          <div class="card-header border-secondary d-flex align-items-center">
            <span class="me-2">⚡</span> <h6 class="mb-0">Prestanda-logg</h6>
          </div>
          <div class="card-body p-0" style="max-height: 400px; overflow-y: auto;">
            <table class="table table-dark table-hover mb-0 small">
              <thead>
                <tr>
                  <th>Provider</th>
                  <th>Format</th>
                  <th class="text-end">Tid</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="(m, i) in metrics" :key="i">
                  <td>{{ m.provider }}</td>
                  <td>{{ m.format }}</td>
                  <td class="text-end text-warning fw-bold">{{ m.time }}ms</td>
                </tr>
                <tr v-if="metrics.length === 0">
                  <td colspan="3" class="text-center text-muted py-4">Inga exporter körda än</td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
  /* Lite extra styling för att det ska se "techy" ut */
  .btn-group .btn {
    min-width: 80px;
  }

  .card {
    transition: transform 0.2s;
  }

    .card:hover {
      transform: translateY(-2px);
    }
</style>
