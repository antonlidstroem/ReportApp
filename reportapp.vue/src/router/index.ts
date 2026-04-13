import { createRouter, createWebHistory } from "vue-router";
import DashboardView from "../views/DashboardView.vue";
import JsReportView from "../views/providers/JsReportView.vue";
import SyncfusionView from "../views/providers/SyncfusionView.vue";

const router = createRouter({
  history: createWebHistory(),
  routes: [
    { path: "/", component: DashboardView },
    { path: "/providers/jsreport", component: JsReportView },
    { path: "/providers/syncfusion", component: SyncfusionView },
    { path: "/providers/js-sync", component: () => import("../views/providers/CompositeJsSyncView.vue") },
    { path: "/providers/play-sync", component: () => import("../views/providers/CompositePlaywrightSyncView.vue") },
  ],
});

export default router;
