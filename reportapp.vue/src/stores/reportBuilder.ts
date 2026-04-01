import { defineStore } from 'pinia'
import { ref, computed } from 'vue'

export type ModuleType = 'summary' | 'questionTable' | 'trend' | 'category' | 'rawData'

export interface ReportModule {
  id: string
  type: ModuleType
  label: string
  enabled: boolean
  order: number
  config: Record<string, unknown>
}

export interface Survey {
  id: number
  title: string
  questionCount: number
}

export interface Question {
  id: number
  text: string
  category: string
  type: string
  responseCount: number
}

const DEFAULT_MODULES: ReportModule[] = [
  { id: 'summary',       type: 'summary',       label: 'Sammanfattning',     enabled: true,  order: 0, config: {} },
  { id: 'questionTable', type: 'questionTable',  label: 'Frågetabell',        enabled: true,  order: 1, config: { showDistribution: false } },
  { id: 'trend',         type: 'trend',          label: 'Månadsvis trend',    enabled: true,  order: 2, config: {} },
  { id: 'category',      type: 'category',       label: 'Kategorianalys',     enabled: true,  order: 3, config: {} },
  { id: 'rawData',       type: 'rawData',         label: 'Rådata (stresstest)',enabled: false, order: 4, config: {} },
]

export const useReportBuilderStore = defineStore('reportBuilder', () => {
  const surveyId     = ref<number | null>(null)
  const surveys      = ref<Survey[]>([])
  const questions    = ref<Question[]>([])
  const selectedIds  = ref<Set<number>>(new Set())
  const modules      = ref<ReportModule[]>(DEFAULT_MODULES.map(m => ({ ...m })))
  const dateStart    = ref<string>('')
  const dateEnd      = ref<string>('')
  const templateHtml = ref<string>('')

  // Sorted enabled modules
  const activeModules = computed(() =>
    modules.value
      .filter(m => m.enabled)
      .sort((a, b) => a.order - b.order)
  )

  const selectedQuestionIds = computed(() => Array.from(selectedIds.value))

  function setSurvey(id: number) {
    surveyId.value = id
    selectedIds.value = new Set()
    questions.value = []
  }

  function toggleQuestion(id: number) {
    if (selectedIds.value.has(id)) selectedIds.value.delete(id)
    else selectedIds.value.add(id)
  }

  function selectAllQuestions() {
    selectedIds.value = new Set(questions.value.map(q => q.id))
  }

  function clearSelection() {
    selectedIds.value = new Set()
  }

  function toggleModule(id: string) {
    const mod = modules.value.find(m => m.id === id)
    if (mod) mod.enabled = !mod.enabled
  }

  function moveModule(id: string, direction: 'up' | 'down') {
    const sorted = [...modules.value].sort((a, b) => a.order - b.order)
    const idx = sorted.findIndex(m => m.id === id)
    const swapIdx = direction === 'up' ? idx - 1 : idx + 1
    if (swapIdx < 0 || swapIdx >= sorted.length) return
    const tmp = sorted[idx].order
    sorted[idx].order = sorted[swapIdx].order
    sorted[swapIdx].order = tmp
  }

  function updateModuleConfig(id: string, config: Record<string, unknown>) {
    const mod = modules.value.find(m => m.id === id)
    if (mod) mod.config = { ...mod.config, ...config }
  }

  function resetModules() {
    modules.value = DEFAULT_MODULES.map(m => ({ ...m }))
  }

  return {
    surveyId, surveys, questions, selectedIds, modules,
    dateStart, dateEnd, templateHtml,
    activeModules, selectedQuestionIds,
    setSurvey, toggleQuestion, selectAllQuestions, clearSelection,
    toggleModule, moveModule, updateModuleConfig, resetModules
  }
})
