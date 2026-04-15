<template>
  <div class="routing-viz">
    <div class="rv-header">
      <span class="rv-eyebrow">ARCHITECTURE</span>
      <h3>How Requests Are Routed</h3>
      <p>Each format triggers a different engine. The C# controller decides at runtime.</p>
    </div>

    <div class="rv-diagram">
      <!-- User request -->
      <div class="rv-node user-node" @click="selectFormat('pdf')">
        <div class="rn-icon">👤</div>
        <div class="rn-label">Client Request</div>
        <div class="format-selector">
          <button
            v-for="f in formats"
            :key="f.id"
            :class="['fs-btn', { active: activeFormat === f.id }]"
            @click.stop="selectFormat(f.id)"
          >{{ f.icon }} {{ f.label }}</button>
        </div>
      </div>

      <!-- Arrow to controller -->
      <div class="rv-arrow">
        <div class="arrow-line"></div>
        <div class="arrow-label">POST /api/reports/export</div>
        <div class="packet" :class="{ animating: isAnimating }"></div>
      </div>

      <!-- Controller -->
      <div class="rv-node controller-node">
        <div class="rn-icon">⚙️</div>
        <div class="rn-label">ReportsController</div>
        <div class="rn-code">
          <code>format switch {</code>
          <code v-for="f in formats" :key="f.id" :class="{ highlight: activeFormat === f.id }">
            "{{ f.id }}" → {{ getEngine(f.id) }}
          </code>
          <code>}</code>
        </div>
      </div>

      <!-- Split to engines -->
      <div class="rv-split">
        <div
          v-for="engine in engines"
          :key="engine.id"
          class="rv-engine-branch"
          :class="{ active: isActiveEngine(engine.id) }"
        >
          <div class="branch-line"></div>
          <div class="rv-node engine-node" :style="`--ec: ${engine.color}`">
            <div class="rn-icon">{{ engine.icon }}</div>
            <div class="rn-label">{{ engine.name }}</div>
            <div class="rn-desc">{{ engine.desc }}</div>
            <div v-if="isActiveEngine(engine.id)" class="active-pulse"></div>
          </div>
        </div>
      </div>

      <!-- Output -->
      <div class="rv-output">
        <div
          v-for="f in formats"
          :key="f.id"
          class="output-file"
          :class="{ active: activeFormat === f.id }"
        >
          <span class="of-icon">{{ f.icon }}</span>
          <span class="of-label">{{ f.output }}</span>
        </div>
      </div>
    </div>

    <!-- Code snippet -->
    <div class="rv-code-panel">
      <div class="rcp-header">
        <span>PlaySyncHybridProvider.cs — Router Logic</span>
        <span class="rcp-lang">C#</span>
      </div>
      <pre class="rcp-code"><code>{{ routerCode }}</code></pre>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'

const props = defineProps<{
  pdfEngine: 'jsreport' | 'playwright'
}>()

const activeFormat = ref('pdf')
const isAnimating = ref(false)

function selectFormat(fmt: string) {
  activeFormat.value = fmt
  isAnimating.value = true
  setTimeout(() => { isAnimating.value = false }, 800)
}

const formats = [
  { id: 'pdf', label: 'PDF', icon: '📕', output: 'report.pdf' },
  { id: 'excel', label: 'Excel', icon: '📗', output: 'report.xlsx' },
  { id: 'ppt', label: 'PPT', icon: '📘', output: 'report.pptx' },
]

const engines = computed(() => {
  const pdfEngine = props.pdfEngine === 'playwright'
    ? { id: 'playwright', name: 'Playwright', icon: '🎭', desc: 'Chromium PDF', color: '#10b981' }
    : { id: 'jsreport', name: 'jsreport', icon: '🌐', desc: 'Chromium + Handlebars', color: '#a855f7' }

  return [
    pdfEngine,
    { id: 'syncfusion-excel', name: 'Syncfusion XlsIO', icon: '📊', desc: 'Native .xlsx', color: '#14b8a6' },
    { id: 'syncfusion-ppt', name: 'Syncfusion Presentation', icon: '📋', desc: 'Native .pptx', color: '#14b8a6' },
  ]
})

