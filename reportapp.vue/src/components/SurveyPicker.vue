<template>
  <div class="survey-picker">
    <div class="picker-header">
      <h3>📋 Enkät & frågor</h3>
      <span class="sel-count" v-if="store.selectedQuestionIds.length > 0">
        {{ store.selectedQuestionIds.length }} valda
      </span>
    </div>

    <!-- Survey list -->
    <div class="survey-list">
      <button
        v-for="s in store.surveys"
        :key="s.id"
        class="survey-item"
        :class="{ active: store.surveyId === s.id }"
        @click="selectSurvey(s)"
      >
        <div class="survey-left">
          <span v-if="s.title.includes('⚡')" class="badge stress">STRESS</span>
          <span class="survey-name">{{ s.title.replace('⚡ ', '') }}</span>
        </div>
        <div class="survey-right">
          <span class="survey-meta">{{ s.questionCount }} frågor</span>
          <span class="survey-company" v-if="s.companyName">{{ s.companyName }}</span>
        </div>
      </button>
    </div>

    <!-- Question panel -->
    <transition name="slide">
      <div v-if="store.surveyId && store.questions.length > 0" class="question-panel">

        <!-- Toolbar -->
        <div class="q-toolbar">
          <div class="q-search">
            <input
              v-model="searchText"
              placeholder="🔍 Sök frågor…"
              class="search-input"
            />
          </div>
          <div class="q-actions">
            <button @click="store.selectAllQuestions()" class="btn-sm">✓ Alla</button>
            <button @click="store.clearSelection()" class="btn-sm btn-ghost">✕ Rensa</button>
          </div>
        </div>

        <!-- Category filter tabs -->
        <div class="cat-filter">
          <button
            class="cat-tab"
            :class="{ active: activeCat === null }"
            @click="activeCat = null">
            Alla <span class="cat-count">{{ store.questions.length }}</span>
          </button>
          <button
            v-for="cat in categoriesWithCounts"
            :key="cat.name"
            class="cat-tab"
            :class="{ active: activeCat === cat.name }"
            @click="activeCat = activeCat === cat.name ? null : cat.name">
            {{ cat.name }}
            <span class="cat-count">{{ cat.count }}</span>
          </button>
        </div>

        <!-- Questions grouped by category when no filter -->
        <div class="question-list" v-if="!loading">
          <template v-if="activeCat === null && !searchText">
            <div v-for="group in questionsByCategory" :key="group.category">
              <!-- Category header with select-all -->
              <div class="cat-group-header">
                <span class="cgh-name">{{ group.category }}</span>
                <span class="cgh-count">{{ group.questions.length }} frågor</span>
                <button class="cgh-select" @click="toggleCategory(group.category)">
                  {{ isCategoryFullySelected(group.category) ? '✕ Avmarkera' : '✓ Välj alla' }}
                </button>
              </div>
              <!-- Questions in this category -->
              <label
                v-for="q in group.questions"
                :key="q.id"
                class="question-row"
                :class="{ selected: store.selectedIds.has(q.id) }">
                <input
                  type="checkbox"
                  :checked="store.selectedIds.has(q.id)"
                  @change="store.toggleQuestion(q.id)" />
                <div class="q-info">
                  <span class="q-text">{{ q.text }}</span>
                  <div class="q-meta">
                    <span class="q-type-badge" :class="q.type === 'Scale' ? 'scale' : 'yesno'">
                      {{ q.type === 'Scale' ? '1–5' : 'Ja/Nej' }}
                    </span>
                    <span class="resp-count">{{ q.responseCount }} svar</span>
                  </div>
                </div>
              </label>
            </div>
          </template>

          <!-- Filtered / searched view -->
          <template v-else>
            <label
              v-for="q in filteredQuestions"
              :key="q.id"
              class="question-row"
              :class="{ selected: store.selectedIds.has(q.id) }">
              <input
                type="checkbox"
                :checked="store.selectedIds.has(q.id)"
                @change="store.toggleQuestion(q.id)" />
              <div class="q-info">
                <span class="q-text">{{ highlightMatch(q.text) }}</span>
                <div class="q-meta">
                  <span class="cat-tag">{{ q.category }}</span>
                  <span class="q-type-badge" :class="q.type === 'Scale' ? 'scale' : 'yesno'">
                    {{ q.type === 'Scale' ? '1–5' : 'Ja/Nej' }}
                  </span>
                  <span class="resp-count">{{ q.responseCount }} svar</span>
                </div>
              </div>
            </label>
            <div v-if="filteredQuestions.length === 0" class="no-results">
              Inga frågor matchar "{{ searchText }}"
            </div>
          </template>
        </div>

        <div v-else-if="loading" class="loading-qs">
          <div class="spinner"></div> Laddar frågor…
        </div>

        <!-- Selection summary bar -->
        <div class="selection-bar" v-if="store.selectedQuestionIds.length > 0">
          <div class="sb-left">
            <span class="sb-count">{{ store.selectedQuestionIds.length }}</span> av
            {{ store.questions.length }} valda
          </div>
          <div class="sb-cats">
            <span v-for="cat in selectedCategoryBreakdown" :key="cat.name" class="sb-cat">
              {{ cat.name }}: {{ cat.count }}
            </span>
          </div>
        </div>

      </div>
    </transition>

    <!-- Date range -->
    <div class="date-range" v-if="store.surveyId">
      <label>
        <span>Från</span>
        <input type="date" v-model="store.dateStart" />
      </label>
      <label>
        <span>Till</span>
        <input type="date" v-model="store.dateEnd" />
      </label>
    </div>

    <!-- Empty state -->
    <div v-if="!store.surveys.length" class="empty-state">
      <div class="es-icon">📡</div>
      <div>Laddar enkäter från API…</div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useReportBuilderStore } from '../stores/reportBuilder'
