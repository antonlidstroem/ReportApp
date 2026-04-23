<template>
  <ApiHealthBanner />

  <nav class="navbar navbar-expand-lg navbar-dark bg-dark mb-4">
    <div class="container">
      <router-link class="navbar-brand" to="/">Reporting Engine PoC</router-link>
      <div class="navbar-nav ms-auto">
        <router-link class="nav-link" to="/providers/jsreport">jsreport</router-link>
        <router-link class="nav-link" to="/providers/syncfusion">Syncfusion</router-link>
        <router-link class="nav-link" to="/providers/js-sync">Hybrid A</router-link>
        <router-link class="nav-link" to="/providers/play-sync">Hybrid B</router-link>
        <router-link class="nav-link" to="/providers/telerik">Telerik</router-link>
        <router-link class="nav-link" to="/providers/devexpress">DevExpress</router-link>
      </div>
    </div>
  </nav>

  <main class="container">
    <router-view />
  </main>
</template>

<script setup lang="ts">
import ApiHealthBanner from './components/ApiHealthBanner.vue'
</script>

<style>
body {
  background-color: #0f172a;
  color: #f8fafc;
}
:root {
  --accent: #3b82f6;
  --surface: #1e293b;
  --surface-2: #334155;
  --border: #475569;
  --text: #f8fafc;
  --text-muted: #94a3b8;
}
</style>
