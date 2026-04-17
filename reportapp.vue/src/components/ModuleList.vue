<template>
  <div class="module-list">
    <div class="ml-header">
      <h3>🧩 Rapportmoduler</h3>
      <div class="ml-hdr-btns">
        <button @click="enableAll" class="hdr-btn">Aktivera alla</button>
        <button @click="store.resetModules()" class="hdr-btn ghost">↺ Återställ</button>
      </div>
    </div>

    <div class="modules">
      <div
        v-for="mod in sortedModules"
        :key="mod.id"
        class="module-card"
        :class="{ disabled: !mod.enabled }"
      >
        <!-- Main row -->
        <div class="module-main">
          <div class="module-order">
            <button @click="store.moveModule(mod.id, 'up')"   title="Flytta upp">↑</button>
            <button @click="store.moveModule(mod.id, 'down')" title="Flytta ned">↓</button>
          </div>
          <div class="module-icon">{{ moduleIcon(mod.type) }}</div>
          <div class="module-info">
            <span class="module-label">{{ mod.label }}</span>
            <span class="module-desc">{{ moduleDesc(mod.type) }}</span>
          </div>
          <div class="module-right">
            <button
              v-if="hasConfig(mod.type)"
              class="cfg-toggle"
              :class="{ open: expandedId === mod.id }"
              @click="expandedId = expandedId === mod.id ? null : mod.id"
              title="Inställningar">⚙</button>
            <label class="toggle">
              <input type="checkbox" :checked="mod.enabled" @change="store.toggleModule(mod.id)" />
              <span class="track"></span>
            </label>
          </div>
        </div>

        <!-- Expandable config panel -->
        <transition name="expand">
          <div v-if="mod.enabled && expandedId === mod.id && hasConfig(mod.type)" class="module-config">

            <!-- Cover config -->
            <template v-if="mod.type === 'summary'">
              <div class="cfg-section-title">Färgschema</div>
              <div class="color-scheme-grid">
                <label v-for="cs in colorSchemes" :key="cs.id"
                  :class="['cs-opt', { active: (mod.config.colorScheme ?? 'corporate') === cs.id }]">
                  <input type="radio" :value="cs.id"
                    :checked="(mod.config.colorScheme ?? 'corporate') === cs.id"
                    @change="store.updateModuleConfig(mod.id, { colorScheme: cs.id })" />
                  <div class="cs-swatch" :style="`background:${cs.color}`"></div>
                  <span>{{ cs.name }}</span>
                </label>
              </div>
              <div class="cfg-row">
                <label>
                  <input type="checkbox"
                    :checked="!!mod.config.showLogo"
                    @change="store.updateModuleConfig(mod.id, { showLogo: !mod.config.showLogo })" />
                  Visa företagslogotyp (placeholder)
                </label>
              </div>
              <div class="cfg-row">
                <label>
                  <input type="checkbox"
                    :checked="mod.config.showKpiStrip !== false"
                    @change="store.updateModuleConfig(mod.id, { showKpiStrip: !(mod.config.showKpiStrip !== false) })" />
                  Visa KPI-remsa (frågor, svar, snitt, kategorier)
                </label>
              </div>
            </template>

            <!-- Question table config -->
            <template v-if="mod.type === 'questionTable'">
              <div class="cfg-row">
                <label>
                  <input type="checkbox"
                    :checked="!!mod.config.showDistribution"
                    @change="store.updateModuleConfig(mod.id, { showDistribution: !mod.config.showDistribution })" />
                  Visa svarsfördelning (1–5 per fråga)
                </label>
              </div>
              <div class="cfg-row">
                <label>
                  <input type="checkbox"
                    :checked="mod.config.showProgressBars !== false"
                    @change="store.updateModuleConfig(mod.id, { showProgressBars: !(mod.config.showProgressBars !== false) })" />
                  Visa progress-bar per fråga
                </label>
              </div>
              <div class="cfg-row">
                <label>
                  <input type="checkbox"
                    :checked="!!mod.config.groupByCategory"
                    @change="store.updateModuleConfig(mod.id, { groupByCategory: !mod.config.groupByCategory })" />
                  Gruppera frågor per kategori
                </label>
              </div>
              <div class="cfg-section-title" style="margin-top:10px">Sortering</div>
              <div class="sort-opts">
                <label v-for="so in sortOptions" :key="so.id"
                  :class="['sort-opt', { active: (mod.config.sortBy ?? 'original') === so.id }]">
                  <input type="radio" :value="so.id"
                    :checked="(mod.config.sortBy ?? 'original') === so.id"
                    @change="store.updateModuleConfig(mod.id, { sortBy: so.id })" />
                  {{ so.label }}
                </label>
              </div>
            </template>

            <!-- Trend config -->
            <template v-if="mod.type === 'trend'">
              <div class="cfg-section-title">Diagramtyp</div>
              <div class="chart-type-grid">
                <label v-for="ct in chartTypeOptions" :key="ct.id"
                  :class="['ct-opt', { active: (mod.config.chartType ?? 'line') === ct.id }]">
                  <input type="radio" :value="ct.id"
                    :checked="(mod.config.chartType ?? 'line') === ct.id"
                    @change="store.updateModuleConfig(mod.id, { chartType: ct.id })" />
                  <span class="ct-icon">{{ ct.icon }}</span>
                  <span>{{ ct.label }}</span>
                </label>
              </div>
              <div class="cfg-row" style="margin-top:10px">
                <label>
                  <input type="checkbox"
                    :checked="!!mod.config.showDataPoints"
                    @change="store.updateModuleConfig(mod.id, { showDataPoints: !mod.config.showDataPoints })" />
                  Visa datapunkter
                </label>
              </div>
              <div class="cfg-row">
                <label>
                  <input type="checkbox"
                    :checked="mod.config.fillArea !== false"
                    @change="store.updateModuleConfig(mod.id, { fillArea: !(mod.config.fillArea !== false) })" />
                  Fyll area under kurva
                </label>
              </div>
            </template>

            <!-- Category config -->
            <template v-if="mod.type === 'category'">
              <div class="cfg-section-title">Diagramtyp</div>
              <div class="chart-type-grid">
                <label v-for="ct in categoryChartTypes" :key="ct.id"
                  :class="['ct-opt', { active: (mod.config.chartType ?? 'bar') === ct.id }]">
                  <input type="radio" :value="ct.id"
                    :checked="(mod.config.chartType ?? 'bar') === ct.id"
                    @change="store.updateModuleConfig(mod.id, { chartType: ct.id })" />
                  <span class="ct-icon">{{ ct.icon }}</span>
                  <span>{{ ct.label }}</span>
                </label>
              </div>
              <div class="cfg-row" style="margin-top:10px">
                <label>
                  <input type="checkbox"
                    :checked="!!mod.config.showQuestionCount"
                    @change="store.updateModuleConfig(mod.id, { showQuestionCount: !mod.config.showQuestionCount })" />
                  Visa antal frågor per kategori
                </label>
              </div>
            </template>

            <!-- Raw data -->
            <template v-if="mod.type === 'rawData'">
              <div class="cfg-warn">⚠ Rådata exporterar alla svar. Kan göra filen mycket stor för tunga enkäter.</div>
              <div class="cfg-row">
                <label>
                  <input type="checkbox"
                    :checked="!!mod.config.includeComments"
                    @change="store.updateModuleConfig(mod.id, { includeComments: !mod.config.includeComments })" />
                  Inkludera fritext-kommentarer
                </label>
              </div>
              <div class="cfg-row">
                <label>
                  <input type="checkbox"
                    :checked="!!mod.config.anonymize"
                    @change="store.updateModuleConfig(mod.id, { anonymize: !mod.config.anonymize })" />
                  Anonymisera respondent-ID
                </label>
              </div>
            </template>

          </div>
        </transition>
      </div>
    </div>

    <div class="ml-footer">
      <span class="active-count">
        <span class="ac-num">{{ store.activeModules.length }}</span> / {{ store.modules.length }} moduler aktiva
      </span>
      <span class="order-hint">Dra ↑↓ för att ändra ordning</span>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { useReportBuilderStore, type ModuleType } from '../stores/reportBuilder'

