import { defineStore } from 'pinia'
import { ref } from 'vue'

export interface BenchmarkResult {
  provider: string
  format: string
  surveyId: number
  surveyTitle: string
  generationMs: number
  fileSizeBytes: number
  timestamp: string
  questionCount: number
}

const STORAGE_KEY = 'reportapp:benchmarks'

function load(): BenchmarkResult[] {
  try {
    return JSON.parse(localStorage.getItem(STORAGE_KEY) ?? '[]')
  } catch {
    return []
  }
}

export const useBenchmarkStore = defineStore('benchmarks', () => {
  const results = ref<BenchmarkResult[]>(load())

  function add(r: BenchmarkResult) {
    results.value.unshift(r)
    // Keep last 50
    if (results.value.length > 50) results.value = results.value.slice(0, 50)
    localStorage.setItem(STORAGE_KEY, JSON.stringify(results.value))
  }

  function clear() {
    results.value = []
    localStorage.removeItem(STORAGE_KEY)
  }

  // Best per provider+format
  function best(provider: string, format: string) {
    return results.value
      .filter(r => r.provider === provider && r.format === format)
      .sort((a, b) => a.generationMs - b.generationMs)[0]
  }

  return { results, add, clear, best }
})
