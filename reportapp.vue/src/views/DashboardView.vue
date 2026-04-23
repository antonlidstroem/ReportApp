<template>
  <div class="dashboard">
    <header class="hero">
      <div class="hero-inner">
        <div class="hero-tag">Proof of Concept · .NET 8 Reporting Engine</div>
        <h1>Modern Reporting Engine</h1>
        <p>Four technical tracks. Compare PDF, Excel, and PowerPoint generation across jsreport, Syncfusion, and two hybrid configurations.</p>
        <div class="hero-pills">
          <div class="pill" v-for="m in meta" :key="m.label">
            <span class="pill-val">{{ m.val }}</span><span class="pill-lbl">{{ m.label }}</span>
          </div>
        </div>
      </div>
    </header>
    <section class="section">
      <h2 class="section-title">Technical Tracks</h2>
      <div class="provider-grid">
        <router-link v-for="p in providers" :key="p.route" :to="p.route" class="pcard" :style="`--c:${p.color}`">
          <div class="pcard-top"><span class="pcard-icon">{{ p.icon }}</span><span class="pcard-track">Track {{ p.track }}</span></div>
          <h3>{{ p.name }}</h3>
          <p>{{ p.description }}</p>
          <div class="pcard-tags"><span v-for="t in p.tags" :key="t" class="tag">{{ t }}</span></div>
          <div class="pcard-foot"><span :class="['pcard-cost', p.costClass]">{{ p.cost }}</span><span class="pcard-arrow">→</span></div>
        </router-link>
      </div>
    </section>
    <section class="section">
      <div class="section-header"><h2 class="section-title">Benchmark History</h2>
        <button v-if="benchmarks.results.length" class="clear-btn" @click="benchmarks.clear()">🗑 Clear</button>
      </div>
      <div v-if="!benchmarks.results.length" class="bench-empty">
        <div style="font-size:32px;margin-bottom:10px">⚡</div>
        <div style="font-size:16px;font-weight:700;color:#e2e8f0;margin-bottom:5px">No benchmarks yet</div>
        <div style="font-size:12px;color:#475569">Go to any provider page and run the Stress Test.</div>
      </div>
      <div v-else class="bench-table-wrap">
        <div class="btw-row btw-header"><span>Provider</span><span>Format</span><span>Time</span><span>Size</span><span>Questions</span></div>
        <div v-for="r in benchmarks.results.slice(0,10)" :key="r.timestamp" class="btw-row">
          <span style="color:#94a3b8;font-weight:600">{{ r.provider }}</span>
          <span style="color:#64748b;font-family:monospace;font-size:10px">{{ r.format.toUpperCase() }}</span>
          <span :class="['br-time', speedClass(r.generationMs)]">{{ r.generationMs }}ms</span>
          <span style="color:#64748b">{{ formatSize(r.fileSizeBytes) }}</span>
          <span style="color:#475569">{{ r.questionCount }}</span>
        </div>
      </div>
    </section>
  </div>