import { fetchSurveys, fetchQuestions } from '../composables/useApi'

const store = useReportBuilderStore()
const loading   = ref(false)
const activeCat = ref<string | null>(null)
const searchText = ref('')

// Surveys with companyName (cast needed because store type is narrower)
interface FullSurvey { id: number; title: string; questionCount: number; companyName?: string }

const categoriesWithCounts = computed(() => {
  const map = new Map<string, number>()
  store.questions.forEach(q => {
    map.set(q.category, (map.get(q.category) ?? 0) + 1)
  })
  return Array.from(map.entries())
    .map(([name, count]) => ({ name, count }))
    .sort((a, b) => a.name.localeCompare(b.name))
})

const questionsByCategory = computed(() => {
  const groups = new Map<string, typeof store.questions[0][]>()
  store.questions.forEach(q => {
    const arr = groups.get(q.category) ?? []
    arr.push(q)
    groups.set(q.category, arr)
  })
  return Array.from(groups.entries())
    .map(([category, questions]) => ({ category, questions }))
    .sort((a, b) => a.category.localeCompare(b.category))
})

const filteredQuestions = computed(() => {
  let qs = store.questions
  if (activeCat.value) qs = qs.filter(q => q.category === activeCat.value)
  if (searchText.value.trim()) {
    const term = searchText.value.toLowerCase()
    qs = qs.filter(q => q.text.toLowerCase().includes(term) || q.category.toLowerCase().includes(term))
  }
  return qs
})

const selectedCategoryBreakdown = computed(() => {
  const map = new Map<string, number>()
  store.questions
    .filter(q => store.selectedIds.has(q.id))
    .forEach(q => { map.set(q.category, (map.get(q.category) ?? 0) + 1) })
  return Array.from(map.entries()).map(([name, count]) => ({ name, count }))
})

function isCategoryFullySelected(category: string): boolean {
  const qs = store.questions.filter(q => q.category === category)
  return qs.length > 0 && qs.every(q => store.selectedIds.has(q.id))
}

function toggleCategory(category: string) {
  const qs = store.questions.filter(q => q.category === category)
  if (isCategoryFullySelected(category)) {
    qs.forEach(q => { if (store.selectedIds.has(q.id)) store.toggleQuestion(q.id) })
  } else {
    qs.forEach(q => { if (!store.selectedIds.has(q.id)) store.toggleQuestion(q.id) })
  }
}

function highlightMatch(text: string): string {
  if (!searchText.value.trim()) return text
  const term = searchText.value.trim()
  const idx = text.toLowerCase().indexOf(term.toLowerCase())
  if (idx === -1) return text
  return text.substring(0, idx) + '**' + text.substring(idx, idx + term.length) + '**' + text.substring(idx + term.length)
}

onMounted(async () => {
  if (!store.surveys.length) {
    store.surveys = await fetchSurveys()
  }
})

