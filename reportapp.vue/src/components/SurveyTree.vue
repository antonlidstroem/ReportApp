<template>
  <div class="survey-tree">
    <!-- Survey selector -->
    <div class="st-surveys">
      <div class="st-label">Survey</div>
      <button
        v-for="s in store.surveys"
        :key="s.id"
        :class="['survey-btn', { active: store.surveyId === s.id }]"
        @click="selectSurvey(s)"
      >
        <span class="sb-title">{{ s.title.replace('⚡ ', '') }}</span>
        <span class="sb-meta">{{ s.questionCount }}q</span>
        <span v-if="s.title.includes('⚡')" class="sb-stress">STRESS</span>
      </button>
    </div>

    <!-- Question tree -->
    <div v-if="store.surveyId && !loading" class="st-questions">
      <div class="sq-toolbar">
        <span class="sq-count">
          <strong>{{ store.selectedCount }}</strong> / {{ store.questions.length }} valda
        </span>
        <div class="sq-actions">
          <button @click="store.selectAllQuestions()" class="sq-btn">Alla</button>
          <button @click="store.clearSelection()" class="sq-btn sq-btn-ghost">Rensa</button>
        </div>
      </div>

      <!-- Category groups -->
      <div
        v-for="cat in categories"
        :key="cat.name"
        class="cat-group"
      >
        <div class="cat-header" @click="toggleCat(cat.name)">
          <div class="cat-left">
            <span class="cat-chevron" :class="{ open: openCats.has(cat.name) }">›</span>
            <label class="cat-checkbox-wrap" @click.stop>
              <input
                type="checkbox"
                :checked="store.isCategoryFullySelected(cat.name)"
                :indeterminate="store.isCategoryPartiallySelected(cat.name)"
                @change="toggleCatSelection(cat.name)"
              />
            </label>
            <span class="cat-name">{{ cat.name }}</span>
          </div>
          <span class="cat-badge">
            {{ cat.selectedCount }}/{{ cat.questions.length }}
          </span>
        </div>

        <Transition name="expand">
          <div v-if="openCats.has(cat.name)" class="cat-questions">
            <label
              v-for="q in cat.questions"
              :key="q.id"
              :class="['q-row', { selected: store.isSelected(q.id) }]"
            >
              <input
                type="checkbox"
                :checked="store.isSelected(q.id)"
                @change="store.toggleQuestion(q.id)"
              />
              <span class="q-text">{{ q.text }}</span>
              <span class="q-resp">{{ q.responseCount }}</span>
            </label>
          </div>
        </Transition>
      </div>
    </div>

    <!-- Loading -->
    <div v-else-if="loading" class="st-loading">
      <div class="spin"></div>
      <span>Laddar frågor…</span>
    </div>

    <!-- Ordered selection (drag-to-reorder) -->
    <div v-if="store.selectedCount > 0" class="st-ordered">
      <div class="so-header">
        <span class="so-label">Exportordning</span>
        <span class="so-hint">Dra för att ändra ordning</span>
      </div>
      <div class="so-list">
        <div
          v-for="(id, idx) in store.orderedSelectedIds"
          :key="id"
          class="so-item"
          :class="{ dragging: dragIdx === idx, over: overIdx === idx }"
          draggable="true"
          @dragstart="onDragStart(idx)"
          @dragover.prevent="onDragOver(idx)"
          @drop="onDrop(idx)"
          @dragend="onDragEnd"
        >
          <span class="so-handle">⠿</span>
          <span class="so-num">{{ idx + 1 }}</span>
          <span class="so-text">{{ getQuestion(id)?.text?.substring(0, 42) ?? '…' }}</span>
          <button class="so-remove" @click="store.toggleQuestion(id)" title="Ta bort">×</button>
        </div>
      </div>
    </div>

    <!-- Date range -->
    <div v-if="store.surveyId" class="st-dates">
      <div class="st-label">Datumintervall (valfritt)</div>
      <div class="date-row">
        <div class="date-field">
          <span>Från</span>
          <input type="date" v-model="store.dateStart" />
        </div>
        <div class="date-field">
          <span>Till</span>
          <input type="date" v-model="store.dateEnd" />
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useReportBuilderStore } from '../stores/reportBuilder'
import { fetchSurveys, fetchQuestions } from '../composables/useApi'

const store = useReportBuilderStore()
const loading = ref(false)
const openCats = ref<Set<string>>(new Set())

// Drag state
const dragIdx = ref<number | null>(null)
const overIdx = ref<number | null>(null)

// Group questions by category
const categories = computed(() => {
  const map = new Map<string, typeof store.questions>()
  store.questions.forEach(q => {
    const arr = map.get(q.category) ?? []
    arr.push(q)
    map.set(q.category, arr)
  })
  return Array.from(map.entries()).map(([name, questions]) => ({
    name,
    questions,
    selectedCount: questions.filter(q => store.isSelected(q.id)).length,
  }))
})