const store = useReportBuilderStore()
const expandedId = ref<string | null>(null)

const sortedModules = computed(() =>
  [...store.modules].sort((a, b) => a.order - b.order)
)

function enableAll() {
  store.modules.forEach(m => {
    if (!m.enabled) store.toggleModule(m.id)
  })
}

function moduleIcon(type: ModuleType) {
  const icons: Record<ModuleType, string> = {
    summary:       '🏠',
    questionTable: '📊',
    trend:         '📈',
    category:      '🗂️',
    rawData:       '🗃️',
  }
  return icons[type]
}

function moduleDesc(type: ModuleType) {
  const descs: Record<ModuleType, string> = {
    summary:       'Titelsida, KPI-remsa, företag och datum',
    questionTable: 'Tabell med alla valda frågor och snitt',
    trend:         'Månadsvis trendlinje med Chart.js',
    category:      'Stapel- eller radardiagram per kategori',
    rawData:       'Fullständig dataexport (kan bli stor)',
  }
  return descs[type]
}

function hasConfig(type: ModuleType) {
  return ['summary', 'questionTable', 'trend', 'category', 'rawData'].includes(type)
}

const colorSchemes = [
  { id: 'corporate', name: 'Marinblå',  color: 'linear-gradient(135deg,#1e3a5f,#0f2d4a)' },
  { id: 'dark',      name: 'Mörkt',     color: 'linear-gradient(135deg,#0f172a,#1e293b)'  },
  { id: 'teal',      name: 'Teal',      color: 'linear-gradient(135deg,#0f766e,#134e4a)'  },
  { id: 'purple',    name: 'Lila',      color: 'linear-gradient(135deg,#4c1d95,#6d28d9)'  },
]

