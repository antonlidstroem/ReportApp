<template>
  <Transition name="banner">
    <div v-if="show" class="health-banner">
      <div class="hb-inner">
        <span class="hb-icon">⚠</span>
        <div class="hb-text">
          <strong>Backend unreachable</strong>
          <span>Cannot connect to <code>{{ url }}</code> — is the .NET API running?
            <span v-if="retryCount > 0" class="hb-retry">Retrying… ({{ retryCount }})</span>
          </span>
        </div>
        <button class="hb-dismiss" @click="dismissed = true" title="Dismiss">✕</button>
      </div>
    </div>
  </Transition>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted, computed } from 'vue'
import { checkHealth, getBaseUrl } from '../composables/useApi'

const url = getBaseUrl()
const healthy = ref(true)
const dismissed = ref(false)
const retryCount = ref(0)
let interval: ReturnType<typeof setInterval> | null = null

const show = computed(() => !healthy.value && !dismissed.value)

async function poll() {
  const ok = await checkHealth()
  if (ok) {
    healthy.value = true
    retryCount.value = 0
    dismissed.value = false // re-show if it goes down again after recovery
  } else {
    healthy.value = false
    retryCount.value++
  }
}

onMounted(() => {
  poll()
  // Poll every 6s while unhealthy, every 30s while healthy
  interval = setInterval(() => {
    poll()
  }, healthy.value ? 30_000 : 6_000)
})

onUnmounted(() => {
  if (interval) clearInterval(interval)
})
</script>

<style scoped>
.health-banner {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  z-index: 9999;
  background: #7f1d1d;
  border-bottom: 1px solid #991b1b;
  padding: 10px 20px;
}

.hb-inner {
  display: flex;
  align-items: center;
  gap: 12px;
  max-width: 1300px;
  margin: 0 auto;
}

.hb-icon {
  font-size: 16px;
  flex-shrink: 0;
}

.hb-text {
  display: flex;
  flex-direction: column;
  gap: 2px;
  flex: 1;
  font-size: 13px;
  color: #fecaca;
}

.hb-text strong {
  color: #fff;
  font-weight: 700;
}

.hb-text code {
  background: rgba(0, 0, 0, 0.3);
  padding: 1px 5px;
  border-radius: 3px;
  font-family: 'Courier New', monospace;
  font-size: 11px;
  color: #fca5a5;
}

.hb-retry {
  margin-left: 8px;
  font-size: 11px;
  color: #fca5a5;
  opacity: 0.8;
}

.hb-dismiss {
  background: rgba(255, 255, 255, 0.1);
  border: 1px solid rgba(255, 255, 255, 0.2);
  color: #fecaca;
  width: 26px;
  height: 26px;
  border-radius: 50%;
  cursor: pointer;
  font-size: 11px;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
  transition: background 0.15s;
}

.hb-dismiss:hover {
  background: rgba(255, 255, 255, 0.2);
  color: white;
}

.banner-enter-active,
.banner-leave-active {
  transition: all 0.25s ease;
}

.banner-enter-from,
.banner-leave-to {
  transform: translateY(-100%);
  opacity: 0;
}
</style>
