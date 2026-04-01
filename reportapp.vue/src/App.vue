<template>
  <nav class="navbar navbar-expand-lg navbar-dark bg-dark mb-4">
    <div class="container">
      <router-link class="navbar-brand" to="/">Reporting Engine PoC</router-link>
    </div>
  </nav>

  <main class="container">
    <router-view />
  </main>
</template>

<style>
/* Globala justeringar för att Bootstrap ska lira med din mörka design */
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
