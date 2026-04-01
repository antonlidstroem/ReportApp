<template>
  <div class="module-list">
    <div class="ml-header">
      <h3>🧩 Rapportmoduler</h3>
      <button @click="store.resetModules()" class="reset-btn" title="Återställ">↺ Återställ</button>
    </div>

    <div class="modules">
      <div
        v-for="mod in sortedModules"
        :key="mod.id"
        class="module-card"
        :class="{ disabled: !mod.enabled }"
      >
        <div class="module-main">
          <div class="module-drag">
            <button @click="store.moveModule(mod.id, 'up')" title="Flytta upp">↑</button>
            <button @click="store.moveModule(mod.id, 'down')" title="Flytta ned">↓</button>
          </div>

          <div class="module-icon">{{ moduleIcon(mod.type) }}</div>

          <div class="module-info">
            <span class="module-label">{{ mod.label }}</span>
            <span class="module-desc">{{ moduleDesc(mod.type) }}</span>
          </div>

          <label class="toggle">
            <input type="checkbox" :checked="mod.enabled" @change="store.toggleModule(mod.id)" />
            <span class="track"></span>
          </label>
        </div>

        <!-- Expandable config -->
        <transition name="expand">
          <div v-if="mod.enabled && hasConfig(mod.type)" class="module-config">
            <template v-if="mod.type === 'questionTable'">
              <label class="cfg-row">
                <input
                  type="checkbox"
                  :checked="!!mod.config.showDistribution"
                  @change="store.updateModuleConfig(mod.id, { showDistribution: !mod.config.showDistribution })"
                />
                Visa svarsfördelning
              </label>
            </template>
          </div>
        </transition>
      </div>
    </div>

    <div class="ml-footer">
      <span class="active-count">
        {{ store.activeModules.length }} / {{ store.modules.length }} aktiva moduler
      </span>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useReportBuilderStore, type ModuleType } from '../stores/reportBuilder'

const store = useReportBuilderStore()

const sortedModules = computed(() =>
  [...store.modules].sort((a, b) => a.order - b.order)
)

function moduleIcon(type: ModuleType) {
  return { summary: '📄', questionTable: '📊', trend: '📈', category: '🗂️', rawData: '🗃️' }[type]
}

function moduleDesc(type: ModuleType) {
  return {
    summary:       'Titel, företag och datumintervall',
    questionTable: 'Tabell med alla valda frågor och snitt',
    trend:         'Månadsvis trendlinje',
    category:      'Gruppering per kategori',
    rawData:       'Fullständig dataexport (tungt!)'
  }[type]
}

function hasConfig(type: ModuleType) {
  return ['questionTable'].includes(type)
}
</script>

<style scoped>
.module-list {
  background: var(--surface);
  border: 1px solid var(--border);
  border-radius: 12px;
  overflow: hidden;
}

.ml-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 14px 18px;
  background: var(--surface-2);
  border-bottom: 1px solid var(--border);
}
.ml-header h3 { font-size: 14px; font-weight: 600; margin: 0; }

.reset-btn {
  font-size: 11px;
  background: transparent;
  border: 1px solid var(--border);
  color: var(--text-muted);
  padding: 4px 10px;
  border-radius: 6px;
  cursor: pointer;
}
.reset-btn:hover { background: var(--surface-2); color: var(--text); }

.modules { padding: 10px; display: flex; flex-direction: column; gap: 6px; }

.module-card {
  border: 1px solid var(--border);
  border-radius: 8px;
  overflow: hidden;
  transition: all 0.15s;
}
.module-card.disabled { opacity: 0.5; }

.module-main {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 10px 12px;
}

.module-drag {
  display: flex;
  flex-direction: column;
  gap: 2px;
}
.module-drag button {
  background: transparent;
  border: none;
  font-size: 12px;
  color: var(--text-muted);
  cursor: pointer;
  padding: 1px 4px;
  border-radius: 3px;
  line-height: 1;
}
.module-drag button:hover { background: var(--surface-2); color: var(--text); }

.module-icon { font-size: 18px; flex-shrink: 0; }

.module-info { flex: 1; }
.module-label { display: block; font-size: 13px; font-weight: 500; }
.module-desc  { display: block; font-size: 11px; color: var(--text-muted); margin-top: 1px; }

/* Toggle switch */
.toggle { position: relative; display: inline-flex; cursor: pointer; }
.toggle input { opacity: 0; width: 0; height: 0; position: absolute; }
.track {
  width: 36px;
  height: 20px;
  background: var(--border);
  border-radius: 100px;
  transition: background 0.2s;
  position: relative;
}
.track::after {
  content: '';
  position: absolute;
  left: 3px;
  top: 3px;
  width: 14px;
  height: 14px;
  border-radius: 50%;
  background: white;
  transition: transform 0.2s;
  box-shadow: 0 1px 3px rgba(0,0,0,0.2);
}
.toggle input:checked ~ .track { background: var(--accent); }
.toggle input:checked ~ .track::after { transform: translateX(16px); }

.module-config {
  border-top: 1px solid var(--border);
  padding: 10px 14px;
  background: var(--surface-2);
}

.cfg-row {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 12px;
  color: var(--text-muted);
  cursor: pointer;
}
.cfg-row input { accent-color: var(--accent); }

.ml-footer {
  padding: 10px 18px;
  border-top: 1px solid var(--border);
  background: var(--surface-2);
}
.active-count { font-size: 11px; color: var(--text-muted); }

.expand-enter-active, .expand-leave-active { transition: all 0.2s ease; max-height: 100px; }
.expand-enter-from, .expand-leave-to { max-height: 0; opacity: 0; }
</style>
