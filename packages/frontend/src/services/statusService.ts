import type {
  ExternalStatusRequest,
  ExternalStatusResult,
  ExternalStatusHistoryItem,
} from './types';

const API_BASE = import.meta.env.VITE_API_BASE || 'http://localhost:5000';

export const parseUrls = (text: string): string[] =>
  text
    .split(/\r?\n|,|;/)
    .map((url) => url.trim())
    .filter(Boolean);

export const fetchExternalStatus = async ({
  urlsText,
  timeoutMs,
  maxConcurrency,
}: ExternalStatusRequest): Promise<ExternalStatusResult[]> => {
  const urls = parseUrls(urlsText);
  const body: Record<string, unknown> = {
    timeoutMs,
    maxConcurrency,
  };

  if (urls.length > 0) {
    body.urls = urls;
  }

  const response = await fetch(`${API_BASE}/api/external-status`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(body),
  });

  if (!response.ok) {
    throw new Error(`Request failed: ${response.status}`);
  }

  return response.json();
};

export const fetchExternalStatusHistory = async (
  limit = 50,
): Promise<ExternalStatusHistoryItem[]> => {
  const response = await fetch(`${API_BASE}/api/external-status/history?limit=${limit}`);

  if (!response.ok) {
    throw new Error(`Request failed: ${response.status}`);
  }

  return response.json();
};
