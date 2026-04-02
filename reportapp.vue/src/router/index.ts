import { createRouter, createWebHistory } from "vue-router";
import DashboardView from "../views/DashboardView.vue";
import QuestView from "../views/providers/QuestView.vue";


const router = createRouter({
  history: createWebHistory(),
  routes: [
    { path: "/", component: DashboardView },
    { path: "/providers/quest", component: QuestView },
    // Vi behöver skapa dessa två (se steg 3):
    { path: "/providers/iron", component: () => import("../views/providers/IronView.vue") },
    { path: "/providers/jsreport", component: () => import("../views/providers/JsReportView.vue") },
    { path: "/providers/syncfusion", component: () => import("../views/providers/SyncfusionView.vue") },
  ],
});

export default router;
