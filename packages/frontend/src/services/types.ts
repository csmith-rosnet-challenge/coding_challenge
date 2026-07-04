export interface ExternalStatusRequest {
  urlsText: string;
  timeoutMs: number;
  maxConcurrency: number;
}

export interface ExternalStatusResult {
  url: string;
  statusCode?: number | null;
  durationMs: number;
  error?: string | null;
}

export interface ExternalStatusHistoryItem {
  id: number;
  url: string;
  statusCode?: number | null;
  durationMs: number;
  error?: string | null;
  checkedAt: string;
}