function toggleCat(name: string) {
  if (openCats.value.has(name)) openCats.value.delete(name)
  else openCats.value.add(name)
}

function toggleCatSelection(name: string) {
  if (store.isCategoryFullySelected(name)) {
    store.deselectCategory(name)
  } else {
    store.selectCategory(name)
  }
}

function getQuestion(id: number) {
  return store.questions.find(q => q.id === id)
}

async function selectSurvey(s: { id: number; title: string; questionCount: number }) {
  store.setSurvey(s.id)
  loading.value = true
  openCats.value = new Set()
  try {
    store.questions = await fetchQuestions(s.id)
    // Auto-open first category
    const firstCat = store.questions[0]?.category
    if (firstCat) openCats.value.add(firstCat)
    store.selectAllQuestions()
  } finally {
    loading.value = false
  }
}

// Drag handlers
function onDragStart(idx: number) {
  dragIdx.value = idx
}

function onDragOver(idx: number) {
  overIdx.value = idx
}

function onDrop(toIdx: number) {
  if (dragIdx.value !== null && dragIdx.value !== toIdx) {
    store.reorderQuestion(dragIdx.value, toIdx)
  }
  dragIdx.value = null
  overIdx.value = null
}

function onDragEnd() {
  dragIdx.value = null
  overIdx.value = null
}

onMounted(async () => {
  if (!store.surveys.length) {
    store.surveys = await fetchSurveys()
  }
})
</script>

