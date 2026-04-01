<template>
  <div class="survey-picker">
    <div class="picker-header">
      <h3>📋 Välj enkät & frågor</h3>
      <span class="sel-count" v-if="store.selectedQuestionIds.length > 0">
        {{ store.selectedQuestionIds.length }} frågor valda
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
        <div class="survey-name">
          <span v-if="s.title.includes('⚡')" class="badge stress">STRESS</span>
          {{ s.title.replace('⚡ ', '') }}
        </div>
        <div class="survey-meta">{{ s.questionCount }} frågor</div>
      </button>
    </div>

    <!-- Question picker -->
    <transition name="slide">
      <div v-if="store.surveyId && store.questions.length > 0" class="question-panel">
        <div class="q-toolbar">
          <span class="q-title">Frågor</span>
          <div class="q-actions">
            <button @click="store.selectAllQuestions()" class="btn-sm">Välj alla</button>
            <button @click="store.clearSelection()" class="btn-sm btn-ghost">Rensa</button>
          </div>
        </div>

        <!-- Category filter -->
        <div class="cat-chips">
          <button
            v-for="cat in categories"
            :key="cat"
            class="chip"
            :class="{ active: activeCat === cat }"
            @click="activeCat = activeCat === cat ? null : cat"
          >{{ cat }}</button>
        </div>

        <div class="question-list" v-if="!loading">
          <label
            v-for="q in filteredQuestions"
            :key="q.id"
            class="question-row"
            :class="{ selected: store.selectedIds.has(q.id) }"
          >
            <input
              type="checkbox"
              :checked="store.selectedIds.has(q.id)"
              @change="store.toggleQuestion(q.id)"
            />
            <div class="q-info">
              <span class="q-text">{{ q.text }}</span>
              <div class="q-meta">
                <span class="cat-tag">{{ q.category }}</span>
                <span class="resp-count">{{ q.responseCount }} svar</span>
                <span class="q-type">{{ q.type === 'Scale' ? '1–5' : 'Ja/Nej' }}</span>
              </div>
            </div>
          </label>
        </div>

        <div v-else class="loading-qs">
          <div class="spinner"></div> Laddar frågor...
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
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import { useReportBuilderStore } from '../stores/reportBuilder'
import { fetchSurveys, fetchQuestions } from '../composables/useApi'

const store = useReportBuilderStore()
const loading = ref(false)
const activeCat = ref<string | null>(null)

const categories = computed(() => {
  const cats = new Set(store.questions.map(q => q.category))
  return Array.from(cats).sort()
})

const filteredQuestions = computed(() =>
  activeCat.value
    ? store.questions.filter(q => q.category === activeCat.value)
    : store.questions
)

onMounted(async () => {
  if (!store.surveys.length) {
    store.surveys = await fetchSurveys()
  }
})

async function selectSurvey(s: { id: number; title: string; questionCount: number }) {
  store.setSurvey(s.id)
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
  background: var(--surface);
  border-radius: 12px;
  border: 1px solid var(--border);
  overflow: hidden;
}

.picker-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 14px 18px;
  background: var(--surface-2);
  border-bottom: 1px solid var(--border);
}
.picker-header h3 { font-size: 14px; font-weight: 600; margin: 0; }

.sel-count {
  font-size: 12px;
  background: var(--accent);
  color: white;
  padding: 2px 10px;
  border-radius: 100px;
}

.survey-list { padding: 10px; display: flex; flex-direction: column; gap: 6px; }

.survey-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 12px 14px;
  border-radius: 8px;
  border: 1px solid var(--border);
  background: transparent;
  cursor: pointer;
  text-align: left;
  transition: all 0.15s;
  color: var(--text);
}
.survey-item:hover { background: var(--surface-2); }
.survey-item.active { border-color: var(--accent); background: color-mix(in srgb, var(--accent) 8%, transparent); }

.survey-name { font-size: 13px; font-weight: 500; display: flex; align-items: center; gap: 8px; }
.survey-meta { font-size: 11px; color: var(--text-muted); }

.badge.stress {
  font-size: 9px;
  background: #ff6b35;
  color: white;
  padding: 2px 6px;
  border-radius: 4px;
  font-weight: 700;
  letter-spacing: 0.5px;
}

.question-panel {
  border-top: 1px solid var(--border);
  padding: 12px;
}

.q-toolbar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 10px;
}
.q-title { font-size: 12px; font-weight: 600; color: var(--text-muted); text-transform: uppercase; letter-spacing: 0.5px; }
.q-actions { display: flex; gap: 6px; }

.btn-sm {
  font-size: 11px;
  padding: 4px 10px;
  border-radius: 6px;
  border: 1px solid var(--border);
  background: var(--accent);
  color: white;
  cursor: pointer;
}
.btn-sm.btn-ghost { background: transparent; color: var(--text-muted); }
.btn-sm:hover { opacity: 0.85; }

.cat-chips { display: flex; flex-wrap: wrap; gap: 6px; margin-bottom: 10px; }
.chip {
  font-size: 11px;
  padding: 3px 10px;
  border-radius: 100px;
  border: 1px solid var(--border);
  background: transparent;
  color: var(--text-muted);
  cursor: pointer;
  transition: all 0.12s;
}
.chip.active { background: var(--accent); color: white; border-color: var(--accent); }

.question-list {
  max-height: 280px;
  overflow-y: auto;
  display: flex;
  flex-direction: column;
  gap: 4px;
}
.question-list::-webkit-scrollbar { width: 4px; }
.question-list::-webkit-scrollbar-thumb { background: var(--border); border-radius: 4px; }

.question-row {
  display: flex;
  align-items: flex-start;
  gap: 10px;
  padding: 8px 10px;
  border-radius: 6px;
  cursor: pointer;
  transition: background 0.1s;
}
.question-row:hover { background: var(--surface-2); }
.question-row.selected { background: color-mix(in srgb, var(--accent) 6%, transparent); }
.question-row input { margin-top: 2px; accent-color: var(--accent); flex-shrink: 0; }

.q-info { flex: 1; }
.q-text { font-size: 12px; line-height: 1.4; }
.q-meta { display: flex; gap: 8px; margin-top: 4px; flex-wrap: wrap; }

.cat-tag {
  font-size: 10px;
  background: color-mix(in srgb, var(--accent) 12%, transparent);
  color: var(--accent);
  padding: 1px 6px;
  border-radius: 4px;
}
.resp-count, .q-type { font-size: 10px; color: var(--text-muted); }

.loading-qs {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 12px;
  color: var(--text-muted);
  padding: 16px;
}
.spinner {
  width: 16px;
  height: 16px;
  border: 2px solid var(--border);
  border-top-color: var(--accent);
  border-radius: 50%;
  animation: spin 0.6s linear infinite;
}

.date-range {
  display: flex;
  gap: 10px;
  padding: 12px;
  border-top: 1px solid var(--border);
}
.date-range label {
  display: flex;
  flex-direction: column;
  gap: 4px;
  flex: 1;
}
.date-range label span { font-size: 11px; color: var(--text-muted); }
.date-range input {
  font-size: 12px;
  padding: 6px 8px;
  border: 1px solid var(--border);
  border-radius: 6px;
  background: var(--surface-2);
  color: var(--text);
}

.slide-enter-active, .slide-leave-active { transition: all 0.2s ease; }
.slide-enter-from, .slide-leave-to { opacity: 0; transform: translateY(-6px); }

@keyframes spin { to { transform: rotate(360deg); } }
</style>
