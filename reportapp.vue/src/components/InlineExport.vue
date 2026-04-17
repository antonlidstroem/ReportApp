<template>
  <div class="ie">
    <div class="ie-formats">
      <button v-for="f in availableFormats" :key="f.id"
        :class="['ie-fmt', { active: selectedFmt === f.id }]"
        @click="selectedFmt = f.id" :title="f.label">{{ f.icon }}</button>
    </div>
    <button :class="['ie-btn', { loading: isLoading }]"
      :disabled="isLoading || !surveyId" @click="doExport">
      <span v-if="!isLoading">⬇ {{ label ?? `Exportera ${selectedFmt.toUpperCase()}` }}</span>
      <span v-else class="ie-spin"></span>
    </button>
    <div v-if="result" :class="['ie-result', result.ok ? 'ok' : 'err']">
      <span v-if="result.ok">✓ {{ result.ms }}ms · {{ result.size }}</span>
      <span v-else>✗ {{ result.msg }}</span>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { useReportBuilderStore } from '../stores/reportBuilder'
import { useBenchmarkStore } from '../stores/benchmarks'
import { exportReport, downloadBlob } from '../composables/useApi'

const props = defineProps<{
  provider: string
  supportsHtmlTemplate: boolean
  htmlTemplate?: string
  surveyIdOverride?: number
  label?: string
}>()

const store = useReportBuilderStore()
const bench = useBenchmarkStore()
const selectedFmt = ref('pdf')
const isLoading = ref(false)
const result = ref<{ ok: boolean; ms?: number; size?: string; msg?: string } | null>(null)
const surveyId = computed(() => props.surveyIdOverride ?? store.surveyId)

const availableFormats = [
  { id: 'pdf',   icon: '📕', label: 'PDF'        },
  { id: 'excel', icon: '📗', label: 'Excel'      },
  { id: 'ppt',   icon: '📘', label: 'PowerPoint' },
]

async function doExport() {
  if (!surveyId.value || isLoading.value) return
  isLoading.value = true
  result.value = null
  try {
    const res = await exportReport({
      provider: props.provider, format: selectedFmt.value, surveyId: surveyId.value,
      htmlTemplate: selectedFmt.value === 'pdf' && props.supportsHtmlTemplate
        ? (props.htmlTemplate ?? '') : undefined,
      questionIds: !props.surveyIdOverride && store.selectedQuestionIds.length > 0
        ? store.selectedQuestionIds.join(',') : undefined,
    })
    downloadBlob(res.blob, res.filename)
    const kb = res.fileSizeBytes > 1_000_000
      ? `${(res.fileSizeBytes/1_000_000).toFixed(1)} MB`
      : `${(res.fileSizeBytes/1024).toFixed(0)} KB`
    result.value = { ok: true, ms: res.generationMs, size: kb }
    bench.add({ provider: props.provider, format: selectedFmt.value, surveyId: surveyId.value,
      surveyTitle: '', generationMs: res.generationMs, fileSizeBytes: res.fileSizeBytes,
      timestamp: new Date().toISOString(), questionCount: 0 })
  } catch (err) {
    result.value = { ok: false, msg: err instanceof Error ? err.message.substring(0, 80) : 'Okänt fel' }
  } finally { isLoading.value = false }
}
</script>

<style scoped>
.ie{display:flex;align-items:center;gap:8px;flex-wrap:wrap}
.ie-formats{display:flex;gap:4px}
.ie-fmt{width:28px;height:28px;border-radius:6px;border:1px solid rgba(255,255,255,.1);background:transparent;cursor:pointer;font-size:14px;display:flex;align-items:center;justify-content:center;transition:all .12s}
.ie-fmt:hover{border-color:rgba(255,255,255,.25)}
.ie-fmt.active{border-color:var(--accent,#a855f7);background:color-mix(in srgb,var(--accent,#a855f7) 12%,transparent)}
.ie-btn{padding:7px 16px;border-radius:7px;border:none;background:var(--accent,#a855f7);color:white;font-size:12px;font-weight:600;cursor:pointer;white-space:nowrap;transition:all .15s;min-width:140px;display:flex;align-items:center;justify-content:center;font-family:inherit}
.ie-btn:hover:not(:disabled){filter:brightness(1.12)}.ie-btn:disabled{opacity:.5;cursor:not-allowed}.ie-btn.loading{cursor:wait}
.ie-spin{width:14px;height:14px;border:2px solid rgba(255,255,255,.2);border-top-color:white;border-radius:50%;animation:spin .7s linear infinite}
@keyframes spin{to{transform:rotate(360deg)}}
.ie-result{font-size:11px;font-weight:600;padding:4px 10px;border-radius:5px}
.ie-result.ok{color:#22c55e;background:rgba(34,197,94,.1)}.ie-result.err{color:#ef4444;background:rgba(239,68,68,.1);max-width:220px}
</style>