<style scoped>
.survey-tree {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

/* ── Survey list ─────────────────────────────────────────── */
.st-label {
  font-size: 10px;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 1px;
  color: var(--text-muted, #64748b);
  margin-bottom: 6px;
}

.st-surveys {
  background: var(--surface, #1e293b);
  border: 1px solid var(--border, #334155);
  border-radius: 10px;
  padding: 12px;
}

.survey-btn {
  display: flex;
  align-items: center;
  gap: 8px;
  width: 100%;
  padding: 9px 11px;
  border-radius: 7px;
  border: 1px solid transparent;
  background: transparent;
  cursor: pointer;
  text-align: left;
  color: var(--text, #f1f5f9);
  font-family: inherit;
  transition: all 0.13s;
  margin-bottom: 4px;
}
.survey-btn:last-child { margin-bottom: 0; }
.survey-btn:hover { background: var(--surface-2, #334155); }
.survey-btn.active {
  border-color: var(--accent, #3b82f6);
  background: color-mix(in srgb, var(--accent, #3b82f6) 10%, transparent);
}

.sb-title { font-size: 12px; font-weight: 500; flex: 1; }
.sb-meta  { font-size: 10px; color: var(--text-muted, #64748b); }
.sb-stress {
  font-size: 8px;
  font-weight: 700;
  background: #f97316;
  color: white;
  padding: 1px 5px;
  border-radius: 3px;
}

/* ── Question tree ───────────────────────────────────────── */
.st-questions {
  background: var(--surface, #1e293b);
  border: 1px solid var(--border, #334155);
  border-radius: 10px;
  overflow: hidden;
}

.sq-toolbar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 9px 12px;
  background: var(--surface-2, #334155);
  border-bottom: 1px solid var(--border, #475569);
}
.sq-count { font-size: 12px; color: var(--text-muted, #94a3b8); }
.sq-count strong { color: var(--accent, #3b82f6); }
.sq-actions { display: flex; gap: 5px; }
.sq-btn {
  font-size: 10px;
  padding: 3px 8px;
  border-radius: 4px;
  border: 1px solid var(--border, #475569);
  background: var(--accent, #3b82f6);
  color: white;
  cursor: pointer;
  font-family: inherit;
}
.sq-btn-ghost { background: transparent; color: var(--text-muted, #94a3b8); }

.cat-group { border-bottom: 1px solid var(--border, #334155); }
.cat-group:last-child { border-bottom: none; }

.cat-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 8px 12px;
  cursor: pointer;
  transition: background 0.1s;
}
.cat-header:hover { background: rgba(255,255,255,0.03); }

.cat-left {
  display: flex;
  align-items: center;
  gap: 8px;
}

.cat-chevron {
  font-size: 14px;
  color: var(--text-muted, #64748b);
  transition: transform 0.2s;
  display: inline-block;
  width: 14px;
}
.cat-chevron.open { transform: rotate(90deg); }

.cat-checkbox-wrap { display: flex; align-items: center; cursor: pointer; }
.cat-checkbox-wrap input { accent-color: var(--accent, #3b82f6); }

.cat-name { font-size: 12px; font-weight: 600; color: var(--text, #f1f5f9); }
.cat-badge {
  font-size: 10px;
  color: var(--text-muted, #64748b);
  background: rgba(255,255,255,0.05);
  padding: 1px 6px;
  border-radius: 100px;
}

.cat-questions {
  padding: 4px 0 8px 36px;
  max-height: 220px;
  overflow-y: auto;
}
.cat-questions::-webkit-scrollbar { width: 3px; }
.cat-questions::-webkit-scrollbar-thumb { background: var(--border, #475569); border-radius: 3px; }

.q-row {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 5px 12px 5px 0;
  cursor: pointer;
  border-radius: 5px;
  transition: background 0.1s;
}
.q-row:hover { background: rgba(255,255,255,0.04); }
.q-row.selected { background: color-mix(in srgb, var(--accent, #3b82f6) 6%, transparent); }
.q-row input { accent-color: var(--accent, #3b82f6); flex-shrink: 0; }
.q-text { flex: 1; font-size: 11px; color: var(--text-muted, #94a3b8); line-height: 1.4; }
.q-row.selected .q-text { color: var(--text, #f1f5f9); }
.q-resp { font-size: 9px; color: var(--text-muted, #64748b); flex-shrink: 0; }

/* expand transition */
.expand-enter-active, .expand-leave-active { transition: all 0.18s ease; overflow: hidden; }
.expand-enter-from, .expand-leave-to { max-height: 0; opacity: 0; }
.expand-enter-to, .expand-leave-from { max-height: 240px; }

/* loading */
.st-loading {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 16px 12px;
  font-size: 12px;
  color: var(--text-muted, #94a3b8);
  background: var(--surface, #1e293b);
  border: 1px solid var(--border, #334155);
  border-radius: 10px;
}
.spin {
  width: 14px; height: 14px;
  border: 2px solid var(--border, #334155);
  border-top-color: var(--accent, #3b82f6);
  border-radius: 50%;
  animation: spin 0.6s linear infinite;
}
@keyframes spin { to { transform: rotate(360deg); } }

/* ── Ordered selection ───────────────────────────────────── */
.st-ordered {
  background: var(--surface, #1e293b);
  border: 1px solid var(--border, #334155);
  border-radius: 10px;
  overflow: hidden;
}

.so-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 8px 12px;
  background: var(--surface-2, #334155);
  border-bottom: 1px solid var(--border, #475569);
}
.so-label { font-size: 10px; font-weight: 700; text-transform: uppercase; letter-spacing: 1px; color: var(--text-muted, #94a3b8); }
.so-hint  { font-size: 9px; color: var(--text-muted, #64748b); }

.so-list {
  max-height: 240px;
  overflow-y: auto;
  padding: 4px;
}
.so-list::-webkit-scrollbar { width: 3px; }
.so-list::-webkit-scrollbar-thumb { background: var(--border, #475569); border-radius: 3px; }

.so-item {
  display: flex;
  align-items: center;
  gap: 7px;
  padding: 6px 8px;
  border-radius: 6px;
  border: 1px solid transparent;
  cursor: grab;
  transition: all 0.12s;
  font-size: 11px;
  color: var(--text, #f1f5f9);
  user-select: none;
}
.so-item:hover { background: rgba(255,255,255,0.04); border-color: var(--border, #475569); }
.so-item.dragging { opacity: 0.4; }
.so-item.over { border-color: var(--accent, #3b82f6); background: color-mix(in srgb, var(--accent, #3b82f6) 8%, transparent); }

.so-handle { color: var(--text-muted, #64748b); font-size: 13px; cursor: grab; flex-shrink: 0; }
.so-num { font-size: 9px; color: var(--text-muted, #64748b); width: 16px; text-align: center; flex-shrink: 0; }
.so-text { flex: 1; overflow: hidden; text-overflow: ellipsis; white-space: nowrap; color: var(--text-muted, #94a3b8); }
.so-remove {
  background: transparent;
  border: none;
  color: var(--text-muted, #64748b);
  cursor: pointer;
  font-size: 14px;
  line-height: 1;
  padding: 0 2px;
  flex-shrink: 0;
  transition: color 0.1s;
}
.so-remove:hover { color: #ef4444; }

/* ── Date range ──────────────────────────────────────────── */
.st-dates {
  background: var(--surface, #1e293b);
  border: 1px solid var(--border, #334155);
  border-radius: 10px;
  padding: 12px;
}

.date-row {
  display: flex;
  gap: 8px;
}
.date-field {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 4px;
}
.date-field span { font-size: 10px; color: var(--text-muted, #64748b); }
.date-field input {
  font-size: 11px;
  padding: 5px 8px;
  border: 1px solid var(--border, #475569);
  border-radius: 5px;
  background: var(--surface-2, #334155);
  color: var(--text, #f1f5f9);
  font-family: inherit;
}
</style>
