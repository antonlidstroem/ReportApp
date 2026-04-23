import { defineStore } from 'pinia'
import { ref, computed } from 'vue'

export interface Survey {
  id: number
  title: string
  companyName?: string
  questionCount: number
}

export interface Question {
  id: number
  text: string
  category: string
  type: string
  responseCount: number
}

export interface ChartConfig {
  type: 'bar' | 'line' | 'radar' | 'doughnut'
  questionCount: number  // how many top questions feed the chart
}

export const useReportBuilderStore = defineStore('reportBuilder', () => {
  // ── Survey / question state ──────────────────────────────────────────────
  const surveyId     = ref<number | null>(null)
  const surveys      = ref<Survey[]>([])
  const questions    = ref<Question[]>([])

  // Ordered array — position in array = position in export
  const orderedSelectedIds = ref<number[]>([])

  // ── Date range ────────────────────────────────────────────────────────────
  const dateStart = ref<string>('')
  const dateEnd   = ref<string>('')

  // ── Template ──────────────────────────────────────────────────────────────
  const templateHtml = ref<string>('')

  // ── Chart configuration (jsreport / Hybrid A only) ────────────────────────
  const chartConfig = ref<ChartConfig>({
    type: 'bar',
    questionCount: 5,
  })

  // ── Computed ──────────────────────────────────────────────────────────────
  const selectedQuestionIds = computed(() => orderedSelectedIds.value)

  const selectedCount = computed(() => orderedSelectedIds.value.length)

  const isSelected = (id: number) => orderedSelectedIds.value.includes(id)

  // Questions in export order (only selected ones, in user-defined order)
  const orderedSelectedQuestions = computed(() =>
    orderedSelectedIds.value
      .map(id => questions.value.find(q => q.id === id))
      .filter(Boolean) as Question[]
  )

  // ── Actions ───────────────────────────────────────────────────────────────
  function setSurvey(id: number) {
    surveyId.value = id
    orderedSelectedIds.value = []
    questions.value = []
  }

  function toggleQuestion(id: number) {
    const idx = orderedSelectedIds.value.indexOf(id)
    if (idx >= 0) {
      orderedSelectedIds.value.splice(idx, 1)
    } else {
      orderedSelectedIds.value.push(id)
    }
  }

  function selectAllQuestions() {
    orderedSelectedIds.value = questions.value.map(q => q.id)
  }

  function clearSelection() {
    orderedSelectedIds.value = []
  }

  function selectCategory(category: string) {
    const catIds = questions.value
      .filter(q => q.category === category)
      .map(q => q.id)
    const existing = new Set(orderedSelectedIds.value)
    catIds.forEach(id => {
      if (!existing.has(id)) orderedSelectedIds.value.push(id)
    })
  }

  function deselectCategory(category: string) {
    const catIds = new Set(
      questions.value.filter(q => q.category === category).map(q => q.id)
    )
    orderedSelectedIds.value = orderedSelectedIds.value.filter(
      id => !catIds.has(id)
    )
  }

  function isCategoryFullySelected(category: string): boolean {
    const catIds = questions.value
      .filter(q => q.category === category)
      .map(q => q.id)
    return catIds.length > 0 && catIds.every(id => orderedSelectedIds.value.includes(id))
  }

  function isCategoryPartiallySelected(category: string): boolean {
    const catIds = questions.value
      .filter(q => q.category === category)
      .map(q => q.id)
    const selectedInCat = catIds.filter(id => orderedSelectedIds.value.includes(id))
    return selectedInCat.length > 0 && selectedInCat.length < catIds.length
  }

  // Drag-to-reorder: swap positions in the ordered selection
  function reorderQuestion(fromIndex: number, toIndex: number) {
    if (fromIndex === toIndex) return
    const arr = [...orderedSelectedIds.value]
    const [moved] = arr.splice(fromIndex, 1)
    arr.splice(toIndex, 0, moved)
    orderedSelectedIds.value = arr
  }

  function updateChartConfig(partial: Partial<ChartConfig>) {
    chartConfig.value = { ...chartConfig.value, ...partial }
  }

  return {
    // state
    surveyId,
    surveys,
    questions,
    orderedSelectedIds,
    dateStart,
    dateEnd,
    templateHtml,
    chartConfig,
    // computed
    selectedQuestionIds,
    selectedCount,
    orderedSelectedQuestions,
    // actions
    setSurvey,
    toggleQuestion,
    selectAllQuestions,
    clearSelection,
    selectCategory,
    deselectCategory,
    isCategoryFullySelected,
    isCategoryPartiallySelected,
    reorderQuestion,
    isSelected,
    updateChartConfig,
  }
})