async function selectSurvey(s: FullSurvey) {
  store.setSurvey(s.id)
  activeCat.value = null
  searchText.value = ''
  loading.value = true
  try {
    store.questions = await fetchQuestions(s.id)
    store.selectAllQuestions()
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
.survey-picker {
  background: var(--surface, #1e293b);
  border-radius: 12px;
  border: 1px solid var(--border, #334155);
  overflow: hidden;
}

.picker-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 13px 18px;
  background: var(--surface-2, #0f172a);
  border-bottom: 1px solid var(--border, #334155);
}
.picker-header h3 { font-size: 13px; font-weight: 600; margin: 0; }
.sel-count {
  font-size: 11px;
  background: var(--accent, #a855f7);
  color: white;
  padding: 2px 10px;
  border-radius: 100px;
  font-weight: 600;
}

.survey-list { padding: 8px; display: flex; flex-direction: column; gap: 4px; }

.survey-item {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  padding: 10px 12px;
  border-radius: 8px;
  border: 1px solid var(--border, #334155);
  background: transparent;
  cursor: pointer;
  text-align: left;
  transition: all 0.15s;
  color: var(--text, #f1f5f9);
  gap: 8px;
}
.survey-item:hover { background: var(--surface-2, #0f172a); }
.survey-item.active { border-color: var(--accent, #a855f7); background: color-mix(in srgb, var(--accent, #a855f7) 8%, transparent); }

.survey-left { display: flex; align-items: center; gap: 6px; flex: 1; min-width: 0; }
.survey-name { font-size: 12px; font-weight: 500; white-space: nowrap; overflow: hidden; text-overflow: ellipsis; }
.survey-right { display: flex; flex-direction: column; align-items: flex-end; gap: 2px; flex-shrink: 0; }
.survey-meta { font-size: 11px; color: var(--accent, #a855f7); font-weight: 600; }
.survey-company { font-size: 10px; color: var(--text-muted, #64748b); }

.badge.stress {
  font-size: 9px;
  background: #f97316;
  color: white;
  padding: 1px 5px;
  border-radius: 3px;
  font-weight: 700;
  letter-spacing: .5px;
  flex-shrink: 0;
}

.question-panel {
  border-top: 1px solid var(--border, #334155);
  padding: 10px;
}

.q-toolbar {
  display: flex;
  align-items: center;
  gap: 8px;
  margin-bottom: 8px;
}
.q-search { flex: 1; }
.search-input {
  width: 100%;
  background: var(--surface-2, #0f172a);
  border: 1px solid var(--border, #334155);
  border-radius: 6px;
  padding: 5px 10px;
  font-size: 12px;
  color: var(--text, #f1f5f9);
  outline: none;
  transition: border-color .15s;
}
.search-input:focus { border-color: var(--accent, #a855f7); }
.search-input::placeholder { color: var(--text-muted, #64748b); }

.q-actions { display: flex; gap: 4px; flex-shrink: 0; }
.btn-sm {
  font-size: 11px;
  padding: 4px 9px;
  border-radius: 5px;
  border: 1px solid var(--border, #334155);
  background: var(--accent, #a855f7);
  color: white;
  cursor: pointer;
}
.btn-sm.btn-ghost { background: transparent; color: var(--text-muted, #64748b); }
.btn-sm:hover { opacity: .85; }

.cat-filter {
  display: flex;
  flex-wrap: wrap;
  gap: 4px;
  margin-bottom: 8px;
}
.cat-tab {
  font-size: 10px;
  padding: 3px 9px;
  border-radius: 100px;
  border: 1px solid var(--border, #334155);
  background: transparent;
  color: var(--text-muted, #64748b);
  cursor: pointer;
  transition: all .12s;
  display: flex;
  align-items: center;
  gap: 4px;
}
.cat-tab.active { background: var(--accent, #a855f7); color: white; border-color: var(--accent, #a855f7); }
.cat-count {
  font-size: 9px;
  background: rgba(255,255,255,.12);
  padding: 0 4px;
  border-radius: 100px;
}

.question-list {
  max-height: 320px;
  overflow-y: auto;
  display: flex;
  flex-direction: column;
  gap: 2px;
}
.question-list::-webkit-scrollbar { width: 4px; }
.question-list::-webkit-scrollbar-thumb { background: var(--border, #334155); border-radius: 4px; }

/* Category group header */
.cat-group-header {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 6px 8px;
  background: var(--surface-2, #0f172a);
  border-radius: 6px;
  margin: 4px 0 2px;
  position: sticky;
  top: 0;
  z-index: 1;
}
.cgh-name { font-size: 11px; font-weight: 700; color: var(--text, #f1f5f9); flex: 1; }
.cgh-count { font-size: 10px; color: var(--text-muted, #64748b); }
.cgh-select {
  font-size: 10px;
  padding: 2px 7px;
  border-radius: 4px;
  border: 1px solid var(--border, #334155);
  background: transparent;
  color: var(--accent, #a855f7);
  cursor: pointer;
}
.cgh-select:hover { background: color-mix(in srgb, var(--accent, #a855f7) 10%, transparent); }

.question-row {
  display: flex;
  align-items: flex-start;
  gap: 9px;
  padding: 6px 8px;
  border-radius: 6px;
  cursor: pointer;
  transition: background .1s;
  border: 1px solid transparent;
}
.question-row:hover { background: var(--surface-2, #0f172a); }
.question-row.selected {
  background: color-mix(in srgb, var(--accent, #a855f7) 6%, transparent);
  border-color: color-mix(in srgb, var(--accent, #a855f7) 20%, transparent);
}
.question-row input { margin-top: 2px; accent-color: var(--accent, #a855f7); flex-shrink: 0; }

.q-info { flex: 1; min-width: 0; }
.q-text { font-size: 11px; line-height: 1.4; display: block; }
.q-meta { display: flex; gap: 6px; margin-top: 3px; flex-wrap: wrap; align-items: center; }

.cat-tag {
  font-size: 9px;
  background: color-mix(in srgb, var(--accent, #a855f7) 12%, transparent);
  color: var(--accent, #a855f7);
  padding: 1px 6px;
  border-radius: 4px;
}
.q-type-badge {
  font-size: 9px;
  padding: 1px 5px;
  border-radius: 3px;
  font-weight: 600;
}
.q-type-badge.scale  { background: rgba(59,130,246,.12);  color: #60a5fa; }
.q-type-badge.yesno  { background: rgba(34,197,94,.12);   color: #22c55e; }
.resp-count { font-size: 9px; color: var(--text-muted, #64748b); }

.no-results { text-align: center; padding: 16px; font-size: 12px; color: var(--text-muted, #64748b); }

/* Selection summary */
.selection-bar {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 6px 8px;
  margin-top: 8px;
  background: color-mix(in srgb, var(--accent, #a855f7) 6%, transparent);
  border: 1px solid color-mix(in srgb, var(--accent, #a855f7) 20%, transparent);
  border-radius: 6px;
  flex-wrap: wrap;
}
.sb-left { font-size: 11px; color: var(--text, #f1f5f9); }
.sb-count { font-weight: 700; color: var(--accent, #a855f7); }
.sb-cats { display: flex; gap: 6px; flex-wrap: wrap; }
.sb-cat { font-size: 9px; color: var(--text-muted, #64748b); }

.loading-qs {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 12px;
  color: var(--text-muted, #64748b);
  padding: 16px 8px;
}
.spinner {
  width: 16px; height: 16px;
  border: 2px solid var(--border, #334155);
  border-top-color: var(--accent, #a855f7);
  border-radius: 50%;
  animation: spin 0.6s linear infinite;
}

.date-range {
  display: flex;
  gap: 8px;
  padding: 10px;
  border-top: 1px solid var(--border, #334155);
}
.date-range label { display: flex; flex-direction: column; gap: 3px; flex: 1; }
.date-range label span { font-size: 10px; color: var(--text-muted, #64748b); }
.date-range input {
  font-size: 11px;
  padding: 5px 8px;
  border: 1px solid var(--border, #334155);
  border-radius: 6px;
  background: var(--surface-2, #0f172a);
  color: var(--text, #f1f5f9);
  outline: none;
}
.date-range input:focus { border-color: var(--accent, #a855f7); }

.empty-state {
  padding: 28px;
  text-align: center;
  color: var(--text-muted, #64748b);
  font-size: 12px;
}
.es-icon { font-size: 24px; margin-bottom: 8px; }

.slide-enter-active, .slide-leave-active { transition: all .2s ease; }
.slide-enter-from, .slide-leave-to { opacity: 0; transform: translateY(-6px); }

@keyframes spin { to { transform: rotate(360deg); } }
</style>