</template>
<script setup lang="ts">
import { onMounted } from 'vue'
import { useBenchmarkStore } from '../stores/benchmarks'
import { fetchSurveys } from '../composables/useApi'
import { useReportBuilderStore } from '../stores/reportBuilder'
const benchmarks = useBenchmarkStore()
const store = useReportBuilderStore()
const meta = [
  { val: '4',    label: 'Providers' },
  { val: '4',    label: 'Surveys'   },
  { val: '100k+',label: 'Responses' },
  { val: '3',    label: 'Formats'   },
]
const providers = [
  { route: '/providers/jsreport',   track: 1, icon: '🌐', name: 'jsreport',              description: 'HTML + CSS + JavaScript → PDF. Chart.js runs in Chromium.', tags: ['Chart.js PDF','Handlebars','HTML Preview'],  cost: 'OSS/Pro',        costClass: 'freemium', color: '#a855f7' },
  { route: '/providers/syncfusion', track: 2, icon: '💎', name: 'Syncfusion',            description: 'Native Office engine. Live Excel formulas, editable PPT charts.', tags: ['Excel Formulas','Native Charts','PDF/UA'], cost: 'Community Free', costClass: 'free',     color: '#14b8a6' },
  { route: '/providers/js-sync',    track: 3, icon: '⚒️', name: 'jsreport + Syncfusion', description: 'jsreport for PDF (Chart.js). Syncfusion for Excel + PPT.', tags: ['Designer PDF','Native Excel','Full Stack'],  cost: 'Enterprise',     costClass: 'paid',     color: '#8b5cf6' },
  { route: '/providers/play-sync',  track: 4, icon: '🎭', name: 'Playwright + Syncfusion', description: 'Playwright replaces jsreport for PDF. Faster cold start.', tags: ['Fast PDF','Native Excel','CI-friendly'],     cost: 'Enterprise',     costClass: 'paid',     color: '#10b981' },
]
function speedClass(ms: number) { return ms < 500 ? 'fast' : ms < 2000 ? 'ok' : 'slow' }
function formatSize(bytes: number) { if (!bytes) return '—'; if (bytes > 1_000_000) return `${(bytes/1_000_000).toFixed(1)} MB`; return `${(bytes/1024).toFixed(0)} KB` }
onMounted(async () => { if (!store.surveys.length) store.surveys = await fetchSurveys() })
</script>
<style scoped>
.dashboard{max-width:1300px;margin:0 auto;color:#e2e8f0}
.hero{background:#050d1f;border-radius:20px;padding:64px;margin-bottom:48px;position:relative;overflow:hidden}
.hero::before{content:'';position:absolute;inset:0;background:radial-gradient(ellipse at 70% 50%,rgba(99,102,241,.12) 0%,transparent 70%)}
.hero-inner{position:relative;z-index:1}
.hero-tag{font-size:11px;text-transform:uppercase;letter-spacing:2px;color:rgba(255,255,255,.35);margin-bottom:16px}
h1{font-size:52px;font-weight:800;letter-spacing:-2px;margin:0 0 14px;color:#fff;line-height:1.1}
.hero>div>p{font-size:17px;color:rgba(255,255,255,.45);margin:0 0 36px;max-width:560px;line-height:1.6}
.hero-pills{display:flex;gap:14px;flex-wrap:wrap}
.pill{background:rgba(255,255,255,.04);border:1px solid rgba(255,255,255,.07);border-radius:12px;padding:12px 20px;text-align:center}
.pill-val{display:block;font-size:24px;font-weight:800;color:#fff}
.pill-lbl{font-size:11px;color:rgba(255,255,255,.35);text-transform:uppercase;letter-spacing:1px}
.section{margin-bottom:48px}
.section-header{display:flex;justify-content:space-between;align-items:center;margin-bottom:16px}
.section-title{font-size:22px;font-weight:700;margin:0 0 16px;color:#f1f5f9}
.provider-grid{display:grid;grid-template-columns:repeat(4,1fr);gap:16px}
.pcard{display:flex;flex-direction:column;background:#050d1f;border:1px solid rgba(255,255,255,.05);border-top:3px solid var(--c);border-radius:14px;padding:22px;text-decoration:none;color:#e2e8f0;transition:all .2s}
.pcard:hover{transform:translateY(-3px);box-shadow:0 16px 40px rgba(0,0,0,.3);border-color:var(--c)}
.pcard-top{display:flex;justify-content:space-between;align-items:center;margin-bottom:10px}
.pcard-icon{font-size:24px}
.pcard-track{font-size:10px;text-transform:uppercase;letter-spacing:1px;color:var(--c)}
.pcard h3{font-size:14px;font-weight:700;margin:0 0 7px;color:#fff}
.pcard>p{font-size:12px;color:#64748b;line-height:1.5;flex:1;margin:0 0 10px}
.pcard-tags{display:flex;flex-wrap:wrap;gap:4px;margin-bottom:12px}
.tag{font-size:9px;padding:2px 7px;background:color-mix(in srgb,var(--c) 10%,transparent);color:var(--c);border-radius:100px;border:1px solid color-mix(in srgb,var(--c) 25%,transparent)}
.pcard-foot{display:flex;justify-content:space-between;align-items:center;padding-top:10px;border-top:1px solid rgba(255,255,255,.05)}
.pcard-cost{font-size:12px;font-weight:700}
.free{color:#22c55e}.paid{color:#f59e0b}.freemium{color:#a855f7}
.pcard-arrow{color:#475569;transition:transform .15s}
.pcard:hover .pcard-arrow{transform:translateX(4px);color:var(--c)}
.bench-empty{padding:48px;text-align:center;background:#050d1f;border:1px dashed #1e293b;border-radius:12px}
.bench-table-wrap{background:#050d1f;border:1px solid #1e293b;border-radius:12px;overflow:hidden}
.btw-row{display:grid;grid-template-columns:100px 55px 65px 65px 50px;padding:7px 14px;border-bottom:1px solid #0f172a;font-size:11px}
.btw-row.btw-header{font-size:10px;color:#475569;font-weight:700;text-transform:uppercase;background:#050d1f}
.br-time.fast{color:#22c55e;font-weight:700}.br-time.ok{color:#f59e0b;font-weight:700}.br-time.slow{color:#ef4444;font-weight:700}
.clear-btn{font-size:12px;padding:5px 12px;border-radius:6px;border:1px solid #1e293b;background:transparent;color:#64748b;cursor:pointer}
.clear-btn:hover{background:#ef4444;color:white;border-color:#ef4444}
</style>