const sortOptions = [
  { id: 'original',    label: 'Ursprunglig ordning' },
  { id: 'score_asc',   label: 'Lägst poäng först'   },
  { id: 'score_desc',  label: 'Högst poäng först'   },
  { id: 'alpha',       label: 'Alfabetisk'           },
]

const chartTypeOptions = [
  { id: 'line',    icon: '📈', label: 'Linje'   },
  { id: 'bar',     icon: '📊', label: 'Stapel'  },
  { id: 'area',    icon: '🌊', label: 'Area'    },
]

const categoryChartTypes = [
  { id: 'bar',     icon: '📊', label: 'Stapel'  },
  { id: 'radar',   icon: '🎯', label: 'Radar'   },
  { id: 'doughnut',icon: '🍩', label: 'Munk'    },
]
</script>

<style scoped>
.module-list {
  background: var(--surface, #1e293b);
  border: 1px solid var(--border, #334155);
  border-radius: 12px;
  overflow: hidden;
}

.ml-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 13px 18px;
  background: var(--surface-2, #0f172a);
  border-bottom: 1px solid var(--border, #334155);
}
.ml-header h3 { font-size: 13px; font-weight: 600; margin: 0; }
.ml-hdr-btns { display: flex; gap: 6px; }

.hdr-btn {
  font-size: 11px;
  padding: 4px 10px;
  border-radius: 5px;
  border: 1px solid var(--border, #334155);
  background: var(--accent, #a855f7);
  color: white;
  cursor: pointer;
}
.hdr-btn.ghost { background: transparent; color: var(--text-muted, #64748b); }
.hdr-btn:hover { opacity: .85; }

.modules { padding: 10px; display: flex; flex-direction: column; gap: 6px; }

.module-card {
  border: 1px solid var(--border, #334155);
  border-radius: 9px;
  overflow: hidden;
  transition: all 0.15s;
  background: var(--surface, #1e293b);
}
.module-card.disabled { opacity: 0.45; }
.module-card:hover { border-color: var(--accent, #a855f7); }

.module-main {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 10px 12px;
}

.module-order {
  display: flex;
  flex-direction: column;
  gap: 1px;
}
.module-order button {
  background: transparent;
  border: none;
  font-size: 11px;
  color: var(--text-muted, #64748b);
  cursor: pointer;
  padding: 0 4px;
  border-radius: 3px;
  line-height: 1.3;
}
.module-order button:hover { background: var(--surface-2, #0f172a); color: var(--text, #f1f5f9); }

.module-icon { font-size: 18px; flex-shrink: 0; }

.module-info { flex: 1; overflow: hidden; }
.module-label { display: block; font-size: 12px; font-weight: 600; white-space: nowrap; overflow: hidden; text-overflow: ellipsis; }
.module-desc  { display: block; font-size: 10px; color: var(--text-muted, #64748b); margin-top: 1px; white-space: nowrap; overflow: hidden; text-overflow: ellipsis; }

.module-right { display: flex; align-items: center; gap: 8px; flex-shrink: 0; }

.cfg-toggle {
  background: transparent;
  border: 1px solid var(--border, #334155);
  color: var(--text-muted, #64748b);
  cursor: pointer;
  font-size: 12px;
  width: 24px; height: 24px;
  border-radius: 5px;
  display: flex; align-items: center; justify-content: center;
  transition: all .12s;
}
.cfg-toggle:hover, .cfg-toggle.open { border-color: var(--accent, #a855f7); color: var(--accent, #a855f7); background: color-mix(in srgb,var(--accent,#a855f7) 8%,transparent); }

/* Toggle switch */
.toggle { position: relative; display: inline-flex; cursor: pointer; }
.toggle input { opacity: 0; width: 0; height: 0; position: absolute; }
.track {
  width: 34px; height: 18px;
  background: var(--border, #334155);
  border-radius: 100px;
  transition: background .2s;
  position: relative;
}
.track::after {
  content: '';
  position: absolute;
  left: 2px; top: 2px;
  width: 14px; height: 14px;
  border-radius: 50%;
  background: white;
  transition: transform .2s;
  box-shadow: 0 1px 3px rgba(0,0,0,.2);
}
.toggle input:checked ~ .track { background: var(--accent, #a855f7); }
.toggle input:checked ~ .track::after { transform: translateX(16px); }

/* Config panel */
.module-config {
  border-top: 1px solid var(--border, #334155);
  padding: 12px 14px;
  background: var(--surface-2, #0f172a);
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.cfg-section-title {
  font-size: 10px;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: .5px;
  color: var(--text-muted, #64748b);
  margin-bottom: 2px;
}

.color-scheme-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 6px;
}
.cs-opt {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 4px;
  cursor: pointer;
  padding: 4px;
  border-radius: 6px;
  border: 1px solid transparent;
  transition: all .12s;
  font-size: 10px;
  color: var(--text-muted, #64748b);
}
.cs-opt:hover { border-color: var(--border, #334155); }
.cs-opt.active { border-color: var(--accent, #a855f7); color: var(--text, #f1f5f9); }
.cs-opt input { display: none; }
.cs-swatch { width: 100%; height: 20px; border-radius: 4px; }

.sort-opts {
  display: flex;
  flex-wrap: wrap;
  gap: 5px;
}
.sort-opt {
  font-size: 10px;
  padding: 3px 9px;
  border-radius: 100px;
  border: 1px solid var(--border, #334155);
  background: transparent;
  color: var(--text-muted, #64748b);
  cursor: pointer;
  transition: all .12s;
}
.sort-opt input { display: none; }
.sort-opt.active {
  background: var(--accent, #a855f7);
  color: white;
  border-color: var(--accent, #a855f7);
}

.chart-type-grid {
  display: flex;
  gap: 6px;
}
.ct-opt {
  display: flex;
  align-items: center;
  gap: 5px;
  font-size: 11px;
  padding: 5px 10px;
  border-radius: 7px;
  border: 1px solid var(--border, #334155);
  background: transparent;
  color: var(--text-muted, #64748b);
  cursor: pointer;
  transition: all .12s;
}
.ct-opt input { display: none; }
.ct-opt.active { border-color: var(--accent, #a855f7); color: var(--text, #f1f5f9); background: color-mix(in srgb,var(--accent,#a855f7) 10%,transparent); }
.ct-icon { font-size: 14px; }

.cfg-row {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 11px;
  color: var(--text-muted, #64748b);
  cursor: pointer;
  user-select: none;
}
.cfg-row input { accent-color: var(--accent, #a855f7); }
.cfg-row label { display: flex; align-items: center; gap: 8px; cursor: pointer; width: 100%; }

.cfg-warn {
  font-size: 11px;
  color: #f59e0b;
  padding: 7px 10px;
  background: rgba(245,158,11,.08);
  border: 1px solid rgba(245,158,11,.25);
  border-radius: 6px;
}

.ml-footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 9px 18px;
  border-top: 1px solid var(--border, #334155);
  background: var(--surface-2, #0f172a);
}
.active-count { font-size: 11px; color: var(--text-muted, #64748b); }
.ac-num { font-weight: 700; color: var(--accent, #a855f7); }
.order-hint { font-size: 10px; color: #374151; }

.expand-enter-active, .expand-leave-active { transition: all .2s ease; max-height: 400px; overflow: hidden; }
.expand-enter-from, .expand-leave-to { max-height: 0; opacity: 0; padding-top: 0; padding-bottom: 0; }
</style>