function getEngine(fmt: string): string {
  if (fmt === 'pdf') return props.pdfEngine === 'playwright' ? 'PlaywrightProvider' : 'JsReportProvider'
  if (fmt === 'excel') return 'SyncfusionExcelGen'
  return 'SyncfusionPptGen'
}

function isActiveEngine(engineId: string): boolean {
  if (activeFormat.value === 'pdf') {
    return engineId === (props.pdfEngine === 'playwright' ? 'playwright' : 'jsreport')
  }
  if (activeFormat.value === 'excel') return engineId === 'syncfusion-excel'
  if (activeFormat.value === 'ppt') return engineId === 'syncfusion-ppt'
  return false
}

const routerCode = computed(() => props.pdfEngine === 'playwright' ? `// PlaySyncHybridProvider.cs
public string Name => "play-sync";

// PDF → Playwright (lightweight Chromium)
public Task<byte[]> GeneratePdfAsync(ReportDataViewModel data) =>
    _playwright.GeneratePdfAsync(BuildHtml(data));

// Excel → Syncfusion XlsIO (native, with formulas)
public Task<byte[]> GenerateExcelAsync(ReportDataViewModel data) =>
    _excel.GenerateAsync(data);

// PPT → Syncfusion Presentation (native, editable charts)
public Task<byte[]> GeneratePptAsync(ReportDataViewModel data) =>
    _ppt.GenerateAsync(data);` :
`// JsSyncHybridProvider.cs
public string Name => "js-sync";

// PDF → jsreport (Handlebars + Chart.js)
public Task<byte[]> GeneratePdfAsync(ReportDataViewModel data) =>
    _pdf.GenerateAsync(data, DefaultTemplate);

// PDF with custom HTML template
public Task<byte[]> GeneratePdfFromTemplateAsync(
    ReportDataViewModel data, string html) =>
    _pdf.GenerateAsync(data, html);

// Excel → Syncfusion XlsIO (native, with formulas)
public Task<byte[]> GenerateExcelAsync(ReportDataViewModel data) =>
    _excel.GenerateAsync(data);`)
</script>

<style scoped>
.routing-viz {
  background: #06101e;
  border: 1px solid rgba(255,255,255,0.06);
  border-radius: 14px;
  overflow: hidden;
  margin-bottom: 24px;
}

.rv-header {
  padding: 24px 28px 16px;
  border-bottom: 1px solid rgba(255,255,255,0.05);
}

.rv-eyebrow {
  font-size: 10px;
  letter-spacing: 3px;
  color: #475569;
  display: block;
  margin-bottom: 4px;
}

.rv-header h3 {
  font-size: 16px;
  font-weight: 700;
  color: #fff;
  margin: 0 0 4px;
}

.rv-header p {
  font-size: 12px;
  color: #475569;
  margin: 0;
}

.rv-diagram {
  padding: 28px;
  display: flex;
  align-items: center;
  gap: 0;
  overflow-x: auto;
}

.rv-node {
  background: #0d1a2e;
  border: 1px solid rgba(255,255,255,0.08);
  border-radius: 10px;
  padding: 16px;
  text-align: center;
  flex-shrink: 0;
  min-width: 140px;
  position: relative;
}

