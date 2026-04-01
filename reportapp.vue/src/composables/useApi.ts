import axios from "axios";

const api = axios.create({ baseURL: "https://localhost:7008/api" });

// Definiera exakt vad export-funktionen behöver
interface ExportParams {
  provider: string;
  format: string;
  surveyId: number;
  htmlTemplate?: string;
  start?: string;
  end?: string;
  questionIds?: string | null;
}

export async function fetchSurveys() {
  const res = await api.get("/reports/surveys");
  return res.data;
}

export async function fetchQuestions(surveyId: number) {
  // Uppdaterad URL för att matcha controllern: /surveys/{id}/questions
  const res = await api.get(`/reports/surveys/${surveyId}/questions`);
  return res.data;
}

// Här använder vi ExportParams istället för any
// Här använder vi ExportParams istället för any
export async function exportReport(params: ExportParams) {
  const startPerf = performance.now();

  // Vi definierar cleanParams som ett objekt där både nycklar och värden är strängar
  // Detta ersätter 'any' och tar bort ESLint-felet
  const cleanParams: Record<string, string> = {};

  if (params.start) cleanParams.start = params.start;
  if (params.end) cleanParams.end = params.end;

  // params.questionIds kan vara string eller null, så vi kollar att det finns en sträng
  if (typeof params.questionIds === "string") {
    cleanParams.questionIds = params.questionIds;
  }

  const res = await api.post(
    `/reports/export/${params.provider}/${params.format}/${params.surveyId}`,
    { htmlTemplate: params.htmlTemplate || "" },
    {
      params: cleanParams,
      responseType: "blob",
    },
  );

  const endPerf = performance.now();
  const ext = params.format.toLowerCase() === "excel" ? "xlsx" : params.format;

  return {
    blob: res.data,
    filename: `Rapport_${params.provider}.${ext}`,
    generationMs: Math.round(endPerf - startPerf),
    fileSizeBytes: res.data.size,
  };
}

export function downloadBlob(blob: Blob, filename: string) {
  const url = window.URL.createObjectURL(blob);
  const link = document.createElement("a");
  link.href = url;
  link.setAttribute("download", filename);
  document.body.appendChild(link);
  link.click();
  link.remove();
}