.rn-icon { font-size: 24px; margin-bottom: 6px; }
.rn-label { font-size: 12px; font-weight: 700; color: #e2e8f0; margin-bottom: 8px; }

.format-selector {
  display: flex;
  flex-direction: column;
  gap: 4px;
  margin-top: 8px;
}

.fs-btn {
  font-size: 11px;
  padding: 4px 8px;
  border-radius: 5px;
  border: 1px solid rgba(255,255,255,0.1);
  background: transparent;
  color: #64748b;
  cursor: pointer;
  transition: all 0.12s;
}

.fs-btn.active {
  border-color: #f59e0b;
  color: #f59e0b;
  background: rgba(245,158,11,0.08);
}

.rv-arrow {
  flex: 1;
  height: 2px;
  background: rgba(255,255,255,0.08);
  position: relative;
  min-width: 60px;
  margin: 0 4px;
}

.arrow-label {
  position: absolute;
  top: -18px;
  left: 50%;
  transform: translateX(-50%);
  font-size: 9px;
  color: #374151;
  white-space: nowrap;
}

.arrow-line {
  position: absolute;
  right: 0;
  top: -3px;
  width: 0;
  height: 0;
  border-top: 4px solid transparent;
  border-bottom: 4px solid transparent;
  border-left: 6px solid rgba(255,255,255,0.15);
}

.packet {
  position: absolute;
  top: -4px;
  left: 0;
  width: 10px;
  height: 10px;
  border-radius: 50%;
  background: #f59e0b;
  opacity: 0;
}

.packet.animating {
  animation: travel 0.7s ease forwards;
}

@keyframes travel {
  0% { left: 0; opacity: 1; }
  100% { left: calc(100% - 10px); opacity: 0; }
}

.rn-code {
  text-align: left;
  margin-top: 8px;
  background: rgba(0,0,0,0.3);
  border-radius: 6px;
  padding: 8px 10px;
}

.rn-code code {
  display: block;
  font-family: 'Courier New', monospace;
  font-size: 9px;
  color: #475569;
  line-height: 1.6;
  transition: color 0.2s;
}

.rn-code code.highlight {
  color: #f59e0b;
  font-weight: 700;
}

.rv-split {
  display: flex;
  flex-direction: column;
  gap: 8px;
  flex-shrink: 0;
}

.rv-engine-branch {
  display: flex;
  align-items: center;
  opacity: 0.3;
  transition: opacity 0.3s;
}

.rv-engine-branch.active { opacity: 1; }

.branch-line {
  width: 24px;
  height: 2px;
  background: rgba(255,255,255,0.1);
}

.rv-engine-branch.active .branch-line {
  background: var(--ec, #14b8a6);
}

.engine-node {
  border-color: color-mix(in srgb, var(--ec) 25%, transparent);
  min-width: 140px;
}

.rv-engine-branch.active .engine-node {
  border-color: color-mix(in srgb, var(--ec) 50%, transparent);
  background: color-mix(in srgb, var(--ec) 5%, #0d1a2e);
}

.rn-desc {
  font-size: 10px;
  color: #475569;
}

.active-pulse {
  position: absolute;
  inset: -3px;
  border-radius: 12px;
  border: 2px solid var(--ec, #14b8a6);
  animation: pulse-border 1.5s ease infinite;
}

@keyframes pulse-border {
  0%, 100% { opacity: 0.6; }
  50% { opacity: 0.1; }
}

.rv-output {
  display: flex;
  flex-direction: column;
  gap: 8px;
  flex-shrink: 0;
  margin-left: 8px;
}

.output-file {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 8px 14px;
  background: rgba(255,255,255,0.03);
  border-radius: 8px;
  border: 1px solid rgba(255,255,255,0.05);
  opacity: 0.3;
  transition: opacity 0.3s;
}

.output-file.active { opacity: 1; }

.of-icon { font-size: 16px; }
.of-label { font-size: 11px; color: #94a3b8; font-family: 'Courier New', monospace; }

.rv-code-panel {
  border-top: 1px solid rgba(255,255,255,0.05);
  background: #020a14;
}

.rcp-header {
  display: flex;
  justify-content: space-between;
  padding: 10px 20px;
  font-size: 11px;
  color: #475569;
  border-bottom: 1px solid rgba(255,255,255,0.04);
}

.rcp-lang { color: #14b8a6; font-weight: 700; }

.rcp-code {
  margin: 0;
  padding: 16px 20px;
  overflow-x: auto;
}

.rcp-code code {
  font-family: 'Courier New', monospace;
  font-size: 11px;
  line-height: 1.7;
  color: #94a3b8;
  white-space: pre;
}
</style>
